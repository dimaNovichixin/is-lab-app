# Этап 1: сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY IsLabApp.csproj .
RUN dotnet restore

# Копируем остальной код и собираем
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Этап 2: запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Копируем опубликованное приложение из этапа сборки
COPY --from=build /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "IsLabApp.dll"]
