# Releasing PSWindowsUpdateNext

A release is cut by pushing a `v*` tag. The workflow in `.github/workflows/build.yml` then builds and tests, signs the module, creates the GitHub release, and publishes to the PowerShell Gallery. Nothing is released from a branch or a pull request.

## What the pipeline does

```
build  ->  sign                      ->  release  ->  publish
(every     (tags, environment         (tags)       (tags, environment "release")
 push/PR)   "signing")
```

| Job | Runs on | Does | Needs |
| --- | --- | --- | --- |
| `build` | every push and PR | On tags only, first checks that the tag, `ModuleVersion`, the csproj `<Version>` and `CHANGELOG.md` agree. Builds, runs the Pester tests in PowerShell 7 and Windows PowerShell 5.1, uploads the unsigned module. | nothing |
| `sign` | `v*` tags | Authenticode-signs the module once, through Azure Artifact Signing or with a PFX, verifies every file in PowerShell 7 and 5.1, re-runs the Pester tests against the signed module, uploads the signed module. Runs in the `signing` environment. | Artifact Signing variables, or the PFX secrets |
| `release` | `v*` tags | Zips the signed module, writes a SHA-256 checksum file, attests build provenance for the zip and each module file, takes the release notes from `CHANGELOG.md`, creates the GitHub release. The only job with `contents: write`. | nothing |
| `publish` | `v*` tags | Runs `Test-ModuleManifest`, then `Publish-PSResource` (falling back to `Publish-Module` only if PSResourceGet is missing) with the signed module. Runs in the `release` environment. | `PSGALLERY_API_KEY` |

The GitHub zip and the Gallery package are made from the same signed files; signing happens exactly once.

Files that are signed: both `PSWindowsUpdateNext.dll` files, `PSWindowsUpdateNext.psm1`, `PSWindowsUpdateNext.psd1`, `PSWindowsUpdateNext.Format.ps1xml`, and `Gui\*.ps1`. SHA-256 with a timestamp. The only shipped file that Authenticode cannot cover is the help XML (`PSWindowsUpdateNext.dll-Help.xml`), which is not code. The SHA-256 file of the GitHub zip covers it, but a Gallery install has no such check.

Everything the console runs is covered by a signature. The console's row types are compiled into `PSWindowsUpdateNext.dll`, and its XAML (`MainWindow.xaml`, `Theme.xaml`, kept under `src\PSWindowsUpdateNext\Gui`) is embedded in the DLL as resources. `Gui\Show-PSWUConsole.ps1` reads them with `GetManifestResourceStream`, so nothing is compiled or parsed from an unsigned file at run time and no C# compiler is needed. The "Restart as administrator" button starts the elevated host with `-NoProfile -EncodedCommand` and does not override the execution policy, so an `AllSigned` or `RemoteSigned` policy applies as the administrator set it.

## One-time setup

### Signing with Azure Artifact Signing (recommended)

Artifact Signing keeps the private key in Microsoft's HSMs. The `sign` job signs in to Azure with GitHub's OIDC token, so no certificate, key, or password is stored in GitHub. It costs about $9.99 a month (Basic) from the day the account is created. Individuals must be in the US or Canada.

