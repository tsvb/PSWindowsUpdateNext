---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Add-WUServiceManager.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Add-WUServiceManager
---

# Add-WUServiceManager

## SYNOPSIS

Registers a Windows Update service.

## SYNTAX

### LocalServiceID (Default)

```
Add-WUServiceManager -ServiceID <string> [-SendReport] [-PSWUSettings <hashtable>]
 [-AddServiceFlag <int>] [-AuthorizationCabPath <string>] [-Silent] [-Debuger] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### RemoteServiceID

```
Add-WUServiceManager -ServiceID <string> [-ComputerName <string[]>] [-SendReport]
 [-PSWUSettings <hashtable>] [-Silent] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### MicrosoftUpdate

```
Add-WUServiceManager -MicrosoftUpdate [-ComputerName <string[]>] [-SendReport]
 [-PSWUSettings <hashtable>] [-Silent] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### OfflineSync

```
Add-WUServiceManager -ScanFileLocation <string> [-SendReport] [-PSWUSettings <hashtable>]
 [-ServiceName <string>] [-Silent] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Registers an update service with the Windows Update Agent so that searches can use it. The cmdlet has four ways to name the service: by ID with -ServiceID, Microsoft Update with -MicrosoftUpdate, a service on a remote computer with -ComputerName, or an offline scan package (wsusscn2.cab) with -ScanFileLocation. It restarts the Windows Update service before and after the registration.

The cmdlet combines the old Add-WUServiceManager and Add-WUOfflineSync functions. Registered services appear in Get-WUServiceManager and can be used with the -ServiceID, -MicrosoftUpdate, and -WindowsUpdate parameters of Get-WindowsUpdate.

## EXAMPLES

### Example 1: Register Microsoft Update

```powershell
Add-WUServiceManager -MicrosoftUpdate
```

```output
Confirm
Are you sure you want to perform this action?
Performing the operation "Register Windows Update Service Manager: 7971f918-a847-4430-9279-4a52d1efe18d" on target "PC01".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): Y

ServiceID                            IsManaged IsDefaultAUService Name
---------                            --------- ------------------ ----
7971f918-a847-4430-9279-4a52d1efe18d False     False              Microsoft Update
```

### Example 2: Register a service by its ID

```powershell
Add-WUServiceManager -ServiceID 7971f918-a847-4430-9279-4a52d1efe18d
```

Registers the service with the default flags (online registration).

### Example 3: Register an offline scan package

```powershell
Add-WUServiceManager -ScanFileLocation C:\Offline\wsusscn2.cab -ServiceName 'Offline Sync Service'
```

Registers the scan package as an update service that you can then search with Get-WindowsUpdate -ServiceID.

## PARAMETERS

### -AddServiceFlag

Specifies how the service is registered, as a combination of these values: 1 (asfAllowPendingRegistration), 2 (asfAllowOnlineRegistration), and 4 (asfRegisterServiceWithAU). The default value 2 allows online registration.

```yaml
Type: System.Int32
DefaultValue: 2
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: LocalServiceID
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -AuthorizationCabPath

Specifies the path of the Microsoft-signed cabinet file (.cab) that has the information required to register the service. If you omit it, the Windows Update Agent looks for the authorization cabinet file when a network connection is available.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: LocalServiceID
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ComputerName

Specifies one or more computers on which to register the service. The default is the local computer. Available in the RemoteServiceID and MicrosoftUpdate parameter sets. Accepts pipeline input by value and by property name.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: RemoteServiceID
  Position: Named
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: MicrosoftUpdate
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

### -MicrosoftUpdate

Registers Microsoft Update (ServiceID 7971f918-a847-4430-9279-4a52d1efe18d).

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: MicrosoftUpdate
  Position: Named
  IsRequired: true
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

### -ScanFileLocation

Specifies the path of the offline scan file (wsusscan.cab or wsusscn2.cab) to register as an update service.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: OfflineSync
  Position: Named
  IsRequired: true
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

Specifies the ID (GUID) of the service to register. Examples: Windows Update 9482f4b4-e343-43b6-b170-9a65bc822c77, Microsoft Update 7971f918-a847-4430-9279-4a52d1efe18d, Windows Store 117cab2d-82b1-4b5a-a08c-4d62dbee7782, Windows Server Update Service 3da21691-e39d-4da6-8a4b-b43877bcb1b7.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: LocalServiceID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: RemoteServiceID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ServiceName

Specifies the name under which the offline scan package is registered. The default is 'Offline Sync Service'.

```yaml
Type: System.String
DefaultValue: Offline Sync Service
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: OfflineSync
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Silent

Does not return the registered service.

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

You can pipe computer names to ComputerName in the RemoteServiceID and MicrosoftUpdate parameter sets.

## OUTPUTS

### PSWindowsUpdateNext.ServiceManager

The registered service, as the IUpdateService COM object with RegistrationStateName (Not Registered, Registration Pending, or Registered) and ComputerName properties added.

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. The cmdlet has a confirmation impact of High, so it asks for confirmation. -AddServiceFlag and -AuthorizationCabPath are available only with -ServiceID on the local computer.

## RELATED LINKS

- [Get-WUServiceManager](Get-WUServiceManager.md)
- [Remove-WUServiceManager](Remove-WUServiceManager.md)
- [Get-WindowsUpdate](Get-WindowsUpdate.md)
