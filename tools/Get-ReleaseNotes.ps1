#Requires -Version 5.1
<#
.SYNOPSIS
    Extracts one version's section from CHANGELOG.md.
.DESCRIPTION
    Finds the heading for the version (for example "## 3.1.0 (2026-10-25)"), returns everything up to the
    next "## " heading, and optionally writes it to a file. Fails if the section is missing or empty.
.PARAMETER Version
    Version without a leading "v", for example 3.1.0.
.PARAMETER ChangelogPath
    Defaults to CHANGELOG.md in the repository root.
.PARAMETER OutputPath
    If given, the notes are also written to this file (UTF-8, no BOM, LF line endings).
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Version,

    [string]$ChangelogPath = (Join-Path -Path (Split-Path -Path $PSScriptRoot -Parent) -ChildPath 'CHANGELOG.md'),

    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $ChangelogPath -PathType Leaf)) { throw "Changelog not found: $ChangelogPath" }

$lines = @(Get-Content -LiteralPath $ChangelogPath -Encoding UTF8)
$heading = '^##\s+\[?v?' + [regex]::Escape($Version) + '\]?(\s|$)'

$start = -1
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match $heading) { $start = $i; break }
}
if ($start -lt 0) {
    throw "CHANGELOG.md has no section for version $Version. Add a heading like '## $Version (yyyy-MM-dd)' before tagging."
}

$end = $lines.Count
for ($i = $start + 1; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^##\s') { $end = $i; break }
}

$body = if ($end -gt $start + 1) { $lines[($start + 1)..($end - 1)] } else { @() }
$notes = (($body -join "`n").Trim())
if (-not $notes) { throw "The CHANGELOG.md section for $Version is empty." }

if ($OutputPath) {
    $full = [System.IO.Path]::GetFullPath((Join-Path -Path (Get-Location).ProviderPath -ChildPath $OutputPath))
    [System.IO.File]::WriteAllText($full, $notes + "`n", (New-Object System.Text.UTF8Encoding($false)))
}

$notes
