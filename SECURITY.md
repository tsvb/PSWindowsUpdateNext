# Security Policy

## Supported versions

Only the latest release of PSWindowsUpdateNext receives security fixes. Older releases are not patched.
Update to the latest release from the [Releases](https://github.com/tsvb/PSWindowsUpdateNext/releases)
page or from the PowerShell Gallery.

## Reporting a vulnerability

Please do not report security problems in a public issue or pull request. Report them privately:

1. Open the [Security tab](https://github.com/tsvb/PSWindowsUpdateNext/security) of the repository.
2. Click **Report a vulnerability**.
3. Describe the problem and include the details listed below.

This uses GitHub's private vulnerability reporting. Only the maintainer can see the report until a fix is
published and the advisory is made public.

## What to include

- The module version (`(Get-Module PSWindowsUpdateNext -ListAvailable).Version`), and the output of
  `$PSVersionTable` and the Windows version.
- The cmdlet, function or file involved, and the parameters or inputs used.
- Steps to reproduce, or a proof of concept. Say whether it needs an elevated or a non-elevated session.
- The impact you expect, for example code running as administrator or SYSTEM, or a bypassed check.
- Whether you have disclosed it anywhere else.

Remove passwords, tokens, and host names you do not want shared.

## Scope

The module runs with administrator rights. `Invoke-WUJob` registers Task Scheduler jobs that run as
SYSTEM. Reports about privilege boundaries are therefore in scope. Examples include a way for a
non-administrator to run code as administrator or SYSTEM, to change files that run with those rights,
or to read credentials. Bypasses of the download checks (Catalog digest, Authenticode signature, and
revocation) are also in scope.

## Response

The maintainer aims to acknowledge a report within 7 days and to give an initial assessment within
14 days. These are goals, not guarantees. Complex reports can take longer. The maintainer will keep you
informed and will agree a disclosure date with you before a fix is published.
