---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Set-WUSettings.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Set-WUSettings
---

# Set-WUSettings

## SYNOPSIS

Sets the Windows Update client policy settings.

## SYNTAX

### __AllParameterSets

```
Set-WUSettings [-ComputerName <string[]>] [-SendReport] [-PSWUSettings <hashtable>]
 [-AcceptTrustedPublisherCerts] [-DisableWindowsUpdateAccess] [-ElevateNonAdmins]
 [-TargetGroup <string>] [-TargetGroupEnabled] [-WUServer <string>] [-NotificationLevel <string>]
 [-AutoInstallMinorUpdates] [-DetectionFrequency <int>] [-DetectionFrequencyEnabled]
 [-IncludeRecommendedUpdates] [-NoAutoRebootWithLoggedOnUsers] [-NoAutoUpdate]
 [-RebootRelaunchTimeout <int>] [-RebootRelaunchTimeoutEnabled] [-RebootWarningTimeout <int>]
 [-RebootWarningTimeoutEnabled] [-ScheduledInstallDay <string>] [-ScheduledInstallTime <int>]
 [-UseWUServer] [-TargetReleaseVersion] [-TargetReleaseVersionInfo <string>]
 [-ProductVersion <string>] [-DeferFeatureUpdatesPeriodInDays <int>]
 [-DeferQualityUpdatesPeriodInDays <int>] [-DisableWUfBSafeguards] [-ManagePreviewBuilds <int>]
 [-AllowAutoWindowsUpdateDownloadOverMeteredNetwork] [-AlwaysAutoRebootAtScheduledTime]
 [-AlwaysAutoRebootAtScheduledTimeMinutes <int>] [-SetAutoRestartNotificationConfig]
 [-AutoRestartNotificationSchedule <int>] [-SetAutoRestartRequiredNotificationDismissal]
 [-AutoRestartRequiredNotificationDismissal <string>] [-SetRestartWarningSchd]
 [-ScheduleRestartWarning <int>] [-ScheduleImminentRestartWarning <int>] [-DeferUpgrade]
 [-DeferUpgradePeriod <int>] [-DeferUpdatePeriod <int>] [-PauseDeferrals]
 [-SetUpdateNotificationLevel] [-UpdateNotificationLevel <string>] [-NoAUShutdownOption]
 [-AUPowerManagement] [-SetDisablePauseUXAccess] [-SetDisableUXWUAccess]
 [-RescheduleWaitTimeEnabled] [-RescheduleWaitTime <int>] [-SetActiveHoursMaxRange]
 [-ActiveHoursMaxRange <int>] [-UseUpdateClassPolicySource]
 [-SetPolicyDrivenUpdateSourceForFeatureUpdates] [-SetPolicyDrivenUpdateSourceForQualityUpdates]
 [-SetPolicyDrivenUpdateSourceForDriverUpdates] [-SetPolicyDrivenUpdateSourceForOtherUpdates]
 [-SetActiveHours] [-ActiveHoursStart <int>] [-ActiveHoursEnd <int>]
 [-SetAutoRestartNotificationDisable] [-EnableFeaturedSoftware] [-SetEDURestart] [-Debuger]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Writes Windows Update policy values to the registry on the local or a remote computer, under HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate and its AU subkey. The keys are created if they do not exist. Most parameters write the registry value that has the same name; -NotificationLevel writes AUOptions. Get-WUSettings reads the same values.

Only the settings you specify are changed. A switch parameter such as -NoAutoUpdate writes 1; -NoAutoUpdate:$false writes 0. Many policies come in pairs: a Set... or ...Enabled switch turns the policy on, and a second parameter holds its value, for example -SetActiveHours with -ActiveHoursStart and -ActiveHoursEnd.

Group Policy overrides these values on the next policy refresh. The cmdlet reports the settings that it changed.

## EXAMPLES

### Example 1: Include recommended updates

```powershell
Set-WUSettings -IncludeRecommendedUpdates
```

```output
Confirm
Are you sure you want to perform this action?
Performing the operation "Set Windows Update settings" on target "PC01".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): Y

