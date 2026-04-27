# CityGuide

## Docker (локально)

Сборка и запуск всех сервисов через compose:

```bash
docker compose -f infrastructure/docker-compose.yml up --build
```

По умолчанию наружу проброшен только API Gateway: `http://localhost:8080`.

Если нужно дергать сервисы напрямую с ПК (не через gateway), они также опубликованы наружу на портах:

- `AuthService`: `http://localhost:8081`
- `UserService`: `http://localhost:8082`
- `ContentService`: `http://localhost:8083`
- `NotificationService`: `http://localhost:8084`

Внутри системы (контейнер → контейнер) обращаться нужно по DNS-именам сервисов и порту `8080`, например из `apigateway` в `userservice`: `http://userservice:8080`.

## Инфраструктура (Docker Desktop)

В compose также поднимаются:

- **Postgres**:
  - с ПК: `localhost:5432`
  - внутри Docker: `postgres:5432`
  - креды по умолчанию: user/password = `cityguide` / `cityguide` (см. `infrastructure/.env`)
  - базы создаются автоматически при первом старте: `user_db`, `content_db`, `notification_db`, `admin_db`
- **Redis**:
  - с ПК: `localhost:6379`
  - внутри Docker: `redis:6379`
- **Redis Commander (UI)**:
  - с ПК: `http://localhost:8085`
- **Kafka**:
  - с ПК: `localhost:9092`
  - внутри Docker: `kafka:9092`
- **Zookeeper** (Kafka dependency):
  - с ПК: `localhost:2181`
  - внутри Docker: `zookeeper:2181`
- **MinIO (S3)**:
  - S3 endpoint с ПК: `http://localhost:9000`
  - Console с ПК: `http://localhost:9001`
  - внутри Docker: `http://minio:9000`
  - root user/pass по умолчанию: `minioadmin` / `minioadmin` (см. `infrastructure/.env`)

### Быстрый “деплой” в Docker Desktop (Windows / PowerShell)

```powershell
.\infrastructure\deploy.ps1
```

Остановка:

```powershell
.\infrastructure\stop.ps1
```



