---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Update-WUModule.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Update-WUModule
---

# Update-WUModule

## SYNOPSIS

Installs or updates PSWindowsUpdateNext on remote computers.

## SYNTAX

### Online (Default)

```
Update-WUModule -Online [-ComputerName <string[]>] [-Credential <pscredential>] [-Debuger] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### Local

```
Update-WUModule -ComputerName <string[]> -Local [-Credential <pscredential>] [-Debuger] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Deploys the module to other computers so that remote operations such as Invoke-WUJob can run there. With -Online, the cmdlet runs Install-Module -Name PSWindowsUpdateNext -Force on the remote computer through PowerShell remoting, after it installs the NuGet package provider. The module must be available in the PowerShell Gallery. With -Local, the cmdlet copies the newest installed copy of the module on this computer to the same path on the remote computer over the administrative share (\\computer\C$\...) and then runs Enable-WURemoting there.

The cmdlet reports the remote module version before and after the update in the verbose stream.

## EXAMPLES

### Example 1: Install the module from the PowerShell Gallery

```powershell
Update-WUModule -ComputerName PC02 -Online
```

Installs or updates the module on PC02 from the PowerShell Gallery.

### Example 2: Copy the module from this computer

```powershell
Update-WUModule -ComputerName PC02 -Local
```

Copies the module to PC02 over SMB. This requires access to the administrative share on PC02.

## PARAMETERS

### -ComputerName

Specifies one or more computers to update. With -Online the default is the local computer; with -Local the parameter is required. Accepts pipeline input by value and by property name.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Local
  Position: Named
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: Online
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

Specifies a user account that has permission to update the module on the computers. The default is the current user.

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

### -Local

Copies the module from this computer to the remote computer over the administrative share. Requires SMB access to the destination.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Local
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Online

Installs the module from the PowerShell Gallery. This is the default parameter set.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Online
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

### System.Object

The verbose and error records of the remote command.

## NOTES

The cmdlet has a confirmation impact of High, so it asks for confirmation. Remoting uses Negotiate authentication. With -Local, Enable-WURemoting changes firewall rules and remoting settings on the remote computer; see its notes.

## RELATED LINKS

- [Enable-WURemoting](Enable-WURemoting.md)
- [Invoke-WUJob](Invoke-WUJob.md)
- [Get-WUApiVersion](Get-WUApiVersion.md)