1. **Azure subscription.** A pay-as-you-go subscription (a free or sponsored one does not work). Register the resource provider: `az provider register --namespace Microsoft.CodeSigning`.
2. **Artifact Signing account.** In the Azure portal, create an Artifact Signing account (Basic tier) in a resource group. The name must be globally unique, 3 to 24 alphanumeric characters, and start with a letter. Note its region: the endpoint must match it, for example `https://eus.codesigning.azure.net` for East US (the full list is in Microsoft's [signing integrations](https://learn.microsoft.com/azure/artifact-signing/how-to-signing-integrations) article).
3. **Identity validation.** It can only be done in the portal. Before starting, make sure the subscription's Azure billing account has the account type **Individual**, and that its legal name and sold-to address match your government ID exactly: the validation copies them from there, and the name, city, state, and country end up in the certificate. Give yourself the **Artifact Signing Identity Verifier** role on the account, open **Identity validations**, choose **Individual**, then **New Identity** > **Public**. When the status changes to **Action Required**, follow the link: verification runs through AU10TIX with your phone's camera and stores a Verified ID in Microsoft Authenticator. Microsoft quotes up to 20 business days if more documents are needed.
4. **Certificate profile.** Once the validation is complete, create a certificate profile of type **Public Trust** that uses it. The subject will carry your validated name.
5. **An identity for GitHub.** In Microsoft Entra ID, create an app registration (for example `PSWindowsUpdateNext signing`). Under **Certificates & secrets > Federated credentials**, add a credential for GitHub Actions with organization `tsvb`, repository `PSWindowsUpdateNext`, entity type **Environment**, and environment `signing`. This makes the subject `repo:tsvb/PSWindowsUpdateNext:environment:signing`, so only jobs in the `signing` environment, which only tags can enter, can sign in as it. Do not create a client secret.
6. **Permission to sign.** On the certificate profile (not the whole account or subscription), give that app registration the **Artifact Signing Certificate Profile Signer** role. It needs no other role.
7. **Variables.** Add these as variables (not secrets; none of them is sensitive) in the `signing` environment:

   | Variable | Value |
   | --- | --- |
   | `AZURE_CLIENT_ID` | The app registration's application (client) ID |
   | `AZURE_TENANT_ID` | Your Microsoft Entra tenant ID |
   | `AZURE_SUBSCRIPTION_ID` | The subscription that holds the signing account |
   | `ARTIFACT_SIGNING_ENDPOINT` | The regional endpoint from step 2 |
   | `ARTIFACT_SIGNING_ACCOUNT` | The Artifact Signing account name |
   | `ARTIFACT_SIGNING_PROFILE` | The certificate profile name |

8. **Turn off unsigned releases.** Delete the repository variable `ALLOW_UNSIGNED_RELEASE`, so a broken signing setup fails the release instead of shipping unsigned files.

When all six variables are set, the `sign` job uses Artifact Signing even if a PFX secret also exists. SignTool and the Artifact Signing plug-in are downloaded from nuget.org at pinned versions and checked against pinned SHA-512 digests (`tools/Install-ArtifactSigningTools.ps1`).

Artifact Signing certificates are valid for three days and are renewed automatically. The RFC 3161 timestamp from `timestamp.acs.microsoft.com` is what keeps a signature valid after that, so the job fails any file without one.

To sign locally with the same account, sign in with `az login` as a user who has the Certificate Profile Signer role, then run `./tools/Sign-Module.ps1 -ModulePath .\out\PSWindowsUpdateNext -ArtifactSigningEndpoint <endpoint> -ArtifactSigningAccount <account> -ArtifactSigningProfile <profile>`.

### Secrets and variable

The PFX secrets are only needed when signing with a PFX instead of Artifact Signing. Create the secrets in the environments described in the next section. Create the variable under **Settings > Secrets and variables > Actions**.

| Name | Kind | Where | Value |
| --- | --- | --- | --- |
| `SIGNING_CERT_PFX_BASE64` | secret | `signing` environment | Base64 of the code-signing `.pfx` (PFX signing only). |
| `SIGNING_CERT_PASSWORD` | secret | `signing` environment | The PFX password (PFX signing only). Leave unset only if the PFX has no password. |
| `PSGALLERY_API_KEY` | secret | `release` environment | PowerShell Gallery API key (below). |
| `ALLOW_UNSIGNED_RELEASE` | variable | repository | Leave unset. Set to `true` only to ship an unsigned release on purpose. |

Keep the secrets in the environments, never as repository secrets. A repository secret is readable by a workflow that anyone with write access pushes on any branch, without a tag. An environment secret is only released to jobs that the environment's deployment rules allow.

Encode the PFX and load it into the secret without printing it:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('.\codesign.pfx')) | gh secret set SIGNING_CERT_PFX_BASE64 --env signing
gh secret set SIGNING_CERT_PASSWORD --env signing      # prompts for the value
gh secret set PSGALLERY_API_KEY --env release          # prompts for the value
```

Delete the local `.pfx` and any base64 copy afterwards, or keep them in a password manager.

**What happens when something is missing**

- Neither the six Artifact Signing variables nor `SIGNING_CERT_PFX_BASE64` on a tag: the `sign` job fails with an error that says so, and nothing is released. If `ALLOW_UNSIGNED_RELEASE` is `true`, it emits a warning annotation instead, the release notes get an "not Authenticode-signed" warning at the top, and the release continues (the Gallery publish too).
- No `PSGALLERY_API_KEY`: the `publish` job warns and skips publishing. The GitHub release is unaffected.
- Branch and PR builds never need any of these.

### The `signing` and `release` environments

Create both environments before the first tag. Create them under **Settings > Environments > New environment**, using exactly these names.

- **`signing`**
  - Under **Deployment branches and tags**, choose **Selected branches and tags** and add only the tag rule `v*`.
  - No reviewers are needed.
  - Add the Artifact Signing variables, or, for PFX signing, `SIGNING_CERT_PFX_BASE64` and `SIGNING_CERT_PASSWORD` as environment secrets.
- **`release`**
  - Use the same `v*` tag rule.
  - Enable **Required reviewers** and add the people allowed to approve a publish. The `publish` job then pauses until someone approves, after the GitHub release exists.
  - Add `PSGALLERY_API_KEY` as an environment secret.

Also add a tag ruleset under **Settings > Rules > Rulesets**, targeting tags matching `v*`, that restricts who can create them. Anyone who can push a `v*` tag can start a run that signs with your certificate.

### PowerShell Gallery API key

Sign in at powershellgallery.com, open **API Keys**, and create a key with **Push new packages and package versions**, scoped to the glob `PSWindowsUpdateNext`, with the shortest expiry you are willing to renew (the Gallery maximum is one year). Put the key in `PSGALLERY_API_KEY`. Gallery keys expire; a failing publish with an authorization error usually means it has.

### Signing with a PFX instead (brief)

This is only needed if you do not use Artifact Signing. You need a publicly trusted code-signing certificate (OV or EV) from a certificate authority such as DigiCert, Sectigo, or SSL.com. The job requires every signature to be `Valid` on the runner, so a self-signed certificate will not work: it fails verification. It would also not be trusted on your users' machines.

Caveat: since 2023 the CA/Browser Forum rules require the private key of a publicly trusted code-signing certificate to be held in certified hardware (a token or cloud HSM). Many CAs therefore no longer hand out an exportable `.pfx`. This pipeline needs a PFX. Before buying, confirm with the CA that you can get an exportable PFX (some offer it, or allow generating the key in software for older or specific products). If you can only get a hardware-token or cloud-HSM certificate from a CA, `tools/Sign-Module.ps1` would need that vendor's signing client; that is not implemented here.

The certificate must have the Code Signing enhanced key usage and must not be expired; the script checks both. Renew before expiry: the RFC 3161 timestamp keeps already-released files valid after the certificate expires, but new releases need a current certificate.

## Cutting a release

Example for version `3.2.0`. Use the same number everywhere.

1. Create a branch and bump the version in all of these places:
   - `module/PSWindowsUpdateNext.psd1`: `ModuleVersion = '3.2.0'`
   - `src/PSWindowsUpdateNext/PSWindowsUpdateNext.csproj`: `<Version>3.2.0</Version>`, `<AssemblyVersion>3.2.0.0</AssemblyVersion>`, `<FileVersion>3.2.0.0</FileVersion>`
2. Add a section at the top of `CHANGELOG.md` with a heading in exactly this shape, and content under it:

   ```markdown
   ## 3.2.0 (2026-11-15)

   ### Added
   - ...
   ```

   The text under that heading, up to the next `## ` heading, becomes the GitHub release body.
