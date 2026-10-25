#Requires -Version 5.1
<#
.SYNOPSIS
    Builds PSWindowsUpdateNext for Windows PowerShell 5.1 and PowerShell 7 and assembles the module folder.
.PARAMETER Configuration
    Release (default) or Debug.
.PARAMETER OutputPath
    Where the finished module folder is written. Default: .\out\PSWindowsUpdateNext
.PARAMETER Install
    Also copy the finished module into the current user's module path.
#>
[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',
    [string]$OutputPath = (Join-Path $PSScriptRoot 'out\PSWindowsUpdateNext'),
    [switch]$Install
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'src\PSWindowsUpdateNext\PSWindowsUpdateNext.csproj'
$moduleSrc = Join-Path $PSScriptRoot 'module'

Write-Host "Building $project ($Configuration)..."
& dotnet build $project -c $Configuration --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit code $LASTEXITCODE" }

if (Test-Path $OutputPath) { Remove-Item $OutputPath -Recurse -Force }
New-Item -ItemType Directory -Path $OutputPath | Out-Null

Copy-Item -Path (Join-Path $moduleSrc '*') -Destination $OutputPath -Recurse
foreach ($tfm in 'net472', 'net8.0-windows') {
    $binDir = Join-Path $PSScriptRoot "src\PSWindowsUpdateNext\bin\$Configuration\$tfm"
    $dest = Join-Path $OutputPath $tfm
    New-Item -ItemType Directory -Path $dest | Out-Null
    Copy-Item -Path (Join-Path $binDir 'PSWindowsUpdateNext.dll') -Destination $dest
    $pdb = Join-Path $binDir 'PSWindowsUpdateNext.pdb'
    if ($Configuration -eq 'Debug' -and (Test-Path $pdb)) { Copy-Item $pdb $dest }
}

Write-Host "Module assembled at $OutputPath"

if ($Install) {
    $userModules = if ($PSEdition -eq 'Core') {
        Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'PowerShell\Modules'
    } else {
        Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'WindowsPowerShell\Modules'
    }
    $version = (Import-PowerShellDataFile (Join-Path $moduleSrc 'PSWindowsUpdateNext.psd1')).ModuleVersion
    $target = Join-Path $userModules "PSWindowsUpdateNext\$version"
    if (Test-Path $target) { Remove-Item $target -Recurse -Force }
    New-Item -ItemType Directory -Path $target -Force | Out-Null
    Copy-Item -Path (Join-Path $OutputPath '*') -Destination $target -Recurse
    Write-Host "Installed to $target"
}
