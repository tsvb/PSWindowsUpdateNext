---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Remove-WindowsUpdate.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Remove-WindowsUpdate
---

# Remove-WindowsUpdate

## SYNOPSIS

Uninstalls a Windows update.

## SYNTAX

### WUSAMode

```
Remove-WindowsUpdate -KBArticleID <string> [-ComputerName <string[]>] [-ScheduleJob <datetime>]
 [-AutoReboot] [-IgnoreReboot] [-ScheduleReboot <datetime>] [-WUSAMode] [-Debuger] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### WUApiMode

```
Remove-WindowsUpdate -UpdateID <string> [-ComputerName <string[]>] [-ScheduleJob <datetime>]
 [-AutoReboot] [-IgnoreReboot] [-ScheduleReboot <datetime>] [-WUSAMode] [-Debuger] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  Uninstall-WindowsUpdate

## DESCRIPTION

Uninstalls an installed update that you identify by KB article number (-KBArticleID) or update ID (-UpdateID). By default the cmdlet uses the Windows Update Agent API. With -WUSAMode and -KBArticleID it runs wusa.exe /uninstall /quiet /norestart instead.

For a remote computer, or when you use -ScheduleJob, the cmdlet registers a scheduled task through Invoke-WUJob that runs Remove-WindowsUpdate on the target. The module must be installed there. The alias Uninstall-WindowsUpdate runs this cmdlet.

When an uninstallation through the API requires a restart, the cmdlet asks whether to restart, unless you use -AutoReboot, -IgnoreReboot, or -ScheduleReboot.

## EXAMPLES

### Example 1: Uninstall an update by KB number

```powershell
Remove-WindowsUpdate -KBArticleID KB5034123
```

Looks for the update among the installed updates and uninstalls it after confirmation. You can also run Uninstall-WindowsUpdate -KBArticleID KB5034123.

### Example 2: Uninstall an update with wusa.exe

```powershell
Remove-WindowsUpdate -KBArticleID KB5034123 -WUSAMode
```

Runs wusa.exe /uninstall /KB:5034123 /quiet /norestart instead of using the API.

### Example 3: Uninstall an update on a remote computer and restart

```powershell
Remove-WindowsUpdate -ComputerName PC02 -KBArticleID KB5034123 -AutoReboot
```

Registers a scheduled task on PC02 that runs the uninstallation and restarts the computer if needed.

### Example 4: Uninstall an update by update ID

```powershell
Remove-WindowsUpdate -UpdateID 2fbbe4d8-3a5f-4b0e-9d16-0c2b1d8f2a11 -IgnoreReboot
```

## PARAMETERS

### -AutoReboot

Restarts the computer without asking when a restart is required after the uninstallation.

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

### -IgnoreReboot

Does not restart the computer and does not ask. A message says that a restart is required.

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

### -KBArticleID

Specifies the KB article number of the update to uninstall, for example KB5034123. This parameter has the alias HotFixID. With the Windows Update Agent API the first installed update that has this KB number is uninstalled.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- HotFixID
ParameterSets:
- Name: WUSAMode
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScheduleJob

Runs the uninstallation as a one-time scheduled task at this date and time instead of immediately. The task runs as SYSTEM. The value must be in the future.

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

### -ScheduleReboot

Schedules a restart for this date and time when a restart is required after the uninstallation. The value must be in the future.

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

### -UpdateID

Specifies the update ID (GUID) of the update to uninstall. The Windows Update Agent API is used; -WUSAMode does not apply to this parameter.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: WUApiMode
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

### -WUSAMode

Uses wusa.exe instead of the Windows Update Agent API to uninstall the update that -KBArticleID names.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe computer names to ComputerName.

## OUTPUTS

### System.Object

When the uninstallation runs through the API, the IInstallationResult COM object of the Windows Update Agent. With -WUSAMode, a remote computer, or -ScheduleJob, the output of Invoke-WUJob (none for the local wusa.exe call).

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. The cmdlet has a confirmation impact of High, so it asks for confirmation. Restarts run shutdown.exe with the force option (-f).

## RELATED LINKS

- [Get-WindowsUpdate](Get-WindowsUpdate.md)
- [Get-WUHistory](Get-WUHistory.md)
- [Invoke-WUJob](Invoke-WUJob.md)
