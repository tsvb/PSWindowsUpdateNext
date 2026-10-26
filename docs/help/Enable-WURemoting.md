---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Enable-WURemoting.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Enable-WURemoting
---

# Enable-WURemoting

## SYNOPSIS

Enables the firewall rules and settings that PSWindowsUpdateNext remoting needs.

## SYNTAX

### __AllParameterSets

```
Enable-WURemoting [-LocalAccountTokenFilterPolicy] [-WinRMPublic] [-Debuger] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Prepares the local computer to be managed remotely by PSWindowsUpdateNext. The cmdlet:

- creates or enables the Windows Firewall rule group PSWindowsUpdateNext, with an inbound rule that allows RPC dynamic ports for dllhost.exe (needed for DCOM calls to the Windows Update Agent);
- enables the Remote Scheduled Tasks Management and Windows Management Instrumentation (WMI) firewall rule groups;
- sets the Remote Registry service start mode to Automatic if it is Disabled, and starts it if it is stopped;
- if the WinRM client TrustedHosts list does not already contain *, starts the WinRM service, sets the LocalAccountTokenFilterPolicy registry value to 1 and the ForceGuest value to 0, and adds * to TrustedHosts.

These changes lower the security of the computer. Use the cmdlet only in a managed environment, and review the notes before you run it.

## EXAMPLES

### Example 1: Enable remoting on the local computer

```powershell
Enable-WURemoting -Verbose
```

Creates or enables the firewall rules, prepares Remote Registry and WinRM, and shows each step.

### Example 2: Allow WinRM from any subnet

```powershell
Enable-WURemoting -WinRMPublic -LocalAccountTokenFilterPolicy
```

Also adds an inbound rule for TCP port 5985 that is not limited to the local subnet, and sets LocalAccountTokenFilterPolicy to 1.

## PARAMETERS

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

### -LocalAccountTokenFilterPolicy

Sets the LocalAccountTokenFilterPolicy registry value to 1 so that remote connections by local administrator accounts get an elevated token. This disables the UAC remote restrictions for local accounts. See the notes for the cases in which the value is also set without this switch.

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

### -WinRMPublic

Adds an inbound firewall rule that allows WinRM (TCP port 5985) from all subnets. By default WinRM is reachable only from the local subnet. Computers in a workgroup need this.

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

### None

## OUTPUTS

### None

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. -WhatIf and -Confirm are accepted but the cmdlet does not check them, so the changes are always made. LocalAccountTokenFilterPolicy=1 disables the UAC remote restrictions for local administrator accounts, and a TrustedHosts value of * lets this computer send credentials to any host over WinRM. The ComputerName and Credential parameters are not available on this cmdlet in the current release, so it always acts on the local computer. Update-WUModule -Local runs this cmdlet on the remote computer after copying the module.

## RELATED LINKS

- [Update-WUModule](Update-WUModule.md)
- [Invoke-WUJob](Invoke-WUJob.md)
