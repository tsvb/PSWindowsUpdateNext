---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Get-WUServiceManager.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WUServiceManager
---

# Get-WUServiceManager

## SYNOPSIS

Gets the registered Windows Update services.

## SYNTAX

### __AllParameterSets

```
Get-WUServiceManager [-ComputerName <string[]>] [-Credential <pscredential>] [-SendReport]
 [-PSWUSettings <hashtable>] [-ServiceID <string>] [-Debuger] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Lists the update services that are registered with the Windows Update Agent, such as Windows Update, Microsoft Update, a WSUS server, or an offline scan package. The IsDefaultAUService property marks the service that Automatic Updates uses, and IsManaged marks services that policy manages.

## EXAMPLES

### Example 1: List the update services

```powershell
Get-WUServiceManager
```

```output
ServiceID                            IsManaged IsDefaultAUService Name
---------                            --------- ------------------ ----
9482f4b4-e343-43b6-b170-9a65bc822c77 False     False              Windows Update
7971f918-a847-4430-9279-4a52d1efe18d False     False              Microsoft Update
3da21691-e39d-4da6-8a4b-b43877bcb1b7 True      True               Windows Server Update Service
```

### Example 2: Get one service by its ID

```powershell
Get-WUServiceManager -ServiceID 7971f918-a847-4430-9279-4a52d1efe18d
```

## PARAMETERS

### -ComputerName

Specifies one or more computers to run against. The default is the local computer. Remote computers are reached over DCOM, so they must allow remote administration; see Enable-WURemoting. Accepts pipeline input by value and by property name.

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

### -Credential

Specifies a user account that has permission to query the computers. The default is the current user.

```yaml
Type: System.Management.Automation.PSCredential
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

### -ServiceID

Returns only the service with this ID. When omitted, all registered services are returned.

```yaml
Type: System.String
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

### System.String[]

You can pipe computer names to ComputerName.

## OUTPUTS

### PSWindowsUpdateNext.ServiceManager

An object that wraps the IUpdateService COM object and adds ComputerName.

## NOTES

The cmdlet writes a warning when the session is not elevated.

## RELATED LINKS

- [Add-WUServiceManager](Add-WUServiceManager.md)
- [Remove-WUServiceManager](Remove-WUServiceManager.md)
- [Get-WindowsUpdate](Get-WindowsUpdate.md)
