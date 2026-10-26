---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Get-WindowsUpdate.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Get-WindowsUpdate
---

# Get-WindowsUpdate

## SYNOPSIS

Gets the updates that match your criteria and optionally downloads, installs, hides, or unhides them.

## SYNTAX

### Default (Default)

```
Get-WindowsUpdate [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>]
 [-SendHistory] [-ScheduleJob <datetime>] [-AcceptAll] [-RecurseCycle <int>] [-Hide] [-Download]
 [-ForceDownload] [-Install] [-ForceInstall] [-AutoReboot] [-IgnoreReboot]
 [-ScheduleReboot <datetime>] [-Criteria <string>] [-UpdateType <string>]
 [-DeploymentAction <string>] [-IsAssigned] [-IsPresent] [-BrowseOnly] [-AutoSelectOnWebSites]
 [-UpdateID <string[]>] [-NotUpdateID <string[]>] [-RevisionNumber <int>] [-CategoryIDs <string[]>]
 [-IsInstalled] [-IsHidden] [-WithHidden] [-ShowPreSearchCriteria] [-RootCategories <string[]>]
 [-Category <string[]>] [-KBArticleID <string[]>] [-Title <string>] [-Severity <string[]>]
 [-NotCategory <string[]>] [-NotKBArticleID <string[]>] [-NotTitle <string>]
 [-NotSeverity <string[]>] [-IgnoreUserInput] [-IgnoreRebootRequired] [-AutoSelectOnly]
 [-MaxSize <long>] [-MinSize <long>] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ServiceID

```
Get-WindowsUpdate [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>]
 [-SendHistory] [-ScheduleJob <datetime>] [-AcceptAll] [-RecurseCycle <int>] [-Hide] [-Download]
 [-ForceDownload] [-Install] [-ForceInstall] [-AutoReboot] [-IgnoreReboot]
 [-ScheduleReboot <datetime>] [-ServiceID <string>] [-Criteria <string>] [-UpdateType <string>]
 [-DeploymentAction <string>] [-IsAssigned] [-IsPresent] [-BrowseOnly] [-AutoSelectOnWebSites]
 [-UpdateID <string[]>] [-NotUpdateID <string[]>] [-RevisionNumber <int>] [-CategoryIDs <string[]>]
 [-IsInstalled] [-IsHidden] [-WithHidden] [-ShowPreSearchCriteria] [-RootCategories <string[]>]
 [-Category <string[]>] [-KBArticleID <string[]>] [-Title <string>] [-Severity <string[]>]
 [-NotCategory <string[]>] [-NotKBArticleID <string[]>] [-NotTitle <string>]
 [-NotSeverity <string[]>] [-IgnoreUserInput] [-IgnoreRebootRequired] [-AutoSelectOnly]
 [-MaxSize <long>] [-MinSize <long>] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### WindowsUpdate

```
Get-WindowsUpdate [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>]
 [-SendHistory] [-ScheduleJob <datetime>] [-AcceptAll] [-RecurseCycle <int>] [-Hide] [-Download]
 [-ForceDownload] [-Install] [-ForceInstall] [-AutoReboot] [-IgnoreReboot]
 [-ScheduleReboot <datetime>] [-WindowsUpdate] [-Criteria <string>] [-UpdateType <string>]
 [-DeploymentAction <string>] [-IsAssigned] [-IsPresent] [-BrowseOnly] [-AutoSelectOnWebSites]
 [-UpdateID <string[]>] [-NotUpdateID <string[]>] [-RevisionNumber <int>] [-CategoryIDs <string[]>]
 [-IsInstalled] [-IsHidden] [-WithHidden] [-ShowPreSearchCriteria] [-RootCategories <string[]>]
 [-Category <string[]>] [-KBArticleID <string[]>] [-Title <string>] [-Severity <string[]>]
 [-NotCategory <string[]>] [-NotKBArticleID <string[]>] [-NotTitle <string>]
 [-NotSeverity <string[]>] [-IgnoreUserInput] [-IgnoreRebootRequired] [-AutoSelectOnly]
 [-MaxSize <long>] [-MinSize <long>] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### MicrosoftUpdate

```
Get-WindowsUpdate [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>]
 [-SendHistory] [-ScheduleJob <datetime>] [-AcceptAll] [-RecurseCycle <int>] [-Hide] [-Download]
 [-ForceDownload] [-Install] [-ForceInstall] [-AutoReboot] [-IgnoreReboot]
 [-ScheduleReboot <datetime>] [-MicrosoftUpdate] [-Criteria <string>] [-UpdateType <string>]
 [-DeploymentAction <string>] [-IsAssigned] [-IsPresent] [-BrowseOnly] [-AutoSelectOnWebSites]
 [-UpdateID <string[]>] [-NotUpdateID <string[]>] [-RevisionNumber <int>] [-CategoryIDs <string[]>]
 [-IsInstalled] [-IsHidden] [-WithHidden] [-ShowPreSearchCriteria] [-RootCategories <string[]>]
 [-Category <string[]>] [-KBArticleID <string[]>] [-Title <string>] [-Severity <string[]>]
 [-NotCategory <string[]>] [-NotKBArticleID <string[]>] [-NotTitle <string>]
 [-NotSeverity <string[]>] [-IgnoreUserInput] [-IgnoreRebootRequired] [-AutoSelectOnly]
 [-MaxSize <long>] [-MinSize <long>] [-Debuger] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  Get-WUList
  Get-WUInstall
  Install-WindowsUpdate
  Download-WindowsUpdate
  Hide-WindowsUpdate
  UnHide-WindowsUpdate
  Show-WindowsUpdate