ComputerName IncludeRecommendedUpdates
------------ -------------------------
PC01         True
```

### Example 2: Pin feature updates to a release

```powershell
Set-WUSettings -TargetReleaseVersion -TargetReleaseVersionInfo 23H2 -ProductVersion 'Windows 11' -Confirm:$false
```

The computer stays on Windows 11 version 23H2 and receives quality updates, but not the next Feature update.

### Example 3: Notify before download on a remote computer

```powershell
Set-WUSettings -ComputerName PC02 -NotificationLevel 'Notify before download'
```

### Example 4: Restrict automatic restarts during working hours

```powershell
Set-WUSettings -SetActiveHours -ActiveHoursStart 8 -ActiveHoursEnd 18
```

## PARAMETERS

### -AcceptTrustedPublisherCerts

Enabled - The WSUS server distributes available signed non-Microsoft updates.

Disabled - The WSUS server does not distribute available signed non-Microsoft updates.

Supported on: At least Windows Server 2003 operating systems or Windows XP Professional with SP1, excluding Windows RT

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

### -ActiveHoursEnd

Sets the hour at which active hours end (0 to 23). During active hours the PC does not restart automatically after updates. Use it together with -SetActiveHours and -ActiveHoursStart.

Supported on: At least Windows 10 Server or Windows 10

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

### -ActiveHoursMaxRange

Sets the maximum number of hours, from the start time, that users can set as their active hours (8 to 18). Use it together with -SetActiveHoursMaxRange.

Supported on: At least Windows 10 Server or Windows 10

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

### -ActiveHoursStart

Sets the hour at which active hours start (0 to 23). During active hours the PC does not restart automatically after updates. Use it together with -SetActiveHours and -ActiveHoursEnd.

Supported on: At least Windows 10 Server or Windows 10

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

### -AllowAutoWindowsUpdateDownloadOverMeteredNetwork

Enabling this policy will automatically download updates, even over metered data connections (charges may apply).

Supported on: At least Windows 10 Server, Windows 10 or Windows 10 RT

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

### -AlwaysAutoRebootAtScheduledTime

Turns on the policy that starts a restart timer immediately after Windows Update installs important updates, instead of first notifying users on the sign-in screen for at least two days. Specify the length with -AlwaysAutoRebootAtScheduledTimeMinutes.

Supported on: At least Windows Server 2012, Windows 8 or Windows RT

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

### -AlwaysAutoRebootAtScheduledTimeMinutes

Sets the restart timer, in minutes (15 to 180). When the timer runs out, the restart proceeds even if the PC has signed-in users. Use it together with -AlwaysAutoRebootAtScheduledTime.

Supported on: At least Windows Server 2012, Windows 8 or Windows RT

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

### -AUPowerManagement

Specifies whether the Windows Update will use the Windows Power Management features to automatically wake up the system from sleep, if there are updates scheduled for installation.

Supported on: Windows Server 2008, Windows 7, Windows Vista, and Windows 10

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

### -AutoInstallMinorUpdates

Silently install minor updates.

Supported on: At least Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3 through Windows 8.1 or Windows Server 2012 R2 with most current service pack. Not supported on Windows 10 and above.

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

### -AutoRestartNotificationSchedule

Sets how long, in minutes, before the restart the reminder notification appears. Valid values are 15, 30, 60, 120, and 240. Use it together with -SetAutoRestartNotificationConfig.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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
AcceptedValues:
- 15
- 30
- 60
- 120
- 240
HelpMessage: ''
```

### -AutoRestartRequiredNotificationDismissal

