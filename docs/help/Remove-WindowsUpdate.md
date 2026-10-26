---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Remove-WindowsUpdate
---

# Remove-WindowsUpdate

## SYNOPSIS

Uninstall update.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Remove-WindowsUpdate -KBArticleID <string> [-AutoReboot] [-ComputerName <string[]>] [-Debuger]
 [-IgnoreReboot] [-ScheduleJob <DateTime>] [-ScheduleReboot <DateTime>] [-WUSAMode]
 [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_2

```
Remove-WindowsUpdate -UpdateID <string> [-AutoReboot] [-ComputerName <string[]>] [-Debuger]
 [-IgnoreReboot] [-ScheduleJob <DateTime>] [-ScheduleReboot <DateTime>] [-WUSAMode]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Remove-WindowsUpdate to uninstall update.

## EXAMPLES

## PARAMETERS

### -AutoReboot

Do not ask for reboot if it needed.

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

### -ComputerName

Specify one or more computer names for remote connection.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Debuger

Debuger return original exceptions.

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

### -HotFixID

KBArticleID that will be uninstalled.

This is an alias of the KBArticleID parameter.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- HotFixID
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -IgnoreReboot

Do not ask for reboot if it needed, but do not reboot automaticaly.

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

### -KBArticleID

KBArticleID that will be uninstalled.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- HotFixID
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScheduleJob

Specify schedule time job.

```yaml
Type: System.DateTime
DefaultValue: 01.01.0001 00:00:00
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScheduleReboot

Specify time when system will be rebooted.

```yaml
Type: System.DateTime
DefaultValue: 01.01.0001 00:00:00
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UpdateID

Update ID that will be uninstalled.

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

### -WUSAMode

Wse wusa.exe instead of WU Api.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

Specify one or more computer names for remote connection.

## OUTPUTS

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

