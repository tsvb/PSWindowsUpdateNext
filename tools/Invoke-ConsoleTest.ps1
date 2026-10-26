#Requires -Version 5.1
<#
.SYNOPSIS
    Runs the desktop console tests and fails unless the window was actually built.
.DESCRIPTION
    Show-PSWUConsole needs a single-threaded apartment (STA), and the Pester test for it skips itself in
    any other host. CI starts this script with `pwsh -STA` or `powershell -STA`, so the test cannot be
    skipped quietly. The script fails when it is not running in an STA, or when the test did not pass.

    Set PSWU_MODULE_PATH to test a module folder other than out\PSWindowsUpdateNext.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$apartment = [System.Threading.Thread]::CurrentThread.GetApartmentState()
if ($apartment -ne 'STA') {
    throw "This host is in the $apartment apartment. Start it with -STA, for example: pwsh -NoProfile -STA -File $PSCommandPath"
}

$cfg = New-PesterConfiguration
$cfg.Run.Path = (Join-Path -Path $PSScriptRoot -ChildPath '..\tests')
$cfg.Run.PassThru = $true
$cfg.Filter.Tag = 'Console'
$cfg.Output.Verbosity = 'Detailed'
$result = Invoke-Pester -Configuration $cfg

$window = @($result.Tests | Where-Object { $_.Name -eq 'builds the window from its XAML and theme' })
$state = if ($window.Count -eq 1) { [string]$window[0].Result } else { 'not found' }
if ($state -ne 'Passed') {
    Write-Error "The test 'builds the window from its XAML and theme' did not pass (result: $state)."
    exit 1
}
if ($result.FailedCount -gt 0) { exit 1 }
exit 0
