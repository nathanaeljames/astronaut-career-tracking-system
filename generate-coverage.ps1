# Check if reportgenerator is installed
if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
    Write-Host "reportgenerator not found. Installing..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --settings:.runsettings

# Generate reports
reportgenerator -reports:"**\TestResults\*\coverage.cobertura.xml" -targetdir:"CoverageReport-UnitTests" -reporttypes:Html -classfilters:"-StargateAPI.Migrations.*;-StargateAPI.Program;-StargateAPI.Controllers.*"

reportgenerator -reports:"**\TestResults\*\coverage.cobertura.xml" -targetdir:"CoverageReport-Total" -reporttypes:Html -classfilters:"-StargateAPI.Migrations.*;-StargateAPI.Program"

# Open reports
start CoverageReport-UnitTests\index.html
start CoverageReport-Total\index.html

Write-Host "Coverage reports generated and opened!" -ForegroundColor Green