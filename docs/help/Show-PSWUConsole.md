---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Show-PSWUConsole.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Show-PSWUConsole
---

# Show-PSWUConsole

## SYNOPSIS

Opens the PSWindowsUpdateNext desktop console, a WPF front end for the module's cmdlets.

## SYNTAX

### __AllParameterSets

```
Show-PSWUConsole [-PassThru] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Opens a dark, keyboard-friendly window over the same cmdlets that the module exports. It has eight pages: Dashboard (reboot state, last search and install, agent version, install statistics, and the client policy), Updates (scan, filter, download, install, hide, and unhide), History, Services (register Microsoft Update or remove a service), Settings (view and edit the Windows Update policy), Catalog (search the Microsoft Update Catalog and download packages with SHA-1 digest and Authenticode verification), Jobs (list, create, and clear the module's Task Scheduler jobs), and Log.

Every operation runs in a background runspace, so the window stays responsive, and verbose, warning, and error output goes to the Log page. Destructive actions ask for confirmation first.

The function blocks until the window is closed. It runs on Windows only, in Windows PowerShell 5.1 or PowerShell 7, and needs a single-threaded apartment (the default for the PowerShell console).

Most actions need an elevated session: scanning, downloading, installing, hiding and unhiding updates, restarting, registering or removing update services, applying settings, and creating or clearing jobs. Without elevation the sidebar offers "Restart as administrator", and read-only pages such as History and the Catalog download still work.

## EXAMPLES

### Example 1: Open the console

```powershell
Show-PSWUConsole
```

Opens the console and waits until you close it.

### Example 2: Open the console in an elevated session

```powershell
Start-Process pwsh -Verb RunAs -ArgumentList '-NoProfile', '-Command', 'Import-Module PSWindowsUpdateNext; Show-PSWUConsole'
```

Opens the console in a new elevated PowerShell 7 session, so that every action is available.

### Example 3: Get the window object

```powershell
$window = Show-PSWUConsole -PassThru
$window.Topmost = $true
$window.ShowDialog() | Out-Null
```

Creates the window without showing it, keeps it above other windows, and then shows it.

## PARAMETERS

### -PassThru

Return the window object without showing it. Call Show() or ShowDialog() yourself. Without this switch the function shows the window modally and returns nothing.

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

This function does not accept pipeline input.

## OUTPUTS

### None

By default the function returns nothing.

### System.Windows.Window

With -PassThru, the console window.

## NOTES

Requires Windows. Importing the module does not load WPF; the assemblies are loaded when the function runs. The console works on the local computer and calls the module's own cmdlets, so it needs the same elevation as they do.

This page is reference documentation for a function that has comment-based help in module\Gui\Show-PSWUConsole.ps1. It is not part of PSWindowsUpdateNext.dll-Help.xml. If you change the help there, change this page too.

## RELATED LINKS

- [Get-WindowsUpdate](Get-WindowsUpdate.md)