## DESCRIPTION

Searches a Windows Update service for updates that match the criteria you give and returns them. With no action switch the cmdlet only lists the updates. Add -Download, -Install, or -Hide, or call one of the aliases, to act on the updates it found.

The aliases select an action:
- Install-WindowsUpdate is the same as -Download -Install.
- Download-WindowsUpdate is the same as -Download.
- Hide-WindowsUpdate is the same as -Hide.
- Show-WindowsUpdate and UnHide-WindowsUpdate are the same as -Hide:$false, and they include hidden updates in the search.
- Get-WUList and Get-WUInstall do not select an action. Get-WUInstall does not install anything unless you add -Install.

Updates are filtered in two stages. Pre-search criteria are sent to the Windows Update service and evaluated there: -Criteria, -UpdateType, -DeploymentAction, -IsAssigned, -IsPresent, -BrowseOnly, -AutoSelectOnWebSites, -UpdateID, -NotUpdateID, -RevisionNumber, -CategoryIDs, -IsInstalled, -IsHidden, and -WithHidden. When you use -Criteria, it replaces the query that the other pre-search parameters would build. Post-search criteria are applied on the computer to the pre-filtered list: -RootCategories, -Category, -NotCategory, -KBArticleID, -NotKBArticleID, -Title, -NotTitle, -Severity, -NotSeverity, -MaxSize, -MinSize, -IgnoreUserInput, -IgnoreRebootRequired, and -AutoSelectOnly. Use -ShowPreSearchCriteria to see the query that was sent.

By default the cmdlet asks for confirmation for each update before it downloads, installs, hides, or unhides it. Use -AcceptAll to skip the prompts. Without -WindowsUpdate, -MicrosoftUpdate, or -ServiceID the search uses the default Automatic Updates service of the computer (for example a WSUS server).

On a remote computer only the search runs interactively. To download or install, or when you use -ScheduleJob, the cmdlet registers a scheduled task through Invoke-WUJob that runs Get-WindowsUpdate -AcceptAll on the target, because the Windows Update Agent does not allow remote downloads and installations. The module must be installed on the target, and the task writes its output to PSWindowsUpdateNext.log in the TEMP folder of the SYSTEM account.

The Status property is a seven-character string:
- Position 1: A for an accepted update, R for a rejected update, or - when no action was requested.
- Position 2: D if the update is downloaded, F if its download failed, - if it is not downloaded, ? if the download runs in a scheduled task.
- Position 3: I if the update is installed, R if it was installed and needs a restart, F if the installation failed, - if it is not installed, ? if the installation runs in a scheduled task.
- Position 4: M for a mandatory update.
- Position 5: H for a hidden update.
- Position 6: U for an uninstallable update.
- Position 7: B for a beta update.
A flag that does not apply is shown as -. After -Hide or -Hide:$false, the string has only positions 2 to 7.

## EXAMPLES

### Example 1: List the available updates

```powershell
Get-WindowsUpdate -Verbose
```

```output
VERBOSE: PC01 (1/16/2024 2:01:10 PM): Connecting to Windows Server Update Service (https://wsus.contoso.com:8531) server. Please wait...
VERBOSE: Found [2] Updates in pre search criteria
VERBOSE: Found [2] Updates in post search criteria

ComputerName Status     KB          Size Title
------------ ------     --          ---- -----
PC01         -------    KB890830    44MB Windows Malicious Software Removal Tool x64 - v5.121 (KB890830)
PC01         -------    KB5034123  580MB 2024-01 Cumulative Update for Windows 11 Version 23H2 for x64-based Systems (KB5034123)
```

