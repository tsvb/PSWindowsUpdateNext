---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Add-WUServiceManager
---

# Add-WUServiceManager

## SYNOPSIS

Register a new Windows Update API Service Manager.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Add-WUServiceManager -ServiceID <string> [-ComputerName <string[]>] [-Debuger]
 [-PSWUSettings <Hashtable>] [-SendReport] [-Silent] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_2

```
Add-WUServiceManager -MicrosoftUpdate [-ComputerName <string[]>] [-Debuger]
 [-PSWUSettings <Hashtable>] [-SendReport] [-Silent] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_3

```
Add-WUServiceManager -ServiceID <string> [-AddServiceFlag <int>] [-AuthorizationCabPath <string>]
 [-Debuger] [-PSWUSettings <Hashtable>] [-SendReport] [-Silent] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_4

```
Add-WUServiceManager -ScanFileLocation <string> [-Debuger] [-PSWUSettings <Hashtable>] [-SendReport]
 [-ServiceName <string>] [-Silent] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Add-WUServiceManager cmdlet to register new Windows Update Service Manager.

It's combination old 'Add-WUServiceManager' and 'Add-WUOfflineSync' functions to register online and offline ServiceManager

## EXAMPLES

## PARAMETERS

### -AddServiceFlag

A combination of AddServiceFlag values: \r\n0x1 - asfAllowPendingRegistration \r\n0x2 - asfAllowOnlineRegistration \r\n0x4 - asfRegisterServiceWithAU

```yaml
Type: System.Int32
DefaultValue: 2
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -AuthorizationCabPath

The path of the Microsoft signed local cabinet file (.cab) that has the information that is required for a service registration. If empty, the update agent searches for the authorization cabinet file (.cab) during service registration when a network connection is available.

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

### -MicrosoftUpdate

Register Microsoft Update Service Manager - '7971f918-a847-4430-9279-4a52d1efe18d'

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

### -ScanFileLocation

Path to Windows Update offline scan file (wsusscan.cab or wsusscn2.cab).

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

### -ServiceID

An identifier for the service to be registered.

Examples Of ServiceID: \r\n \r\n -- Windows Update 9482f4b4-e343-43b6-b170-9a65bc822c77 \r\n -- Microsoft Update 7971f918-a847-4430-9279-4a52d1efe18d \r\n -- Windows Store 117cab2d-82b1-4b5a-a08c-4d62dbee7782 \r\n -- Windows Server Update Service 3da21691-e39d-4da6-8a4b-b43877bcb1b7

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

### -ServiceName

Name under which it will be registered Windows Update offline service. Default name is 'Offline Sync Service'.

```yaml
Type: System.String
DefaultValue: Offline Sync Service
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Silent

Don't return output.

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

### PSWindowsUpdateNext.ServiceManager

OutputType for ServiceManager objects.

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

