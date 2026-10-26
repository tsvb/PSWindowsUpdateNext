---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Invoke-WUJob
---

# Invoke-WUJob

## SYNOPSIS

Invoke WUJobs by Task Schduler.

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

Use Invoke-WUJobs cmdlet to invoke PSWindowsUpdateNext actions remotly. It Based on TaskScheduler because CreateUpdateDownloader() and CreateUpdateInstaller() methods can't be called from a remote computer - E_ACCESSDENIED.

Note: Because we do not have the ability to interact, is recommended use -AcceptAll for Install-WindowsUpdate.

## EXAMPLES

## PARAMETERS

### -ComputerName

Specify one or more computer names for remote connection.

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

Specify alternative credential.

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

Debuger return original exceptions.

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

Specify EndBoundary for Task Scheduler trigger.

```yaml
Type: System.DateTime
DefaultValue: 01.01.0001 00:00:00
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

Force update old Task Scheduler Job if it's available.

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

Specify if task must be hidden. Default is true.

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

Run Task Scheduler job immediately.

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

Specify PowerShell script that you what to run. Default is {ipmo PSWindowsUpdateNext; Get-WindowsUpdate -AcceptAll -Install | Out-File $Env:TEMP\PSWindowsUpdateNext.log}

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

Specify custom name for Task Scheduler job. Default is 'PSWindowsUpdateNext'.

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

Specify system startup trigger for Task Scheduler job.

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

Specify Time trigger for Task Scheduler job.

```yaml
Type: System.DateTime
DefaultValue: 01.01.0001 00:00:00
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

Specify one or more computer names for remote connection.

## OUTPUTS

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