Searches the default update service of the local computer.

### Example 2: List the installed drivers and show the search query

```powershell
Get-WindowsUpdate -WindowsUpdate -UpdateType Driver -IsInstalled -ShowPreSearchCriteria -Verbose
```

```output
PreSearchCriteria: IsInstalled = 1 and Type = 'Driver' and IsHidden = 0
VERBOSE: PC01 (1/16/2024 2:05:41 PM): Connecting to Windows Update server. Please wait...
VERBOSE: Found [1] Updates in pre search criteria
VERBOSE: Found [1] Updates in post search criteria

ComputerName Status     KB          Size Title
------------ ------     --          ---- -----
PC01         -DI----                 3MB Intel - System - 10.1.1.45
```

Searches the public Windows Update service, which ignores a configured WSUS server.

### Example 3: Find updates by title on a remote computer

```powershell
Get-WindowsUpdate -ComputerName PC02 -MicrosoftUpdate -Title 'Cumulative Update.*Windows 11'
```

Searches Microsoft Update on PC02 for updates whose title matches the regular expression. The search is not case sensitive.

### Example 4: Hide an update

```powershell
Hide-WindowsUpdate -KBArticleID KB5034123
```

```output
Confirm
Are you sure you want to perform this action?
Performing the operation "Hide 2024-01 Cumulative Update for Windows 11 Version 23H2 for x64-based Systems (KB5034123)[580MB]" on target "PC01".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): Y

ComputerName Status     KB          Size Title
------------ ------     --          ---- -----
PC01         ---H--     KB5034123  580MB 2024-01 Cumulative Update for Windows 11 Version 23H2 for x64-based Systems (KB5034123)
```

The equivalent command is Get-WindowsUpdate -KBArticleID KB5034123 -Hide.

### Example 5: Unhide an update

```powershell
Show-WindowsUpdate -KBArticleID KB5034123 -AcceptAll
```

The equivalent command is Get-WindowsUpdate -KBArticleID KB5034123 -WithHidden -Hide:$false -AcceptAll.

### Example 6: Install all updates from Microsoft Update and restart if needed

```powershell
Install-WindowsUpdate -MicrosoftUpdate -AcceptAll -AutoReboot
```

Downloads and installs every update that the search returns, without prompting. If an installation requires a restart, the computer restarts automatically.

### Example 7: Schedule the installation of one update

```powershell
Install-WindowsUpdate -MicrosoftUpdate -UpdateID 2fbbe4d8-3a5f-4b0e-9d16-0c2b1d8f2a11 -RevisionNumber 200 -ScheduleJob (Get-Date -Hour 18 -Minute 0 -Second 0) -AcceptAll -AutoReboot -Verbose
```

```output
VERBOSE: PC01 (1/16/2024 2:10:03 PM): Connecting to Microsoft Update server. Please wait...
VERBOSE: Found [1] Updates in pre search criteria
VERBOSE: Found [1] Updates in post search criteria

X ComputerName Result     KB          Size Title
- ------------ ------     --          ---- -----
1 PC01         Accepted   KB5034123  580MB 2024-01 Cumulative Update for Windows 11 Version 23H2 for x64-based Systems (KB5034123)
VERBOSE: Accepted [1] Updates ready to Download
VERBOSE: Invoke-WUJob: PC01 (1/16/2024 6:00:00 PM):
```

Registers a one-time scheduled task that runs the installation at 6:00 PM today as SYSTEM.

### Example 8: Install updates on a remote computer and email a report

```powershell
Install-WindowsUpdate -ComputerName PC02 -MicrosoftUpdate -AcceptAll -AutoReboot -SendReport -PSWUSettings @{ SmtpServer = 'smtp.contoso.com'; From = 'wu@contoso.com'; To = 'admins@contoso.com'; Port = 25 }
```

The installation runs in a scheduled task on PC02. Instead of passing -PSWUSettings every time, save the settings once with Set-PSWUSettings.

## PARAMETERS

### -AcceptAll

Accepts every update that matches the criteria without asking for confirmation. It applies to download, installation, hide, and unhide.

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

### -AutoReboot

Restarts the computer without asking when a restart is required after an installation.

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

### -AutoSelectOnly

Post-search criteria. Keeps only the updates that Windows Update flags to be selected automatically (AutoSelectOnWebSites) and accepts them without asking.

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

### -AutoSelectOnWebSites

Pre-search criteria. -AutoSelectOnWebSites finds updates that Windows Update flags to be selected automatically. -AutoSelectOnWebSites:$false finds updates that are not flagged. The criterion is added only when you specify the parameter.

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