Sets how the auto-restart required notification is dismissed: 'Auto' dismisses it after 25 seconds, 'User Action' keeps it until the user dismisses it. Use it together with -SetAutoRestartRequiredNotificationDismissal.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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
- Auto
- User Action
HelpMessage: ''
```

### -ComputerName

Specifies one or more computers to run against. The default is the local computer. Remote computers are reached through the remote registry, so the Remote Registry service must be running there; see Enable-WURemoting. Accepts pipeline input by value and by property name.

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

### -DeferFeatureUpdatesPeriodInDays

Sets how many days Feature updates are deferred (0 to 365). The cmdlet also turns on the policy that defers Feature updates.

Supported on: At least Windows 10 Server or Windows 10

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

### -DeferQualityUpdatesPeriodInDays

Sets how many days quality updates are deferred (0 to 30). The cmdlet also turns on the policy that defers quality updates.

Supported on: At least Windows 10 Server or Windows 10

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

### -DeferUpdatePeriod

Allows IT Admins to specify update delays for up to four weeks. Supported values are 0-4, which refers to the number of weeks to defer updates.

Supported on: At least Windows 10 Server or Windows 10

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

### -DeferUpgrade

You can also choose to delay updates for up to one month. If you do not delay updates, your PC will remain up to date with security updates as they become available.

Supported on: At least Windows 10 Server or Windows 10

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

### -DeferUpgradePeriod

Allows IT Admins to specify additional upgrade delays for up to 8 months. Supported values are 0-8, which refers to the number of months to defer upgrades.

Supported on: At least Windows 10 Server or Windows 10

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

### -DetectionFrequency

Sets the time between detection cycles, in hours (1 to 22). Use it together with -DetectionFrequencyEnabled.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -DetectionFrequencyEnabled

Turns on the policy that sets the time between detection cycles. Specify the interval with -DetectionFrequency.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -DisableWindowsUpdateAccess

Removes access to Windows Update (writes DisableWindowsUpdateAccess=1). Use -DisableWindowsUpdateAccess:$false to restore it (writes 0).

Supported on: At least Windows Server 2003 operating systems, Windows XP Professional Service Pack 1, or Windows 2000 Service Pack 3

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

### -DisableWUfBSafeguards

This policy setting specifies that a Windows Update for Business device should skip safeguards.

Supported on: At least Windows Server, Windows 10 Version 1903

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

### -ElevateNonAdmins

This policy setting allows you to control whether non-administrative users will receive update notifications based on the "Configure Automatic Updates" policy setting.

Supported on: At least Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3 through Windows 8.1 or Windows Server 2012 R2 with most current service pack. Not supported on Windows 10 and above.

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

### -EnableFeaturedSoftware

This policy setting allows you to control whether users see detailed enhanced notification messages about featured software from the Microsoft Update service.

Supported on: Windows Server 2008, Windows 7, and Windows Vista

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

### -IncludeRecommendedUpdates

Includes recommended updates, not only important updates, when Automatic Updates installs updates (writes 1). Use -IncludeRecommendedUpdates:$false to exclude them (writes 0).

Supported on: At least Windows Vista through Windows 8.1 or Windows Server 2012 R2 with most current service pack. Not supported on Windows 10 and above.

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

### -ManagePreviewBuilds

Controls which updates the computer receives before they are released to the world. Valid values are 0 (disable Preview builds), 1 (disable Preview builds once the next release is public), 2 (enable Preview builds), and 3 (the Preview builds setting is left to the user, which is the default of the policy).

Supported on: At least Windows Server, Windows 10 Version 1709

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

### -NoAUShutdownOption

This policy setting allows you to manage whether the 'Install Updates and Shut Down' option is displayed in the Shut Down Windows dialog box.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows XP SP2

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

### -NoAutoRebootWithLoggedOnUsers

Logged-on user can decide whether to restart the client computer.

Supported on: Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3

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

### -NoAutoUpdate

Disables Automatic Updates (writes NoAutoUpdate=1). Use -NoAutoUpdate:$false to enable Automatic Updates (writes 0).

Supported on: Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3

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

### -NotificationLevel

Sets the Automatic Updates notification mode (the AUOptions policy value). Valid values are 'Not configured', 'Disabled', 'Notify before download', 'Notify before installation', 'Scheduled installation', and 'Users configure'. This parameter has the alias AUOptions.

Supported on: Windows XP Professional Service Pack 1 or later, and Windows 2000 Service Pack 3 or later.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- AUOptions
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- Not configured
- Disabled
- Notify before download
- Notify before installation
- Scheduled installation
- Users configure
HelpMessage: ''
```

### -PauseDeferrals

Allows IT Admins to pause updates and upgrades for up to 5 weeks.

Supported on: At least Windows 10 Server or Windows 10

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

