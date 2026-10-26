# Contributing to PSWindowsUpdateNext

Thanks for helping. Report security problems privately as described in [SECURITY.md](SECURITY.md),
not in a public issue.

## Prerequisites

- The .NET 8 SDK.
- PowerShell 7.4 or later, and Windows PowerShell 5.1. The module targets both, and the build and tests
  run on Windows.

## Build

```powershell
./build.ps1
```

This compiles `src/PSWindowsUpdateNext` for both targets and assembles the module in `out/PSWindowsUpdateNext`.

## Tests

The tests use Pester 5. Run them after `./build.ps1`:

```powershell
Invoke-Pester ./tests
```

CI runs the tests in PowerShell 7 and in Windows PowerShell 5.1. Pester 5.5 or later is required.

## Help

Get-Help content is written as markdown in `docs/help/*.md`. After you edit a page:

```powershell
./tools/Build-Help.ps1
```

This regenerates the compiled help XML in `module/`. Commit the regenerated XML with your markdown change.
CI fails if the XML is out of date with `docs/help`.

## Versioning and the changelog

Add an entry for your change under `## Unreleased` at the top of `CHANGELOG.md`. Version numbers change only
when a release is cut; see [docs/RELEASING.md](docs/RELEASING.md). Every build checks that the module
manifest and the project file agree on the version.

## Pull requests

- CI must be green before a pull request is merged.
- Keep each pull request to one topic.
- Describe what changed and why.

## Releasing

Maintainers cut releases as described in [docs/RELEASING.md](docs/RELEASING.md).
