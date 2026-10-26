---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Enable-WURemoting
---

# Enable-WURemoting

## SYNOPSIS

Enable firewall rules for PSWindowsUpdateNext remoting.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Enable-WURemoting [-Debuger] [-LocalAccountTokenFilterPolicy] [-WinRMPublic] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Enable-WURemoting cmdlet to enable nessesery firewall rules for PSWindowsUpdateNext remoting.

## EXAMPLES

## PARAMETERS

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

### -LocalAccountTokenFilterPolicy

Set LocalAccountTokenFilterPolicy registry entry to builds an elevated token on the target remote computer.

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

### -WinRMPublic

Enable WinRM Public access from all subnets. Default access is only enabled from local subnet. Required for workgroup computers.

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

## OUTPUTS

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