### -ProductVersion

Sets the product for which the maximum Feature update version applies, for example 'Windows 11', '11', or 'Windows 10'. Use it together with -TargetReleaseVersion and -TargetReleaseVersionInfo.

Supported on: At least Windows 10 Server or Windows 10

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

### -RebootRelaunchTimeout

Sets the time between prompts for a scheduled restart, in minutes (1 to 1440). Use it together with -RebootRelaunchTimeoutEnabled.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1, Windows 2000 SP4, Windows 2000 SP3

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

### -RebootRelaunchTimeoutEnabled

Turns on the policy that sets the time Automatic Updates waits before it prompts again for a scheduled restart. Specify the time with -RebootRelaunchTimeout.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1, Windows 2000 SP4, Windows 2000 SP3

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

### -RebootWarningTimeout

Sets the length of the restart warning countdown, in minutes (1 to 30), after updates with a deadline or scheduled updates were installed. Use it together with -RebootWarningTimeoutEnabled.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1, Windows 2000 SP4, Windows 2000 SP3

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

### -RebootWarningTimeoutEnabled

Turns on the policy that sets the restart warning countdown. Specify the length with -RebootWarningTimeout.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1, Windows 2000 SP4, Windows 2000 SP3

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

### -RescheduleWaitTime

Sets how long, in minutes (1 to 60), Automatic Updates waits after system startup before it runs a scheduled installation that was missed. Use it together with -RescheduleWaitTimeEnabled.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1, Windows 2000 SP4, Windows 2000 SP3

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

### -RescheduleWaitTimeEnabled

Turns on the policy that sets how long Automatic Updates waits after startup before it runs a missed scheduled installation. Specify the time with -RescheduleWaitTime.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1, Windows 2000 SP4, Windows 2000 SP3

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

### -ScheduledInstallDay

Sets the day on which Automatic Updates installs updates. Valid values are 'Every Day', 'Every Sunday', 'Every Monday', 'Every Tuesday', 'Every Wednesday', 'Every Thursday', 'Every Friday', and 'EverySaturday'. It is used only when NotificationLevel is 'Scheduled installation'.

Starting with Windows 8 and Windows Server 2012, the Windows Update Agent no longer reports this value reliably. The operation appears to succeed but has no effect.

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
- Every Day
- Every Sunday
- Every Monday
- Every Tuesday
- Every Wednesday
- Every Thursday
- Every Friday
- EverySaturday
HelpMessage: ''
```

### -ScheduledInstallTime

Sets the hour at which Automatic Updates installs updates, in 24-hour format (0 to 23). It is used only when NotificationLevel is 'Scheduled installation'.

Starting with Windows 8 and Windows Server 2012, the Windows Update Agent no longer reports this value reliably. The operation appears to succeed but has no effect.

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

### -ScheduleImminentRestartWarning

Sets the period, in minutes, for the final warning that a restart is imminent. Valid values are 15 (the default of the policy), 30, and 60. Use it together with -SetRestartWarningSchd.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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
AcceptedValues:
- 15
- 30
- 60
HelpMessage: ''
```

### -ScheduleRestartWarning

Sets the period, in hours, for the warning notification about a scheduled restart. Valid values are 2, 4 (the default of the policy), 8, 12, and 24. Use it together with -SetRestartWarningSchd.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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
AcceptedValues:
- 2
- 4
- 8
- 12
- 24
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

### -SetActiveHours

Turns on the policy that prevents automatic restarts during active hours. Specify the hours with -ActiveHoursStart and -ActiveHoursEnd. The PC attempts to restart outside of active hours.

Supported on: At least Windows 10 Server or Windows 10

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

### -SetActiveHoursMaxRange

Turns on the policy that limits the range of active hours users can set. Specify the maximum with -ActiveHoursMaxRange.

Supported on: At least Windows 10 Server or Windows 10

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

### -SetAutoRestartNotificationConfig

Turns on the policy that sets the schedule of the auto-restart reminder notification. Specify the value with -AutoRestartNotificationSchedule.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -SetAutoRestartNotificationDisable

