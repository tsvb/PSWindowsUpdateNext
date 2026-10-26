---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Get-WUJob.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WUJob
---

# Get-WUJob

## SYNOPSIS

Gets the Task Scheduler jobs created by the module.

## SYNTAX

### __AllParameterSets

```
Get-WUJob [-ComputerName <string[]>] [-Credential <pscredential>] [-PSWUSettings <hashtable>]
 [-TaskName <string>] [-ClearExpired] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  Clear-WUJob

## DESCRIPTION

Lists the scheduled tasks that PSWindowsUpdateNext registered, for example through Invoke-WUJob, or through Get-WindowsUpdate with -ScheduleJob or when it targets a remote computer. Only tasks whose source is PSWindowsUpdateNext are returned, unless you name a task with -TaskName.

The alias Clear-WUJob runs the same cmdlet with the clean-up behavior described in the notes.

## EXAMPLES

### Example 1: Get the jobs on a remote computer

```powershell
Get-WUJob -ComputerName PC02
```

```output
ComputerName Name                 Action
------------ ----                 ------
PC02         PSWindowsUpdateNext  powershell.exe -NoProfile -NonInteractive -EncodedCommand ...
```

Returns each registered job together with its state.

### Example 2: Get a specific job

```powershell
Get-WUJob -TaskName PSWindowsUpdateNext_History
```

## PARAMETERS

### -ClearExpired

After listing the jobs, deletes the module's jobs whose trigger end boundary has passed and that are not running. The Clear-WUJob alias turns this switch on.

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

### -ComputerName

Specifies one or more computers to run against. The default is the local computer. Remote computers are reached through the Task Scheduler service, so they must allow remote scheduled-task management; see Enable-WURemoting. Accepts pipeline input by value and by property name.

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

Specifies a user account that has permission to connect to the computers and read their Task Scheduler. The default is the current user.

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

Not used. Get-WUJob has no -SendReport parameter, so it never sends a report.

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

### -TaskName

Specifies the name of one job to return. When omitted, all jobs created by PSWindowsUpdateNext are returned.

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

### PSWindowsUpdateNext.WUJob

An object that wraps the IRegisteredTask COM object and adds ComputerName and StateName (Disabled, Queued, Ready, Running, or Unknown).

## NOTES

After it lists the jobs, the cmdlet deletes the module's jobs whose trigger end boundary has passed and that are not running. This clean-up runs only with -ClearExpired or when you call the Clear-WUJob alias. Jobs whose end boundary cannot be read are left in place. Earlier releases inverted the switch and cleaned up only when it was not given. The deletion honors -WhatIf and -Confirm. The cmdlet writes a warning when the session is not elevated.

## RELATED LINKS

- [Invoke-WUJob](Invoke-WUJob.md)
- [Get-WindowsUpdate](Get-WindowsUpdate.md)
