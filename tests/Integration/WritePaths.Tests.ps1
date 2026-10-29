#Requires -Modules @{ ModuleName = 'Pester'; ModuleVersion = '5.0' }
<#
    Write-path integration tests. They change Windows Update state on the machine they run on:
    hide and unhide an update, download one, register and remove an update service, write and
    remove a policy value, register and clear a Task Scheduler job, and stop a scan in flight.

    They only run when PSWU_WRITE_PATHS=1 and the session is elevated, so the regular test run
    never touches a developer's machine. CI runs them on a disposable hosted runner
    (.github/workflows/integration.yml). Set PSWU_WRITE_PATHS_INSTALL=1 to also install the
    smallest available update, which takes longer and may leave the runner needing a restart.

    The calls mirror what Show-PSWUConsole sends to the module, so passing here means the
    console's buttons drive working code paths.
#>

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments', '', Justification = 'Pester shares variables between BeforeAll/BeforeDiscovery and It blocks.')]
param()

BeforeDiscovery {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $elevated = ([Security.Principal.WindowsPrincipal]$identity).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    $enabled = ($env:PSWU_WRITE_PATHS -eq '1') -and $elevated
    $installEnabled = $enabled -and ($env:PSWU_WRITE_PATHS_INSTALL -eq '1')
    if (-not $enabled) {
        Write-Host 'Write-path tests are off. Set PSWU_WRITE_PATHS=1 in an elevated session on a disposable machine to run them.'
    }
}

BeforeAll {
    $script:modulePath = if ($env:PSWU_MODULE_PATH) { $env:PSWU_MODULE_PATH } else { Join-Path $PSScriptRoot '..\..\out\PSWindowsUpdateNext' }
    $script:manifest = Join-Path $script:modulePath 'PSWindowsUpdateNext.psd1'
    $script:MicrosoftUpdateServiceId = '7971f918-a847-4430-9279-4a52d1efe18d'

    # Get-WindowsUpdate in list mode can hand back one collection object in a pipeline; flatten the same
    # way the console does so every test sees one object per update.
    function Get-UpdateList {
        param([hashtable]$Arguments)
        $raw = @(Get-WindowsUpdate @Arguments -WarningAction SilentlyContinue -ErrorAction Stop)
        foreach ($o in $raw) {
            if ($o -is [System.Collections.ICollection] -and $o -isnot [string]) { foreach ($i in $o) { $i } } else { $o }
        }
    }

    function Wait-Until {
        param([scriptblock]$Condition, [int]$TimeoutSeconds = 120, [int]$IntervalSeconds = 3)
        $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
        do {
            if (& $Condition) { return $true }
            Start-Sleep -Seconds $IntervalSeconds
        } while ((Get-Date) -lt $deadline)
        return $false
    }
}

