---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WULastResults
---

# Get-WULastResults

## SYNOPSIS

Get Windows Update results.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Get-WULastResults [-ComputerName <string[]>] [-Debuger] [-PSWUSettings <Hashtable>] [-SendReport]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Get-WULastResults cmdlet to get Windows Update LastSearchSuccessDate and LastInstallationSuccessDate.

## EXAMPLES

## PARAMETERS

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

Specify one or more computer names for remote connection.

## OUTPUTS

### PSWindowsUpdateNext.LastResults

OutputType for LastResult objects.

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

