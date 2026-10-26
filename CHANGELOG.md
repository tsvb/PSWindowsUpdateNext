# Changelog

## 3.2.0 (2026-10-26)

First release published to GitHub Releases and the PowerShell Gallery.

### Security
- `Get-WUOfflineMSU` now requires each downloaded file to be signed by Microsoft. The signer's chain must end at a Microsoft root CA: Root CA 2010 or 2011, ECC Product Root 2018, or the legacy 1997 and 2001 roots. A valid signature from any other publisher is rejected.
- `Get-WUOfflineMSU` checks certificate revocation for the signing chain, and a revoked certificate fails. If revocation cannot be checked because the machine is offline or behind a proxy, a file that matched the Catalog's SHA-1 digest is kept with a warning.
- Release files are Authenticode-signed with SHA-256 and timestamped. The release workflow refuses to publish unsigned files unless the repository variable `ALLOW_UNSIGNED_RELEASE` is `true`.
- The signing certificate is only available to tag builds through the `signing` environment, and the third-party release action is pinned to a commit.

### Added
- Release pipeline: a tag must match the manifest, the project file, and this changelog. Signing runs in its own job. The GitHub release carries a zip and a SHA-256 checksum file, and publishing to the PowerShell Gallery waits for approval through the `release` environment. See `docs/RELEASING.md`.
- `Get-WUOfflineMSU` output files report `SignatureStatus` and `SignerSubject`, and failure messages say why a signature was rejected.
- Help source in `docs/help` as platyPS markdown, one page per cmdlet. `tools/Build-Help.ps1` regenerates the compiled help, and CI rebuilds it from the markdown on every build.
- Tests that run the signature check on a Microsoft-signed file, a third-party-signed file, and an unsigned file.

### Changed
- Compiled help is rewritten against the compiled cmdlets. `-SkipSignatureCheck` and `-UpdateID` are documented. `-Silent`, `-HotFixID`, and `-AUOptions` are shown as the aliases they are. Parameter sets match the cmdlets. The swapped descriptions of `-WindowsUpdate` and `-MicrosoftUpdate` are corrected, and examples no longer reference the old module or its author's sites.
- `Show-PSWUConsole` has complete comment-based help.

### Fixed
- `Set-WUSettings -NoAutoUpdate` read and wrote `TargetGroupEnabled` instead of `NoAutoUpdate`, so it never turned Automatic Updates on or off.
- `Set-WUSettings -SetDisableUXWUAccess` wrote `SetDisablePauseUXAccess`, hiding the pause option instead of removing scan access.
- An exception inside signature verification left the downloaded file in place. It now counts as a failed check, and the file is deleted unless `-SkipSignatureCheck` is set.
- Signature verification leaked a native copy of the file path on every call.

### Removed
- Help pages for `Get-WindowsUpdate_v2` and `Get-WUTest`, which the module does not export.
- The vendored copy of Pester 6.2.0 that was committed by mistake.

## 3.1.0 (2026-10-25)

### Added
- `Show-PSWUConsole`: a WPF desktop console shipped in the module's `Gui` folder. Dark theme, custom title bar, navigation rail, and eight pages: Dashboard, Updates, History, Services, Settings, Catalog, Jobs, and Log. Operations run in a background runspace; verbose, warning, and error streams go to the Log page. Works in Windows PowerShell 5.1 and PowerShell 7. `-PassThru` returns the window without showing it.
- `Get-WUOfflineMSU -UpdateID`: limit a Catalog search to specific entries so a chosen subset can be downloaded with `-AcceptAll` and no prompts.
- Pester test that constructs the console window.

### Changed
- Module version 3.1.0; `Show-PSWUConsole` is exported alongside the cmdlets.

## 3.0.0 (2026-10-25)

First release of PSWindowsUpdateNext, forked from PSWindowsUpdate 2.2.1.3.

### Added
- Full C# source, recovered from the MIT-licensed binary with ILSpy, under `src/`.
- `net8.0-windows` build so the module loads natively in PowerShell 7.4+. The `.psm1` loader selects the binary by `$PSEdition`.
- `build.ps1` that compiles both targets and assembles the module folder; `-Install` copies it to the user module path.
- Pester smoke tests and a GitHub Actions workflow that builds, tests on both editions, and attaches a zip to tagged releases.
- `Get-WUOfflineMSU` verifies the SHA-1 digest and Authenticode signature of every downloaded file, reports a SHA-256 per file, and exposes all files of an update in a `Files` property. New `-SkipSignatureCheck` switch.

### Changed
- Module, namespace, and output type names are `PSWindowsUpdateNext.*` so this module can be installed next to the Gallery module. Cmdlet and parameter names are unchanged.
- `WUImpersonator` is a static helper built on `WindowsIdentity.RunImpersonated`; the `WindowsImpersonationContext` API it used does not exist on modern .NET.
- `Invoke-WUJob` passes the script to `powershell.exe` with `-NoProfile -NonInteractive -EncodedCommand`. The readable script is stored in the task description.
- `Set-PSWUSettings -SaveAsSystem` uses DPAPI (LocalMachine scope) instead of AES with a key derived from the current minute.
- HTTP calls use `HttpClient`, require HTTPS, and URL-encode form data. TLS 1.2/1.3 are enabled explicitly on .NET Framework.
- The HTML email report is built with a `StringBuilder` and HTML-encodes values; `System.Web` is no longer referenced.
- Console reads use `Host.UI.ReadLine()`; `Reset-WUComponents` step headings use `Host.UI.WriteLine()`.
- Settings and log paths resolve to the module root (next to the manifest) rather than the per-framework DLL folder.
- Help and format files are UTF-8 without a signature block.

### Fixed
- `Get-WUOfflineMSU` found no download links on the current Catalog (downloads moved to `catalog.sf.dl.delivery.mp.microsoft.com`) and depended on the deprecated MSHTML COM engine. It now parses the Catalog HTML directly.
- BITS download loop busy-waited at 100 percent CPU.
- Debug stream leaked the first character of passwords passed via `-Credential`.
- `PSWUModulePath` used `Assembly.CodeBase`, which is obsolete on .NET 5+.

### Removed
- `Unblock-File` over the module folder on import.
- Code Access Security attributes (`PermissionSet`, `SecurityPermission`), which are no-ops or compile errors on modern .NET.
- Unused `WUServiceController` class and the `mshtml` interop types.
- Vendored binaries, `PSGetModuleInfo.xml`, and the sample `PSWUSettings.xml.tmp`.

### Known gaps
- The compiled help file is carried over from 2.2.1.3 with the module name replaced. The new `-SkipSignatureCheck` parameter is documented in code but not yet in `Get-Help`.
- `Get-WindowsUpdate_v2` and `Get-WUTest` remain in the assembly but are not exported, same as upstream.
- Email reports still use `System.Net.Mail.SmtpClient`, which Microsoft lists as not recommended for new code.
