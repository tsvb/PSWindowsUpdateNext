# Changelog

## 3.2.0 (2026-10-26)

First release published to GitHub Releases and the PowerShell Gallery.

### Security
- `Get-WUOfflineMSU` now requires each downloaded file to be signed by Microsoft. The signer's chain must end at a Microsoft root CA: Root CA 2010 or 2011, ECC Product Root 2018, or the legacy 1997 and 2001 roots. A valid signature from any other publisher is rejected.
- `Get-WUOfflineMSU` checks certificate revocation for the signing chain, and a revoked certificate fails. If revocation cannot be checked because the machine is offline or behind a proxy, a file that matched the Catalog's SHA-1 digest is kept with a warning.
- Release files are Authenticode-signed with SHA-256 and timestamped. The release workflow refuses to publish unsigned files unless the repository variable `ALLOW_UNSIGNED_RELEASE` is `true`.
- The signing certificate is only available to tag builds through the `signing` environment, and the third-party release action is pinned to a commit.
- `Show-PSWUConsole` no longer compiles `Gui\Models.cs` with `Add-Type` or loads loose XAML files at run time. The row types are now part of `PSWindowsUpdateNext.dll` and the two XAML files are embedded in it, so under an `AllSigned` policy everything the console executes is covered by the signature. The console no longer calls `Add-Type` on source code, so it needs no C# compiler. `Gui\MainWindow.xaml`, `Gui\Theme.xaml`, and `Gui\Models.cs` are removed from the module folder.
- The console's "Restart as administrator" button no longer starts the elevated host with `-ExecutionPolicy Bypass`. It respects the configured execution policy, and passes on a policy only when the user set one for the current session. The command goes through `-EncodedCommand` with PowerShell's own quoting, so a module path that contains an apostrophe or a typographic quote no longer breaks it.
- `Get-WUOfflineMSU` hands BITS the canonical form of the download URL it validated, instead of the raw string from the Catalog page, so the host BITS connects to cannot differ from the host the allowlist approved.

### Added
- Release pipeline: a tag must match the manifest, the project file, and this changelog. Signing runs in its own job. The GitHub release carries a zip and a SHA-256 checksum file, and publishing to the PowerShell Gallery waits for approval through the `release` environment. See `docs/RELEASING.md`.
- `Get-WUOfflineMSU` output files report `SignatureStatus` and `SignerSubject`, and failure messages say why a signature was rejected.
- Help source in `docs/help` as platyPS markdown, one page per cmdlet. `tools/Build-Help.ps1` regenerates the compiled help, and CI fails if the committed help is out of date with the markdown.
- Tests that run the signature check on a Microsoft-signed file, a third-party-signed file, and an unsigned file.
- `SECURITY.md` explains how to report a vulnerability privately. `CONTRIBUTING.md` explains how to build, test, update help and open a pull request.
- Dependabot checks the GitHub Actions and NuGet dependencies every week.
- Static analysis in CI. The C# project builds with the .NET analyzers (`AnalysisLevel` 8.0, default rule set plus every security rule) and treats warnings as errors; the settings and each suppression with its reason are in `.editorconfig`. A new CI step runs PSScriptAnalyzer (pinned to 1.24.0, `PSScriptAnalyzerSettings.psd1`) over the loader, `build.ps1`, `tools`, `tests`, and `module/Gui`, and fails on any Error or Warning.
- C# unit tests in `tests/PSWindowsUpdateNext.UnitTests` (xUnit, `net8.0-windows` and `net472`), run in CI right after the module build. They cover Catalog page parsing (search rows, download dialog, HTML entities, malformed input), the download host allowlist, the `Invoke-WUJob` `-EncodedCommand` round trip, the `WUFileVerifier` digests, and the DPAPI helpers (Windows only; skipped elsewhere). `PSWindowsUpdateNext.sln` now ties the module and the test project together.

