---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Get-WUOfflineMSU.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WUOfflineMSU
---

# Get-WUOfflineMSU

## SYNOPSIS

Downloads update packages from the Microsoft Update Catalog.

## SYNTAX

### __AllParameterSets

```
Get-WUOfflineMSU -KBArticleID <string> -Destination <string> [-AcceptAll] [-SkipSignatureCheck]
 [-UpdateID <string[]>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Searches the Microsoft Update Catalog (catalog.update.microsoft.com) for a KB article, lists the catalog entries it finds, and downloads every file of the entries you accept. Use it to get .msu and .cab packages for computers that cannot reach Windows Update. The cmdlet asks for confirmation for each entry unless you use -AcceptAll; use -UpdateID to limit the search to chosen entries.

The cmdlet first returns every catalog entry it considered, with Result set to Accepted, Rejected, or Error. Error means the Catalog listed no files for the entry. After the downloads, it returns only the entries whose files all downloaded and passed verification, with Result set to Downloaded. Entries that were canceled or failed are not returned a second time; their errors and warnings explain why. The Files property lists every file of an entry with its Url, FileName, and Digest. After the download it also has SignatureStatus, SignerSubject, Path, and Sha256.

Files are downloaded with BITS into -Destination, which is created if it does not exist; existing files with the same name are overwritten. Only HTTPS URLs on windowsupdate.com or microsoft.com hosts are accepted. Each file is checked against the SHA-1 digest that the Catalog publishes, and its Authenticode signature is verified. A file that fails the digest check is deleted. A file whose signature fails verification, is revoked, or does not chain to a Microsoft root is also deleted, unless you use -SkipSignatureCheck. If revocation cannot be checked, for example because the computer is offline or behind a proxy, a file that matched the Catalog digest is kept and a warning is written.

## EXAMPLES

### Example 1: Download the packages for a KB article

```powershell
Get-WUOfflineMSU -KBArticleID KB5034123 -Destination C:\Updates
```

```output
Confirm
Are you sure you want to perform this action?
Performing the operation "(1/16/2024 2:27:17 PM) 2024-01 Cumulative Update for Windows 11 Version 23H2 for x64-based Systems (KB5034123) [580.4 MB]" on target "PC01".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): Y

X Result     Title                                                              LastUpdated Size
- ------     -----                                                              ----------- ----
1 Accepted   2024-01 Cumulative Update for Windows 11 Version 23H2 for x64-ba... 1/9/2024    580.4 MB
2 Downloaded 2024-01 Cumulative Update for Windows 11 Version 23H2 for x64-ba... 1/9/2024    580.4 MB
```

The KB number is accepted with or without the KB prefix. The answer to the prompt applies to one catalog entry; the cmdlet asks again for each entry that matches.

### Example 2: Download one catalog entry without prompts

```powershell
Get-WUOfflineMSU -KBArticleID 5034123 -Destination C:\Updates -UpdateID 2fbbe4d8-3a5f-4b0e-9d16-0c2b1d8f2a11 -AcceptAll
```

The update ID is the Identity of the entry. Run the cmdlet with -WhatIf to list the entries and their Identity values without downloading anything.

### Example 3: Download packages for several KB articles

```powershell
'KB5034123', 'KB5034765' | Get-WUOfflineMSU -Destination C:\Updates -AcceptAll
```

### Example 4: Keep a package whose signature check fails

```powershell
Get-WUOfflineMSU -KBArticleID KB5034123 -Destination C:\Updates -AcceptAll -SkipSignatureCheck
```

Keeps downloaded files even if their Authenticode signature fails verification or does not chain to a Microsoft root. The SHA-1 digest check still applies.

## PARAMETERS

### -AcceptAll

Downloads every catalog entry that matches without asking for confirmation.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases:
- cf
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Destination

Specifies the folder to download the files to. The folder is created if it does not exist. Existing files with the same name are overwritten.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -KBArticleID

Specifies the KB article number to search for, with or without the KB prefix, for example KB5034123 or 5034123. Accepts pipeline input by value and by property name.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SkipSignatureCheck

Keeps downloaded files whose Authenticode signature fails verification or does not chain to a Microsoft root, instead of deleting them. A warning is written for each file that is kept. The SHA-1 digest check still applies: a file that does not match the digest published by the Catalog is always deleted. Use this switch only for packages that you have verified in another way.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UpdateID

Limits the search results to the catalog entries that have these update IDs. The update ID is the GUID in the Identity property of an entry. Combine it with -AcceptAll to download a chosen subset without prompts. The comparison is not case sensitive.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf

Runs the command in a mode that only reports what would happen without performing the actions.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases:
- wi
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

You can pipe KB article numbers to KBArticleID.

## OUTPUTS

### PSWindowsUpdateNext.OfflineMSU

A catalog entry with X, Identity, Title, Product, Classification, LastUpdated, Version, Size, OriginalSize, ChooseResult, Status, Result, Files, DownloadUrl, and FileName properties. After the download it also has DownloadResult.

## NOTES

The cmdlet writes a warning when the session is not elevated. It has a confirmation impact of High, so it asks for confirmation for each catalog entry. The ComputerName and Credential parameters are not available on this cmdlet in the current release, so it always downloads to the local computer. The cmdlet reads the HTML of the Catalog pages itself, so Internet Explorer is not required.

## RELATED LINKS

- [Get-WindowsUpdate](Get-WindowsUpdate.md)
- [Get-WUHistory](Get-WUHistory.md)
