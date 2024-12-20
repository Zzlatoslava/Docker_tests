# 1. Используем легковесный .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src

# 2. Копируем файлы проекта и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# 3. Копируем исходники и собираем проект
COPY . ./
RUN dotnet publish -c Release -o /app

# 4. Используем легковесный ASP.NET Runtime для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app

# 5. Добавляем непривилегированного пользователя
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

# 6. Копируем собранное приложение
COPY --from=build /app ./

# 7. Очищаем кэш для уменьшения размера образа
RUN rm -rf /tmp/* /var/cache/apk/*

# 8. Устанавливаем переменные окружения для конфигурации
ENV DOTNET_ENVIRONMENT=Production
ENV Kafka__BootstrapServers=kafka:9092
ENV PostgreSQL__ConnectionString="Host=postgres;Port=5432;Database=route256;Username=postgres;Password=pwd"

# 9. Выполняем миграции перед запуском приложения
CMD ["sh", "-c", "dotnet KafkaHomework.dll --migrate && dotnet KafkaHomework.dll"]
