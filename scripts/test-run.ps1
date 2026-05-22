param(
    [switch]$NoBuild
)

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$TestProject = Join-Path $RepoRoot "Umbraco.Extension.Test\Umbraco.Extension.Test.csproj"

$dotnetArgs = @(
    "test",
    $TestProject,
    "--no-restore",
    "--nologo",
    "--verbosity", "minimal",
    "--filter", "FullyQualifiedName~Validators",
    "--logger", "console;verbosity=detailed"
)

if ($NoBuild) {
    $dotnetArgs += "--no-build"
}

$output = & dotnet @dotnetArgs 2>&1
$exitCode = $LASTEXITCODE

$testLines = $output | Where-Object {
    $_ -match "^\s+Passed\s" -or
    $_ -match "^\s+Failed\s" -or
    $_ -match "^\s+Skipped\s" -or
    $_ -match "^Test Run" -or
    $_ -match "^Total tests:" -or
    $_ -match "^\s+Passed:" -or
    $_ -match "^\s+Failed:" -or
    $_ -match "^\s+Skipped:"
}

$addBlankBeforeNextLine = $false

foreach ($line in $testLines) {
    if ($addBlankBeforeNextLine) {
        Write-Host ""
        $addBlankBeforeNextLine = $false
    }

    if ($line -match "^Test Run Successful") {
        Write-Host $line -ForegroundColor Green
        $addBlankBeforeNextLine = $true
        continue
    }

    if ($line -match "^Test Run Failed") {
        Write-Host $line -ForegroundColor Red
        $addBlankBeforeNextLine = $true
        continue
    }

    if ($line -match "^\s+Passed\s" -or $line -match "^\s+Passed:") {
        Write-Host $line -ForegroundColor Green
    }
    elseif ($line -match "^\s+Failed\s" -or $line -match "^\s+Failed:") {
        Write-Host $line -ForegroundColor Red
    }
    elseif ($line -match "^\s+Skipped\s" -or $line -match "^\s+Skipped:") {
        Write-Host $line -ForegroundColor Yellow
    }
    else {
        Write-Host $line
    }
}

if ($exitCode -eq 0) {
    Write-Host ""
    Write-Host "PASS: Validator tests passed" -ForegroundColor Green
}
else {
    Write-Host ""
    Write-Host "FAIL: Validator tests failed" -ForegroundColor Red

    $errorLines = $output | Where-Object {
        $_ -match "^\s+Failed\s" -or
        $_ -match "Error Message:" -or
        $_ -match "Expected" -or
        $_ -match "But was" -or
        $_ -match "\.cs\(\d+,\d+\): error" -or
        $_ -match ": error "
    }

    if ($errorLines) {
        Write-Host ""
        Write-Host "Error details:" -ForegroundColor Red

        foreach ($line in $errorLines) {
            Write-Host $line -ForegroundColor Red
        }
    }
}

exit $exitCode