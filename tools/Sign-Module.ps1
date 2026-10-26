#Requires -Version 5.1
<#
.SYNOPSIS
    Authenticode-signs every signable file in an assembled PSWindowsUpdateNext module folder, then verifies them.
.DESCRIPTION
    Signs, with SHA-256 and an RFC 3161 timestamp:
      - net472\PSWindowsUpdateNext.dll and net8.0-windows\PSWindowsUpdateNext.dll
      - *.psm1, *.psd1, *.ps1xml and *.ps1 (this includes Gui\*.ps1)

    The certificate comes from a base64-encoded PFX (normally the SIGNING_CERT_PFX_BASE64 and
    SIGNING_CERT_PASSWORD secrets, passed in as environment variables). The PFX is written to a
    temporary file and imported into the current user's store; both are always removed again.

    After signing, every signable file is checked with Get-AuthenticodeSignature. The script
    throws if any status is not 'Valid', if a file has no signer, or if a signature has no
    timestamp. Use -VerifyOnly to run just that check (for example under Windows PowerShell 5.1
    after signing under PowerShell 7).

    Files that cannot be Authenticode-signed (.xaml, .cs, *-Help.xml) are left alone.
.PARAMETER ModulePath
    The module folder, for example .\PSWindowsUpdateNext.
.PARAMETER PfxBase64
    Base64 text of the PFX file. Defaults to $env:SIGNING_CERT_PFX_BASE64.
.PARAMETER PfxPassword
    PFX password. Defaults to $env:SIGNING_CERT_PASSWORD. May be empty if the PFX has none.
.PARAMETER TimestampServer
    One or more RFC 3161 timestamp servers, tried in order with retries.
.PARAMETER VerifyOnly
    Do not sign; only verify that every signable file has a valid, timestamped signature.
#>
[CmdletBinding(DefaultParameterSetName = 'Sign')]
param(
    [Parameter(Mandatory)]
    [string]$ModulePath,

    [Parameter(ParameterSetName = 'Sign')]
    [string]$PfxBase64 = $env:SIGNING_CERT_PFX_BASE64,

    [Parameter(ParameterSetName = 'Sign')]
    [string]$PfxPassword = $env:SIGNING_CERT_PASSWORD,

    [Parameter(ParameterSetName = 'Sign')]
    [string[]]$TimestampServer = @(
        'http://timestamp.digicert.com',
        'http://timestamp.sectigo.com',
        'http://timestamp.globalsign.com/tsa/r6advanced1'
    ),

    [Parameter(Mandatory, ParameterSetName = 'Verify')]
    [switch]$VerifyOnly
)

$ErrorActionPreference = 'Stop'

$root = (Resolve-Path -LiteralPath $ModulePath).ProviderPath

function Get-SignableFile {
    param([string]$Root)

    $all = @(Get-ChildItem -LiteralPath $Root -Recurse -File)
    $files = @($all | Where-Object {
            $ext = $_.Extension.ToLowerInvariant()
            if ($ext -eq '.dll') { $_.Name -eq 'PSWindowsUpdateNext.dll' }
            else { $ext -in '.psm1', '.psd1', '.ps1xml', '.ps1' }
        })

    # Guard against a half-assembled module being "successfully" signed.
    $required = @(
        'PSWindowsUpdateNext.psd1',
        'PSWindowsUpdateNext.psm1',
        'PSWindowsUpdateNext.Format.ps1xml',
        'Gui\Show-PSWUConsole.ps1',
        'net472\PSWindowsUpdateNext.dll',
        'net8.0-windows\PSWindowsUpdateNext.dll'
    )
    foreach ($rel in $required) {
        $full = Join-Path -Path $Root -ChildPath $rel
        if (-not (Test-Path -LiteralPath $full -PathType Leaf)) {
            throw "Expected module file is missing: $rel (in $Root)"
        }
    }
    $files
}

