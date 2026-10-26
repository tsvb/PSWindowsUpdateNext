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

Describe 'Desktop console' -Tag 'Console' {
    BeforeDiscovery {
        # Pester 5 evaluates -Skip while it discovers tests, before any BeforeAll runs, so the apartment
        # check has to happen here. WPF needs a single-threaded apartment to build a window.
        $sta = [System.Threading.Thread]::CurrentThread.GetApartmentState() -eq 'STA'
    }

    BeforeAll {
        Import-Module $script:manifest -Force -ErrorAction Stop
    }

    It 'embeds its XAML, theme and row types in the module assembly' {
        $asm = (Get-Command Get-WUApiVersion).ImplementingType.Assembly
        $asm.GetManifestResourceNames() | Should -Contain 'PSWindowsUpdateNext.Gui.MainWindow.xaml'
        $asm.GetManifestResourceNames() | Should -Contain 'PSWindowsUpdateNext.Gui.Theme.xaml'
        $asm.GetType('PSWindowsUpdateNext.Gui.UpdateRow') | Should -Not -BeNullOrEmpty
    }

    It 'ships no loose code or XAML next to the console script' {
        $names = @(Get-ChildItem -LiteralPath (Join-Path $script:modulePath 'Gui') -File | ForEach-Object { $_.Name })
        $names | Should -Be @('Show-PSWUConsole.ps1')
    }

    It 'builds the window from its XAML and theme' -Skip:(-not $sta) {
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

Describe 'Authenticode verification of downloaded files' {
    BeforeDiscovery {
        # Both files ship with an embedded Authenticode signature on GitHub's windows-latest image.
        # pwsh.exe is signed by Microsoft; git.exe is signed by the Git for Windows maintainers.
        $msSigned = Join-Path $env:ProgramFiles 'PowerShell\7\pwsh.exe'
        $thirdParty = Join-Path $env:ProgramFiles 'Git\cmd\git.exe'
        # Only run a signed-file case when Windows itself reports the embedded signature as valid, so a
        # runner image that ships these binaries unsigned skips instead of failing for the wrong reason.
        $script:hasMsSigned = (Test-Path -LiteralPath $msSigned) -and (Get-AuthenticodeSignature -LiteralPath $msSigned).Status -eq 'Valid'
        $script:hasThirdParty = (Test-Path -LiteralPath $thirdParty) -and (Get-AuthenticodeSignature -LiteralPath $thirdParty).Status -eq 'Valid'
    }

    BeforeAll {
        Import-Module $script:manifest -Force -ErrorAction Stop
        $script:msSigned = Join-Path $env:ProgramFiles 'PowerShell\7\pwsh.exe'
        $script:thirdParty = Join-Path $env:ProgramFiles 'Git\cmd\git.exe'
        # Accepted outcomes for a genuine Microsoft signature: revocation may be unreachable from a runner.
        $script:accepted = 'Valid', 'RevocationUnknown'
        # Microsoft root CAs that WUFileVerifier accepts (SHA-1 thumbprints).
        $script:microsoftRoots = @(
            '3B1EFD3A66EA28B16697394703A72CA340A05BD5', # Root CA 2010
            '8F43288AD272F3103B6FB1428485EA3014C0BCFE', # Root CA 2011
            '06F1AA330B927B753A40E68CDF22E34BCBEF3352', # ECC Product Root 2018
            'CDD4EEAE6000AC7F40C3802C171E30148030C072', # Root CA (2001)
            'A43489159A520F0D93D032CCAF37E7FE20A8B419'  # Root Authority (1997)
        )
    }

    It 'accepts a Microsoft-signed file' -Skip:(-not $script:hasMsSigned) {
        $result = [PSWindowsUpdateNext.WUFileVerifier]::VerifyAuthenticode($script:msSigned)
        $result.Status.ToString() | Should -BeIn $script:accepted -Because $result.Message
        $result.RootThumbprint | Should -BeIn $script:microsoftRoots
    }

    It 'rejects a validly signed file whose root is not Microsoft' -Skip:(-not $script:hasThirdParty) {
        $result = [PSWindowsUpdateNext.WUFileVerifier]::VerifyAuthenticode($script:thirdParty)
        $result.Status.ToString() | Should -Be 'NotMicrosoftRoot' -Because $result.Message
        $result.IsValid | Should -BeFalse
    }

    It 'rejects an unsigned file' {
        $path = Join-Path $TestDrive 'unsigned.ps1'
        Set-Content -LiteralPath $path -Value 'Write-Output "unsigned"'
        $result = [PSWindowsUpdateNext.WUFileVerifier]::VerifyAuthenticode($path)
        $result.Status.ToString() | Should -Be 'NotSigned' -Because $result.Message
        $result.IsValid | Should -BeFalse
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
