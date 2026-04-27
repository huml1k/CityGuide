$ErrorActionPreference = "Stop"

$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"
$envFile = Join-Path $PSScriptRoot ".env"

Write-Host "Stopping CityGuide containers..."
docker compose --env-file $envFile -f $composeFile down

