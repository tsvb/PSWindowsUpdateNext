#Requires -Version 5.1
<#
.SYNOPSIS
    Regenerates module\PSWindowsUpdateNext.dll-Help.xml from the markdown pages in docs\help.
.DESCRIPTION
    The markdown in docs\help is the source of truth for the module's help. This script validates the pages,
    converts them to MAML with Microsoft.PowerShell.PlatyPS, and writes module\PSWindowsUpdateNext.dll-Help.xml.
    The XML file name and location are fixed because PowerShell finds the help of a binary module by the name
    of its assembly (PSWindowsUpdateNext.dll-Help.xml next to the module manifest).

    Do not edit the XML by hand. Edit the markdown and run this script. Commit both.

    PREREQUISITE: Microsoft.PowerShell.PlatyPS 1.x (the C# rewrite, not the legacy platyPS 0.14 module).
    On a normal machine install it from the PowerShell Gallery:

        Install-PSResource -Name Microsoft.PowerShell.PlatyPS -Scope CurrentUser

    Install-PSResource comes with PowerShell 7.4 and later. In Windows PowerShell 5.1 without PSResourceGet use
    Install-Module -Name Microsoft.PowerShell.PlatyPS -Scope CurrentUser instead.

    What the script does:
      1. Checks that docs\help has one page for every name in CmdletsToExport of the module manifest and no
         page for a name the module does not export (Show-PSWUConsole is a function and has a reference page
         that is not compiled into the XML).
      2. Validates each cmdlet page (Test-MarkdownCommandHelp) and fails on unfilled {{ placeholders }}.
      3. Imports the pages (Import-MarkdownCommandHelp) and exports MAML (Export-MamlCommandHelp) to a
         temporary folder.
      4. Checks that the XML is well-formed and contains exactly the exported cmdlets.
      5. Copies the XML to module\ (or, with -Check, compares it with the committed file and fails if they differ).

    Writing the markdown. The pages follow the Microsoft.PowerShell.PlatyPS 1.x schema. Syntax, parameter types,
    parameter sets, aliases, and pipeline binding must match the compiled cmdlets; the prose is hand-written.
    After the cmdlets change, build the module, import it, and run this script with -UpdateMarkdown to add new
    parameters to the pages as {{ Fill ... }} placeholders, then write their descriptions. Update-MarkdownCommandHelp
    only adds and changes; it does not remove parameters that no longer exist, so review the diff.
.PARAMETER HelpPath
    Folder that holds the markdown pages. Default: ..\docs\help relative to this script.
.PARAMETER ModulePath
    Folder that holds PSWindowsUpdateNext.psd1 and the help XML. Default: ..\module relative to this script.
.PARAMETER Check
    Do not write anything. Fail (exit code 1) if the committed XML differs from what the markdown produces.
    Use it in CI to catch markdown edits that were not rebuilt. Pin the PlatyPS version in CI, because a different
    version can format the XML differently.
.PARAMETER UpdateMarkdown
    Before building, import the module from ModulePath and run Update-MarkdownCommandHelp on every cmdlet page
    so that the pages pick up parameters added to the cmdlets. Needs Windows, because the module loads
    Windows-only assemblies. Backups are not kept; use git to review the changes.
.EXAMPLE
    .\tools\Build-Help.ps1

    Rebuilds module\PSWindowsUpdateNext.dll-Help.xml from docs\help.
.EXAMPLE
    .\tools\Build-Help.ps1 -Check

    Verifies that the committed XML is in sync with the markdown (for CI).
.EXAMPLE
    .\tools\Build-Help.ps1 -UpdateMarkdown

    Refreshes the markdown from the built module, then rebuilds the XML.
#>
[CmdletBinding()]
param(
    [string]$HelpPath = (Join-Path $PSScriptRoot '..\docs\help'),
    [string]$ModulePath = (Join-Path $PSScriptRoot '..\module'),
    [switch]$Check,
    [switch]$UpdateMarkdown
)

$ErrorActionPreference = 'Stop'

$moduleName = 'PSWindowsUpdateNext'
$xmlName = "$moduleName.dll-Help.xml"
$HelpPath = (Resolve-Path -LiteralPath $HelpPath).Path
$ModulePath = (Resolve-Path -LiteralPath $ModulePath).Path
$manifestPath = Join-Path $ModulePath "$moduleName.psd1"
$targetPath = Join-Path $ModulePath $xmlName

if (-not (Get-Module -ListAvailable -Name Microsoft.PowerShell.PlatyPS)) {
    throw ("Microsoft.PowerShell.PlatyPS is not installed. Run: Install-PSResource -Name Microsoft.PowerShell.PlatyPS -Scope CurrentUser " +
        "(or Install-Module -Name Microsoft.PowerShell.PlatyPS -Scope CurrentUser). The legacy platyPS 0.14 module is not supported.")
}
Import-Module Microsoft.PowerShell.PlatyPS -MinimumVersion 1.0.1 -ErrorAction Stop
$platyPS = Get-Module Microsoft.PowerShell.PlatyPS
Write-Host "Using Microsoft.PowerShell.PlatyPS $($platyPS.Version)"

$manifest = Import-PowerShellDataFile -LiteralPath $manifestPath
$cmdlets = @($manifest.CmdletsToExport)
$functions = @($manifest.FunctionsToExport)

# 1. Pages and exports must correspond.
$problems = New-Object System.Collections.Generic.List[string]
foreach ($name in $cmdlets) {
    if (-not (Test-Path -LiteralPath (Join-Path $HelpPath "$name.md"))) {
        $problems.Add("Missing page for exported cmdlet $name (expected docs\help\$name.md).")
    }
}
$knownPages = @($cmdlets) + @($functions) + @($moduleName)
foreach ($page in Get-ChildItem -LiteralPath $HelpPath -Filter '*.md') {
    if ($page.BaseName -notin $knownPages) {
        $problems.Add("Page $($page.Name) does not match anything the module exports. Remove it or add the name to the manifest.")
    }
}
foreach ($name in $functions) {
    if (-not (Test-Path -LiteralPath (Join-Path $HelpPath "$name.md"))) {
        $problems.Add("Missing reference page for exported function $name.")
    }
}
if ($problems.Count) { throw ($problems -join [Environment]::NewLine) }

# Optional: refresh the markdown from the compiled cmdlets.
if ($UpdateMarkdown) {
    Import-Module -Name $manifestPath -Force -ErrorAction Stop
    foreach ($name in $cmdlets) {
        Update-MarkdownCommandHelp -Path (Join-Path $HelpPath "$name.md") -NoBackup | Out-Null
    }
    Write-Host 'Markdown refreshed from the module. Review the changes with git diff and fill in any {{ placeholders }}.'
}

# 2. Validate the cmdlet pages.
foreach ($name in $cmdlets) {
    $file = Join-Path $HelpPath "$name.md"
    if (-not (Test-MarkdownCommandHelp -Path $file)) {
        $detail = Test-MarkdownCommandHelp -Path $file -DetailView
        $problems.Add("Page $name.md does not match the PlatyPS schema:" + [Environment]::NewLine + ($detail | Out-String))
    }
    $placeholders = Select-String -LiteralPath $file -Pattern '\{\{.*?\}\}' -AllMatches
    foreach ($hit in $placeholders) {
        $problems.Add("${name}.md line $($hit.LineNumber) still has a placeholder: $($hit.Line.Trim())")
    }
}
if ($problems.Count) { throw ($problems -join [Environment]::NewLine) }

# 3. Convert to MAML in a temporary folder.
$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ([System.IO.Path]::GetRandomFileName())
try {
    $commandHelp = foreach ($name in $cmdlets) {
        Import-MarkdownCommandHelp -Path (Join-Path $HelpPath "$name.md")
    }
    Export-MamlCommandHelp -CommandHelp $commandHelp -OutputFolder $tempRoot -Force | Out-Null

    # Export-MamlCommandHelp writes <OutputFolder>\<Module Name>\<external help file>.
    $generated = Join-Path (Join-Path $tempRoot $moduleName) $xmlName
    if (-not (Test-Path -LiteralPath $generated)) {
        throw "Export-MamlCommandHelp did not create $xmlName. Check the 'Module Name' and 'external help file' front matter of the pages."
    }

    # 4. Well-formed, and exactly the exported cmdlets.
    $text = Get-Content -LiteralPath $generated -Raw -Encoding UTF8
    $xml = New-Object System.Xml.XmlDocument
    $xml.LoadXml($text)
    $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $ns.AddNamespace('command', 'http://schemas.microsoft.com/maml/dev/command/2004/10')
    $inXml = @($xml.SelectNodes('//command:command/command:details/command:name', $ns) | ForEach-Object { $_.InnerText })
    $diff = Compare-Object -ReferenceObject $cmdlets -DifferenceObject $inXml
    if ($diff) {
        throw ("The generated XML does not contain exactly the exported cmdlets:" + [Environment]::NewLine + ($diff | Out-String))
    }

    # 5. Write, or compare.
    if ($Check) {
        $current = if (Test-Path -LiteralPath $targetPath) { Get-Content -LiteralPath $targetPath -Raw -Encoding UTF8 } else { '' }
        $a = $current -replace "`r`n", "`n"
        $b = $text -replace "`r`n", "`n"
        if ($a -ne $b) {
            Write-Error "$xmlName is out of date with docs\help. Run tools\Build-Help.ps1 and commit the result."
            exit 1
        }
        Write-Host "$xmlName is up to date ($($inXml.Count) cmdlets)."
    }
    else {
        Copy-Item -LiteralPath $generated -Destination $targetPath -Force
        Write-Host "Wrote $targetPath ($($inXml.Count) cmdlets)."
    }
}
finally {
    if (Test-Path -LiteralPath $tempRoot) { Remove-Item -LiteralPath $tempRoot -Recurse -Force -ErrorAction SilentlyContinue }
}
