---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WURebootStatus
---

# Get-WURebootStatus

## SYNOPSIS

Get Windows Update reboot status.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Get-WURebootStatus -AutoReboot [-ComputerName <string[]>] [-Debuger] [-PSWUSettings <Hashtable>]
 [-SendReport] [-Silent] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_2

```
Get-WURebootStatus -ScheduleReboot <DateTime> [-ComputerName <string[]>] [-Debuger]
 [-PSWUSettings <Hashtable>] [-SendReport] [-Silent] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_3

```
Get-WURebootStatus [-ComputerName <string[]>] [-Debuger] [-PSWUSettings <Hashtable>] [-SendReport]
 [-Silent] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_4

```
Get-WURebootStatus -CancelReboot [-ComputerName <string[]>] [-Debuger] [-PSWUSettings <Hashtable>]
 [-SendReport] [-Silent] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Get-WURebootStatus cmdlet to check if reboot is needed.

## EXAMPLES

## PARAMETERS

### -AutoReboot

Do not ask for reboot if it needed, but do it now.

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

### -CancelReboot

Cancel scheduled reboot.

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

### -PSWUSettings

Required parameter for -SendReport.

Passes the parameters (as hashtable) necessary to send the report: \r\n@{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25];[Subject="Alternative Subject"];[Properties="Alternative object properties"];[Style="Table|List"]}

Send parameters can also be saved to a PSWUSettings.xml file in ModuleBase path: \r\nExport-Clixml @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25]}"

```yaml
Type: System.Collections.Hashtable
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScheduleReboot

Specify schedule time for reboot.

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

### -SendReport

Send report email to specific recipients.

Requires the parameter -PSWUSettings or declare the PSWUSettings.xml file in ModuleBase path.

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

### -Silent

Return true/false only.

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

### System.Management.Automation.SwitchParameter

Return true/false only.

## OUTPUTS

### PSWindowsUpdateNext.RebootStatus

OutputType for RebootStatus objects.

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

