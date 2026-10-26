---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Invoke-WUJob.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Invoke-WUJob
---

# Invoke-WUJob

## SYNOPSIS

Runs a PowerShell script on local or remote computers through a Task Scheduler job.

## SYNTAX

### RunNow (Default)

```
Invoke-WUJob -RunNow [-ComputerName <string[]>] [-Credential <pscredential>] [-TaskName <string>]
 [-Hidden] [-EndBoundary <datetime>] [-Script <string>] [-Force] [-Debuger] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### TriggerDate

```
Invoke-WUJob -TriggerDate <datetime> [-ComputerName <string[]>] [-Credential <pscredential>]
 [-TaskName <string>] [-Hidden] [-EndBoundary <datetime>] [-Script <string>] [-Force] [-Debuger]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### TriggerAtStart

```
Invoke-WUJob -TriggerAtStart [-ComputerName <string[]>] [-Credential <pscredential>]
 [-TaskName <string>] [-Hidden] [-EndBoundary <datetime>] [-Script <string>] [-Force] [-Debuger]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Registers a scheduled task on each computer and runs your script in it. The Windows Update Agent refuses download and install calls from a remote session (E_ACCESSDENIED), so PSWindowsUpdateNext uses Task Scheduler to run those operations; Get-WindowsUpdate, Remove-WindowsUpdate, and other cmdlets call this cmdlet internally.

The task runs as SYSTEM with the highest privileges. It starts powershell.exe with -NoProfile -NonInteractive -EncodedCommand, so quotation marks in -Script cannot change the command line; the readable script is stored in the task description. The task is hidden by default. Its trigger expires (see -EndBoundary), and Task Scheduler deletes the task 30 minutes after that.

Because the script cannot interact with a user, use -AcceptAll with Install-WindowsUpdate. By default the script is: ipmo PSWindowsUpdateNext; Get-WindowsUpdate -AcceptAll -Install | Out-File $Env:TEMP\PSWindowsUpdateNext.log

## EXAMPLES

### Example 1: Install updates on a remote computer at 6:00 PM

```powershell
Invoke-WUJob -ComputerName PC02 -Script "ipmo PSWindowsUpdateNext; Install-WindowsUpdate -AcceptAll | Out-File C:\Temp\PSWindowsUpdateNext.log" -TriggerDate (Get-Date -Hour 18 -Minute 0 -Second 0)
```

```output
Confirm
Are you sure you want to perform this action?
Performing the operation "Invoke WU job: powershell.exe -NoProfile -NonInteractive -Command "ipmo PSWindowsUpdateNext; Install-WindowsUpdate -AcceptAll | Out-File C:\Temp\PSWindowsUpdateNext.log"" on target "PC02".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): Y
```

### Example 2: Run the default script now

```powershell
Invoke-WUJob -ComputerName PC02 -RunNow -Confirm:$false
```

Registers the task and starts it about five seconds later.

### Example 3: Run a script at the next startup

```powershell
Invoke-WUJob -TriggerAtStart -TaskName PSWindowsUpdateNext_Startup -Script 'ipmo PSWindowsUpdateNext; Get-WUHistory -Last 5 | Out-File C:\Temp\history.txt'
```

## PARAMETERS

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

Specifies a user account that has permission to register tasks on the computers. The task itself always runs as SYSTEM.

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

### -EndBoundary

Specifies when the trigger of the task expires. The value must be in the future. The default is one hour after the start for -RunNow and -TriggerDate, and one day from now for -TriggerAtStart.

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

### -Force

Stops a running task that has the same name and registers the new one. Without it, the cmdlet stops with an error if a task with that name is still running.

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

### -Hidden

Hides the task in Task Scheduler. The default is $true, so the task is hidden. Use -Hidden:$false to make it visible.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: True
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

### -RunNow

Runs the task immediately. The task is registered and started about five seconds later. This is the default parameter set.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: RunNow
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Script

Specifies the PowerShell script that the task runs. The script is encoded and passed to powershell.exe with -EncodedCommand. The default is: ipmo PSWindowsUpdateNext; Get-WindowsUpdate -AcceptAll -Install | Out-File $Env:TEMP\PSWindowsUpdateNext.log

```yaml
Type: System.String
DefaultValue: ipmo PSWindowsUpdateNext; Get-WindowsUpdate -AcceptAll -Install | Out-File $Env:TEMP\PSWindowsUpdateNext.log
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

Specifies the name of the scheduled task. A task with the same name is replaced. The default is 'PSWindowsUpdateNext'. Get-WindowsUpdate and the other cmdlets that call this cmdlet use their own task names.

```yaml
Type: System.String
DefaultValue: PSWindowsUpdateNext
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

### -TriggerAtStart

Runs the task at system startup, after a delay of 30 seconds.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: TriggerAtStart
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -TriggerDate

Runs the task once at this date and time. The value must be in the future.

```yaml
Type: System.DateTime
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: TriggerDate
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

This cmdlet requires an elevated session and stops with an error when the session is not elevated. It checks that PSWindowsUpdateNext 2.0.0.0 or later is installed on the destination and reports an error if it is not. The task description has the form 'TaskName: Script'. The cmdlet also deletes expired tasks that PSWindowsUpdateNext created. It returns no output. Anyone who can run this cmdlet can run code as SYSTEM on the target computers.

## RELATED LINKS

- [Get-WUJob](Get-WUJob.md)
- [Get-WindowsUpdate](Get-WindowsUpdate.md)
- [Enable-WURemoting](Enable-WURemoting.md)