Describe 'Write paths' -Tag 'WritePath' -Skip:(-not $enabled) {
    BeforeAll {
        Import-Module $script:manifest -Force -ErrorAction Stop
        # Microsoft Update exposes more than the default service on a fresh image, so the scan has something to work with.
        Add-WUServiceManager -MicrosoftUpdate -Confirm:$false -WarningAction SilentlyContinue -ErrorAction SilentlyContinue | Out-Null
        $script:scan = @(Get-UpdateList @{ MicrosoftUpdate = $true; Verbose = $false })
        Write-Host "Scan found $($script:scan.Count) updates"
        $script:scan | ForEach-Object { Write-Host ("  {0,-12} {1,10:N0}  {2}" -f $_.KB, [decimal]$_.MaxDownloadSize, $_.Title) }
        $script:smallest = $script:scan |
            Where-Object { -not $_.IsInstalled -and -not $_.IsHidden -and [decimal]$_.MaxDownloadSize -gt 0 } |
            Sort-Object { [decimal]$_.MaxDownloadSize } | Select-Object -First 1
        $script:target = if ($script:smallest) { "$($script:smallest.Identity.UpdateID)" } else { $null }
        if ($script:target) { Write-Host "Target update: $($script:smallest.Title) ($script:target)" }
    }

    Context 'Scan' {
        It 'returns one object per update' {
            $script:scan.Count | Should -BeGreaterThan 0
            foreach ($u in $script:scan) { $u.Identity.UpdateID | Should -Match '^[0-9a-f-]{36}$' }
        }
    }

    Context 'Hide and unhide' {
        It 'has an update to work with' { $script:target | Should -Not -BeNullOrEmpty }

        It 'hides the update' {
            Get-UpdateList @{ MicrosoftUpdate = $true; UpdateID = $script:target; Hide = $true; AcceptAll = $true; Confirm = $false } | Out-Null
            $hidden = @(Get-UpdateList @{ MicrosoftUpdate = $true; IsHidden = $true })
            @($hidden | Where-Object { "$($_.Identity.UpdateID)" -eq $script:target }).Count | Should -Be 1
        }

        It 'does not list the hidden update in a normal scan' {
            $visible = @(Get-UpdateList @{ MicrosoftUpdate = $true })
            @($visible | Where-Object { "$($_.Identity.UpdateID)" -eq $script:target }).Count | Should -Be 0
        }

        It 'unhides the update' {
            Get-UpdateList @{ MicrosoftUpdate = $true; IsHidden = $true; UpdateID = $script:target; Hide = $false; AcceptAll = $true; Confirm = $false } | Out-Null
            $visible = @(Get-UpdateList @{ MicrosoftUpdate = $true })
            @($visible | Where-Object { "$($_.Identity.UpdateID)" -eq $script:target }).Count | Should -Be 1
        }
    }

    Context 'Download' {
        It 'downloads the update and reports it as downloaded' {
            $script:target | Should -Not -BeNullOrEmpty
            $result = @(Get-UpdateList @{ MicrosoftUpdate = $true; UpdateID = $script:target; Download = $true; AcceptAll = $true; IgnoreReboot = $true; Confirm = $false })
            $result.Count | Should -BeGreaterThan 0
            $after = @(Get-UpdateList @{ MicrosoftUpdate = $true }) | Where-Object { "$($_.Identity.UpdateID)" -eq $script:target }
            [bool]$after.IsDownloaded | Should -BeTrue
        }
    }

    Context 'Install' -Skip:(-not $installEnabled) {
        It 'installs the update' {
            $script:target | Should -Not -BeNullOrEmpty
            $result = @(Get-UpdateList @{ MicrosoftUpdate = $true; UpdateID = $script:target; Install = $true; AcceptAll = $true; IgnoreReboot = $true; Confirm = $false })
            $result | ForEach-Object { Write-Host "  $($_.Status)  $($_.KB)  $($_.Title)  ->  $($_.Result)" }
            @($result | Where-Object { $_.Result -in 'Installed', 'InstallationSucceeded', 'Succeeded' }).Count | Should -BeGreaterThan 0
        }
    }

    Context 'Update services' {
        It 'lists Microsoft Update as registered' {
            $services = @(Get-WUServiceManager -WarningAction SilentlyContinue -ErrorAction Stop)
            $services.ServiceID | Should -Contain $script:MicrosoftUpdateServiceId
        }

        It 'removes and re-registers Microsoft Update' {
            Remove-WUServiceManager -ServiceID $script:MicrosoftUpdateServiceId -Confirm:$false -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null
            $services = @(Get-WUServiceManager -WarningAction SilentlyContinue -ErrorAction Stop)
            $services.ServiceID | Should -Not -Contain $script:MicrosoftUpdateServiceId
            Add-WUServiceManager -MicrosoftUpdate -Confirm:$false -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null
            $services = @(Get-WUServiceManager -WarningAction SilentlyContinue -ErrorAction Stop)
            $services.ServiceID | Should -Contain $script:MicrosoftUpdateServiceId
        }
    }

    Context 'Client policy' {
        AfterAll {
            Remove-ItemProperty -Path 'HKLM:\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate' -Name 'DeferQualityUpdatesPeriodInDays' -ErrorAction SilentlyContinue
            Remove-ItemProperty -Path 'HKLM:\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate' -Name 'DeferQualityUpdates' -ErrorAction SilentlyContinue
        }

        It 'writes a deferral and reads it back' {
            Set-WUSettings -DeferQualityUpdatesPeriodInDays 1 -Confirm:$false -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null
            $settings = Get-WUSettings -WarningAction SilentlyContinue -ErrorAction Stop
            "$($settings.DeferQualityUpdatesPeriodInDays)" | Should -Be '1'
        }

        It 'changes the deferral in place' {
            Set-WUSettings -DeferQualityUpdatesPeriodInDays 2 -Confirm:$false -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null
            $settings = Get-WUSettings -WarningAction SilentlyContinue -ErrorAction Stop
            "$($settings.DeferQualityUpdatesPeriodInDays)" | Should -Be '2'
        }
    }

    Context 'Scheduled jobs' {
        BeforeAll {
            $script:jobName = 'PSWUIntegration'
            $script:jobMarker = Join-Path $env:SystemRoot 'Temp\PSWUIntegration.txt'
            Remove-Item $script:jobMarker -Force -ErrorAction SilentlyContinue
        }
        AfterAll {
            $service = New-Object -ComObject Schedule.Service
            $service.Connect()
            try { $service.GetFolder('\').DeleteTask($script:jobName, 0) } catch { Write-Verbose "Job already gone: $($_.Exception.Message)" }
            Remove-Item $script:jobMarker -Force -ErrorAction SilentlyContinue
        }

        It 'registers a job that runs as SYSTEM right away' {
            $marker = $script:jobMarker.Replace('\', '\\')
            $job = "[Environment]::UserName | Out-File -FilePath '$($script:jobMarker)' -Encoding ascii"
            Invoke-WUJob -TaskName $script:jobName -Script $job -RunNow -Force -EndBoundary (Get-Date).AddMinutes(2) -Confirm:$false -WarningAction SilentlyContinue -ErrorAction Stop | Out-Null
            (Wait-Until -Condition { Test-Path $script:jobMarker } -TimeoutSeconds 120) | Should -BeTrue -Because "the task should write $marker"
            # SYSTEM reports its name as "SYSTEM" or as the computer account "<NAME>$".
            (Get-Content $script:jobMarker -Raw).Trim() | Should -Match '^(SYSTEM|.+\$)$' -Because 'the job runs under the SYSTEM account'
        }

        It 'lists the job' {
            $jobs = @(Get-WUJob -WarningAction SilentlyContinue -ErrorAction Stop)
            $jobs.Name | Should -Contain $script:jobName
        }

        It 'clears the job once it has expired' {
            $cleared = Wait-Until -TimeoutSeconds 180 -IntervalSeconds 10 -Condition {
                Get-WUJob -ClearExpired -WarningAction SilentlyContinue -ErrorAction SilentlyContinue | Out-Null
                @(Get-WUJob -WarningAction SilentlyContinue -ErrorAction SilentlyContinue).Name -notcontains $script:jobName
            }
            $cleared | Should -BeTrue
        }
    }

    Context 'Cancel a running scan' {
        It 'stops a scan in a background runspace the way the console does' {
            $iss = [System.Management.Automation.Runspaces.InitialSessionState]::CreateDefault()
            $iss.ImportPSModule(@($script:manifest))
            $rs = [System.Management.Automation.Runspaces.RunspaceFactory]::CreateRunspace($iss)
            $rs.Open()
            $ps = [PowerShell]::Create()
            $ps.Runspace = $rs
            [void]$ps.AddScript('Get-WindowsUpdate -MicrosoftUpdate -WarningAction SilentlyContinue')
            $handle = $ps.BeginInvoke()
            Start-Sleep -Seconds 3
            $sw = [Diagnostics.Stopwatch]::StartNew()
            [void]$ps.BeginStop($null, $null)
            $stopped = $handle.AsyncWaitHandle.WaitOne(60000)
            $sw.Stop()
            Write-Host "Stop completed in $([int]$sw.Elapsed.TotalSeconds)s, state $($ps.InvocationStateInfo.State)"
            $stopped | Should -BeTrue
            $ps.InvocationStateInfo.State | Should -BeIn 'Stopped', 'Completed'
            $ps.Dispose(); $rs.Dispose()
        }
    }
}
