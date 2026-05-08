# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ARG APP_UID=10001
RUN if id -u app >/dev/null 2>&1; then echo "user app already exists"; else useradd --create-home --uid $APP_UID app; fi
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
USER app


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AdminService/AdminService.Api/AdminService.Api.csproj", "src/AdminService/AdminService.Api/"]
COPY ["src/AdminService/AdminService.Application/AdminService.Application.csproj", "src/AdminService/AdminService.Application/"]
COPY ["src/AdminService/AdminService.Domain/AdminService.Domain.csproj", "src/AdminService/AdminService.Domain/"]
COPY ["src/AdminService/AdminService.Infrastructure/AdminService.Infrastructure.csproj", "src/AdminService/AdminService.Infrastructure/"]
RUN dotnet restore "./src/AdminService/AdminService.Api/AdminService.Api.csproj"
COPY . .
WORKDIR "/src/src/AdminService/AdminService.Api"
RUN dotnet build "./AdminService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdminService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminService.Api.dll"]