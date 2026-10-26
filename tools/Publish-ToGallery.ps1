#Requires -Version 5.1
<#
.SYNOPSIS
    Validates the module manifest and publishes the module folder to the PowerShell Gallery.
.DESCRIPTION
    Runs Test-ModuleManifest, then Publish-PSResource (PSResourceGet). Publish-Module (PowerShellGet) is
    used only when Publish-PSResource is not installed. The API key is read from $env:PSGALLERY_API_KEY
    and is never written to the log.
.PARAMETER ModulePath
    The module folder; its name must be PSWindowsUpdateNext and it must contain PSWindowsUpdateNext.psd1.
.PARAMETER ApiKey
    Gallery API key. Defaults to $env:PSGALLERY_API_KEY.
.PARAMETER ManifestOnly
    Only run Test-ModuleManifest; do not publish.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ModulePath,

    [string]$ApiKey = $env:PSGALLERY_API_KEY,

    [switch]$ManifestOnly
)

$ErrorActionPreference = 'Stop'

$ModulePath = (Resolve-Path -LiteralPath $ModulePath).ProviderPath.TrimEnd('\', '/')
$manifest = Join-Path -Path $ModulePath -ChildPath 'PSWindowsUpdateNext.psd1'
if (-not (Test-Path -LiteralPath $manifest -PathType Leaf)) { throw "Manifest not found: $manifest" }
if ((Split-Path -Path $ModulePath -Leaf) -ne 'PSWindowsUpdateNext') {
    throw "The module folder must be named PSWindowsUpdateNext (got '$(Split-Path -Path $ModulePath -Leaf)')."
}

Write-Host "Test-ModuleManifest $manifest"
$info = Test-ModuleManifest -Path $manifest -ErrorAction Stop
Write-Host "Manifest OK: $($info.Name) $($info.Version)"

if ($ManifestOnly) { return }
if ([string]::IsNullOrWhiteSpace($ApiKey)) { throw 'No API key. Set PSGALLERY_API_KEY.' }
if ($env:GITHUB_ACTIONS -eq 'true') { Write-Host "::add-mask::$ApiKey" }

if (Get-Command -Name Publish-PSResource -ErrorAction SilentlyContinue) {
    Write-Host "Publishing $($info.Name) $($info.Version) with Publish-PSResource..."
    Publish-PSResource -Path $ModulePath -Repository PSGallery -ApiKey $ApiKey -Verbose
} else {
    Write-Warning 'Publish-PSResource is not available; falling back to Publish-Module.'
    Publish-Module -Path $ModulePath -Repository PSGallery -NuGetApiKey $ApiKey -Verbose
}

Write-Host "Published $($info.Name) $($info.Version) to the PowerShell Gallery."
