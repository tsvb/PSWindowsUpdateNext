---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WindowsUpdate
---

# Get-WindowsUpdate

## SYNOPSIS

Get list of available updates meeting the criteria.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Get-WindowsUpdate [-AcceptAll] [-AutoReboot] [-AutoSelectOnly] [-AutoSelectOnWebSites] [-BrowseOnly]
 [-Category <string[]>] [-CategoryIDs <string[]>] [-ComputerName <string[]>] [-Criteria <string>]
 [-Debuger] [-DeploymentAction <string>] [-Download] [-ForceDownload] [-ForceInstall] [-Hide]
 [-IgnoreReboot] [-IgnoreRebootRequired] [-IgnoreUserInput] [-Install] [-IsAssigned] [-IsHidden]
 [-IsInstalled] [-IsPresent] [-KBArticleID <string[]>] [-MaxSize <long>] [-MinSize <long>]
 [-NotCategory <string[]>] [-NotKBArticleID <string[]>] [-NotSeverity <string[]>]
 [-NotTitle <string>] [-NotUpdateID <string[]>] [-PSWUSettings <Hashtable>] [-RecurseCycle <int>]
 [-RevisionNumber <int>] [-RootCategories <string[]>] [-ScheduleJob <DateTime>]
 [-ScheduleReboot <DateTime>] [-SendHistory] [-SendReport] [-ServiceID <string>]
 [-Severity <string[]>] [-ShowPreSearchCriteria] [-Title <string>] [-UpdateID <string[]>]
 [-UpdateType <string>] [-WithHidden] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_2

```
Get-WindowsUpdate [-AcceptAll] [-AutoReboot] [-AutoSelectOnly] [-AutoSelectOnWebSites] [-BrowseOnly]
 [-Category <string[]>] [-CategoryIDs <string[]>] [-ComputerName <string[]>] [-Criteria <string>]
 [-Debuger] [-DeploymentAction <string>] [-Download] [-ForceDownload] [-ForceInstall] [-Hide]
 [-IgnoreReboot] [-IgnoreRebootRequired] [-IgnoreUserInput] [-Install] [-IsAssigned] [-IsHidden]
 [-IsInstalled] [-IsPresent] [-KBArticleID <string[]>] [-MaxSize <long>] [-MinSize <long>]
 [-NotCategory <string[]>] [-NotKBArticleID <string[]>] [-NotSeverity <string[]>]
 [-NotTitle <string>] [-NotUpdateID <string[]>] [-PSWUSettings <Hashtable>] [-RecurseCycle <int>]
 [-RevisionNumber <int>] [-RootCategories <string[]>] [-ScheduleJob <DateTime>]
 [-ScheduleReboot <DateTime>] [-SendHistory] [-SendReport] [-Severity <string[]>]
 [-ShowPreSearchCriteria] [-Title <string>] [-UpdateID <string[]>] [-UpdateType <string>]
 [-WindowsUpdate] [-WithHidden] [<CommonParameters>]
```

### UNNAMED_PARAMETER_SET_3

