---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Get-WUSettings.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WUSettings
---

# Get-WUSettings

## SYNOPSIS

Gets the Windows Update client policy settings.

## SYNTAX

### __AllParameterSets

```
Get-WUSettings [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>] [-Debuger]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Reads the Windows Update policy values from HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate and its AU subkey and returns them as one object. The AUOptions and ScheduledInstallDay values are translated to readable text. Nothing is returned, and a verbose message says so, when no policy is configured.

These are the settings that Set-WUSettings writes.

## EXAMPLES

### Example 1: Get the current policy settings

```powershell
Get-WUSettings
```

```output
ComputerName                  : PC01
WUServer                      : https://wsus.contoso.com:8531
WUStatusServer                : https://wsus.contoso.com:8531
DetectionFrequencyEnabled     : 1
DetectionFrequency            : 2
NoAutoUpdate                  : 0
AUOptions                     : 2 - Notify before download
ScheduledInstallDay           : 0 - Every Day
ScheduledInstallTime          : 4
UseWUServer                   : 1
```

Only the values that exist in the registry are shown.

## PARAMETERS

### -ComputerName

Specifies one or more computers to run against. The default is the local computer. Remote computers are reached through the remote registry, so the Remote Registry service must be running there; see Enable-WURemoting. Accepts pipeline input by value and by property name.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
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

### -Debuger

Returns the original exception instead of the module's short error message when an operation fails. Set $DebugPreference to 'Continue' for more diagnostic output.

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

### -PSWUSettings

Specifies the email settings used by -SendReport, as a hashtable. SmtpServer, From, and To are required. Optional keys are Port (default 25), EnableSsl (default $false), Subject, Properties, and Style. Properties is an array of property names to include, '*' for every property of the first result object, or '**' for every property of every result object. Style is Table or List. Keys that you do not supply are read from PSWUSettings.xml in the module folder (see Set-PSWUSettings). The SMTP credential is read from Windows Credential Manager.

```yaml
Type: System.Collections.Hashtable
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

### -SendReport

Sends the results by email when the cmdlet finishes. The SMTP settings come from -PSWUSettings or from the PSWUSettings.xml file saved by Set-PSWUSettings. If the settings are incomplete, an error is written and no report is sent.

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

### System.String[]

You can pipe computer names to ComputerName.

## OUTPUTS

### PSWindowsUpdateNext.WUSettings

An object with ComputerName and one property for each policy value found in the registry.

## NOTES

The cmdlet asks for confirmation before it runs when $ConfirmPreference is set to Medium or lower.

## RELATED LINKS

- [Set-WUSettings](Set-WUSettings.md)
- [Get-WUServiceManager](Get-WUServiceManager.md)
