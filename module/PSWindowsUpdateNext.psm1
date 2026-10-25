# PSWindowsUpdateNext loader.
# Picks the binary built for the running PowerShell edition and re-exports its cmdlets.

$tfm = if ($PSEdition -eq 'Core') { 'net8.0-windows' } else { 'net472' }
$dll = Join-Path -Path $PSScriptRoot -ChildPath "$tfm\PSWindowsUpdateNext.dll"

if (-not (Test-Path -LiteralPath $dll)) {
    throw "PSWindowsUpdateNext: binary for $tfm not found at '$dll'. Run build.ps1 or reinstall the module."
}

if ($PSEdition -eq 'Core' -and -not $IsWindows) {
    throw 'PSWindowsUpdateNext only works on Windows: it drives the Windows Update Agent COM API.'
}

Import-Module -Name $dll -Scope Local -ErrorAction Stop

# Desktop console (WPF). Loaded lazily inside the function so headless hosts pay nothing.
. (Join-Path -Path $PSScriptRoot -ChildPath 'Gui\Show-PSWUConsole.ps1')

# Aliases kept from the original PSWindowsUpdate module.
New-Alias -Name Get-WUList -Value Get-WindowsUpdate
New-Alias -Name Get-WUInstall -Value Get-WindowsUpdate
New-Alias -Name Install-WindowsUpdate -Value Get-WindowsUpdate
New-Alias -Name Download-WindowsUpdate -Value Get-WindowsUpdate
New-Alias -Name Hide-WindowsUpdate -Value Get-WindowsUpdate
New-Alias -Name UnHide-WindowsUpdate -Value Get-WindowsUpdate
New-Alias -Name Show-WindowsUpdate -Value Get-WindowsUpdate
New-Alias -Name Uninstall-WindowsUpdate -Value Remove-WindowsUpdate
New-Alias -Name Clear-WUJob -Value Get-WUJob

Export-ModuleMember -Cmdlet * -Alias * -Function Show-PSWUConsole
