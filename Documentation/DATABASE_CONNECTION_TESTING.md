# Guía para Probar la Conexión a la Base de Datos

Esta guía te ayudará a verificar que la conexión a MongoDB esté funcionando correctamente en tu aplicación Real Estate API.

## 🚀 Verificación Automática al Startup

**¡NUEVA FUNCIONALIDAD!** La aplicación ahora verifica automáticamente la conexión a MongoDB cada vez que se inicia. Ya no necesitas hacer pruebas manuales para verificar que la conexión funcione.

### ¿Qué sucede al iniciar la aplicación?

Cuando ejecutes `dotnet run`, verás logs como estos:

```
🔄 Iniciando verificación de conexión a MongoDB...
🔍 Probando conexión a MongoDB...
✅ Conexión a MongoDB exitosa!
📊 Base de datos: RealEstateDB_Dev
🖥️  Servidor: cluster0-shard-00-01.oabkbuh.mongodb.net
📦 Versión MongoDB: 6.0.12
🔍 Verificando configuración de colecciones...
📁 Colección Properties: Properties
📁 Colección Owners: Owners
📁 Colección PropertyImages: PropertyImages
📁 Colección PropertyTraces: PropertyTraces
📁 Colección Users: Users
🚀 Sistema de base de datos listo para usar!
```

### Si hay problemas de conexión:

```
❌ Error de MongoDB durante startup: Authentication failed
💡 Verifica:
   - Que la variable MONGODB_PASSWORD esté configurada
   - Que el cluster de MongoDB Atlas esté activo
   - Que tu IP esté en la whitelist de MongoDB Atlas
   - Que las credenciales sean correctas
⚠️  Continuando en modo desarrollo sin MongoDB
   Algunos endpoints pueden no funcionar correctamente
```

### Comportamiento por Ambiente:

- **Desarrollo**: Si falla la conexión, la app continúa ejecutándose con advertencias
- **Producción**: Si falla la conexión, la app no se inicia (fail-fast)

## 📋 Requisitos Previos

1. **Variable de entorno configurada**: Asegúrate de que `MONGODB_PASSWORD` esté configurada
2. **API ejecutándose**: La aplicación debe estar corriendo (puerto 5000 por defecto)
3. **MongoDB Atlas accesible**: Tu cluster debe estar activo y accesible

## 🔧 Configuración Rápida

### Configurar la variable de entorno:

```bash
# En tu terminal (macOS/Linux):
export MONGODB_PASSWORD="tu_password_aqui"

# O en Windows (PowerShell):
$env:MONGODB_PASSWORD="tu_password_aqui"

# O en Windows (CMD):
set MONGODB_PASSWORD=tu_password_aqui
```

### Ejecutar la aplicación:

```bash
cd /Users/abrahamortega/Desktop/TECHNICAL\ TEST/RealEstateApp/Backend
dotnet run
```

## 🧪 Endpoints de Diagnóstico Adicionales (Opcionales)

Aunque la aplicación ya verifica automáticamente la conexión al iniciar, también tienes endpoints adicionales para diagnósticos más detallados:

### 1. Health Check General

**Endpoint**: `GET /api/diagnostic/health`
**Propósito**: Verificar que la API esté funcionando

```bash
curl -X GET "https://localhost:5001/api/diagnostic/health" \
  -H "accept: application/json"
```

**Respuesta esperada**:

```json
{
  "isSuccess": true,
  "data": {
    "status": "Healthy",
    "timestamp": "2024-01-20T10:30:00Z",
    "version": "1.0.0",
    "environment": "Development"
  },
  "message": "API funcionando correctamente"
}
```

### 2. Test de Conexión MongoDB

**Endpoint**: `GET /api/diagnostic/mongodb/connection`
**Propósito**: Verificar la conexión básica a MongoDB

```bash
curl -X GET "https://localhost:5001/api/diagnostic/mongodb/connection" \
  -H "accept: application/json"
```

**Respuesta exitosa**:

```json
{
  "isSuccess": true,
  "data": {
    "status": "Connected",
    "databaseName": "RealEstateDB_Dev",
    "serverTime": "2024-01-20T10:30:00Z",
    "connectionString": "mongodb+srv://abraham:****@cluster0.oabkbuh.mongodb.net/..."
  },
  "message": "Conexión a MongoDB exitosa"
}
```

### 3. Verificación de Colecciones

**Endpoint**: `GET /api/diagnostic/mongodb/collections`
**Propósito**: Verificar que las colecciones estén disponibles

