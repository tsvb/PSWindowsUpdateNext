---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Set-PSWUSettings.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Set-PSWUSettings
---

# Set-PSWUSettings

## SYNOPSIS

Saves the email settings that PSWindowsUpdateNext uses for reports.

## SYNTAX

### __AllParameterSets

```
Set-PSWUSettings [-Credential <pscredential>] [-SendReport] [-SmtpServer <string>] [-SmtpPort <int>]
 [-SmtpEnableSsl <bool>] [-SmtpCredential <pscredential>] [-SmtpTo <string>] [-SmtpFrom <string>]
 [-SmtpSubject <string>] [-Properties <string>] [-Style <string>] [-SaveAsSystem <string>]
 [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Stores the SMTP settings used by the -SendReport parameter of the other cmdlets. Server, port, SSL, recipient, sender, subject, properties, and style are saved in PSWUSettings.xml in the module folder (next to PSWindowsUpdateNext.psd1). Only the settings you specify are changed; the others keep their values.

A credential given with -SmtpCredential is saved in Windows Credential Manager under the target PSWindowsUpdateNext. Scheduled tasks run as SYSTEM, which has its own Credential Manager, so the cmdlet also registers a short-lived task through Invoke-WUJob that saves the credential for SYSTEM. The password is passed to that task protected with DPAPI (machine scope) through -SaveAsSystem.

## EXAMPLES

### Example 1: Use Microsoft 365 as the SMTP server

```powershell
Set-PSWUSettings -SmtpServer smtp.office365.com -SmtpPort 587 -SmtpEnableSsl $true -SmtpSubject 'Windows Update report' -SmtpTo admins@contoso.com -SmtpFrom wu-reports@contoso.com -SmtpCredential (Get-Credential wu-reports@contoso.com)
```

Saves the settings and the credential. Cmdlets that you run later with -SendReport use them.

### Example 2: Include every property and use the list layout

```powershell
Set-PSWUSettings -Properties '**' -Style List
```

Reports then show every property of every result object as a list.

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

### -Credential

Specifies a user account that has permission to register the task that saves the SMTP credential for SYSTEM. The default is the current user.

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

### -Properties

Specifies which properties of the result objects the report contains and saves it in PSWUSettings.xml. Use '*' for all properties of the first object or '**' for all properties of all objects. To report a list of property names, pass a Properties array in the -PSWUSettings hashtable of the cmdlet that sends the report.

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

### -SaveAsSystem

Used internally. Contains the SMTP credential, protected with DPAPI for the local computer, that the task registered by this cmdlet stores in the Credential Manager of the SYSTEM account. Do not set it yourself.

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

### -SendReport

Sends the saved settings by email as a test report after they are saved.

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

### -SmtpCredential

Specifies the credential for the SMTP server and saves it in Windows Credential Manager, for the current user and for SYSTEM.

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

### -SmtpEnableSsl

Specifies whether to use SSL to connect to the SMTP server and saves it in PSWUSettings.xml. Pass a Boolean value, for example -SmtpEnableSsl $true.

```yaml
Type: System.Boolean
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

### -SmtpFrom

Specifies the sender address and saves it in PSWUSettings.xml.

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

### -SmtpPort

Specifies the SMTP port and saves it in PSWUSettings.xml.

```yaml
Type: System.Int32
DefaultValue: 25
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

### -SmtpServer

Specifies the SMTP server and saves it in PSWUSettings.xml.

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

### -SmtpSubject

Specifies the subject of the report email and saves it in PSWUSettings.xml. If you do not set a subject, reports use the computer name, the text 'Windows Update report', and the date.

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

### -SmtpTo

Specifies the recipient address and saves it in PSWUSettings.xml.

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

### -Style

Specifies how the report presents the results, Table or List, and saves it in PSWUSettings.xml.

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
AcceptedValues:
- Table
- List
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

### None

## OUTPUTS

### System.Object

A collection with one object per computer that shows the settings that were saved.

## NOTES

The cmdlet writes a warning when the session is not elevated. -WhatIf and -Confirm are accepted but the cmdlet does not check them, so the settings are always saved. The ComputerName parameter is not available on this cmdlet in the current release; it always saves the settings on the local computer. Email reports use System.Net.Mail.SmtpClient, which Microsoft recommends against for new development.

## RELATED LINKS

- [Get-WindowsUpdate](Get-WindowsUpdate.md)
- [Invoke-WUJob](Invoke-WUJob.md)