function Get-RelativeName {
    param([string]$Root, [string]$FullName)
    $FullName.Substring($Root.Length).TrimStart('\', '/')
}

function Confirm-Signature {
    param([string]$Root, [System.IO.FileInfo[]]$Files, [string]$ExpectedThumbprint)

    $failures = New-Object System.Collections.Generic.List[string]
    foreach ($file in $Files) {
        $name = Get-RelativeName -Root $Root -FullName $file.FullName
        $sig = Get-AuthenticodeSignature -LiteralPath $file.FullName
        $problem = $null
        if ($sig.Status -ne 'Valid') {
            $problem = "status is $($sig.Status): $($sig.StatusMessage)"
        } elseif ($null -eq $sig.SignerCertificate) {
            $problem = 'status is Valid but no signer certificate was returned'
        } elseif ($ExpectedThumbprint -and $sig.SignerCertificate.Thumbprint -ne $ExpectedThumbprint) {
            $problem = "signed by an unexpected certificate ($($sig.SignerCertificate.Thumbprint))"
        } elseif ($null -eq $sig.TimeStamperCertificate) {
            $problem = 'signature has no timestamp'
        }

        if ($problem) {
            $failures.Add("$name - $problem")
            Write-Host ("  FAIL  {0}  {1}" -f $name, $problem)
        } else {
            Write-Host ("  OK    {0}  signer={1}  timestamp={2}" -f $name, $sig.SignerCertificate.Subject, $sig.TimeStamperCertificate.Subject)
        }
    }
    if ($failures.Count -gt 0) {
        throw ("Signature verification failed for {0} of {1} file(s):`n  {2}" -f $failures.Count, $Files.Count, ($failures -join "`n  "))
    }
}

# Authenticode on script files hashes the decoded text. Windows PowerShell 5.1 decodes a file without a BOM
# using the ANSI code page, so a BOM-less UTF-8 script containing non-ASCII characters would verify as
# HashMismatch there. Make sure such files carry a UTF-8 BOM before they are signed.
function Set-Utf8BomIfNeeded {
    param([string]$Path)

    $bytes = [System.IO.File]::ReadAllBytes($Path)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) { return }
    if ($bytes.Length -ge 2 -and (($bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) -or ($bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF))) { return }

    $hasHighByte = $false
    foreach ($b in $bytes) { if ($b -gt 127) { $hasHighByte = $true; break } }
    if (-not $hasHighByte) { return }

    # Throws if the file is not valid UTF-8, which is better than signing garbage.
    $strict = New-Object System.Text.UTF8Encoding($false, $true)
    [void]$strict.GetString($bytes)
    $withBom = New-Object byte[] ($bytes.Length + 3)
    $withBom[0] = 0xEF; $withBom[1] = 0xBB; $withBom[2] = 0xBF
    [System.Array]::Copy($bytes, 0, $withBom, 3, $bytes.Length)
    [System.IO.File]::WriteAllBytes($Path, $withBom)
    Write-Warning "Added a UTF-8 BOM to '$Path' (it contains non-ASCII characters) so Windows PowerShell 5.1 reads it the way it was signed."
}

$files = @(Get-SignableFile -Root $root)
Write-Host "Found $($files.Count) signable file(s) under $root"

if ($VerifyOnly) {
    Write-Host "Verifying signatures (PowerShell $($PSVersionTable.PSVersion), $PSEdition)..."
    Confirm-Signature -Root $root -Files $files
    Write-Host "All $($files.Count) file(s) have a valid, timestamped signature."
    return
}

if ([string]::IsNullOrWhiteSpace($PfxBase64)) {
    throw 'No signing certificate supplied. Set SIGNING_CERT_PFX_BASE64 (base64 of the .pfx) or pass -PfxBase64.'
}
if (-not [string]::IsNullOrEmpty($PfxPassword) -and $env:GITHUB_ACTIONS -eq 'true') {
    # GitHub masks secrets already; this also covers a password that was passed some other way.
    Write-Host "::add-mask::$PfxPassword"
}

$tempRoot = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [System.IO.Path]::GetTempPath() }
$pfxPath = Join-Path -Path $tempRoot -ChildPath ('pswunext-sign-{0}.pfx' -f [guid]::NewGuid().ToString('N'))
$importedThumbprints = @()

