# Changelog

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
