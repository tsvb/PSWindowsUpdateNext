#Requires -Version 5.1
<#
.SYNOPSIS
    Fails unless a release tag, the module manifest, the csproj and CHANGELOG.md all agree on the version.
.DESCRIPTION
    Checks that
      - the tag looks like v<version>,
      - ModuleVersion in module\PSWindowsUpdateNext.psd1 equals <version>,
      - <Version> in src\PSWindowsUpdateNext\PSWindowsUpdateNext.csproj equals <version>
        (and AssemblyVersion / FileVersion, when present, start with <version>.),
      - CHANGELOG.md has a non-empty section for <version>.
.PARAMETER Tag
    The tag name, for example v3.1.0 (in GitHub Actions: github.ref_name).
.PARAMETER RepoRoot
    Repository root. Defaults to the parent of this script's folder.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Tag,

    [string]$RepoRoot = (Split-Path -Path $PSScriptRoot -Parent)
)

$ErrorActionPreference = 'Stop'

if (-not ($Tag -match '^v(?<v>\d+\.\d+\.\d+(\.\d+)?)$')) {
    throw "Release tag '$Tag' is not of the form v<major>.<minor>.<patch> (for example v3.1.0)."
}
$version = $Matches['v']

$manifestPath = Join-Path -Path $RepoRoot -ChildPath 'module/PSWindowsUpdateNext.psd1'
$csprojPath = Join-Path -Path $RepoRoot -ChildPath 'src/PSWindowsUpdateNext/PSWindowsUpdateNext.csproj'

$manifestVersion = [string](Import-PowerShellDataFile -LiteralPath $manifestPath).ModuleVersion

[xml]$csproj = Get-Content -LiteralPath $csprojPath -Raw
$props = @($csproj.Project.PropertyGroup)
function Get-CsprojProperty([string]$Name) {
    foreach ($group in $props) {
        $value = $group.$Name
        if ($value) { return ([string]@($value)[0]).Trim() }
    }
    $null
}
$csprojVersion = Get-CsprojProperty 'Version'

$errors = New-Object System.Collections.Generic.List[string]
if ($manifestVersion -ne $version) { $errors.Add("module/PSWindowsUpdateNext.psd1 ModuleVersion is '$manifestVersion'") }
if ($csprojVersion -ne $version) { $errors.Add("PSWindowsUpdateNext.csproj <Version> is '$csprojVersion'") }
foreach ($name in 'AssemblyVersion', 'FileVersion') {
    $value = Get-CsprojProperty $name
    if ($value -and -not $value.StartsWith("$version.")) { $errors.Add("PSWindowsUpdateNext.csproj <$name> is '$value'") }
}

try {
    [void](& (Join-Path -Path $PSScriptRoot -ChildPath 'Get-ReleaseNotes.ps1') -Version $version -ChangelogPath (Join-Path -Path $RepoRoot -ChildPath 'CHANGELOG.md'))
} catch {
    $errors.Add($_.Exception.Message)
}

if ($errors.Count -gt 0) {
    $list = ($errors | ForEach-Object { "  - $_" }) -join "`n"
    throw "Tag $Tag (version $version) does not match the repository:`n$list`nBump the versions / changelog, commit, and re-tag."
}

Write-Host "Version check passed: tag $Tag = ModuleVersion = csproj Version = $version, CHANGELOG section present."
