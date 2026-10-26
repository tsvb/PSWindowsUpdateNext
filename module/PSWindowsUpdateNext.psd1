@{
    # The .psm1 picks the right build (net472 for Windows PowerShell, net8.0 for PowerShell 7).
    RootModule = 'PSWindowsUpdateNext.psm1'
    ModuleVersion = '3.2.0'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = '5f0c9b4e-8a6d-4e2b-9c1f-2d7a3e6b8c01'
    Author = 'tsvb'
    CompanyName = 'Community'
    Copyright = '(c) 2011-2023 Michal Gajda. Modifications (c) 2026 tsvb. MIT License.'
    Description = 'Cmdlets to manage the Windows Update client. Community-maintained continuation of PSWindowsUpdate with PowerShell 7 support and security fixes.'
    PowerShellVersion = '5.1'
    DotNetFrameworkVersion = '4.7.2'
    FormatsToProcess = @('PSWindowsUpdateNext.Format.ps1xml')
    FunctionsToExport = @('Show-PSWUConsole')
    CmdletsToExport = @(
        'Add-WUServiceManager',
        'Enable-WURemoting',
        'Get-WindowsUpdate',
        'Get-WUApiVersion',
        'Get-WUHistory',
        'Get-WUInstallerStatus',
        'Get-WUJob',
        'Get-WULastResults',
        'Get-WUOfflineMSU',
        'Get-WURebootStatus',
        'Get-WUServiceManager',
        'Get-WUSettings',
        'Invoke-WUJob',
        'Remove-WindowsUpdate',
        'Remove-WUServiceManager',
        'Reset-WUComponents',
        'Set-PSWUSettings',
        'Set-WUSettings',
        'Update-WUModule'
    )
    VariablesToExport = @()
    AliasesToExport = @(
        'Get-WUList',
        'Get-WUInstall',
        'Install-WindowsUpdate',
        'Download-WindowsUpdate',
        'Hide-WindowsUpdate',
        'UnHide-WindowsUpdate',
        'Show-WindowsUpdate',
        'Uninstall-WindowsUpdate',
        'Clear-WUJob'
    )
    FileList = @(
        'PSWindowsUpdateNext.psd1',
        'PSWindowsUpdateNext.psm1',
        'PSWindowsUpdateNext.Format.ps1xml',
        'PSWindowsUpdateNext.dll-Help.xml',
        'net472\PSWindowsUpdateNext.dll',
        'net8.0-windows\PSWindowsUpdateNext.dll',
        'Gui\Show-PSWUConsole.ps1'
    )
    PrivateData = @{
        PSData = @{
            Tags = @('Microsoft', 'Windows', 'Update', 'WindowsUpdate', 'WUA', 'PSEdition_Desktop', 'PSEdition_Core', 'Windows')
            LicenseUri = 'https://github.com/tsvb/PSWindowsUpdateNext/blob/main/LICENSE'
            ProjectUri = 'https://github.com/tsvb/PSWindowsUpdateNext'
            ReleaseNotes = 'See CHANGELOG.md in the repository.'
        }
    }
}