3. Optionally check locally (any PowerShell 5.1 or 7): `./tools/Assert-ReleaseVersion.ps1 -Tag v3.2.0`
4. Open a pull request, wait for the build to pass, merge to `main`, and make sure the build on `main` is green.
5. Tag the merge commit and push the tag:

   ```bash
   git checkout main
   git pull
   git tag -a v3.2.0 -m "PSWindowsUpdateNext 3.2.0"
   git push origin v3.2.0
   ```

6. Watch the run in the **Actions** tab. When `release` has finished, open the run and approve the pending `publish` deployment to the `release` environment.
7. Check the result:
   - The GitHub release has `PSWindowsUpdateNext-v3.2.0.zip`, `PSWindowsUpdateNext-v3.2.0.zip.sha256`, and the changelog text as its body.
   - In an extracted zip: `Get-ChildItem -Recurse -Include *.dll,*.psm1,*.psd1,*.ps1xml,*.ps1 | Get-AuthenticodeSignature | Select-Object Path, Status`. All should be `Valid`.
   - The checksum: `(Get-FileHash .\PSWindowsUpdateNext-v3.2.0.zip).Hash` matches the `.sha256` file (compare case-insensitively).
   - The provenance: `gh attestation verify .\PSWindowsUpdateNext-v3.2.0.zip --repo tsvb/PSWindowsUpdateNext` passes. Each file inside the zip is attested too.
   - `Find-PSResource PSWindowsUpdateNext -Repository PSGallery` (or `Find-Module`) shows 3.2.0. Gallery indexing can take a few minutes.

The version check is strict: the tag must be `v` plus exactly the manifest and csproj version. Pre-release tags such as `v3.2.0-beta1` are rejected.

### If a run fails

- **Version check or changelog error:** nothing has been released. Fix the files, then move the tag: `git tag -d v3.2.0 && git push origin :refs/tags/v3.2.0`, commit the fix, and tag again.
- **Signing error** (bad password, expired certificate, timestamp servers unreachable, untrusted chain): nothing has been released. Fix the cause and re-run the failed job, or re-tag as above.
- **`release` succeeded but `publish` failed or was rejected:** re-run the `publish` job. Do not re-tag. A Gallery version can never be replaced or reused, so if a bad version did get published, bump to the next version.

## Local test of the signing script

On a Windows machine with a test certificate whose chain is trusted locally:

```powershell
./build.ps1
$env:SIGNING_CERT_PFX_BASE64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes('.\codesign.pfx'))
$env:SIGNING_CERT_PASSWORD = '...'
./tools/Sign-Module.ps1 -ModulePath .\out\PSWindowsUpdateNext
./tools/Sign-Module.ps1 -ModulePath .\out\PSWindowsUpdateNext -VerifyOnly
```

This modifies the files in `out\`; rebuild to get unsigned files back.
