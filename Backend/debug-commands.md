# 🚀 COMANDOS DE DEBUGGING PARA TU API

## 1. Ejecutar con logs detallados:
dotnet run --verbosity detailed

## 2. Ejecutar con configuración específica:
dotnet run --configuration Debug --environment Development

## 3. Ver logs en tiempo real:
dotnet run | tee api-logs.txt

## 4. Ejecutar con profiling:
dotnet run --configuration Release

## 5. Debugging con attach:
dotnet run --no-launch-profile

## 6. Ver información de la aplicación:
dotnet --info

## 7. Limpiar y rebuilder:
dotnet clean && dotnet build --verbosity normal

## 8. Ejecutar tests con logs:
dotnet test --verbosity normal --logger console

## 9. Verificar configuración:
dotnet run --dry-run 2>/dev/null || echo 'Comando no soportado en esta versión'

## 10. Monitorear performance:
dotnet-counters monitor --process-id $(pgrep -f 'RealEstateAPI')