### Changed
- Compiled help is rewritten against the compiled cmdlets. `-SkipSignatureCheck` and `-UpdateID` are documented. `-Silent`, `-HotFixID`, and `-AUOptions` are shown as the aliases they are. Parameter sets match the cmdlets. The swapped descriptions of `-WindowsUpdate` and `-MicrosoftUpdate` are corrected, and examples no longer reference the old module or its author's sites.
- `Show-PSWUConsole` has complete comment-based help.
- Every build checks that `ModuleVersion` and the project file agree on the version, not only release tags.
- The GitHub Actions used by the build are pinned to commit SHAs.
- `Set-WUSettings` no longer discards why a write failed. The error for a value it could not set now names the computer and the value and carries the exception message instead of a fixed "Access denied.". Reading the current value of a setting that is not set is still silent, but the reason now appears with `-Debug`.
- Failures of service restarts (`Add-WUServiceManager`, `Remove-WUServiceManager`), service stop and start and external commands (`Reset-WUComponents`), and saving or loading `PSWUSettings.xml` (`Set-PSWUSettings`) are reported as warnings. They were silent.
- All native calls resolve their DLL from System32 only.
- `Set-PSWUSettings` no longer computes the unused minute-based key from the original module and no longer prints it with `-Debug`. The `-SaveAsSystem` value has been protected with machine-scope DPAPI since 3.0.0; a value that cannot be decrypted now produces a clear error.
- Internal only, no change in behavior: the Catalog parsing and download host check moved out of `Get-WUOfflineMSU` into an internal `MsuCatalogParser` class, and the `Invoke-WUJob` argument builder into `WUJobCommandLine`, so they can be tested.

### Fixed
- `Set-WUSettings -NoAutoUpdate` read and wrote `TargetGroupEnabled` instead of `NoAutoUpdate`, so it never turned Automatic Updates on or off.
- `Set-WUSettings -SetDisableUXWUAccess` wrote `SetDisablePauseUXAccess`, hiding the pause option instead of removing scan access.
- An exception inside signature verification left the downloaded file in place. It now counts as a failed check, and the file is deleted unless `-SkipSignatureCheck` is set.
- Signature verification leaked a native copy of the file path on every call.
- `Get-WUJob -ClearExpired` was inverted: a plain `Get-WUJob` deleted expired jobs, and `-ClearExpired` skipped the clean-up. It now deletes them only with `-ClearExpired` or through `Clear-WUJob`, and a job whose end boundary cannot be read no longer stops the clean-up for the whole computer.
- `Set-WUSettings -ScheduledInstallDay` threw for Saturday, because the accepted value `EverySaturday` had no matching entry. `Every Saturday` is now accepted, `EverySaturday` still works, and all four value lists match case-insensitively like their `ValidateSet`.
- `Set-WUSettings -TargetGroup` read a value named by a line break instead of `TargetGroup`, so it reported no current value and rewrote the setting on every run.
- `Enable-WURemoting` no longer loosens remoting security without being asked. Whenever TrustedHosts did not already contain `*` (in practice, on the first run), it set `LocalAccountTokenFilterPolicy=1` and `ForceGuest=0` and added `*` to TrustedHosts, and it never actually enabled remoting because of a typo (`Enable -PSRemoting`). It now runs `Enable-PSRemoting -Force` in Windows PowerShell, which configures the endpoint the module's remoting uses (adding `-SkipNetworkProfileCheck` with `-WinRMPublic`), and reports its errors. The two registry values are set only with `-LocalAccountTokenFilterPolicy`; otherwise the token filter value that `Enable-PSRemoting` sets on computers outside a domain is restored. TrustedHosts is left alone.
- `Reset-WUComponents` passed `%SystemRoot%\<name>.dll` to regsvr32, but the DLLs live in System32, so step 6 registered nothing. It now uses the System32 path and skips DLLs that current Windows versions no longer ship.
- The desktop console passed install days without spaces (`EveryMonday`), which `Set-WUSettings` rejects, so only Saturday could be set. It now passes the accepted values.
- Removed the three compiler warnings (each reported once per target): an ignored attribute in `INetFwPolicy2`, an unused variable in `ConvertSize`, and a `catch` that only rethrew in `WUCimSession.InstallUpdates`.

### Removed
- Help pages for `Get-WindowsUpdate_v2` and `Get-WUTest`, which the module does not export.
- The vendored copy of Pester 6.2.0 that was committed by mistake.

### Tests
- The desktop console test now runs. Pester 5 decides `-Skip` while it discovers tests, and the old apartment check ran too late, so the test was always skipped. CI also runs it in explicit STA hosts for PowerShell 7 and Windows PowerShell 5.1 and fails if the window is not built.

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
