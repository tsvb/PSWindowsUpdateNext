---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Remove-WUServiceManager.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Remove-WUServiceManager
---

# Remove-WUServiceManager

## SYNOPSIS

Unregisters a Windows Update service.

## SYNTAX

### __AllParameterSets

```
Remove-WUServiceManager -ServiceID <string> [-ComputerName <string[]>] [-ScheduleJob <datetime>]
 [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Removes the registration of an update service from the Windows Update Agent. Use Get-WUServiceManager to find the ServiceID. For a remote computer, or when -ScheduleJob is given, the cmdlet creates a scheduled task through Invoke-WUJob that runs Remove-WUServiceManager on the target. The module must be installed there.

## EXAMPLES

### Example 1: Unregister Microsoft Update

```powershell
Remove-WUServiceManager -ServiceID 7971f918-a847-4430-9279-4a52d1efe18d
```

```output
Confirm
Are you sure you want to perform this action?
Performing the operation "Unregister Windows Update Service Manager: 7971f918-a847-4430-9279-4a52d1efe18d" on target "PC01".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"):
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

### -ScheduleJob

Runs the removal as a one-time scheduled task at this date and time instead of immediately. The task runs as SYSTEM.

```yaml
Type: System.DateTime
DefaultValue: None
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

Specifies the ID of the service to unregister. Use Get-WUServiceManager to list the IDs.

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

### None

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. It returns no output.

## RELATED LINKS

- [Get-WUServiceManager](Get-WUServiceManager.md)
- [Add-WUServiceManager](Add-WUServiceManager.md)
- [Invoke-WUJob](Invoke-WUJob.md)
