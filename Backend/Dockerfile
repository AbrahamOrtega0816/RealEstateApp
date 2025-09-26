# Usar la imagen base de .NET 9.0 SDK para compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código fuente
COPY . ./

# Compilar la aplicación
RUN dotnet publish -c Release -o out

# Usar la imagen de runtime para la aplicación final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Instalar curl para health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copiar la aplicación compilada
COPY --from=build /app/out .

# Crear directorio para uploads
RUN mkdir -p /app/wwwroot/uploads/properties

# Exponer puertos
EXPOSE 8080

# Configurar variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/api/diagnostic/health || exit 1

# Punto de entrada
ENTRYPOINT ["dotnet", "RealEstateAPI.dll"]
