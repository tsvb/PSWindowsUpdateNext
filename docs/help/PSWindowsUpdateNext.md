---
document type: module
Help Version: 3.1.0.0
HelpInfoUri: ''
Locale: en-US
Module Guid: 5f0c9b4e-8a6d-4e2b-9c1f-2d7a3e6b8c01
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: PSWindowsUpdateNext Module
---

# PSWindowsUpdateNext Module

## Description

Cmdlets to manage the Windows Update client. Community-maintained continuation of PSWindowsUpdate with PowerShell 7 support and security fixes. The cmdlets run in Windows PowerShell 5.1 and PowerShell 7 on Windows; most of them need an elevated session.

The module also exports these aliases: Install-WindowsUpdate, Download-WindowsUpdate, Hide-WindowsUpdate, UnHide-WindowsUpdate, Show-WindowsUpdate, Get-WUList, and Get-WUInstall (for Get-WindowsUpdate); Uninstall-WindowsUpdate (for Remove-WindowsUpdate); and Clear-WUJob (for Get-WUJob).

The markdown pages in this folder are the source of the compiled help file module\PSWindowsUpdateNext.dll-Help.xml. Do not edit the XML by hand; edit the pages and run tools\Build-Help.ps1.

## Cmdlets

### [Add-WUServiceManager](Add-WUServiceManager.md)

Registers a Windows Update service.

### [Enable-WURemoting](Enable-WURemoting.md)

Enables the firewall rules and settings that PSWindowsUpdateNext remoting needs.

### [Get-WindowsUpdate](Get-WindowsUpdate.md)

Gets the updates that match your criteria and optionally downloads, installs, hides, or unhides them.

### [Get-WUApiVersion](Get-WUApiVersion.md)

Gets the Windows Update Agent version.

### [Get-WUHistory](Get-WUHistory.md)

Gets the Windows Update history.

### [Get-WUInstallerStatus](Get-WUInstallerStatus.md)

Gets the Windows Update Installer status.

### [Get-WUJob](Get-WUJob.md)

Gets the Task Scheduler jobs created by the module.

### [Get-WULastResults](Get-WULastResults.md)

Gets the date of the last Windows Update search and installation.

### [Get-WUOfflineMSU](Get-WUOfflineMSU.md)

Downloads update packages from the Microsoft Update Catalog.

### [Get-WURebootStatus](Get-WURebootStatus.md)

Gets the Windows Update reboot status and optionally restarts the computer.

### [Get-WUServiceManager](Get-WUServiceManager.md)

Gets the registered Windows Update services.

### [Get-WUSettings](Get-WUSettings.md)

Gets the Windows Update client policy settings.

### [Invoke-WUJob](Invoke-WUJob.md)

Runs a PowerShell script on local or remote computers through a Task Scheduler job.

### [Remove-WindowsUpdate](Remove-WindowsUpdate.md)

Uninstalls a Windows update.

### [Remove-WUServiceManager](Remove-WUServiceManager.md)

Unregisters a Windows Update service.

### [Reset-WUComponents](Reset-WUComponents.md)

Resets the Windows Update components on the local computer.

### [Set-PSWUSettings](Set-PSWUSettings.md)

Saves the email settings that PSWindowsUpdateNext uses for reports.

### [Set-WUSettings](Set-WUSettings.md)

Sets the Windows Update client policy settings.

### [Update-WUModule](Update-WUModule.md)

Installs or updates PSWindowsUpdateNext on remote computers.

## Functions

### [Show-PSWUConsole](Show-PSWUConsole.md)

Opens the PSWindowsUpdateNext desktop console, a WPF front end for the module's cmdlets.

