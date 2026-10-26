# Captured when the module dot-sources this file; $PSScriptRoot is empty inside the function at call time.
$script:PSWUGuiDir = $PSScriptRoot

function Show-PSWUConsole {
    <#
    .SYNOPSIS
        Opens the PSWindowsUpdateNext desktop console, a WPF front end for the module's cmdlets.
    .DESCRIPTION
        Opens a dark, keyboard-friendly window over the same cmdlets that the module exports. It has eight
        pages: Dashboard (reboot state, last search and install, agent version, install statistics, and the
        client policy), Updates (scan, filter, download, install, hide, and unhide), History, Services
        (register Microsoft Update or remove a service), Settings (view and edit the Windows Update policy),
        Catalog (search the Microsoft Update Catalog and download packages with SHA-1 digest and Authenticode
        verification), Jobs (list, create, and clear the module's Task Scheduler jobs), and Log.

        Every operation runs in a background runspace, so the window stays responsive, and verbose, warning,
        and error output goes to the Log page. Destructive actions ask for confirmation first.

        The function blocks until the window is closed. It runs on Windows only, in Windows PowerShell 5.1 or
        PowerShell 7, and needs a single-threaded apartment (the default for the PowerShell console).

        Most actions need an elevated session: scanning, downloading, installing, hiding and unhiding updates,
        restarting, registering or removing update services, applying settings, and creating or clearing jobs.
        Without elevation the sidebar offers "Restart as administrator", and read-only pages such as History
        and the Catalog download still work.
    .PARAMETER PassThru
        Return the window object without showing it. Call Show() or ShowDialog() yourself. Without this
        switch the function shows the window modally and returns nothing.
    .EXAMPLE
        Show-PSWUConsole

        Opens the console and waits until you close it.
    .EXAMPLE
        Start-Process pwsh -Verb RunAs -ArgumentList '-NoProfile', '-Command', 'Import-Module PSWindowsUpdateNext; Show-PSWUConsole'

        Opens the console in a new elevated PowerShell 7 session, so that every action is available.
    .EXAMPLE
        $window = Show-PSWUConsole -PassThru
        $window.Topmost = $true
        $window.ShowDialog() | Out-Null

        Creates the window without showing it, keeps it above other windows, and then shows it.
    .INPUTS
        None. This function does not accept pipeline input.
    .OUTPUTS
        None by default.

        System.Windows.Window when you use -PassThru.
    .NOTES
        Requires Windows. Importing the module does not load WPF; the assemblies are loaded when the function
        runs. The console works on the local computer and calls the module's own cmdlets, so it needs the
        same elevation as they do.
    .LINK
        https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Show-PSWUConsole.md
    .LINK
        Get-WindowsUpdate
    #>
    [CmdletBinding()]
    param(
        [switch]$PassThru
    )

    if ($PSEdition -eq 'Core' -and -not $IsWindows) {
        throw 'Show-PSWUConsole needs Windows: it hosts a WPF window over the Windows Update Agent.'
    }

    Add-Type -AssemblyName PresentationFramework, PresentationCore, WindowsBase, System.Xaml
    Add-Type -AssemblyName System.Windows.Forms

    # The window's event handlers fire long after this function returns, so everything they touch
    # (state, helper functions, the worker runspace) lives in a dynamic module that stays alive
    # as long as the handlers reference it.
    New-Module -Name 'PSWUConsole' -ReturnResult -ArgumentList $script:PSWUGuiDir, $PassThru.IsPresent -ScriptBlock $script:PSWUConsoleBody
}

$script:PSWUConsoleBody = {
    param([string]$guiDir, [bool]$PassThru)

    $moduleRoot = Split-Path $guiDir -Parent
    $manifest = Join-Path $moduleRoot 'PSWindowsUpdateNext.psd1'

    # The row types and the XAML live inside the signed PSWindowsUpdateNext.dll, so nothing here is compiled
    # or parsed from an unsigned file on disk.
    $asm = [PSWindowsUpdateNext.Gui.UpdateRow].Assembly

    function Get-GuiResource {
        param([string]$Name)
        $stream = $asm.GetManifestResourceStream($Name)
        if (-not $stream) { throw "PSWindowsUpdateNext: embedded resource '$Name' is missing from $($asm.Location)." }
        $sr = New-Object System.IO.StreamReader ($stream, [System.Text.Encoding]::UTF8)
        try { $sr.ReadToEnd() } finally { $sr.Dispose() }
    }

    # ---------- window ----------
    $xamlText = (Get-GuiResource 'PSWindowsUpdateNext.Gui.MainWindow.xaml').Replace('<!--THEME-->', (Get-GuiResource 'PSWindowsUpdateNext.Gui.Theme.xaml'))
    $reader = New-Object System.Xml.XmlNodeReader ([xml]$xamlText)
    $window = [Windows.Markup.XamlReader]::Load($reader)
    $window.MaxHeight = [System.Windows.SystemParameters]::MaximizedPrimaryScreenHeight + 8

    $ui = @{}
    foreach ($m in [regex]::Matches($xamlText, 'x:Name="([^"]+)"')) {
        $name = $m.Groups[1].Value
        $el = $window.FindName($name)
        if ($el) { $ui[$name] = $el }
    }

    # ---------- state ----------
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $isAdmin = ([Security.Principal.WindowsPrincipal]$identity).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    $moduleVersion = (Import-PowerShellDataFile $manifest).ModuleVersion
    $state = @{
        IsAdmin      = $isAdmin
        AllUpdates   = @()
        History      = @()
        Catalog      = New-Object System.Collections.ObjectModel.ObservableCollection[object]
        Log          = New-Object System.Collections.ObjectModel.ObservableCollection[object]
        Queue        = New-Object System.Collections.Generic.Queue[object]
        Task         = $null
        Loaded       = @{}
        LastScanArgs = $null
    }

    $ui.ListLog.ItemsSource = $state.Log
    $ui.ListCatalog.ItemsSource = $state.Catalog
    $ui.TxtComputer.Text = $env:COMPUTERNAME
    $ui.TxtVersion.Text = "PSWindowsUpdateNext $moduleVersion"
    $ui.TxtEdition.Text = "PowerShell $($PSVersionTable.PSVersion)  ·  $PSEdition"
    $docs = [Environment]::GetFolderPath('MyDocuments')
    if (-not $docs) { $docs = Join-Path $HOME 'Downloads' }
    $ui.TxtCatalogDest.Text = Join-Path $docs 'WindowsUpdates'
    $ui.TxtJobScript.Text = 'Import-Module PSWindowsUpdateNext; Install-WindowsUpdate -AcceptAll -IgnoreReboot | Out-File $Env:TEMP\PSWindowsUpdateNext.log'

    if ($isAdmin) {
        $ui.DotElevation.Fill = $window.FindResource('B.Success')
        $ui.TxtElevation.Text = 'Administrator'
        $ui.TxtElevationHint.Text = "Running as $($identity.Name)."
        $ui.BtnElevate.Visibility = 'Collapsed'
    } else {
        $ui.TxtElevationHint.Text = "Running as $($identity.Name). Scanning and installing need an elevated session."
    }

    # ---------- helpers ----------
    function Add-Log {
        param([string]$Level, [string]$Message)
        if ([string]::IsNullOrWhiteSpace($Message)) { return }
        $entry = New-Object PSWindowsUpdateNext.Gui.LogEntry
        $entry.Time = Get-Date
        $entry.Level = $Level
        $entry.Message = $Message.TrimEnd()
        $state.Log.Add($entry)
        while ($state.Log.Count -gt 4000) { $state.Log.RemoveAt(0) }
        if ($ui.PageLog.Visibility -eq 'Visible') { $ui.LogScroll.ScrollToEnd() }
    }

    function Write-Status {
        param([string]$Text, [string]$Color = 'B.Success')
        $ui.TxtStatus.Text = $Text
        $ui.DotStatus.Fill = $window.FindResource($Color)
    }

    function Show-BusyState {
        param([bool]$Busy, [string]$TaskName)
        $ui.BusyBar.Visibility = if ($Busy) { 'Visible' } else { 'Hidden' }
        foreach ($n in 'BtnScan', 'BtnDashScan', 'BtnDashInstallAll', 'BtnDashReboot', 'BtnDownload', 'BtnInstall', 'BtnHide', 'BtnUnhide',
                       'BtnHistoryRefresh', 'BtnServicesRefresh', 'BtnAddMicrosoftUpdate', 'BtnSettingsRefresh', 'BtnApplySettings',
                       'BtnCatalogSearch', 'BtnCatalogDownload', 'BtnJobsRefresh', 'BtnJobsClear', 'BtnJobCreate') {
            $ui[$n].IsEnabled = -not $Busy
        }
        if ($Busy) { Write-Status -Text $TaskName -Color 'B.Accent' } else { Write-Status -Text 'Ready'; $ui.TxtProgress.Text = '' }
    }

    function Show-Message {
        param([string]$Text, [string]$Title = 'PSWindowsUpdateNext', [string]$Icon = 'Information')
        [System.Windows.MessageBox]::Show($window, $Text, $Title, 'OK', $Icon) | Out-Null
    }

    function Confirm-Action {
        param([string]$Text, [string]$Title = 'Confirm')
        $r = [System.Windows.MessageBox]::Show($window, $Text, $Title, 'YesNo', 'Warning')
        return $r -eq 'Yes'
    }

    function Test-Elevated {
        if ($state.IsAdmin) { return $true }
        Show-Message -Text 'This action needs an elevated session. Use "Restart as administrator" in the sidebar.' -Icon 'Warning'
        return $false
    }

    # Background runspace with the module imported once.
    $iss = [System.Management.Automation.Runspaces.InitialSessionState]::CreateDefault()
    $iss.ImportPSModule(@($manifest))
    $runspace = [System.Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace($iss)
    $runspace.Open()

    function Invoke-ConsoleTask {
        param([string]$Name, [string]$Script, [hashtable]$Params = @{}, [scriptblock]$OnDone)
        $job = @{ Name = $Name; Script = $Script; Params = $Params; OnDone = $OnDone }
        if ($state.Task) { $state.Queue.Enqueue($job); return }
        Invoke-Job $job
    }

    function Invoke-Job {
        param($Job)
        $ps = [PowerShell]::Create()
        $ps.Runspace = $runspace
        [void]$ps.AddScript($Job.Script)
        foreach ($k in $Job.Params.Keys) { [void]$ps.AddParameter($k, $Job.Params[$k]) }
        $output = New-Object 'System.Management.Automation.PSDataCollection[psobject]'
        $handle = $ps.BeginInvoke([System.Management.Automation.PSDataCollection[psobject]](New-Object 'System.Management.Automation.PSDataCollection[psobject]'), $output)
        $state.Task = @{ PS = $ps; Handle = $handle; Output = $output; Job = $Job; Idx = @{ V = 0; W = 0; E = 0; I = 0; D = 0 } }
        Add-Log -Level 'task' -Message ">> $($Job.Name)"
        Show-BusyState -Busy $true -TaskName $Job.Name
    }

    function Receive-TaskStream {
        $t = $state.Task
        if (-not $t) { return }
        $s = $t.PS.Streams
        for ($i = $t.Idx.V; $i -lt $s.Verbose.Count; $i++) { Add-Log 'verbose' $s.Verbose[$i].Message }
        $t.Idx.V = $s.Verbose.Count
        for ($i = $t.Idx.W; $i -lt $s.Warning.Count; $i++) { Add-Log 'warning' $s.Warning[$i].Message }
        $t.Idx.W = $s.Warning.Count
        for ($i = $t.Idx.E; $i -lt $s.Error.Count; $i++) { Add-Log 'error' ($s.Error[$i].ToString()) }
        $t.Idx.E = $s.Error.Count
        for ($i = $t.Idx.I; $i -lt $s.Information.Count; $i++) { Add-Log 'info' ($s.Information[$i].MessageData.ToString()) }
        $t.Idx.I = $s.Information.Count
        if ($s.Progress.Count -gt 0) {
            $p = $s.Progress[$s.Progress.Count - 1]
            $ui.TxtProgress.Text = if ($p.RecordType -eq 'Completed') { '' } else { "$($p.Activity): $($p.StatusDescription)" }
        }
    }

    $timer = New-Object System.Windows.Threading.DispatcherTimer
    $timer.Interval = [TimeSpan]::FromMilliseconds(150)
    $timer.Add_Tick({
        $t = $state.Task
        if (-not $t) { return }
        try { Receive-TaskStream } catch { Add-Log 'error' "Stream read failed: $($_.Exception.Message) at $($_.ScriptStackTrace)" }
        if (-not $t.Handle.IsCompleted) { return }
        try {
            Complete-Task $t
        } catch {
            $state.Task = $null
            Show-BusyState -Busy $false
            Add-Log 'error' "Task completion failed: $($_.Exception.Message) at $($_.ScriptStackTrace)"
        }
    })

    function Complete-Task {
        param($t)
        $errors = @()
        try {
            $t.PS.EndInvoke($t.Handle) | Out-Null
        } catch {
            $errors += $_
            Add-Log 'error' $_.Exception.Message
        }
        Receive-TaskStream
        $errors += @($t.PS.Streams.Error)
        $result = @($t.Output)
        $t.PS.Dispose()
        $state.Task = $null
        Add-Log 'ok' "<< $($t.Job.Name) ($($result.Count) objects, $($errors.Count) errors)"
        Show-BusyState -Busy $false
        if ($t.Job.OnDone) {
            try { & $t.Job.OnDone $result $errors } catch { Add-Log 'error' "UI handler failed: $($_.Exception.Message)" }
        }
        if ($state.Queue.Count -gt 0) { Invoke-Job $state.Queue.Dequeue() }
    }
    $timer.Start()

    # ---------- navigation ----------
    $pages = @{ NavDashboard = 'PageDashboard'; NavUpdates = 'PageUpdates'; NavHistory = 'PageHistory'; NavServices = 'PageServices';
                NavSettings = 'PageSettings'; NavCatalog = 'PageCatalog'; NavJobs = 'PageJobs'; NavLog = 'PageLog' }
    function Show-Page {
        param([string]$Nav)
        foreach ($p in $pages.Values) { $ui[$p].Visibility = 'Collapsed' }
        $ui[$pages[$Nav]].Visibility = 'Visible'
        if ($Nav -eq 'NavLog') { $ui.LogScroll.ScrollToEnd() }
        if (-not $state.Loaded[$Nav]) {
            $state.Loaded[$Nav] = $true
            switch ($Nav) {
                'NavHistory'  { Show-HistoryPage }
                'NavServices' { Show-ServicePage }
                'NavSettings' { Show-SettingPage }
                'NavJobs'     { Show-JobPage }
            }
        }
    }
    foreach ($nav in $pages.Keys) {
        $ui[$nav].Add_Checked([System.Windows.RoutedEventHandler]{ param($s) Show-Page $s.Name })
    }

    # ---------- window chrome ----------
    $ui.BtnMin.Add_Click({ $window.WindowState = 'Minimized' })
    $ui.BtnMax.Add_Click({ $window.WindowState = if ($window.WindowState -eq 'Maximized') { 'Normal' } else { 'Maximized' } })
    $ui.BtnClose.Add_Click({ $window.Close() })
    $window.Add_StateChanged({ $ui.BtnMax.Content = if ($window.WindowState -eq 'Maximized') { [char]0xE923 } else { [char]0xE922 } })
    $ui.BtnElevate.Add_Click({
        $exe = (Get-Process -Id $PID).Path
        # No -ExecutionPolicy override: the elevated host runs under the policy the administrator configured,
        # and the signed module loads under RemoteSigned or AllSigned. -EncodedCommand keeps a manifest path
        # that contains a quote or other special character from breaking the command line.
        # EscapeSingleQuotedStringContent also doubles the typographic quotes PowerShell treats as '.
        $quoted = [System.Management.Automation.Language.CodeGeneration]::EscapeSingleQuotedStringContent($manifest)
        $cmd = "Import-Module -Name '$quoted'; Show-PSWUConsole"
        $encoded = [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes($cmd))
        $hostArgs = @('-NoProfile')
        # A policy the user set for this session only (for example powershell -ExecutionPolicy Bypass)
        # is not inherited by the elevated process, so pass it on explicitly. Machine and user policies
        # apply on their own, and Group Policy still overrides the parameter.
        $sessionPolicy = Get-ExecutionPolicy -Scope Process
        if ($sessionPolicy -ne 'Undefined') { $hostArgs += @('-ExecutionPolicy', "$sessionPolicy") }
        $hostArgs += @('-EncodedCommand', $encoded)
        try {
            Start-Process -FilePath $exe -Verb RunAs -ArgumentList $hostArgs | Out-Null
            $window.Close()
        } catch {
            Show-Message -Text "Could not start an elevated session: $($_.Exception.Message)" -Icon 'Error'
        }
    })

    # ---------- dashboard ----------
    $dashboardScript = @'
param()
$o = [ordered]@{}
$os = Get-CimInstance Win32_OperatingSystem -ErrorAction SilentlyContinue
if ($os) { $o.Os = "$($os.Caption) $($os.Version)" }
try { $api = Get-WUApiVersion -WarningAction SilentlyContinue -ErrorAction Stop; $o.Agent = "$($api.ApiVersion) / $($api.WuapiDllVersion)" } catch { $o.Agent = 'n/a' }
try { $last = Get-WULastResults -WarningAction SilentlyContinue -ErrorAction Stop; $o.LastSearch = $last.LastSearchSuccessDate; $o.LastInstall = $last.LastInstallationSuccessDate } catch { }
try { $rb = Get-WURebootStatus -Silent -WarningAction SilentlyContinue -ErrorAction Stop; $o.RebootRequired = [bool]$rb.RebootRequired } catch { $o.RebootError = $_.Exception.Message }
try { $o.Busy = [bool](Get-WUInstallerStatus -WarningAction SilentlyContinue -ErrorAction Stop).IsBusy } catch { }
try { $o.DefaultService = "$((Get-WUServiceManager -WarningAction SilentlyContinue -ErrorAction Stop | Where-Object { $_.IsDefaultAUService -eq $true } | Select-Object -First 1).Name)" } catch { $o.DefaultService = '' }
try {
    $o.History = @(Get-WUHistory -Last 300 -WarningAction SilentlyContinue -ErrorAction Stop | ForEach-Object {
        [pscustomobject]@{ Date = $_.Date; Title = "$($_.Title)"; KB = "$($_.KB)"; Result = "$($_.Result)"; Operation = "$($_.OperationName)" } })
} catch { $o.History = @() }
try {
    $s = Get-WUSettings -WarningAction SilentlyContinue -ErrorAction Stop
    $o.Settings = @($s.PSObject.Properties | Where-Object { $_.Name -notin 'ComputerName','CmdletStart','CmdletEnd','CmdletLine' } | ForEach-Object { [pscustomobject]@{ Name = $_.Name; Value = "$($_.Value)" } })
} catch { $o.Settings = @() }
[pscustomobject]$o
'@

    function Show-DashboardPage {
        Invoke-ConsoleTask -Name 'Reading Windows Update status' -Script $dashboardScript -OnDone {
            param($result)
            $d = $result | Select-Object -First 1
            if (-not $d) { return }
            if ($d.Os) { $ui.TxtOs.Text = $d.Os }
            $ui.TxtAgent.Text = $d.Agent
            $ui.TxtLastSearch.Text = if ($d.LastSearch) { ([datetime]$d.LastSearch).ToString('g') } else { 'never' }
            $ui.TxtLastInstall.Text = if ($d.LastInstall) { ([datetime]$d.LastInstall).ToString('g') } else { 'never' }
            $ui.TxtDefaultService.Text = if ($d.DefaultService) { $d.DefaultService } else { '—' }
            if ($d.PSObject.Properties['RebootRequired']) {
                $ui.BadgeReboot.Visibility = if ($d.RebootRequired) { 'Visible' } else { 'Collapsed' }
                $ui.TxtHeroTitle.Text = if ($d.RebootRequired) { 'Restart pending' } elseif ($d.Busy) { 'Installer busy' } else { 'Up to date as of last scan' }
                $ui.TxtHeroSub.Text = if ($d.RebootRequired) { 'An installed update is waiting for a restart to finish.' } else { 'Run a scan to check Microsoft for new updates.' }
            } else {
                $ui.TxtHeroTitle.Text = 'Elevation required'
                $ui.TxtHeroSub.Text = 'Reboot state and update scans are only available to administrators. History and services still work.'
            }
            $state.History = @($d.History | ForEach-Object {
                $r = New-Object PSWindowsUpdateNext.Gui.HistoryRow
                $r.Date = $_.Date; $r.Title = $_.Title; $r.KB = $_.KB; $r.Result = $_.Result; $r.Operation = $_.Operation
                $r })
            $ui.ListDashHistory.ItemsSource = @($state.History | Select-Object -First 12)
            $cut = (Get-Date).AddDays(-30)
            $recent = @($state.History | Where-Object { $_.Date -ge $cut -and $_.Operation -eq 'Installation' })
            $ui.MetricInstalled30.Text = @($recent | Where-Object Result -eq 'Succeeded').Count
            $failed = @($recent | Where-Object Result -ne 'Succeeded').Count
            $ui.MetricFailed30.Text = if ($failed) { "$failed failed or partial" } else { 'no failures' }
            $rows = @($d.Settings | ForEach-Object { $s = New-Object PSWindowsUpdateNext.Gui.SettingRow; $s.Name = $_.Name; $s.Value = $_.Value; $s })
            $ui.ListDashSettings.ItemsSource = @($rows | Select-Object -First 10)
            $ui.TxtDashSettingsEmpty.Visibility = if ($rows.Count) { 'Collapsed' } else { 'Visible' }
            if (-not $state.Loaded.NavSettings) { Show-SettingList $rows; $state.Loaded.NavSettings = $true }
            Show-HistoryList
            $state.Loaded.NavHistory = $true
        }
    }

    $ui.BtnDashScan.Add_Click({ $ui.NavUpdates.IsChecked = $true; Invoke-UpdateScan })
    $ui.BtnDashHistoryAll.Add_Click({ $ui.NavHistory.IsChecked = $true })
    $ui.BtnDashInstallAll.Add_Click({
        if (-not (Test-Elevated)) { return }
        if (Confirm-Action -Text 'Install every available update from the default service? Updates that need a restart will wait for you to reboot.') {
            Invoke-UpdateAction -Action 'Install' -All
        }
    })
    $ui.BtnDashReboot.Add_Click({
        if (-not (Test-Elevated)) { return }
        if (Confirm-Action -Text 'Restart this computer now? Unsaved work in other programs will be lost.' -Title 'Reboot') {
            Invoke-ConsoleTask -Name 'Restarting computer' -Script 'Get-WURebootStatus -AutoReboot -Confirm:$false -WarningAction SilentlyContinue | Out-Null'
        }
    })

    # ---------- updates ----------
    $scanScript = @'
param([string]$Source, [bool]$WithHidden, [bool]$Installed)
$p = @{ WarningAction = 'SilentlyContinue'; ErrorAction = 'Stop'; Verbose = $true }
switch ($Source) { 'MicrosoftUpdate' { $p.MicrosoftUpdate = $true } 'WindowsUpdate' { $p.WindowsUpdate = $true } }
if ($WithHidden) { $p.WithHidden = $true }
if ($Installed) { $p.IsInstalled = $true }
function Get-Scalar($value) { if ($value -is [System.Collections.IEnumerable] -and $value -isnot [string]) { @($value)[0] } else { $value } }
function Get-Num($value) { try { [decimal](Get-Scalar $value) } catch { 0 } }
function Get-Bool($value) { try { [bool](Get-Scalar $value) } catch { $false } }
# Get-WindowsUpdate can emit one collection object instead of one object per update; flatten it.
$raw = @(Get-WindowsUpdate @p)
$items = foreach ($o in $raw) { if ($o -is [System.Collections.ICollection] -and $o -isnot [string]) { foreach ($i in $o) { $i } } else { $o } }
$items | ForEach-Object {
    $u = $_
    try {
        $cats = try { @($u.Categories | ForEach-Object { "$($_.Name)" }) -join ', ' } catch { '' }
        $more = try { "$(@($u.MoreInfoUrls)[0])" } catch { '' }
        [pscustomobject]@{
            KB = "$($u.KB)"; Title = "$($u.Title)"; Size = "$($u.Size)"; SizeBytes = (Get-Num $u.MaxDownloadSize); Status = "$($u.Status)"
            UpdateID = "$($u.Identity.UpdateID)"; IsHidden = (Get-Bool $u.IsHidden); IsDownloaded = (Get-Bool $u.IsDownloaded); IsInstalled = (Get-Bool $u.IsInstalled)
            RebootRequired = (Get-Bool $u.RebootRequired); Category = $cats; Severity = "$($u.MsrcSeverity)"
            Description = "$($u.Description)"; MoreInfoUrl = $more; Published = $(try { [datetime](Get-Scalar $u.LastDeploymentChangeTime) } catch { [datetime]::MinValue })
        }
    } catch {
        Write-Warning "Skipped one update ($($u.Title)): $($_.Exception.Message)"
    }
}
'@

    $actionScript = @'
param([string]$Action, [string[]]$UpdateID, [string]$Source, [bool]$AutoReboot)
$p = @{ WarningAction = 'SilentlyContinue'; ErrorAction = 'Stop'; Verbose = $true; AcceptAll = $true; Confirm = $false }
switch ($Source) { 'MicrosoftUpdate' { $p.MicrosoftUpdate = $true } 'WindowsUpdate' { $p.WindowsUpdate = $true } }
if ($UpdateID) { $p.UpdateID = $UpdateID }
switch ($Action) {
    'Install'  { $p.Install = $true; if ($AutoReboot) { $p.AutoReboot = $true } else { $p.IgnoreReboot = $true } }
    'Download' { $p.Download = $true; $p.IgnoreReboot = $true }
    'Hide'     { $p.Hide = $true }
    'Unhide'   { $p.IsHidden = $true; $p.Hide = $false }
}
$raw = @(Get-WindowsUpdate @p)
$items = foreach ($o in $raw) { if ($o -is [System.Collections.ICollection] -and $o -isnot [string]) { foreach ($i in $o) { $i } } else { $o } }
$items | ForEach-Object { [pscustomobject]@{ KB = "$($_.KB)"; Title = "$($_.Title)"; Status = "$($_.Status)"; Result = "$($_.Result)" } }
'@

    function Get-Source {
        switch ($ui.CmbSource.SelectedIndex) { 1 { 'MicrosoftUpdate' } 2 { 'WindowsUpdate' } default { 'Default' } }
    }

    function Invoke-UpdateScan {
        if (-not (Test-Elevated)) { return }
        $scanArgs = @{ Source = (Get-Source); WithHidden = [bool]$ui.ChipHidden.IsChecked; Installed = [bool]$ui.ChipInstalled.IsChecked }
        $state.LastScanArgs = $scanArgs
        $ui.TxtUpdatesEmpty.Text = 'Scanning…'
        Invoke-ConsoleTask -Name "Scanning ($($scanArgs.Source))" -Script $scanScript -Params $scanArgs -OnDone {
            param($result, $errors)
            $state.AllUpdates = @($result | ForEach-Object {
                $r = New-Object PSWindowsUpdateNext.Gui.UpdateRow
                foreach ($p in 'KB','Title','Size','SizeBytes','Status','UpdateID','IsHidden','IsDownloaded','IsInstalled','RebootRequired','Category','Severity','Description','MoreInfoUrl','Published') {
                    $r.$p = $_.$p
                }
                $r })
            Show-UpdateList
            $pending = @($state.AllUpdates | Where-Object { -not $_.IsInstalled -and -not $_.IsHidden })
            $ui.MetricPending.Text = $pending.Count
            $ui.MetricPendingSub.Text = "$([math]::Round(($pending | Measure-Object SizeBytes -Sum).Sum / 1MB)) MB to download"
            $ui.MetricDownloaded.Text = @($pending | Where-Object IsDownloaded).Count
            $ui.MetricHidden.Text = @($state.AllUpdates | Where-Object IsHidden).Count
            $ui.TxtUpdatesSub.Text = "$($state.AllUpdates.Count) updates from $(Get-Source) at $((Get-Date).ToString('t'))"
            $ui.TxtUpdatesEmpty.Text = if ($errors.Count) { 'Scan failed. See the log.' } else { 'No updates match.' }
            if (-not $errors.Count) {
                $ui.TxtHeroTitle.Text = if ($pending.Count) { "$($pending.Count) updates available" } else { 'No updates available' }
                $ui.TxtHeroSub.Text = if ($pending.Count) { 'Review them on the Updates page, or install everything from here.' } else { 'This computer has everything the service offers.' }
            }
        }
    }

    function Show-UpdateList {
        $text = $ui.TxtUpdateFilter.Text
        $rows = $state.AllUpdates
        if ($ui.ChipRebootOnly.IsChecked) { $rows = @($rows | Where-Object RebootRequired) }
        if ($text) { $rows = @($rows | Where-Object { $_.Title -like "*$text*" -or $_.KB -like "*$text*" }) }
        $ui.ListUpdates.ItemsSource = @($rows)
        $ui.TxtUpdatesEmpty.Visibility = if ($rows.Count) { 'Collapsed' } else { 'Visible' }
    }

    function Get-SelectedUpdate { @($state.AllUpdates | Where-Object Selected) }

    function Invoke-UpdateAction {
        param([string]$Action, [switch]$All)
        if (-not (Test-Elevated)) { return }
        $ids = @()
        if (-not $All) {
            $ids = @((Get-SelectedUpdate).UpdateID)
            if (-not $ids.Count) { Show-Message -Text 'Select at least one update first.' -Icon 'Warning'; return }
        }
        $verb = switch ($Action) { 'Install' { 'install' } 'Download' { 'download' } 'Hide' { 'hide' } 'Unhide' { 'unhide' } }
        $what = if ($All) { 'all available updates' } else { "$($ids.Count) selected update(s)" }
        if ($Action -in 'Install', 'Hide' -and -not (Confirm-Action -Text "Really $verb $what?")) { return }
        $params = @{ Action = $Action; UpdateID = $ids; Source = (Get-Source); AutoReboot = [bool]$ui.SwAutoReboot.IsChecked }
        Invoke-ConsoleTask -Name "$Action $what" -Script $actionScript -Params $params -OnDone {
            param($result)
            foreach ($r in $result) { Add-Log 'info' "$($r.Status)  $($r.KB)  $($r.Title)  ->  $($r.Result)" }
            if ($state.LastScanArgs) { Invoke-ConsoleTask -Name 'Rescanning' -Script $scanScript -Params $state.LastScanArgs -OnDone $state.RescanDone }
            else { Invoke-UpdateScan }
        }
    }
    # Reuse the scan completion handler for rescans after an action.
    $state.RescanDone = {
        param($result)
        $state.AllUpdates = @($result | ForEach-Object {
            $r = New-Object PSWindowsUpdateNext.Gui.UpdateRow
            foreach ($p in 'KB','Title','Size','SizeBytes','Status','UpdateID','IsHidden','IsDownloaded','IsInstalled','RebootRequired','Category','Severity','Description','MoreInfoUrl','Published') { $r.$p = $_.$p }
            $r })
        Show-UpdateList
        $pending = @($state.AllUpdates | Where-Object { -not $_.IsInstalled -and -not $_.IsHidden })
        $ui.MetricPending.Text = $pending.Count
        $ui.MetricDownloaded.Text = @($pending | Where-Object IsDownloaded).Count
        $ui.MetricHidden.Text = @($state.AllUpdates | Where-Object IsHidden).Count
        Show-DashboardPage
    }

    $ui.BtnScan.Add_Click({ Invoke-UpdateScan })
    $ui.BtnInstall.Add_Click({ Invoke-UpdateAction -Action 'Install' })
    $ui.BtnDownload.Add_Click({ Invoke-UpdateAction -Action 'Download' })
    $ui.BtnHide.Add_Click({ Invoke-UpdateAction -Action 'Hide' })
    $ui.BtnUnhide.Add_Click({ Invoke-UpdateAction -Action 'Unhide' })
    $ui.TxtUpdateFilter.Add_TextChanged({ Show-UpdateList })
    $ui.ChipRebootOnly.Add_Click({ Show-UpdateList })
    $ui.ChipHidden.Add_Click({ if ($state.AllUpdates.Count) { Invoke-UpdateScan } })
    $ui.ChipInstalled.Add_Click({ if ($state.AllUpdates.Count) { Invoke-UpdateScan } })
    $ui.ChkSelectAll.Add_Click({
        $on = [bool]$ui.ChkSelectAll.IsChecked
        foreach ($r in @($ui.ListUpdates.ItemsSource)) { $r.Selected = $on }
    })
    $ui.ListUpdates.AddHandler([System.Windows.UIElement]::MouseLeftButtonUpEvent, [System.Windows.Input.MouseButtonEventHandler]{
        $e = $args[1]  # (sender, MouseButtonEventArgs)
        $src = $e.OriginalSource
        $row = $null
        while ($src -and -not $row) {
            if ($src.DataContext -is [PSWindowsUpdateNext.Gui.UpdateRow]) { $row = $src.DataContext }
            $src = if ($src -is [System.Windows.FrameworkElement]) { $src.Parent } else { $null }
        }
        if ($row) { Show-Detail $row }
    })

    function Show-Detail {
        param($Row)
        $state.Detail = $Row
        $ui.PanelDetail.Visibility = 'Visible'
        $ui.DetTitle.Text = $Row.Title
        $ui.DetKB.Text = $Row.KB
        $ui.DetSize.Text = $Row.Size
        $ui.DetSeverity.Text = if ($Row.Severity) { $Row.Severity } else { 'unrated' }
        $ui.DetReboot.Text = if ($Row.RebootRequired) { 'required' } else { 'not required' }
        $ui.DetPublished.Text = $Row.PublishedText
        $ui.DetState.Text = $Row.StateText
        $ui.DetId.Text = $Row.UpdateID
        $ui.DetDescription.Text = $Row.Description
        $ui.BtnMoreInfo.IsEnabled = [bool]$Row.MoreInfoUrl
    }
    $ui.BtnMoreInfo.Add_Click({ if ($state.Detail.MoreInfoUrl) { Start-Process $state.Detail.MoreInfoUrl } })

    # ---------- history ----------
    function Show-HistoryPage {
        $count = @(25, 50, 100, 250, 1000)[$ui.CmbHistoryCount.SelectedIndex]
        Invoke-ConsoleTask -Name "Reading history (last $count)" -Script 'param([int]$Last) Get-WUHistory -Last $Last -WarningAction SilentlyContinue -ErrorAction Stop | ForEach-Object { [pscustomobject]@{ Date = $_.Date; Title = "$($_.Title)"; KB = "$($_.KB)"; Result = "$($_.Result)"; Operation = "$($_.OperationName)" } }' -Params @{ Last = $count } -OnDone {
            param($result)
            $state.History = @($result | ForEach-Object {
                $r = New-Object PSWindowsUpdateNext.Gui.HistoryRow
                $r.Date = $_.Date; $r.Title = $_.Title; $r.KB = $_.KB; $r.Result = $_.Result; $r.Operation = $_.Operation
                $r })
            Show-HistoryList
        }
    }
    function Show-HistoryList {
        $text = $ui.TxtHistoryFilter.Text
        $rows = $state.History
        if ($text) { $rows = @($rows | Where-Object { $_.Title -like "*$text*" -or $_.KB -like "*$text*" }) }
        $ui.ListHistory.ItemsSource = @($rows)
        $ui.TxtHistorySub.Text = "$($rows.Count) entries"
    }
    $ui.BtnHistoryRefresh.Add_Click({ Show-HistoryPage })
    $ui.CmbHistoryCount.Add_SelectionChanged({ if ($state.Loaded.NavHistory) { Show-HistoryPage } })
    $ui.TxtHistoryFilter.Add_TextChanged({ Show-HistoryList })

    # ---------- services ----------
    function Show-ServicePage {
        Invoke-ConsoleTask -Name 'Reading update services' -Script 'Get-WUServiceManager -WarningAction SilentlyContinue -ErrorAction Stop | ForEach-Object { [pscustomobject]@{ Name = "$($_.Name)"; ServiceID = "$($_.ServiceID)"; IsDefault = [bool]$_.IsDefaultAUService; IsManaged = [bool]$_.IsManaged; ServiceUrl = "$($_.ServiceUrl)" } }' -OnDone {
            param($result)
            $ui.ListServices.ItemsSource = @($result | ForEach-Object {
                $r = New-Object PSWindowsUpdateNext.Gui.ServiceRow
                $r.Name = $_.Name; $r.ServiceID = $_.ServiceID; $r.IsDefault = $_.IsDefault; $r.IsManaged = $_.IsManaged; $r.ServiceUrl = $_.ServiceUrl
                $r })
        }
    }
    $ui.BtnServicesRefresh.Add_Click({ Show-ServicePage })
    $ui.BtnAddMicrosoftUpdate.Add_Click({
        if (-not (Test-Elevated)) { return }
        Invoke-ConsoleTask -Name 'Registering Microsoft Update' -Script 'Add-WUServiceManager -MicrosoftUpdate -Confirm:$false -Verbose -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null' -OnDone { Show-ServicePage }
    })
    $ui.ListServices.AddHandler([System.Windows.Controls.Primitives.ButtonBase]::ClickEvent, [System.Windows.RoutedEventHandler]{
        $e = $args[1]  # (sender, RoutedEventArgs)
        $btn = $e.OriginalSource
        if ($btn -is [System.Windows.Controls.Button] -and $btn.Tag) {
            if (-not (Test-Elevated)) { return }
            $row = $btn.DataContext
            if (Confirm-Action -Text "Unregister '$($row.Name)' ($($row.ServiceID))?") {
                Invoke-ConsoleTask -Name "Removing service $($row.Name)" -Script 'param([string]$Id) Remove-WUServiceManager -ServiceID $Id -Confirm:$false -Verbose -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null' -Params @{ Id = $row.ServiceID } -OnDone { Show-ServicePage }
            }
        }
    })

    # ---------- settings ----------
    function Show-SettingList {
        param($Rows)
        $ui.ListSettings.ItemsSource = @($Rows)
        $ui.TxtSettingsEmpty.Visibility = if ($Rows.Count) { 'Collapsed' } else { 'Visible' }
        $lookup = @{}
        foreach ($r in $Rows) { $lookup[$r.Name] = $r.Value }
        $ui.SwNoAutoUpdate.IsChecked = ($lookup['NoAutoUpdate'] -eq '1')
        $ui.SwTargetRelease.IsChecked = ($lookup['TargetReleaseVersion'] -eq '1')
        $ui.TxtTargetRelease.Text = "$($lookup['TargetReleaseVersionInfo'])"
        $ui.TxtProductVersion.Text = "$($lookup['ProductVersion'])"
        $ui.TxtDeferQuality.Text = "$($lookup['DeferQualityUpdatesPeriodInDays'])"
        $ui.TxtDeferFeature.Text = "$($lookup['DeferFeatureUpdatesPeriodInDays'])"
        $ui.TxtInstallHour.Text = "$($lookup['ScheduledInstallTime'])"
        $ui.CmbAUOptions.SelectedIndex = 0
        $ui.CmbInstallDay.SelectedIndex = 0
    }
    function Show-SettingPage {
        Invoke-ConsoleTask -Name 'Reading client policy' -Script '$s = Get-WUSettings -WarningAction SilentlyContinue -ErrorAction Stop; if ($s) { $s.PSObject.Properties | Where-Object { $_.Name -notin "ComputerName","CmdletStart","CmdletEnd","CmdletLine" } | ForEach-Object { [pscustomobject]@{ Name = $_.Name; Value = "$($_.Value)" } } }' -OnDone {
            param($result)
            Show-SettingList @($result | ForEach-Object { $s = New-Object PSWindowsUpdateNext.Gui.SettingRow; $s.Name = $_.Name; $s.Value = $_.Value; $s })
        }
    }
    $ui.BtnSettingsRefresh.Add_Click({ Show-SettingPage })
    $ui.BtnApplySettings.Add_Click({
        if (-not (Test-Elevated)) { return }
        $p = @{}
        $p.NoAutoUpdate = [bool]$ui.SwNoAutoUpdate.IsChecked
        if ($ui.CmbAUOptions.SelectedIndex -gt 0) { $p.NotificationLevel = @('', 'Notify before download', 'Notify before installation', 'Scheduled installation', 'Users configure')[$ui.CmbAUOptions.SelectedIndex] }
        if ($ui.CmbInstallDay.SelectedIndex -gt 0) { $p.ScheduledInstallDay = @('', 'EveryDay', 'EverySunday', 'EveryMonday', 'EveryTuesday', 'EveryWednesday', 'EveryThursday', 'EveryFriday', 'EverySaturday')[$ui.CmbInstallDay.SelectedIndex] }
        if ($ui.TxtInstallHour.Text -match '^\d{1,2}$') { $p.ScheduledInstallTime = [int]$ui.TxtInstallHour.Text }
        if ($ui.SwTargetRelease.IsChecked) {
            $p.TargetReleaseVersion = $true
            if ($ui.TxtTargetRelease.Text) { $p.TargetReleaseVersionInfo = $ui.TxtTargetRelease.Text }
            if ($ui.TxtProductVersion.Text) { $p.ProductVersion = $ui.TxtProductVersion.Text }
        } elseif ($ui.TxtTargetRelease.Text -or $ui.TxtProductVersion.Text) {
            $p.TargetReleaseVersion = $false
        }
        if ($ui.TxtDeferQuality.Text -match '^\d+$') { $p.DeferQualityUpdatesPeriodInDays = [int]$ui.TxtDeferQuality.Text }
        if ($ui.TxtDeferFeature.Text -match '^\d+$') { $p.DeferFeatureUpdatesPeriodInDays = [int]$ui.TxtDeferFeature.Text }
        $summary = ($p.GetEnumerator() | ForEach-Object { "$($_.Key) = $($_.Value)" }) -join "`n"
        if (-not (Confirm-Action -Text "Write these policy values?`n`n$summary")) { return }
        Invoke-ConsoleTask -Name 'Writing client policy' -Script 'param([hashtable]$P) Set-WUSettings @P -Confirm:$false -Verbose -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null' -Params @{ P = $p } -OnDone { Show-SettingPage; Show-DashboardPage }
    })

    # ---------- catalog ----------
    $catalogSearchScript = @'
param([string]$KB, [string]$Destination)
Get-WUOfflineMSU -KBArticleID $KB -Destination $Destination -WhatIf -WarningAction SilentlyContinue -ErrorAction Stop | ForEach-Object {
    [pscustomobject]@{ Identity = "$($_.Identity)"; Title = "$($_.Title)"; Product = "$($_.Product)"; Classification = "$($_.Classification)"; LastUpdated = "$($_.LastUpdated)"; Size = "$($_.Size)"; SizeBytes = $(try { [long]$_.OriginalSize } catch { 0 }) }
}
'@
    $catalogDownloadScript = @'
param([string]$KB, [string]$Destination, [string[]]$UpdateID, [bool]$SkipSignature)
$p = @{ KBArticleID = $KB; Destination = $Destination; UpdateID = $UpdateID; AcceptAll = $true; Confirm = $false; Verbose = $true; WarningAction = 'SilentlyContinue'; ErrorAction = 'Stop' }
if ($SkipSignature) { $p.SkipSignatureCheck = $true }
Get-WUOfflineMSU @p | ForEach-Object { [pscustomobject]@{ Identity = "$($_.Identity)"; Result = "$($_.Result)"; X = $_.X } }
'@
    $ui.BtnCatalogSearch.Add_Click({
        $kb = $ui.TxtCatalogKB.Text.Trim()
        if (-not $kb) { Show-Message -Text 'Enter a KB number first.' -Icon 'Warning'; return }
        $state.Catalog.Clear()
        $ui.TxtCatalogEmpty.Text = 'Searching the Catalog…'
        $ui.TxtCatalogEmpty.Visibility = 'Visible'
        Invoke-ConsoleTask -Name "Searching Catalog for $kb" -Script $catalogSearchScript -Params @{ KB = $kb; Destination = $ui.TxtCatalogDest.Text } -OnDone {
            param($result, $errors)
            foreach ($r in ($result | Sort-Object Title)) {
                $row = New-Object PSWindowsUpdateNext.Gui.CatalogRow
                $row.Identity = $r.Identity; $row.Title = $r.Title; $row.Product = $r.Product; $row.Classification = $r.Classification
                $row.LastUpdated = $r.LastUpdated; $row.Size = $r.Size; $row.SizeBytes = $r.SizeBytes; $row.Result = 'Listed'
                $state.Catalog.Add($row)
            }
            $ui.TxtCatalogEmpty.Text = if ($errors.Count) { 'Search failed. See the log.' } else { 'No packages found for that KB.' }
            $ui.TxtCatalogEmpty.Visibility = if ($state.Catalog.Count) { 'Collapsed' } else { 'Visible' }
        }
    })
    $ui.TxtCatalogKB.Add_KeyDown({ if ($args[1].Key -eq 'Return') { $ui.BtnCatalogSearch.RaiseEvent((New-Object System.Windows.RoutedEventArgs ([System.Windows.Controls.Button]::ClickEvent))) } })
    $ui.ChkCatalogAll.Add_Click({ $on = [bool]$ui.ChkCatalogAll.IsChecked; foreach ($r in $state.Catalog) { $r.Selected = $on } })
    $ui.BtnCatalogBrowse.Add_Click({
        $dlg = New-Object System.Windows.Forms.FolderBrowserDialog
        $dlg.Description = 'Choose where downloaded packages are saved'
        if ($ui.TxtCatalogDest.Text) { $dlg.SelectedPath = $ui.TxtCatalogDest.Text }
        if ($dlg.ShowDialog() -eq 'OK') { $ui.TxtCatalogDest.Text = $dlg.SelectedPath }
    })
    $ui.BtnCatalogDownload.Add_Click({
        $sel = @($state.Catalog | Where-Object Selected)
        if (-not $sel.Count) { Show-Message -Text 'Select at least one package.' -Icon 'Warning'; return }
        $total = [math]::Round(($sel | Measure-Object SizeBytes -Sum).Sum / 1MB)
        if (-not (Confirm-Action -Text "Download $($sel.Count) package(s), about $total MB, to`n$($ui.TxtCatalogDest.Text)?")) { return }
        foreach ($r in $sel) { $r.Result = 'Accepted' }
        $params = @{ KB = $ui.TxtCatalogKB.Text.Trim(); Destination = $ui.TxtCatalogDest.Text; UpdateID = @($sel.Identity); SkipSignature = [bool]$ui.SwSkipSignature.IsChecked }
        Invoke-ConsoleTask -Name "Downloading $($sel.Count) package(s)" -Script $catalogDownloadScript -Params $params -OnDone {
            param($result)
            foreach ($r in $result) {
                $row = $state.Catalog | Where-Object Identity -eq $r.Identity | Select-Object -First 1
                if ($row) { $row.Result = $r.Result }
            }
            foreach ($row in $state.Catalog) { if ($row.Result -eq 'Accepted') { $row.Result = 'Error' } }
        }
    })

    # ---------- jobs ----------
    function Show-JobPage {
        Invoke-ConsoleTask -Name 'Reading scheduled jobs' -Script 'Get-WUJob -WarningAction SilentlyContinue -ErrorAction Stop | ForEach-Object { [pscustomobject]@{ Name = "$($_.Name)"; State = "$($_.StateName)"; LastRun = "$($_.LastRunTime)"; NextRun = "$($_.NextRunTime)"; Description = "$($_.Definition.RegistrationInfo.Description)" } }' -OnDone {
            param($result, $errors)
            $rows = @($result | ForEach-Object { $j = New-Object PSWindowsUpdateNext.Gui.JobRow; $j.Name = $_.Name; $j.State = $_.State; $j.LastRun = $_.LastRun; $j.NextRun = $_.NextRun; $j.Description = $_.Description; $j })
            $ui.ListJobs.ItemsSource = $rows
            $ui.TxtJobsEmpty.Text = if ($errors.Count) { "Could not read scheduled jobs: $($errors[0].ToString()) Try an elevated session." } else { 'No PSWindowsUpdateNext jobs are registered.' }
            $ui.TxtJobsEmpty.Visibility = if ($rows.Count) { 'Collapsed' } else { 'Visible' }
        }
    }
    $ui.BtnJobsRefresh.Add_Click({ Show-JobPage })
    $ui.BtnJobsClear.Add_Click({
        if (-not (Test-Elevated)) { return }
        Invoke-ConsoleTask -Name 'Clearing expired jobs' -Script 'Get-WUJob -ClearExpired -Verbose -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null' -OnDone { Show-JobPage }
    })
    $ui.CmbJobTrigger.Add_SelectionChanged({ $ui.TxtJobTime.IsEnabled = ($ui.CmbJobTrigger.SelectedIndex -eq 2); if ($ui.CmbJobTrigger.SelectedIndex -eq 2 -and -not $ui.TxtJobTime.Text) { $ui.TxtJobTime.Text = (Get-Date).AddHours(1).ToString('yyyy-MM-dd HH:mm') } })
    $ui.BtnJobCreate.Add_Click({
        if (-not (Test-Elevated)) { return }
        $script = $ui.TxtJobScript.Text.Trim()
        $name = $ui.TxtJobName.Text.Trim()
        if (-not $script -or -not $name) { Show-Message -Text 'A task name and a script are required.' -Icon 'Warning'; return }
        $p = @{ Script = $script; TaskName = $name }
        switch ($ui.CmbJobTrigger.SelectedIndex) {
            0 { $p.RunNow = $true }
            1 { $p.TriggerAtStart = $true }
            2 {
                try { $p.TriggerDate = [datetime]::Parse($ui.TxtJobTime.Text) } catch { Show-Message -Text 'Enter the time as yyyy-MM-dd HH:mm.' -Icon 'Warning'; return }
            }
        }
        if (-not (Confirm-Action -Text "Register task '$name' to run as SYSTEM?`n`n$script")) { return }
        Invoke-ConsoleTask -Name "Creating job $name" -Script 'param([hashtable]$P) Invoke-WUJob @P -Confirm:$false -Force -Verbose -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null' -Params @{ P = $p } -OnDone { Show-JobPage }
    })

    # ---------- log ----------
    function Get-LogText { ($state.Log | ForEach-Object { "$($_.TimeText) [$($_.Level)] $($_.Message)" }) -join "`r`n" }
    $ui.BtnLogClear.Add_Click({ $state.Log.Clear() })
    $ui.BtnLogCopy.Add_Click({ [System.Windows.Clipboard]::SetText((Get-LogText)) })
    $ui.BtnLogSave.Add_Click({
        $dlg = New-Object Microsoft.Win32.SaveFileDialog
        $dlg.Filter = 'Log files (*.log)|*.log|Text files (*.txt)|*.txt'
        $dlg.FileName = "PSWindowsUpdateNext-$(Get-Date -Format yyyyMMdd-HHmm).log"
        if ($dlg.ShowDialog($window)) { [IO.File]::WriteAllText($dlg.FileName, (Get-LogText)) }
    })

    # ---------- lifecycle ----------
    $window.Add_Closed({
        $timer.Stop()
        if ($state.Task) { try { $state.Task.PS.Stop(); $state.Task.PS.Dispose() } catch { Write-Verbose "Background task did not stop cleanly while the window closed: $($_.Exception.Message)" } }
        $runspace.Dispose()
    })
    $window.Add_ContentRendered({ Show-DashboardPage })

    Add-Log 'info' "PSWindowsUpdateNext $moduleVersion console on $env:COMPUTERNAME ($($identity.Name), $(if ($isAdmin) { 'elevated' } else { 'not elevated' }))"

    if ($PassThru) { return $window }
    $window.ShowDialog() | Out-Null
}