```bash
curl -X GET "https://localhost:5001/api/diagnostic/mongodb/collections" \
  -H "accept: application/json"
```

**Respuesta esperada**:

```json
{
  "isSuccess": true,
  "data": {
    "databaseName": "RealEstateDB_Dev",
    "totalExistingCollections": 5,
    "existingCollections": ["Properties", "Owners", "Users", ...],
    "expectedCollections": [
      {
        "name": "Properties",
        "collectionName": "Properties",
        "exists": true,
        "documentCount": 10,
        "status": "OK"
      }
    ]
  },
  "message": "Verificación de colecciones completada"
}
```

### 4. Test Completo del Sistema

**Endpoint**: `GET /api/diagnostic/mongodb/full-test`
**Propósito**: Ejecutar todas las pruebas de conectividad

```bash
curl -X GET "https://localhost:5001/api/diagnostic/mongodb/full-test" \
  -H "accept: application/json"
```

**Respuesta exitosa**:

```json
{
  "isSuccess": true,
  "data": {
    "connectionTest": { "status": "OK", "message": "Ping exitoso" },
    "serverInfo": {
      "version": "6.0.12",
      "uptime": 1234567,
      "host": "cluster0-shard-00-01.oabkbuh.mongodb.net"
    },
    "databaseInfo": {
      "name": "RealEstateDB_Dev",
      "collections": 5,
      "dataSize": 12345,
      "storageSize": 54321
    },
    "writeReadTest": {
      "status": "OK",
      "message": "Escritura y lectura exitosas"
    }
  },
  "message": "Todas las pruebas de MongoDB completadas exitosamente"
}
```

### 5. Información de Configuración

**Endpoint**: `GET /api/diagnostic/configuration`
**Propósito**: Ver la configuración actual (sin datos sensibles)

```bash
curl -X GET "https://localhost:5001/api/diagnostic/configuration" \
  -H "accept: application/json"
```

## 🌐 Usando Swagger UI

1. **Abre tu navegador** y ve a: `https://localhost:5001` (o `http://localhost:5000`)
2. **Busca la sección "Diagnósticos"** en la interfaz de Swagger
3. **Haz clic en cada endpoint** para probarlo directamente desde la interfaz
4. **Presiona "Try it out"** y luego "Execute" para cada prueba

## 🚨 Solución de Problemas Comunes

### Error: "MongoDB password not found"

```bash
# Solución: Configurar la variable de entorno
export MONGODB_PASSWORD="tu_password_real"
```

### Error: "Connection timeout"

- Verifica que tu IP esté en la whitelist de MongoDB Atlas
- Comprueba que el cluster esté activo
- Revisa la configuración de red/firewall

### Error: "Authentication failed"

- Verifica que el usuario y password sean correctos
- Comprueba que el usuario tenga permisos en la base de datos

### Error: "Database not found"

- La base de datos se creará automáticamente al insertar el primer documento
- Verifica que el nombre de la base de datos en la configuración sea correcto

## 📊 Interpretando los Resultados

### ✅ Conexión Exitosa

- `status: "Connected"` en el test de conexión
- `writeReadTest.status: "OK"` en el test completo
- Números positivos en `documentCount` para colecciones existentes

### ❌ Problemas de Conexión

- `isSuccess: false` en cualquier respuesta
- Mensajes de error en el campo `message`
- Excepciones en los logs de la aplicación

## 🔍 Logs Adicionales

Para ver más detalles, revisa los logs de la aplicación mientras ejecutas las pruebas:

```bash
# Los logs aparecerán en la consola donde ejecutaste 'dotnet run'
# Busca líneas como:
# 🔵 REQUEST: GET /api/diagnostic/mongodb/connection
# 🟢 RESPONSE: 200 for GET /api/diagnostic/mongodb/connection
```

## 🎯 Próximos Pasos

Una vez que confirmes que la conexión funciona:

1. **Ejecuta las migraciones** si las tienes
2. **Crea datos de prueba** usando los otros endpoints de la API
3. **Configura el frontend** para conectarse a la API
4. **Implementa monitoreo** para producción

## 📞 Soporte

Si encuentras problemas:

1. Revisa los logs detallados en la consola
2. Verifica la configuración en `appsettings.Development.json`
3. Comprueba la conectividad de red a MongoDB Atlas
4. Consulta la documentación de MongoDB Atlas para problemas específicos de la plataforma
