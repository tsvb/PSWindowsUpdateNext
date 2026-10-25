#Requires -Modules @{ ModuleName = 'Pester'; ModuleVersion = '5.0' }
<#
    Smoke tests for the assembled module. Run build.ps1 first, or set $env:PSWU_MODULE_PATH
    to the folder that holds PSWindowsUpdateNext.psd1.
#>

BeforeDiscovery {
    # Discovery-time values feed -ForEach; run-time blocks recompute the path (Pester 5 keeps the scopes apart).
    $modulePath = if ($env:PSWU_MODULE_PATH) { $env:PSWU_MODULE_PATH } else { Join-Path $PSScriptRoot '..\out\PSWindowsUpdateNext' }
    $expectedCmdlets = (Import-PowerShellDataFile (Join-Path $modulePath 'PSWindowsUpdateNext.psd1')).CmdletsToExport
}

BeforeAll {
    $script:modulePath = if ($env:PSWU_MODULE_PATH) { $env:PSWU_MODULE_PATH } else { Join-Path $PSScriptRoot '..\out\PSWindowsUpdateNext' }
    $script:manifest = Join-Path $script:modulePath 'PSWindowsUpdateNext.psd1'
}

Describe 'PSWindowsUpdateNext module' {
    BeforeAll {
        Import-Module $script:manifest -Force -ErrorAction Stop
    }

    It 'imports on this edition (<_>)' -ForEach @($PSEdition) {
        Get-Module PSWindowsUpdateNext | Should -Not -BeNullOrEmpty
    }

    It 'loads the binary that matches the edition' {
        $expected = if ($PSEdition -eq 'Core') { 'net8.0-windows' } else { 'net472' }
        $dll = (Get-Command Get-WUApiVersion).DLL
        Split-Path (Split-Path $dll -Parent) -Leaf | Should -Be $expected
    }

    It 'exports cmdlet <_>' -ForEach $expectedCmdlets {
        Get-Command -Name $_ -Module PSWindowsUpdateNext -CommandType Cmdlet | Should -Not -BeNullOrEmpty
    }

    It 'exports the legacy aliases' {
        foreach ($alias in 'Install-WindowsUpdate', 'Download-WindowsUpdate', 'Hide-WindowsUpdate', 'Uninstall-WindowsUpdate', 'Get-WUList', 'Clear-WUJob') {
            (Get-Alias $alias).ResolvedCommand.Module.Name | Should -Be 'PSWindowsUpdateNext'
        }
    }

    It 'has help for cmdlet <_>' -ForEach $expectedCmdlets {
        (Get-Help $_).Synopsis | Should -Not -Match '^\s*$'
    }

    It 'does not export the hidden diagnostic cmdlets' {
        Get-Command -Name Get-WUTest, Get-WindowsUpdate_v2 -ErrorAction SilentlyContinue | Should -BeNullOrEmpty
    }

    It 'exports the Show-PSWUConsole function' {
        (Get-Command Show-PSWUConsole -Module PSWindowsUpdateNext).CommandType | Should -Be 'Function'
    }
}

Describe 'Desktop console' {
    BeforeAll {
        Import-Module $script:manifest -Force -ErrorAction Stop
        $script:sta = [System.Threading.Thread]::CurrentThread.GetApartmentState() -eq 'STA'
    }

    It 'builds the window from its XAML and theme' -Skip:(-not $script:sta) {
        $window = Show-PSWUConsole -PassThru
        try {
            $window | Should -BeOfType System.Windows.Window
            $window.Title | Should -Be 'PSWindowsUpdateNext Console'
            foreach ($name in 'NavDashboard', 'NavUpdates', 'NavHistory', 'NavServices', 'NavSettings', 'NavCatalog', 'NavJobs', 'NavLog', 'ListUpdates', 'BtnScan') {
                $window.FindName($name) | Should -Not -BeNullOrEmpty -Because "$name is wired by the runtime"
            }
        } finally {
            $window.Close()
        }
    }
}

Describe 'Windows Update Agent access' {
    BeforeAll {
        Import-Module $script:manifest -Force -ErrorAction Stop
    }

    It 'Get-WUApiVersion reports the agent version' {
        $info = Get-WUApiVersion -WarningAction SilentlyContinue
        $info.ApiVersion | Should -Not -BeNullOrEmpty
        $info.WuapiDllVersion | Should -Not -BeNullOrEmpty
    }

    It 'Get-WUHistory returns history entries' {
        $history = Get-WUHistory -Last 3 -WarningAction SilentlyContinue
        @($history).Count | Should -BeGreaterThan 0
    }

    It 'Get-WUServiceManager lists the Windows Update service' {
        $services = Get-WUServiceManager -WarningAction SilentlyContinue
        $services.ServiceID | Should -Contain '9482f4b4-e343-43b6-b170-9a65bc822c77'
    }
}

Describe 'Microsoft Update Catalog' -Tag 'Network' {
    BeforeAll {
        Import-Module $script:manifest -Force -ErrorAction Stop
    }

    It 'parses search results without downloading (-WhatIf)' {
        $rows = Get-WUOfflineMSU -KBArticleID 4023057 -Destination $TestDrive -WhatIf -WarningAction SilentlyContinue
        @($rows).Count | Should -BeGreaterThan 0
        $rows[0].Identity | Should -Match '^[0-9a-f-]{36}$'
        $rows[0].Title | Should -Match 'KB4023057'
        $rows[0].OriginalSize | Should -Match '^\d+$'
    }
}
