#!/usr/bin/env pwsh
try {
    Push-Location $PSScriptRoot
    dotnet run --project "./tools/build" -- $args
    if ($LASTEXITCODE) { Throw "Build failed with exit code $LASTEXITCODE." }
}
finally {
    Pop-Location
}