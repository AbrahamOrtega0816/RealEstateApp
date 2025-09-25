# 🌐 Configuración de MongoDB Atlas - Real Estate API

## ✅ **CONFIGURACIÓN COMPLETADA**

Tu API está configurada para usar **MongoDB Atlas** con manejo seguro de credenciales.

### **🔗 Configuración Actual**

- **Cluster:** `cluster0.oabkbuh.mongodb.net`
- **Usuario:** `abraham`
- **Cadena de conexión:** `mongodb+srv://abraham:{MONGODB_PASSWORD}@cluster0.oabkbuh.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0`
- **Base de datos producción:** `RealEstateDB`
- **Base de datos desarrollo:** `RealEstateDB_Dev`

---

## 🔐 **CONFIGURAR LA CONTRASEÑA DE MONGODB**

### **Opción 1: Variable de Entorno (🎯 Recomendado)**

```bash
# En macOS/Linux/Git Bash
export MONGODB_PASSWORD="tu_password_real_de_mongodb_atlas"

# En Windows Command Prompt
set MONGODB_PASSWORD=tu_password_real_de_mongodb_atlas

# En Windows PowerShell
$env:MONGODB_PASSWORD="tu_password_real_de_mongodb_atlas"
```

### **Opción 2: Archivo .env (Para desarrollo)**

1. **Crear archivo .env:**
   ```bash
   cp env.example .env
   ```

2. **Editar .env:**
   ```bash
   # MongoDB Atlas Configuration
   MONGODB_PASSWORD=tu_password_real_de_mongodb_atlas
   ```

3. **Asegurar que esté en .gitignore:**
   ```bash
   echo ".env" >> .gitignore
   ```

### **Opción 3: launchSettings.json (Para IDE)**

1. **Editar `launchSettings.Development.json`:**
   ```json
   {
     "profiles": {
       "RealEstateAPI-Development": {
         "environmentVariables": {
           "MONGODB_PASSWORD": "tu_password_real_aqui"
         }
       }
     }
   }
   ```

2. **⚠️ IMPORTANTE:** NO subir este archivo con la contraseña real a git.

### **Opción 4: Script Automático**

```bash
# Ejecutar el script de configuración
./setup-mongodb.sh
```

Este script te pedirá la contraseña y configurará automáticamente:
- Variable de entorno
- Archivo .env
- launchSettings.Development.json

---

## 🚀 **EJECUTAR LA API**

### **Paso 1: Configurar la contraseña**
Usa cualquiera de las opciones anteriores.

### **Paso 2: Ejecutar la aplicación**
```bash
dotnet run
```

### **Paso 3: Verificar la conexión**
La API se ejecutará en `https://localhost:7000` y verás en los logs:
```
info: RealEstateAPI.Features.Authentication.Services.UserService[0]
      MongoDB indexes for Users created successfully
```

---

## 🧪 **PROBAR LA CONFIGURACIÓN**

### **1. Verificar que la API inicie sin errores**
```bash
dotnet run
```

### **2. Probar registro de usuario en Swagger**
1. Ve a `https://localhost:7000`
2. Usa el endpoint `POST /api/Auth/register`
3. Si funciona, la conexión a MongoDB Atlas está correcta

### **3. Verificar en MongoDB Atlas**
1. Ve a tu cluster en MongoDB Atlas
2. Navega a "Collections"
3. Deberías ver la base de datos `RealEstateDB_Dev` (o `RealEstateDB` en producción)
4. Con las colecciones: `Users`, `Properties`, etc.

---

## 🔒 **SEGURIDAD Y MEJORES PRÁCTICAS**

### **✅ Qué SÍ hacer:**
- ✅ Usar variables de entorno en producción
- ✅ Mantener `.env` en `.gitignore`
- ✅ Usar diferentes bases de datos para desarrollo y producción
- ✅ Rotar contraseñas periódicamente
- ✅ Usar MongoDB Atlas con IP whitelisting

### **❌ Qué NO hacer:**
- ❌ Hardcodear contraseñas en el código
- ❌ Subir archivos `.env` a git
- ❌ Usar la misma contraseña para desarrollo y producción
- ❌ Compartir credenciales por medios inseguros

---

## 🌍 **CONFIGURACIÓN PARA DIFERENTES ENTORNOS**

### **Desarrollo Local**
```bash
export MONGODB_PASSWORD="dev_password"
export ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```
- Usa base de datos: `RealEstateDB_Dev`
- Tokens JWT válidos por 120 minutos

### **Producción**
```bash
export MONGODB_PASSWORD="prod_password_super_seguro"
export ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```
- Usa base de datos: `RealEstateDB`
- Tokens JWT válidos por 60 minutos

---

## 🆘 **SOLUCIÓN DE PROBLEMAS**

### **Error: "MongoDB password not found"**
**Causa:** La variable de entorno `MONGODB_PASSWORD` no está configurada.
**Solución:**
```bash
export MONGODB_PASSWORD="tu_password_real"
```

### **Error: "MongoAuthenticationException"**
**Causa:** Contraseña incorrecta o usuario no autorizado.
**Solución:**
1. Verifica que la contraseña sea correcta
2. Verifica que el usuario `abraham` tenga permisos
3. Verifica la configuración de IP whitelist en MongoDB Atlas

### **Error: "MongoServerSelectionTimeoutException"**
**Causa:** No puede conectar al cluster.
**Solución:**
1. Verifica tu conexión a internet
2. Verifica que tu IP esté en la whitelist de MongoDB Atlas
3. Verifica que el cluster esté activo

### **Error: "Connection string format"**
**Causa:** Error en el formato de la cadena de conexión.
**Solución:** Verifica que la cadena sea exactamente:
```
mongodb+srv://abraham:{MONGODB_PASSWORD}@cluster0.oabkbuh.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0
```

---

## 📊 **Monitoreo de MongoDB Atlas**

### **Métricas importantes a revigar:**
- **Conexiones activas**
- **Operaciones por segundo**
- **Uso de almacenamiento**
- **Performance de queries**

### **Configurar alertas:**
1. Ve a "Alerts" en MongoDB Atlas
2. Configura alertas para:
   - Conexiones máximas
   - CPU alto
   - Memoria alta
   - Errores de conexión

---

## 🎯 **RESUMEN**

✅ **MongoDB Atlas configurado** con cadena de conexión segura
✅ **Variables de entorno** implementadas para manejo seguro de contraseñas
✅ **Configuraciones separadas** para desarrollo y producción
✅ **Scripts de configuración** automatizada
✅ **Documentación completa** de seguridad y troubleshooting

**¡Tu API está lista para usar MongoDB Atlas de forma segura!** 🚀