This policy setting allows you to control whether users receive notifications for auto restarts for update installations including reminder and warning notifications.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -SetAutoRestartRequiredNotificationDismissal

Turns on the policy that sets how the auto-restart required notification is dismissed. Specify the method with -AutoRestartRequiredNotificationDismissal.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -SetDisablePauseUXAccess

This setting allows to remove access to "Pause updates" feature. Once enabled user access to pause updates is removed.

Supported on: At least Windows Server, Windows 10 Version 1809

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

### -SetDisableUXWUAccess

This setting allows you to remove access to scan Windows Update. If you enable this setting, Windows Update scan access is removed.

Supported on: At least Windows 10 Server or Windows 10

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

### -SetEDURestart

Enabling this policy for EDU devices that remain on Carts overnight will skip power checks to ensure update reboots will happen at the scheduled install time.

Supported on: At least Windows 10 Server, Windows 10 or Windows 10 RT

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

### -SetPolicyDrivenUpdateSourceForDriverUpdates

Configure this policy to specify whether to receive Windows Driver Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004

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

### -SetPolicyDrivenUpdateSourceForFeatureUpdates

Configure this policy to specify whether to receive Windows Feature Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004

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

### -SetPolicyDrivenUpdateSourceForOtherUpdates

Configure this policy to specify whether to receive Other Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004

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

### -SetPolicyDrivenUpdateSourceForQualityUpdates

Configure this policy to specify whether to receive Windows Quality Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004

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

### -SetRestartWarningSchd

Turns on the policy that controls when notifications warn users about a scheduled restart for the update installation deadline. Specify the times with -ScheduleRestartWarning and -ScheduleImminentRestartWarning.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -SetUpdateNotificationLevel

Turns on the policy that defines which Windows Update notifications users see. Specify the level with -UpdateNotificationLevel.

Supported on: At least Windows 10 Server or Windows 10

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

### -TargetGroup

Name of the computer group to which the computer belongs.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -TargetGroupEnabled

Turns on client-side targeting. Specify the computer group with -TargetGroup.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -TargetReleaseVersion

Turns on the policy that limits the maximum version of Feature updates. Specify the version with -TargetReleaseVersionInfo and the product with -ProductVersion.

Supported on: At least Windows 10 Server or Windows 10

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

### -TargetReleaseVersionInfo

Sets the maximum version of Feature updates that the computer receives, for example '22H2'. Use it together with -TargetReleaseVersion and -ProductVersion.

Supported on: At least Windows 10 Server or Windows 10

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

### -UpdateNotificationLevel

Sets what Windows Update notifications users see: 'DefaultOS', 'DisableAllExcludingRestart', or 'DisableAllIncludingRestart'. This policy does not control how and when updates are downloaded and installed. Use it together with -SetUpdateNotificationLevel.

Supported on: At least Windows 10 Server or Windows 10

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
- DefaultOS
- DisableAllExcludingRestart
- DisableAllIncludingRestart
HelpMessage: ''
```

### -UseUpdateClassPolicySource

When this policy is enabled, devices will receive Windows updates for the classes listed from the specified update source: either Windows Update or Windows Server Update Service.

Supported on: At least Windows 10 Server or Windows 10

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

### -UseWUServer

Makes the computer get its updates from the WSUS server in -WUServer (writes 1). Use -UseWUServer:$false to get updates from Microsoft Update or Windows Update instead (writes 0).

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -WUServer

Sets the HTTP or HTTPS URL of the WSUS server that Automatic Updates and API callers use. It takes effect when -UseWUServer is on.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe computer names to ComputerName.

## OUTPUTS

### PSWindowsUpdateNext.WUSettings

An object with ComputerName and one property for each setting that you specified, plus RegistrySubKey1 and RegistrySubKey2 when the cmdlet had to create a registry key.

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. The cmdlet has a confirmation impact of High, so it asks for confirmation. Remote computers are changed through the remote registry, so the Remote Registry service must run there; see Enable-WURemoting. Which policies take effect depends on the Windows version. The description of each parameter shows the versions it supports.

## RELATED LINKS

- [Get-WUSettings](Get-WUSettings.md)
- [Get-WUServiceManager](Get-WUServiceManager.md)
