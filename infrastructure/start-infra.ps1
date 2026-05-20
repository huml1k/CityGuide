$ErrorActionPreference = "Stop"

$composeFile = Join-Path $PSScriptRoot "docker-compose.yml"
$envFile = Join-Path $PSScriptRoot ".env"

if (-not (Test-Path $composeFile)) { throw "Compose file not found: $composeFile" }
if (-not (Test-Path $envFile)) { throw "Env file not found: $envFile" }

Write-Host "Starting infrastructure (Postgres, Redis, Kafka, MinIO, pgAdmin)..."
Write-Host "API services are NOT started. Use: .\infrastructure\deploy.ps1"
Write-Host ""

docker compose --env-file $envFile -f $composeFile --profile infra up -d

$dbServices = @("user-db", "admin-db", "content-db", "notification-db")
$timeoutSec = 120
$intervalSec = 3
$elapsed = 0

Write-Host ""
Write-Host "Waiting for PostgreSQL databases to become healthy (up to $timeoutSec sec)..."

while ($elapsed -lt $timeoutSec) {
    $pending = @()

    foreach ($service in $dbServices) {
        $prevEap = $ErrorActionPreference
        $ErrorActionPreference = "SilentlyContinue"
        $health = docker compose --env-file $envFile -f $composeFile ps $service --format "{{.Health}}" 2>$null
        $ErrorActionPreference = $prevEap

        if ($health -ne "healthy") {
            $pending += $service
        }
    }

    if ($pending.Count -eq 0) {
        Write-Host "All databases are healthy."
        break
    }

    Start-Sleep -Seconds $intervalSec
    $elapsed += $intervalSec
}

if ($elapsed -ge $timeoutSec) {
    Write-Host "Warning: timeout waiting for databases. Still not healthy:" -ForegroundColor Yellow
    $pending | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    Write-Host "Check logs: docker compose -f infrastructure/docker-compose.yml --profile infra logs" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Postgres on localhost (see infrastructure/.env):"
Write-Host "  admin_db         -> localhost:6000"
Write-Host "  user_db          -> localhost:6001"
Write-Host "  content_db       -> localhost:6002"
Write-Host "  notification_db  -> localhost:6003"
Write-Host "Redis -> localhost:6379 | Kafka -> localhost:9092 | MinIO -> localhost:9000"
Write-Host ""
Write-Host "Running infrastructure containers:"
docker compose --env-file $envFile -f $composeFile --profile infra ps