try {
    [System.IO.File]::WriteAllBytes($pfxPath, [System.Convert]::FromBase64String($PfxBase64.Trim()))

    $importArgs = @{
        FilePath          = $pfxPath
        CertStoreLocation = 'Cert:\CurrentUser\My'
    }
    if (-not [string]::IsNullOrEmpty($PfxPassword)) {
        $importArgs.Password = ConvertTo-SecureString -String $PfxPassword -AsPlainText -Force
    }
    $imported = @(Import-PfxCertificate @importArgs)
    $importedThumbprints = @($imported | ForEach-Object { $_.Thumbprint })

    $cert = $imported | Where-Object { $_.HasPrivateKey } | Select-Object -First 1
    if (-not $cert) { throw 'The PFX does not contain a certificate with a private key.' }
    if ($cert.NotAfter -lt (Get-Date)) { throw "The signing certificate expired on $($cert.NotAfter.ToString('u'))." }
    $codeSigningOid = '1.3.6.1.5.5.7.3.3'
    $eku = @($cert.Extensions | Where-Object { $_ -is [System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension] })
    if ($eku.Count -gt 0 -and -not ($eku[0].EnhancedKeyUsages | Where-Object { $_.Value -eq $codeSigningOid })) {
        throw 'The certificate is not valid for code signing (missing Code Signing enhanced key usage).'
    }
    Write-Host "Signing with: $($cert.Subject)  (thumbprint $($cert.Thumbprint), expires $($cert.NotAfter.ToString('yyyy-MM-dd')))"

    $utf8Bom = New-Object System.Text.UTF8Encoding($true)
    foreach ($file in $files) {
        $name = Get-RelativeName -Root $root -FullName $file.FullName
        $isText = $file.Extension.ToLowerInvariant() -ne '.dll'
        if ($isText) { Set-Utf8BomIfNeeded -Path $file.FullName }

        $signed = $false
        $lastProblem = $null
        $fatal = $false
        :attempts foreach ($attempt in 1..3) {
            foreach ($server in $TimestampServer) {
                try {
                    $params = @{
                        LiteralPath     = $file.FullName
                        Certificate     = $cert
                        HashAlgorithm   = 'SHA256'
                        TimestampServer = $server
                        ErrorAction     = 'Stop'
                    }
                    if ($isText) { $params.Encoding = $utf8Bom }
                    $result = Set-AuthenticodeSignature @params
                    if ($result.Status -eq 'Valid') { $signed = $true; break attempts }
                    $lastProblem = "$($result.Status): $($result.StatusMessage)"
                    if ($result.SignerCertificate -and $result.TimeStamperCertificate) {
                        # Signed and timestamped but not trusted: retrying will not help.
                        $fatal = $true
                        throw "Signed '$name' but the signature is not valid ($lastProblem). Is the certificate chained to a trusted root?"
                    }
                } catch {
                    if ($fatal) { throw }
                    $lastProblem = $_.Exception.Message
                    Write-Warning "Signing '$name' via $server failed (attempt $attempt): $lastProblem"
                }
            }
            if ($attempt -lt 3) { Start-Sleep -Seconds (5 * $attempt) }
        }
        if (-not $signed) { throw "Could not sign '$name'. Last error: $lastProblem" }
        Write-Host "  signed $name"
    }

    Write-Host 'Verifying signatures...'
    Confirm-Signature -Root $root -Files $files -ExpectedThumbprint $cert.Thumbprint
    Write-Host "Signed and verified $($files.Count) file(s)."
} finally {
    if (Test-Path -LiteralPath $pfxPath) { Remove-Item -LiteralPath $pfxPath -Force -ErrorAction SilentlyContinue }
    foreach ($thumb in $importedThumbprints) {
        $certPath = "Cert:\CurrentUser\My\$thumb"
        if (Test-Path -LiteralPath $certPath) { Remove-Item -LiteralPath $certPath -Force -ErrorAction SilentlyContinue }
    }
}
