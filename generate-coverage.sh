#!/bin/bash

echo "Running tests with coverage..."
dotnet test --collect:"XPlat Code Coverage" --settings:.runsettings

if [ $? -ne 0 ]; then
    echo "Tests failed! Aborting coverage generation."
    exit 1
fi

echo ""
echo "Generating unit test coverage report (excludes controllers)..."
reportgenerator \
  -reports:"**/TestResults/*/coverage.cobertura.xml" \
  -targetdir:"CoverageReport-UnitTests" \
  -reporttypes:Html \
  -classfilters:"-StargateAPI.Migrations.*;-StargateAPI.Program;-StargateAPI.Controllers.*"

echo ""
echo "Generating total coverage report (includes controllers)..."
reportgenerator \
  -reports:"**/TestResults/*/coverage.cobertura.xml" \
  -targetdir:"CoverageReport-Total" \
  -reporttypes:Html \
  -classfilters:"-StargateAPI.Migrations.*;-StargateAPI.Program"

echo ""
echo "Coverage reports generated!"
echo "Unit Tests: $(pwd)/CoverageReport-UnitTests/index.html"
echo "Total:      $(pwd)/CoverageReport-Total/index.html"
echo ""
echo "Opening reports..."
open CoverageReport-UnitTests/index.html
open CoverageReport-Total/index.html
