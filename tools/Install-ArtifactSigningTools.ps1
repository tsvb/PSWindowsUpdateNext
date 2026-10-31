#Requires -Version 5.1
<#
.SYNOPSIS
    Downloads SignTool and the Azure Artifact Signing plug-in at pinned versions and returns their paths.
.DESCRIPTION
    Fetches two NuGet packages straight from nuget.org, checks each against a pinned SHA-512 digest
    before extracting it, and returns an object with SignToolPath and DlibPath (both x64). A package
    whose digest does not match is deleted and the script throws.

      - Microsoft.Windows.SDK.BuildTools: signtool.exe
      - Microsoft.ArtifactSigning.Client: Azure.CodeSigning.Dlib.dll, the SignTool plug-in that signs
        through an Artifact Signing account instead of a local private key

    The plug-in needs the .NET 8 runtime. To move to a newer version, update the version and digest
    together; nuget.org shows the SHA-512 of every package version.
.PARAMETER Destination
    Folder to extract into. Defaults to a folder under RUNNER_TEMP, or the user's temp folder.
#>
[CmdletBinding()]
param(
    [string]$Destination
)

$ErrorActionPreference = 'Stop'

$packages = @(
    @{
        Id      = 'microsoft.windows.sdk.buildtools'
        Version = '10.0.28000.2705'
        Sha512  = 'e3b929e3678c6fcac5ddb8c3b991d59331916fd16bef0c85643213dac5abdd2b709d30a80c7222b037b448f586c1eaf0554c3f79040efcec436f46181db7cdbc'
    }
    @{
        Id      = 'microsoft.artifactsigning.client'
        Version = '1.0.146'
        Sha512  = 'fc6d211bf543212ea9b16db76f9b7557f3912209f43d500eb468fdc3e8643e602e238a5e9a97f0956e741361ad1ec727cfdfb9433de16c2e2671bede8f6627d4'
    }
)

if (-not $Destination) {
    $tempRoot = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [System.IO.Path]::GetTempPath() }
    $Destination = Join-Path -Path $tempRoot -ChildPath 'pswunext-signing-tools'
}
$null = New-Item -ItemType Directory -Path $Destination -Force

# Windows PowerShell 5.1 does not offer TLS 1.2 by default.
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor [System.Net.SecurityProtocolType]::Tls12

Add-Type -AssemblyName System.IO.Compression.FileSystem

$folders = @{}
foreach ($package in $packages) {
    $name = "$($package.Id).$($package.Version)"
    $nupkg = Join-Path -Path $Destination -ChildPath "$name.nupkg"
    $folder = Join-Path -Path $Destination -ChildPath $name
    $url = "https://api.nuget.org/v3-flatcontainer/$($package.Id)/$($package.Version)/$name.nupkg"

    Write-Host "Downloading $name"
    Invoke-WebRequest -Uri $url -OutFile $nupkg -UseBasicParsing

    $actual = (Get-FileHash -LiteralPath $nupkg -Algorithm SHA512).Hash
    if ($actual -ne $package.Sha512) {
        Remove-Item -LiteralPath $nupkg -Force
        throw "SHA-512 of $name does not match the pinned digest. Expected $($package.Sha512), got $($actual.ToLowerInvariant())."
    }

    if (Test-Path -LiteralPath $folder) { Remove-Item -LiteralPath $folder -Recurse -Force }
    [System.IO.Compression.ZipFile]::ExtractToDirectory($nupkg, $folder)
    $folders[$package.Id] = $folder
}

$signTool = @(Get-ChildItem -LiteralPath (Join-Path $folders['microsoft.windows.sdk.buildtools'] 'bin') -Recurse -Filter signtool.exe |
        Where-Object { $_.Directory.Name -eq 'x64' })
if ($signTool.Count -ne 1) { throw "Expected one x64 signtool.exe in the build tools package, found $($signTool.Count)." }

$dlib = Join-Path -Path $folders['microsoft.artifactsigning.client'] -ChildPath 'bin\x64\Azure.CodeSigning.Dlib.dll'
if (-not (Test-Path -LiteralPath $dlib -PathType Leaf)) { throw "Azure.CodeSigning.Dlib.dll is missing from the Artifact Signing package ($dlib)." }

[pscustomobject]@{
    SignToolPath = $signTool[0].FullName
    DlibPath     = $dlib
}
