$ErrorActionPreference = "Stop"

$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"
$envFile = Join-Path $PSScriptRoot ".env"

if (-not (Test-Path $composeFile)) { throw "Compose file not found: $composeFile" }
if (-not (Test-Path $envFile)) { throw "Env file not found: $envFile" }

Write-Host "Building and starting CityGuide API services (profile: apps)..."
Write-Host "Infrastructure is not started by this script. Run first: .\infrastructure\start-infra.ps1"
Write-Host ""

docker compose --env-file $envFile -f $composeFile --profile apps up -d --build --no-deps

Write-Host ""
Write-Host "Running API containers:"
docker compose --env-file $envFile -f $composeFile --profile apps ps