### -BrowseOnly

Pre-search criteria. -BrowseOnly finds updates that are considered optional. -BrowseOnly:$false finds updates that are not optional. The criterion is added only when you specify the parameter.

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

### -Category

Post-search criteria. Finds updates that belong to a category whose name matches one of these regular expressions, for example 'Security Updates' or 'Critical Updates'. The match is not case sensitive.

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

### -CategoryIDs

Pre-search criteria. Finds updates that belong to these categories, given as category IDs (GUIDs), for example 0fa1201d-4330-4fa8-8ae9-b877473b6441. An update that matches any of the IDs is found.

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

### -ComputerName

Specifies one or more computers to run against. The default is the local computer. Searching works interactively on remote computers through DCOM. Downloading and installing register a scheduled task through Invoke-WUJob instead. Accepts pipeline input by value and by property name.

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

### -Criteria

Pre-search criteria. Specifies the search string in the native query syntax of the Windows Update Agent (IUpdateSearcher::Search). It replaces the query that the other pre-search parameters would build.

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

### -DeploymentAction

Pre-search criteria. Finds updates that are deployed for Installation or for Uninstallation. If you omit it, the search implies DeploymentAction='Installation'.

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
- Installation
- Uninstallation
HelpMessage: ''
```

### -Download

Downloads the accepted updates but does not install them.

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

### -ForceDownload

Forces the download of updates that are already downloaded or that cannot be installed. Works only with -Download.

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

### -ForceInstall

Forces the installation of an update even if its metadata says that it is already installed. Before you use it, confirm that the update is available and not installed, because a forced installation of an update that is not installed fails. Works only with -Install.

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

### -Hide

Hides the matching updates. Use -Hide:$false to unhide them; hidden updates are found only when you also use -WithHidden or -IsHidden.

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

### -IgnoreRebootRequired

Post-search criteria. Excludes updates that can require a restart. Only updates whose restart behavior is that they never restart the computer are kept.

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

### -IgnoreUserInput

Post-search criteria. Excludes updates whose installation or uninstallation can prompt for user input. This parameter has the alias Silent.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases:
- Silent
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

### -Install

Installs the accepted updates. Updates that are not yet downloaded are downloaded first.

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

### -IsAssigned

Pre-search criteria. -IsAssigned finds updates that are intended for deployment by Automatic Updates (at most one assigned driver update is returned for each device). -IsAssigned:$false finds updates that are not. The criterion is added only when you specify the parameter.

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

### -IsHidden

Pre-search criteria. Finds only the updates that are hidden. By default hidden updates are excluded. When you use -IsHidden and -WithHidden together, -IsHidden wins.

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

### -IsInstalled

Pre-search criteria. Finds updates that are installed. By default only updates that are not installed are found.

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

### -IsPresent

Pre-search criteria. -IsPresent finds updates that are present on the computer; an update that applies to several products is present if it is installed for at least one of them. -IsPresent:$false finds updates that are not installed for any product. The criterion is added only when you specify the parameter.

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

Post-search criteria. Finds updates that have one of these KB article numbers, for example KB982861. Updates without a KB article are excluded.

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

### -MaxSize

Post-search criteria. Finds updates whose maximum download size is smaller than this value, in bytes.

```yaml
Type: System.Int64
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

### -MicrosoftUpdate

Searches Microsoft Update (ServiceID 7971f918-a847-4430-9279-4a52d1efe18d), which also offers updates for other Microsoft products. If Microsoft Update is not registered on the computer, the cmdlet registers it first and restarts the Windows Update service.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: MicrosoftUpdate
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MinSize

Post-search criteria. Finds updates whose maximum download size is larger than this value, in bytes.

```yaml
Type: System.Int64
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

### -NotCategory

Post-search criteria. Excludes updates that belong to a category whose name matches one of these regular expressions. The match is not case sensitive.

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

### -NotKBArticleID

Post-search criteria. Excludes updates that have one of these KB article numbers, for example KB982861.

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

### -NotSeverity

Post-search criteria. Excludes updates that have one of these MSRC severities: Critical, Important, Moderate, Low, or Unspecified (updates without a severity).

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
AcceptedValues:
- Critical
- Important
- Moderate
- Low
- Unspecified
HelpMessage: ''
```

### -NotTitle

Post-search criteria. Excludes updates whose title matches this regular expression. The match is not case sensitive.

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

### -NotUpdateID

Pre-search criteria. Excludes updates with these update IDs (GUIDs).

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

### -RecurseCycle

