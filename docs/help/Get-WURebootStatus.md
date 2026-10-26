---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Get-WURebootStatus.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WURebootStatus
---

# Get-WURebootStatus

## SYNOPSIS

Gets the Windows Update reboot status and optionally restarts the computer.

## SYNTAX

### ManualReboot (Default)

```
Get-WURebootStatus [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>] [-Silent]
 [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### AutoReboot

```
Get-WURebootStatus -AutoReboot [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>]
 [-Silent] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ScheduleReboot

```
Get-WURebootStatus -ScheduleReboot <datetime> [-ComputerName <string[]>] [-SendReport]
 [-PSWUSettings <hashtable>] [-Silent] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### CancelReboot

```
Get-WURebootStatus -CancelReboot [-ComputerName <string[]>] [-SendReport]
 [-PSWUSettings <hashtable>] [-Silent] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Checks whether a restart is pending because of Windows Update. On the local computer the cmdlet asks the Windows Update Agent; for a remote computer it looks for the RebootRequired key in the Windows Update registry settings.

When a restart is required, the default behavior is to ask whether to restart now. Use -AutoReboot to restart without asking, -ScheduleReboot to restart at a given time, or -Silent to return only True or False and never prompt. -CancelReboot cancels a restart that was scheduled earlier.

## EXAMPLES

### Example 1: Check whether a restart is required

```powershell
Get-WURebootStatus
```

```output
Reboot is required. Do it now? [Y / N] (default is 'N')
ComputerName RebootRequired RebootScheduled
------------ -------------- ---------------
PC01         True
```

Reports the status and, because a restart is required, asks whether to restart now.

### Example 2: Schedule the restart

```powershell
Get-WURebootStatus -ScheduleReboot (Get-Date -Hour 18 -Minute 0 -Second 0)
```

If a restart is required, it is scheduled for 6:00 PM today.

### Example 3: Return only a Boolean value

```powershell
if (Get-WURebootStatus -Silent) { 'Restart pending' }
```

### Example 4: Cancel a scheduled restart

```powershell
Get-WURebootStatus -CancelReboot
```

## PARAMETERS

### -AutoReboot

Restarts the computer without asking when a restart is required.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: AutoReboot
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CancelReboot

Cancels a scheduled restart.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: CancelReboot
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ComputerName

Specifies one or more computers to check. The default is the local computer. Remote computers are checked through the remote registry and restarted with shutdown.exe.

```yaml
Type: System.String[]
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

### -ScheduleReboot

Schedules a restart for the given date and time when a restart is required. Any restart that was scheduled earlier is canceled first.

```yaml
Type: System.DateTime
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ScheduleReboot
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

### -Silent

Returns only True or False and does not ask whether to restart. Accepts pipeline input by value and by property name.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: AutoReboot
  Position: Named
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: ScheduleReboot
  Position: Named
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: ManualReboot
  Position: Named
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: CancelReboot
  Position: Named
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
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

### System.Management.Automation.SwitchParameter

You can pipe a value to -Silent.

## OUTPUTS

### PSWindowsUpdateNext.RebootStatus

An object with ComputerName, RebootRequired, and RebootScheduled properties. With -Silent, a Boolean value (System.Boolean) is returned instead.

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. Restarts run shutdown.exe with the force option (-f), so applications are closed without saving.

## RELATED LINKS

- [Get-WindowsUpdate](Get-WindowsUpdate.md)
- [Get-WUInstallerStatus](Get-WUInstallerStatus.md)
