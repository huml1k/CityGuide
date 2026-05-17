$ErrorActionPreference = "Stop"

$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"
$envFile = Join-Path $PSScriptRoot ".env"

if (-not (Test-Path $composeFile)) { throw "Compose file not found: $composeFile" }
if (-not (Test-Path $envFile)) { throw "Env file not found: $envFile" }

Write-Host "Starting infrastructure only (Postgres, Redis, Kafka, MinIO)..."
Write-Host "API services are NOT started — use 'dotnet run' locally with appsettings.Development.json"
Write-Host ""

docker compose --env-file $envFile -f $composeFile up -d

Write-Host ""
Write-Host "Postgres on localhost (see infrastructure/.env):"
Write-Host "  admin_db         -> localhost:6000"
Write-Host "  user_db          -> localhost:6001"
Write-Host "  content_db       -> localhost:6002"
Write-Host "  notification_db  -> localhost:6003"
Write-Host "Redis -> localhost:6379 | Kafka -> localhost:9092 | MinIO -> localhost:9000"
Write-Host ""
docker compose --env-file $envFile -f $composeFile ps