Specifies the total number of search-and-install cycles. The first run is cycle 1, so the value must be greater than 1 when you use the parameter. After each installation the cmdlet registers another cycle as a scheduled task, which runs five minutes later or, if a restart is required, at the next startup, until the count is reached.

```yaml
Type: System.Int32
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

### -RevisionNumber

Pre-search criteria. Finds updates with this revision number, for example 200. Use it together with -UpdateID; it applies to every ID you pass.

```yaml
Type: System.Int32
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

### -RootCategories

Post-search criteria. Returns only updates that belong to these root categories of the search result. Valid values are Critical Updates, Definition Updates, Drivers, Feature Packs, Security Updates, Service Packs, Tools, Update Rollups, Updates, Upgrades, and Microsoft.

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
AcceptedValues:
- Critical Updates
- Definition Updates
- Drivers
- Feature Packs
- Security Updates
- Service Packs
- Tools
- Update Rollups
- Updates
- Upgrades
- Microsoft
HelpMessage: ''
```

### -ScheduleJob

Runs the download, installation, or hide operation as a one-time scheduled task at this date and time instead of immediately. The task runs as SYSTEM. The value must be in the future.

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

Schedules a restart for this date and time when a restart is required after an installation. The value must be in the future.

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

### -SendHistory

After updates are installed, sends the update history (Get-WUHistory) by email through a scheduled task. If a restart is required, the report is sent after the next startup. Requires the email settings described under -SendReport.

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

### -SendReport

Sends the results by email when the cmdlet finishes. The SMTP settings come from -PSWUSettings or from the PSWUSettings.xml file saved by Set-PSWUSettings. The default properties are ComputerName, Status, KB, Size, and Title.

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

### -ServiceID

Searches the registered update service that has this ID. Use Get-WUServiceManager to list the IDs. Examples: Windows Update 9482f4b4-e343-43b6-b170-9a65bc822c77, Microsoft Update 7971f918-a847-4430-9279-4a52d1efe18d, Windows Store 117cab2d-82b1-4b5a-a08c-4d62dbee7782, Windows Server Update Service 3da21691-e39d-4da6-8a4b-b43877bcb1b7.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ServiceID
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Severity

Post-search criteria. Finds updates that have one of these MSRC severities: Critical, Important, Moderate, Low, or Unspecified (updates without a severity).

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
AcceptedValues:
- Critical
- Important
- Moderate
- Low
- Unspecified
HelpMessage: ''
```

### -ShowPreSearchCriteria

Writes the query that is sent to the update service to the host. It shows only the pre-search criteria.

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

### -Title

Post-search criteria. Finds updates whose title matches this regular expression, for example '.NET Framework 4'. The match is not case sensitive.

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

### -UpdateID

Pre-search criteria. Finds updates with these update IDs (GUIDs), for example 12345678-9abc-def0-1234-56789abcdef0. When you pass several IDs, an update that matches any of them is found.

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

### -UpdateType

Pre-search criteria. Finds updates of one type: Driver or Software. By default all types are found.

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
- Driver
- Software
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

### -WindowsUpdate

Searches the public Windows Update service (ServiceID 9482f4b4-e343-43b6-b170-9a65bc822c77) and ignores a configured WSUS server.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: WindowsUpdate
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WithHidden

Pre-search criteria. Includes hidden updates together with updates that are not hidden. It has no effect when -IsHidden is also specified.

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

### PSWindowsUpdateNext.WindowsUpdate

The update, as the IUpdate COM object of the Windows Update Agent with ComputerName, KB, Size, and Status properties added.

### PSWindowsUpdateNext.WindowsUpdateJob

When you download, install, hide, or unhide, the same objects are returned with the X, ChooseResult, Result, DownloadResult, InstallResult, and HideResult properties that describe each stage.

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. The cmdlet has a confirmation impact of High, so it prompts for each update unless you use -AcceptAll or set $ConfirmPreference to None. After a local installation that requires a restart, the cmdlet asks whether to restart unless you use -AutoReboot, -IgnoreReboot, or -ScheduleReboot. Restarts run shutdown.exe with the force option (-f). Email reports are sent only when the cmdlet returned at least one update.

## RELATED LINKS

- [Get-WUHistory](Get-WUHistory.md)
- [Get-WURebootStatus](Get-WURebootStatus.md)
- [Get-WUServiceManager](Get-WUServiceManager.md)
- [Invoke-WUJob](Invoke-WUJob.md)
- [Set-PSWUSettings](Set-PSWUSettings.md)
- [Remove-WindowsUpdate](Remove-WindowsUpdate.md)