```
Get-WindowsUpdate [-AcceptAll] [-AutoReboot] [-AutoSelectOnly] [-AutoSelectOnWebSites] [-BrowseOnly]
 [-Category <string[]>] [-CategoryIDs <string[]>] [-ComputerName <string[]>] [-Criteria <string>]
 [-Debuger] [-DeploymentAction <string>] [-Download] [-ForceDownload] [-ForceInstall] [-Hide]
 [-IgnoreReboot] [-IgnoreRebootRequired] [-IgnoreUserInput] [-Install] [-IsAssigned] [-IsHidden]
 [-IsInstalled] [-IsPresent] [-KBArticleID <string[]>] [-MaxSize <long>] [-MicrosoftUpdate]
 [-MinSize <long>] [-NotCategory <string[]>] [-NotKBArticleID <string[]>] [-NotSeverity <string[]>]
 [-NotTitle <string>] [-NotUpdateID <string[]>] [-PSWUSettings <Hashtable>] [-RecurseCycle <int>]
 [-RevisionNumber <int>] [-RootCategories <string[]>] [-ScheduleJob <DateTime>]
 [-ScheduleReboot <DateTime>] [-SendHistory] [-SendReport] [-Severity <string[]>]
 [-ShowPreSearchCriteria] [-Title <string>] [-UpdateID <string[]>] [-UpdateType <string>]
 [-WithHidden] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Get-WindowsUpdate (aka Get-WUList) cmdlet to get list of available or installed updates meeting specific criteria.

Use Download-WindowsUpdate alias to get list of updates and download it. Equivalent Get-WindowsUpdate -Download.

Use Install-WindowsUpdate (aka Get-WUInstall) alias to get list of updates and install it. Equivalent Get-WindowsUpdate -Install.

Use Hide-WindowsUpdate alias to get list of updates and hide it. Equivalent Get-WindowsUpdate -Hide.

Use Show-WindowsUpdate (aka UnHide-WindowsUpdate) alias to get list of updates and unhide it. Equivalent Get-WindowsUpdate -Hide:$false.

There are two types of filtering update: Pre search criteria, Post search criteria.

- Pre search works on server side, like example: (IsInstalled = 0 and IsHidden = 0 and CategoryIds contains '0fa1201d-4330-4fa8-8ae9-b877473b6441' )

- Post search work on client side after get the pre-filtered list of updates, like example $KBArticleID -match $Update.KBArticleIDs

Status info list:\r\n[A|R]DIMHUB\r\nA-IsAccetped\r\nR-IsRejected\r\n D-IsDownloaded\r\n F-DownloadFailed\r\n ?-IsInvoked\r\n I-IsInstalled\r\n F-InstallFailed\r\n ?-IsInvoked\r\n R-RebootRequired\r\n M-IsMandatory\r\n H-IsHidden\r\n U-IsUninstallable\r\n B-IsBeta

## EXAMPLES

## PARAMETERS

### -AcceptAll

Do not ask confirmation for updates. Download or Install all available updates.

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

### -AutoReboot

Do not ask for reboot if it needed.

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

### -AutoSelectOnly

Install only the updates that have status AutoSelectOnWebsites on true.

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

### -AutoSelectOnWebSites

Pre search criteria - native for WUAPI. Finds updates where the AutoSelectOnWebSites property has the specified value.

"AutoSelectOnWebSites=1" finds updates that are flagged to be automatically selected by Windows Update.

"AutoSelectOnWebSites=0" finds updates that are not flagged for Automatic Updates.

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

### -BrowseOnly

Pre search criteria - native for WUAPI. "BrowseOnly=1" finds updates that are considered optional. "BrowseOnly=0" finds updates that are not considered optional.

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

### -Category

Post search criteria. Finds updates that contain a specified category name (or sets of categories name), such as 'Updates', 'Security Updates', 'Critical Updates', etc...

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CategoryIDs

Pre search criteria - native for WUAPI. Finds updates that belong to a specified category (or sets of UUIDs), such as '0fa1201d-4330-4fa8-8ae9-b877473b6441'.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ComputerName

Specify one or more computer names for remote connection. Interactive remote connection works only for checking updates. For download or install cmdlet creates an Invoke-WUJob task.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Criteria

Pre search criteria - native for WUAPI. Set own string that specifies the search criteria. https://docs.microsoft.com/pl-pl/windows/desktop/api/wuapi/nf-wuapi-iupdatesearcher-search

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Debuger

Debuger return original exceptions. For additional debug information use $DebugPreference = "Continue"

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

### -DeploymentAction

Pre search criteria - native for WUAPI. Finds updates that are deployed for a specific action, such as an installation or uninstallation that the administrator of a server specifies. "DeploymentAction='Installation'" finds updates that are deployed for installation on a destination computer. "DeploymentAction='Uninstallation'" depends on the other query criteria.

"DeploymentAction='Uninstallation'" finds updates that are deployed for uninstallation on a destination computer. "DeploymentAction='Uninstallation'" depends on the other query criteria.

If this criterion is not explicitly specified, each group of criteria that is joined to an AND operator implies "DeploymentAction='Installation'".

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Download

Get list of updates and download approved updates, but do not install it.

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

### -ForceDownload

Forces the download of updates that are already installed or that cannot be installed. Works only with -Download.

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

### -ForceInstall

A forced installation is an installation in which an update is installed even if the metadata indicates that the update is already installed. Before you use ForceInstall to force an installation, determine whether the update is installed and available. If an update is not installed, a forced installation fails. Works only with -Install.

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

### -Hide

Get list of updates and hide/unhide approved updates.

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

### -IgnoreReboot

Do not ask for reboot if it needed, but do not reboot automaticaly.

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

### -IgnoreRebootRequired

Post search criteria. Finds updates that specifies the restart behavior that not occurs when you install or uninstall the update.

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

### -IgnoreUserInput

Post search criteria. Finds updates that the installation or uninstallation of an update can't prompt for user input.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases:
- Silent
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Install

Get list of updates and install approved updates.

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

### -IsAssigned

Pre search criteria - native for WUAPI. Finds updates that are intended for deployment by Automatic Updates. "IsAssigned=1" finds updates that are intended for deployment by Automatic Updates, which depends on the other query criteria.At most, one assigned Windows-based driver update is returned for each local device on a destination computer.

"IsAssigned=0" finds updates that are not intended to be deployed by Automatic Updates.

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

### -IsHidden

Pre search criteria - native for WUAPI. Finds updates that are marked as hidden on the destination computer. Default search criteria is only not hidden upadates.

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

### -IsInstalled

Pre search criteria - native for WUAPI. Finds updates that are installed on the destination computer.

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

### -IsPresent

Pre search criteria - native for WUAPI. When set to 1, finds updates that are present on a computer.

"IsPresent=1" finds updates that are present on a destination computer.If the update is valid for one or more products, the update is considered present if it is installed for one or more of the products.

"IsPresent=0" finds updates that are not installed for any product on a destination computer.

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

### -KBArticleID

Post search criteria. Finds updates that contain a KBArticleID (or sets of KBArticleIDs), such as 'KB982861'.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MaxSize

Post search criteria. Finds updates that have MaxDownloadSize less or equal. Size is in Bytes.

```yaml
Type: System.Int64
DefaultValue: 0
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MicrosoftUpdate

Use Windows Update Service Manager - '9482f4b4-e343-43b6-b170-9a65bc822c77'

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

### -MinSize

Post search criteria. Finds updates that have MaxDownloadSize greater or equal. Size is in Bytes.

```yaml
Type: System.Int64
DefaultValue: 0
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NotCategory

Post search criteria. Finds updates that not contain a specified category name (or sets of categories name), such as 'Updates', 'Security Updates', 'Critical Updates', etc...

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NotKBArticleID

Post search criteria. Finds updates that not contain a KBArticleID (or sets of KBArticleIDs), such as 'KB982861'.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NotSeverity

Post search criteria. Finds updates that not match part of severity.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NotTitle

Post search criteria. Finds updates that not match part of title (case sensitive).

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NotUpdateID

Pre search criteria - native for WUAPI. Finds updates without a specific UUID (or sets of UUIDs), such as '12345678-9abc-def0-1234-56789abcdef0'.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PSWUSettings

Required parameter for -SendReport.

Passes the parameters (as hashtable) necessary to send the report: \r\n@{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25];[Subject="Alternative Subject"];[Properties="Alternative object properties"];[Style="Table|List"]}

Send parameters can also be saved to a PSWUSettings.xml file in ModuleBase path: \r\nExport-Clixml @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25]}"

```yaml
Type: System.Collections.Hashtable
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RecurseCycle

Specify number of cycles for check updates after successful update installation or system startup. First run is always main cycle (-RecurseCycle 1 or none). Second (-RecurseCycle 2) and n (-RecurseCycle n) cycle are recursive.

```yaml
Type: System.Int32
DefaultValue: 0
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RevisionNumber

Pre search criteria - native for WUAPI. Finds updates with a specific RevisionNumber, such as '100'. This criterion must be combined with the UpdateID param.

```yaml
Type: System.Int32
DefaultValue: 0
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RootCategories

Post search criteria. Finds updates that contain a specified root category name 'Critical Updates', 'Definition Updates', 'Drivers', 'Feature Packs', 'Security Updates', 'Service Packs', 'Tools', 'Update Rollups', 'Updates', 'Upgrades', 'Microsoft'.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScheduleJob

Specify time when job will start.

```yaml
Type: System.DateTime
DefaultValue: 01.01.0001 00:00:00
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScheduleReboot

Specify time when system will be rebooted.

```yaml
Type: System.DateTime
DefaultValue: 01.01.0001 00:00:00
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SendHistory

Send install history (Get-WUHistory) report after successful update installation or system startup.

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

### -SendReport

Send report email to specific recipients.

Requires the parameter -PSWUSettings or declare the PSWUSettings.xml file (more preferred) in ModuleBase path.

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

### -ServiceID

Use specific Service Manager if it's available.

Examples Of ServiceID: \r\n \r\n -- Windows Update 9482f4b4-e343-43b6-b170-9a65bc822c77 \r\n -- Microsoft Update 7971f918-a847-4430-9279-4a52d1efe18d \r\n -- Windows Store 117cab2d-82b1-4b5a-a08c-4d62dbee7782 \r\n -- Windows Server Update Service 3da21691-e39d-4da6-8a4b-b43877bcb1b7

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Severity

Post search criteria. Finds updates that match part of severity, such as 'Important', 'Critical', 'Moderate', etc...

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ShowPreSearchCriteria

Show choosen search criteria. Only works for pre search criteria.

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

### -Silent

Post search criteria. Finds updates that the installation or uninstallation of an update can't prompt for user input.

This is an alias of the IgnoreUserInput parameter.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases:
- Silent
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Title

Post search criteria. Finds updates that match part of title (case sensitive), such as '.NET Framework 4'.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UpdateID

Pre search criteria - native for WUAPI. Finds updates with a specific UUID (or sets of UUIDs), such as '12345678-9abc-def0-1234-56789abcdef0'.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UpdateType

Pre search criteria - native for WUAPI. Finds updates with a specific type, such as 'Driver' and 'Software'. Default value contains all updates.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WindowsUpdate

Use Microsoft Update Service Manager - '7971f918-a847-4430-9279-4a52d1efe18d'

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

### -WithHidden

Pre search criteria - native for WUAPI. Finds updates that are both hidden and not on the destination computer. Overwrite IsHidden param. Default search criteria is only not hidden upadates.

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

### System.String[]

Specify one or more computer names for remote connection. Interactive remote connection works only for checking updates. For download or install cmdlet creates an Invoke-WUJob task.

## OUTPUTS

### PSWindowsUpdateNext.WindowsUpdate

OutputType for WindowsUpdate objects.

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

