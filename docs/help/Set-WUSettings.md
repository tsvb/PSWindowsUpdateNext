---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: 
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Set-WUSettings
---

# Set-WUSettings

## SYNOPSIS

Set Windows Update Client settings.

## SYNTAX

### UNNAMED_PARAMETER_SET_1

```
Set-WUSettings [-AcceptTrustedPublisherCerts] [-ActiveHoursEnd <int>] [-ActiveHoursMaxRange <int>]
 [-ActiveHoursStart <int>] [-AllowAutoWindowsUpdateDownloadOverMeteredNetwork]
 [-AlwaysAutoRebootAtScheduledTime] [-AlwaysAutoRebootAtScheduledTimeMinutes <int>]
 [-AUPowerManagement] [-AutoInstallMinorUpdates] [-AutoRestartNotificationSchedule <int>]
 [-AutoRestartRequiredNotificationDismissal <string>] [-ComputerName <string[]>] [-Debuger]
 [-DeferFeatureUpdatesPeriodInDays <int>] [-DeferQualityUpdatesPeriodInDays <int>]
 [-DeferUpdatePeriod <int>] [-DeferUpgrade] [-DeferUpgradePeriod <int>] [-DetectionFrequency <int>]
 [-DetectionFrequencyEnabled] [-DisableWindowsUpdateAccess] [-DisableWUfBSafeguards]
 [-ElevateNonAdmins] [-EnableFeaturedSoftware] [-IncludeRecommendedUpdates]
 [-ManagePreviewBuilds <int>] [-NoAUShutdownOption] [-NoAutoRebootWithLoggedOnUsers] [-NoAutoUpdate]
 [-NotificationLevel <string>] [-PauseDeferrals] [-ProductVersion <string>]
 [-PSWUSettings <Hashtable>] [-RebootRelaunchTimeout <int>] [-RebootRelaunchTimeoutEnabled]
 [-RebootWarningTimeout <int>] [-RebootWarningTimeoutEnabled] [-RescheduleWaitTime <int>]
 [-RescheduleWaitTimeEnabled] [-ScheduledInstallDay <string>] [-ScheduledInstallTime <int>]
 [-ScheduleImminentRestartWarning <int>] [-ScheduleRestartWarning <int>] [-SendReport]
 [-SetActiveHours] [-SetActiveHoursMaxRange] [-SetAutoRestartNotificationConfig]
 [-SetAutoRestartNotificationDisable] [-SetAutoRestartRequiredNotificationDismissal]
 [-SetDisablePauseUXAccess] [-SetDisableUXWUAccess] [-SetEDURestart]
 [-SetPolicyDrivenUpdateSourceForDriverUpdates] [-SetPolicyDrivenUpdateSourceForFeatureUpdates]
 [-SetPolicyDrivenUpdateSourceForOtherUpdates] [-SetPolicyDrivenUpdateSourceForQualityUpdates]
 [-SetRestartWarningSchd] [-SetUpdateNotificationLevel] [-TargetGroup <string>]
 [-TargetGroupEnabled] [-TargetReleaseVersion] [-TargetReleaseVersionInfo <string>]
 [-UpdateNotificationLevel <string>] [-UseUpdateClassPolicySource] [-UseWUServer]
 [-WUServer <string>] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

Use Set-WUSettings cmdlet to Set configuration of Windows Update Client.

## EXAMPLES

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
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ActiveHoursEnd

If you enable this policy, the PC will not automatically restart after updates during active hours. The PC will attempt to restart outside of active hours.

Supported on: At least Windows 10 Server or Windows 10

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

### -ActiveHoursMaxRange

Enable this policy to specify the maximum number of hours from the start time that users can set their active hours. The max active hours range can be set between 8 and 18 hours.

Supported on: At least Windows 10 Server or Windows 10

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

### -ActiveHoursStart

If you enable this policy, the PC will not automatically restart after updates during active hours. The PC will attempt to restart outside of active hours.

Supported on: At least Windows 10 Server or Windows 10

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

### -AllowAutoWindowsUpdateDownloadOverMeteredNetwork

Enabling this policy will automatically download updates, even over metered data connections (charges may apply).

Supported on: At least Windows 10 Server, Windows 10 or Windows 10 RT

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

### -AlwaysAutoRebootAtScheduledTime

If you enable this policy, a restart timer will always begin immediately after Windows Update installs important updates, instead of first notifying users on the login screen for at least two days..

Supported on: At least Windows Server 2012, Windows 8 or Windows RT

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

### -AlwaysAutoRebootAtScheduledTimeMinutes

The restart timer can be configured to start with any value from 15 to 180 minutes. When the timer runs out, the restart will proceed even if the PC has signed-in users.

Supported on: At least Windows Server 2012, Windows 8 or Windows RT​

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

### -AUOptions

Notify mode: "Not configured", "Disabled", "Notify before download", "Notify before installation", "Scheduled installation", "Users configure"

Supported on: Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3 Option 7 only supported on servers of at least Windows Server 2016 edition​

This is an alias of the NotificationLevel parameter.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- AUOptions
ParameterSets: []
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
ParameterSets: []
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
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -AutoRestartNotificationSchedule

Specifies the type of notification used when notifying the user about a scheduled restart to install updates.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -AutoRestartRequiredNotificationDismissal

Enable this policy to specify the method by which the auto-restart required notification is dismissed. When a restart is required to install updates, the auto-restart required notification is displayed. By default, the notification is automatically dismissed after 25 seconds.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -ComputerName

Specify one or more computer names for remote connection.

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

### -DeferFeatureUpdatesPeriodInDays

Enable this policy to specify when to receive Feature Updates.

Supported on: At least Windows 10 Server or Windows 10​

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

### -DeferQualityUpdatesPeriodInDays

Enable this policy to specify when to receive quality updates.

Supported on: At least Windows 10 Server or Windows 10​

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

### -DeferUpdatePeriod

Allows IT Admins to specify update delays for up to four weeks. Supported values are 0-4, which refers to the number of weeks to defer updates.

Supported on: At least Windows 10 Server or Windows 10​

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

### -DeferUpgrade

You can also choose to delay updates for up to one month. If you do not delay updates, your PC will remain up to date with security updates as they become available.

Supported on: At least Windows 10 Server or Windows 10​

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

### -DeferUpgradePeriod

Allows IT Admins to specify additional upgrade delays for up to 8 months. Supported values are 0-8, which refers to the number of months to defer upgrades.

Supported on: At least Windows 10 Server or Windows 10​

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

### -DetectionFrequency

Time between detection cycles. Time in hours (1–22).

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -DetectionFrequencyEnabled

Enable/Disable detection frequency.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -DisableWindowsUpdateAccess

Disables/Enables access to Windows Update.

Supported on: At least Windows Server 2003 operating systems, Windows XP Professional Service Pack 1, or Windows 2000 Service Pack 3

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

### -DisableWUfBSafeguards

This policy setting specifies that a Windows Update for Business device should skip safeguards.

Supported on: At least Windows Server, Windows 10 Version 1903​

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

### -ElevateNonAdmins

This policy setting allows you to control whether non-administrative users will receive update notifications based on the "Configure Automatic Updates" policy setting.

Supported on: At least Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3 through Windows 8.1 or Windows Server 2012 R2 with most current service pack. Not supported on Windows 10 and above..

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

### -EnableFeaturedSoftware

This policy setting allows you to control whether users see detailed enhanced notification messages about featured software from the Microsoft Update service.

Supported on: Windows Server 2008, Windows 7, and Windows Vista​

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

### -IncludeRecommendedUpdates

Enable/Disable recommended updates.

Supported on: At least Windows Vista through Windows 8.1 or Windows Server 2012 R2 with most current service pack. Not supported on Windows 10 and above.

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

### -ManagePreviewBuilds

Enable this policy to manage which updates you receive prior to the update being released to the world.

0	Disable Preview builds.

1	Disable Preview builds once the next release is public.

2	Enable Preview builds.

3 (Default) Preview builds is left to user selection.

Supported on: At least Windows Server, Windows 10 Version 1709​

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

### -NoAUShutdownOption

This policy setting allows you to manage whether the 'Install Updates and Shut Down' option is displayed in the Shut Down Windows dialog box.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows XP SP2

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

### -NoAutoRebootWithLoggedOnUsers

Logged-on user can decide whether to restart the client computer.

Supported on: Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3​

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

### -NoAutoUpdate

Enable/Disable Automatic Updates.

Supported on: Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3 Option 7 only supported on servers of at least Windows Server 2016 edition​

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

### -NotificationLevel

Notify mode: "Not configured", "Disabled", "Notify before download", "Notify before installation", "Scheduled installation", "Users configure"

Supported on: Windows XP Professional Service Pack 1 or At least Windows 2000 Service Pack 3 Option 7 only supported on servers of at least Windows Server 2016 edition​

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- AUOptions
ParameterSets: []
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PauseDeferrals

Allows IT Admins to pause updates and upgrades for up to 5 weeks.

Supported on: At least Windows 10 Server or Windows 10​

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

### -ProductVersion

ProductVersion allow to control maximum version of Feature updates. For example "Windows 11" or "11" or "Windows 10".

Supported on: At least Windows 10 Server or Windows 10​

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

### -RebootRelaunchTimeout

Time between prompts for a scheduled restart. Time in minutes (1–1440).

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1 , Windows 2000 SP4, Windows 2000 SP3

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

### -RebootRelaunchTimeoutEnabled

Specifies the amount of time for Automatic Updates to wait before prompting again with a scheduled restart.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1 , Windows 2000 SP4, Windows 2000 SP3

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

### -RebootWarningTimeout

Length, in minutes, of the restart warning countdown after updates have been installed that have a deadline or scheduled updates. Time in minutes (1–30).

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1 , Windows 2000 SP4, Windows 2000 SP3​

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

### -RebootWarningTimeoutEnabled

Enable/Disable RebootWarningTimeout.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1 , Windows 2000 SP4, Windows 2000 SP3​

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

### -RescheduleWaitTime

Specifies the amount of time for Automatic Updates to wait, following system startup, before proceeding with a scheduled installation that was missed previously.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1 , Windows 2000 SP4, Windows 2000 SP3

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

### -RescheduleWaitTimeEnabled

Specifies the amount of time for Automatic Updates to wait, following system startup, before proceeding with a scheduled installation that was missed previously.

Supported on: Windows 7, Windows Server 2008 R2, Windows Vista, Windows Server 2003, Windows XP SP2, Windows XP SP1 , Windows 2000 SP4, Windows 2000 SP3

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

### -ScheduledInstallDay

Scheduled day of install: "Every day", "Every Sunday", "Every Monday", "Every Tuesday", "Every Wednesday", "Every Thursday", ""Every Friday", "EverySaturday". Only valid if NotificationLevel (AUOptions) = "Scheduled installation"

Starting with Windows 8 and Windows Server 2012, ScheduledInstallationDay are not supported and will return unreliable values.If you try to modify these properties, the operation will appear to succeed but will have no effect.

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

### -ScheduledInstallTime

Scheduled time of install in 24-hour format(0–23).

Starting with Windows 8 and Windows Server 2012, ScheduledInstallTime are not supported and will return unreliable values.If you try to modify these properties, the operation will appear to succeed but will have no effect.

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

### -ScheduleImminentRestartWarning

Enable this policy to control when notifications are displayed to warn users about a scheduled restart for the update installation deadline. 15 Minutes (Default), 30 Minutes or 60 Minutes.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -ScheduleRestartWarning

Enable this policy to control when notifications are displayed to warn users about a scheduled restart for the update installation deadline. 2 Hours, 4 Hours (Default), 8 Hours, 12 Hours, 24 Hours.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -SendReport

Send report email to specific recipients.

Requires the parameter -PSWUSettings or declare the PSWUSettings.xml file in ModuleBase path.

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

### -SetActiveHours

If you enable this policy, the PC will not automatically restart after updates during active hours. The PC will attempt to restart outside of active hours.

Supported on: At least Windows 10 Server or Windows 10

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

### -SetActiveHoursMaxRange

Enable this policy to specify the maximum number of hours from the start time that users can set their active hours. The max active hours range can be set between 8 and 18 hours.

Supported on: At least Windows 10 Server or Windows 10

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

### -SetAutoRestartNotificationConfig

Specifies the type of notification used when notifying the user about a scheduled restart to install updates.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -SetAutoRestartNotificationDisable

This policy setting allows you to control whether users receive notifications for auto restarts for update installations including reminder and warning notifications.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10​

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

### -SetAutoRestartRequiredNotificationDismissal

Enable this policy to specify the method by which the auto-restart required notification is dismissed. When a restart is required to install updates, the auto-restart required notification is displayed. By default, the notification is automatically dismissed after 25 seconds.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10​

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

### -SetDisablePauseUXAccess

This setting allows to remove access to "Pause updates" feature. Once enabled user access to pause updates is removed.

Supported on: At least Windows Server, Windows 10 Version 1809

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

### -SetDisableUXWUAccess

This setting allows you to remove access to scan Windows Update. If you enable this setting, Windows Update scan access is removed.

Supported on: At least Windows 10 Server or Windows 10

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

### -SetEDURestart

Enabling this policy for EDU devices that remain on Carts overnight will skip power checks to ensure update reboots will happen at the scheduled install time.

Supported on: At least Windows 10 Server, Windows 10 or Windows 10 RT​

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

### -SetPolicyDrivenUpdateSourceForDriverUpdates

Configure this policy to specify whether to receive Windows Driver Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004​

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

### -SetPolicyDrivenUpdateSourceForFeatureUpdates

Configure this policy to specify whether to receive Windows Feature Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004​

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

### -SetPolicyDrivenUpdateSourceForOtherUpdates

Configure this policy to specify whether to receive Other Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004​

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

### -SetPolicyDrivenUpdateSourceForQualityUpdates

Configure this policy to specify whether to receive Windows Quality Updates from Windows Update endpoint, managed by Windows Update for Business policies, or through your configured Windows Server Update Service (WSUS) server.

Supported on: At least Windows Server 2022, or Windows 10 Version 2004​

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

### -SetRestartWarningSchd

Enable this policy to control when notifications are displayed to warn users about a scheduled restart for the update installation deadline.

Supported on: Windows Server 2016 through Windows Server 2022, or Windows 10

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

### -SetUpdateNotificationLevel

This policy allows you to define what Windows Update notifications users see. This policy doesn't control how and when updates are downloaded and installed.

Supported on: At least Windows 10 Server or Windows 10

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

### -TargetGroup

Name of the computer group to which the computer belongs.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -TargetGroupEnabled

Use/Do not use client-side targeting.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -TargetReleaseVersion

Enable settings for control maximum version of Feature updates.

Supported on: At least Windows 10 Server or Windows 10​

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

### -TargetReleaseVersionInfo

TargetReleaseVersionInfo allow to control maximum version of Feature updates. For example "22H2".

Supported on: At least Windows 10 Server or Windows 10​

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

### -UpdateNotificationLevel

This policy allows you to define what Windows Update notifications users see. This policy doesn't control how and when updates are downloaded and installed.

Supported on: At least Windows 10 Server or Windows 10

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

### -UseUpdateClassPolicySource

When this policy is enabled, devices will receive Windows updates for the classes listed from the specified update source: either Windows Update or Windows Server Update Service.

Supported on: At least Windows 10 Server or Windows 10

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

### -UseWUServer

The computer gets its updates from a WSUS server or from Microsoft Update.

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### -WUServer

HTTP(S) URL of the WSUS server that is used by Automatic Updates and API callers (by default).

Supported on: At least Windows XP Professional Service Pack 1 or Windows 2000 Service Pack 3, excluding Windows RT

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

Specify one or more computer names for remote connection.

## OUTPUTS

### PSWindowsUpdateNext.WUSettings

OutputType for WUSettings objects.

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

