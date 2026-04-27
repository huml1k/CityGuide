#!/usr/bin/env bash
set -euo pipefail

dbs=(
  "user_db"
  "content_db"
  "notification_db"
  "admin_db"
)

echo "Creating additional databases (if not exist): ${dbs[*]}"

for db in "${dbs[@]}"; do
  psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "postgres" <<-EOSQL
    SELECT 'CREATE DATABASE ${db}' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '${db}')\\gexec
EOSQL
done

