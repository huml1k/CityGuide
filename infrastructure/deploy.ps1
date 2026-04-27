$ErrorActionPreference = "Stop"

$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"
$envFile = Join-Path $PSScriptRoot ".env"

if (-not (Test-Path $composeFile)) { throw "Compose file not found: $composeFile" }
if (-not (Test-Path $envFile)) { throw "Env file not found: $envFile" }

Write-Host "Building & starting CityGuide containers..."
docker compose --env-file $envFile -f $composeFile up -d --build

Write-Host ""
Write-Host "Running containers:"
docker compose --env-file $envFile -f $composeFile ps

