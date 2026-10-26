---
document type: cmdlet
external help file: PSWindowsUpdateNext.dll-Help.xml
HelpUri: https://github.com/tsvb/PSWindowsUpdateNext/blob/main/docs/help/Reset-WUComponents.md
Locale: en-US
Module Name: PSWindowsUpdateNext
ms.date: 10/26/2026
PlatyPS schema version: 2024-05-01
title: Reset-WUComponents
---

# Reset-WUComponents

## SYNOPSIS

Resets the Windows Update components on the local computer.

## SYNTAX

### __AllParameterSets

```
Reset-WUComponents [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Resets the Windows Update client to a clean state in ten steps: it stops the BITS, Windows Update, Application Identity, and Cryptographic Services services; deletes the BITS queue files (qmgr*.dat); renames the SoftwareDistribution and catroot2 folders to .bak copies; deletes the old WindowsUpdate.log; resets the BITS and Windows Update service security descriptors; re-registers the related DLLs in System32 with regsvr32, skipping any that are not present; resets Winsock and the WinHTTP proxy with netsh; restarts the services; and asks the agent to re-detect updates (wuauclt /resetauthorization /detectnow).

Use it when updates fail to download or install because the local update store is damaged. Progress headings are written to the host; use -Verbose to see the individual operations.

## EXAMPLES

### Example 1: Reset the Windows Update components

```powershell
Reset-WUComponents -Verbose
```

```output
Step 1: Stop Windows Update services
VERBOSE: Background Intelligent Transfer Service (BITS)
VERBOSE: Windows Update (wuauserv)
VERBOSE: Application Identity (appidsvc)
VERBOSE: Cryptographic Services (cryptsvc)
Step 2: Delete the qmgr*.dat files
Step 3: Back up software distribution folders
VERBOSE: Renaming Software Distribution folder to C:\WINDOWS\SoftwareDistribution.bak
VERBOSE: Renaming CatRoot  folder to C:\WINDOWS\System32\Catroot2.bak
Step 4: Remove old Windows Update logs
Step 5: Reset Windows Update services
Step 6: Re-register DLLs
...
Step 7: Reset WinSock
Step 8: Reset Proxy
Step 9: Start Windows Update services
Step 10: Start Windows Update services
```

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

### None

## NOTES

This cmdlet requires an elevated session and stops with an error when the session is not elevated. It runs without asking for confirmation. -WhatIf and -Confirm are accepted but the cmdlet does not check them, so the reset always runs. The cmdlet takes no other parameters and always acts on the local computer. Restart the computer afterwards if Windows Update still reports problems.

## RELATED LINKS

- [Get-WindowsUpdate](Get-WindowsUpdate.md)
- [Get-WUInstallerStatus](Get-WUInstallerStatus.md)
