#Requires -Version 5.1
<#
.SYNOPSIS
    Fails unless the module manifest and the csproj agree on the version. With -Tag, also checks the tag and CHANGELOG.md.
.DESCRIPTION
    Always checks that
      - ModuleVersion in module\PSWindowsUpdateNext.psd1 is <major>.<minor>.<patch>[.<revision>],
      - <Version> in src\PSWindowsUpdateNext\PSWindowsUpdateNext.csproj equals ModuleVersion
        (and AssemblyVersion / FileVersion, when present, start with ModuleVersion + ".").
    When -Tag is given, also checks that
      - the tag looks like v<version>,
      - ModuleVersion equals <version>,
      - CHANGELOG.md has a non-empty section for <version>.
.PARAMETER Tag
    Optional. The tag name, for example v3.1.0 (in GitHub Actions: github.ref_name). When omitted,
    the version is ModuleVersion and CHANGELOG.md is not checked, because unreleased work is allowed.
.PARAMETER RepoRoot
    Repository root. Defaults to the parent of this script's folder.
#>
[CmdletBinding()]
param(
    [string]$Tag,

    [string]$RepoRoot = (Split-Path -Path $PSScriptRoot -Parent)
)

$ErrorActionPreference = 'Stop'
# Use PSBoundParameters so that an empty -Tag is still checked as a tag, not treated as omitted.
$tagGiven = $PSBoundParameters.ContainsKey('Tag')

if ($tagGiven) {
    if (-not ($Tag -match '^v(?<v>\d+\.\d+\.\d+(\.\d+)?)$')) {
        throw "Release tag '$Tag' is not of the form v<major>.<minor>.<patch> (for example v3.1.0)."
    }
    $version = $Matches['v']
}

$manifestPath = Join-Path -Path $RepoRoot -ChildPath 'module/PSWindowsUpdateNext.psd1'
$csprojPath = Join-Path -Path $RepoRoot -ChildPath 'src/PSWindowsUpdateNext/PSWindowsUpdateNext.csproj'

$manifestVersion = [string](Import-PowerShellDataFile -LiteralPath $manifestPath).ModuleVersion

if (-not $tagGiven) {
    $version = $manifestVersion
    if ($version -notmatch '^\d+\.\d+\.\d+(\.\d+)?$') {
        throw "ModuleVersion '$version' in module/PSWindowsUpdateNext.psd1 is not of the form <major>.<minor>.<patch>."
    }
}

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

if ($tagGiven) {
    try {
        [void](& (Join-Path -Path $PSScriptRoot -ChildPath 'Get-ReleaseNotes.ps1') -Version $version -ChangelogPath (Join-Path -Path $RepoRoot -ChildPath 'CHANGELOG.md'))
    } catch {
        $errors.Add($_.Exception.Message)
    }
}

if ($errors.Count -gt 0) {
    $list = ($errors | ForEach-Object { "  - $_" }) -join "`n"
    if ($tagGiven) {
        throw "Tag $Tag (version $version) does not match the repository:`n$list`nBump the versions / changelog, commit, and re-tag."
    }
    throw "Version numbers in the repository do not agree (ModuleVersion $version):`n$list`nSet the same version in module/PSWindowsUpdateNext.psd1 and the csproj."
}

if ($tagGiven) {
    Write-Host "Version check passed: tag $Tag = ModuleVersion = csproj Version = $version, CHANGELOG section present."
}
else {
    Write-Host "Version check passed: ModuleVersion = csproj Version = $version (no tag given, so CHANGELOG.md was not checked)."
}
