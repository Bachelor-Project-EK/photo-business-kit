# run with: ./coverage-report.ps1

Push-Location $PSScriptRoot

Remove-Item -Recurse -Force "coveragereport" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "TestResults" -ErrorAction SilentlyContinue

if (-not (Test-Path "dotnet-tools.json")) {
dotnet new tool-manifest --force
}

dotnet tool restore

dotnet tool install dotnet-reportgenerator-globaltool 2>$null

dotnet test Umbraco.Extension.Test/Umbraco.Extension.Test.csproj --collect:"XPlat Code Coverage"

dotnet tool run reportgenerator `
    -reports:"Umbraco.Extension.Test/TestResults/**/coverage.cobertura.xml" `
    -targetdir:"coveragereport" `
    -reporttypes:Html `
    -classfilters:"+*.Services.*;+*.Validators.*"

Start-Process "coveragereport/index.html"

Pop-Location