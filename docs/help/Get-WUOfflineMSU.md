---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WUOfflineMSU
---

# Get-WUOfflineMSU

## SYNOPSIS

Get offline MSU package.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Get-WUOfflineMSU -Destination <string> -KBArticleID <string> [-AcceptAll] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Get-WUOfflineMSU cmdlet to download MSU package from Microsoft Update Catalog website.

## EXAMPLES

## PARAMETERS

### -AcceptAll

Do not ask confirmation for download updates.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Destination

Destination for downloaded files.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -KBArticleID

Finds updates that contain a KBArticleID.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
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

Finds updates that contain a KBArticleID.

## OUTPUTS

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

