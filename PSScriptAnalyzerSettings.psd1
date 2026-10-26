@{
    # CI fails on anything reported here (see the "Lint PowerShell" step in .github/workflows/build.yml).
    # Information-level rules (positional parameters, missing comment help, and so on) are advice, not gates.
    Severity     = @('Error', 'Warning')

    # All other default rules run. Exclusions need a reason.
    ExcludeRules = @(
        # build.ps1 and tools\*.ps1 are build tooling that talks to the person or the CI log on purpose.
        # Write-Host writes to the information stream (PowerShell 5.0 and later), so it can be captured
        # and redirected, and these scripts do not return data that it would pollute.
        'PSAvoidUsingWriteHost'
    )

    Rules        = @{
        # The module has to load in Windows PowerShell 5.1 and PowerShell 7. This turns the rule on (it is off by
        # default) and flags syntax that 5.1 cannot parse, for example the ternary operator, ?. and ??, pipeline
        # chains (&& and ||), and classes or using statements when targeting an older version.
        #
        # PSUseCompatibleCommands and PSUseCompatibleTypes are left out on purpose. They need a named
        # target profile from the PSScriptAnalyzer profile catalogue, and a wrong profile name makes
        # the rule fail rather than report. The Pester suite already runs in both editions in CI.
        PSUseCompatibleSyntax = @{
            Enable         = $true
            TargetVersions = @('5.1', '7.0')
        }
    }
}
