#!/bin/bash

# Check if reportgenerator is installed
if ! command -v reportgenerator &> /dev/null
then
    echo "reportgenerator not found. Installing..."
    dotnet tool install -g dotnet-reportgenerator-globaltool
fi

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --settings:.runsettings

# Generate Unit Tests coverage report (excludes Controllers)
reportgenerator -reports:"**/TestResults/*/coverage.cobertura.xml" -targetdir:"CoverageReport-UnitTests" -reporttypes:Html -classfilters:"-StargateAPI.Migrations.*;-StargateAPI.Program;-StargateAPI.Controllers.*"

# Generate Total coverage report (includes Controllers)
reportgenerator -reports:"**/TestResults/*/coverage.cobertura.xml" -targetdir:"CoverageReport-Total" -reporttypes:Html -classfilters:"-StargateAPI.Migrations.*;-StargateAPI.Program"

# Open reports in default browser
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS
    open CoverageReport-UnitTests/index.html
    open CoverageReport-Total/index.html
else
    # Linux
    xdg-open CoverageReport-UnitTests/index.html 2>/dev/null || echo "Please open CoverageReport-UnitTests/index.html manually"
    xdg-open CoverageReport-Total/index.html 2>/dev/null || echo "Please open CoverageReport-Total/index.html manually"
fi

echo "Coverage reports generated and opened!"