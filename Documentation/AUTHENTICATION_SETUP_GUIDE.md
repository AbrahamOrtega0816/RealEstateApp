# 🔐 Guía de Configuración de Autenticación - Real Estate API

## ✅ **CONFIGURACIÓN COMPLETADA**

Tu API ya tiene configurada la autenticación JWT con MongoDB. Aquí está todo lo que se ha implementado:

### **🔧 Componentes Implementados**

1. **✅ Configuración JWT** - `Program.cs`
2. **✅ Servicios de Usuario** - `UserService.cs` + `IUserService.cs`
3. **✅ Servicio JWT actualizado** - `JwtService.cs`
4. **✅ Middleware de autenticación**
5. **✅ Swagger con autenticación**
6. **✅ Controlador de autenticación** - `AuthController.cs`

---

## 🚀 **PASOS PARA USAR LA AUTENTICACIÓN**

### **PASO 1: Configurar MongoDB Atlas**

✅ **Ya tienes MongoDB Atlas configurado!** Tu API está configurada para usar:

- **Cluster:** `cluster0.oabkbuh.mongodb.net`
- **Usuario:** `abraham`
- **Base de datos:** `RealEstateDB` (producción) / `RealEstateDB_Dev` (desarrollo)

#### **🔐 Configurar la Contraseña de MongoDB**

Elige una de estas opciones para configurar tu contraseña de MongoDB Atlas:

**Opción A: Variable de Entorno (Recomendado)**

```bash
# En macOS/Linux
export MONGODB_PASSWORD="tu_password_real_aqui"

# En Windows
set MONGODB_PASSWORD=tu_password_real_aqui
```

**Opción B: Archivo .env (Para desarrollo)**

1. Copia el archivo `env.example` como `.env`
2. Edita `.env` y reemplaza `tu_password_de_mongodb_atlas_aqui` con tu contraseña real
3. Asegúrate de que `.env` esté en `.gitignore`

**Opción C: launchSettings.json (Para desarrollo en IDE)**

1. Edita `launchSettings.Development.json`
2. Reemplaza `TU_PASSWORD_MONGODB_ATLAS_AQUI` con tu contraseña real
3. **⚠️ NO subas este archivo a git con la contraseña real**

**Opción D: Configuración directa en appsettings.json**

```json
{
  "MONGODB_PASSWORD": "tu_password_real_aqui"
}
```

**⚠️ SOLO para desarrollo local - NO para producción**

### **PASO 2: Ejecutar la API**

```bash
cd RealEstateApp/Backend/RealEstateAPI
dotnet run
```

La API se ejecutará en: `https://localhost:7000` (HTTPS) o `http://localhost:5000` (HTTP)

---

## 🧪 **PRUEBAS DE AUTENTICACIÓN**

### **1. Registrar un Usuario**

**Endpoint:** `POST /api/Auth/register`

**Body:**

```json
{
  "email": "usuario@ejemplo.com",
  "password": "MiPassword123!",
  "firstName": "Juan",
  "lastName": "Pérez",
  "role": "User"
}
```

**Respuesta:**

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresAt": "2025-09-25T15:30:00Z",
  "user": {
    "id": "66f4a1234567890abcdef123",
    "email": "usuario@ejemplo.com",
    "firstName": "Juan",
    "lastName": "Pérez",
    "role": "User"
  }
}
```

### **2. Iniciar Sesión**

**Endpoint:** `POST /api/Auth/login`

**Body:**

```json
{
  "email": "usuario@ejemplo.com",
  "password": "MiPassword123!",
  "rememberMe": false
}
```

### **3. Obtener Perfil (Requiere Autenticación)**

**Endpoint:** `GET /api/Auth/profile`

**Headers:**

```
Authorization: Bearer YOUR_ACCESS_TOKEN_HERE
```

### **4. Renovar Token**

**Endpoint:** `POST /api/Auth/refresh`

**Body:**

```json
{
  "refreshToken": "your-refresh-token-here"
}
```

### **5. Cerrar Sesión (Revocar Token)**

**Endpoint:** `POST /api/Auth/revoke`

**Body:**

```json
{
  "refreshToken": "your-refresh-token-here"
}
```

---

## 🔍 **USAR SWAGGER PARA PROBAR**

1. **Ejecuta la API:** `dotnet run`
2. **Abre Swagger:** Ve a `https://localhost:7000` en tu navegador
3. **Registra un usuario:** Usa el endpoint `/api/Auth/register`
4. **Copia el token:** Del campo `accessToken` en la respuesta
5. **Autorízate en Swagger:**
   - Haz clic en el botón **"Authorize"** (🔒) en la parte superior
   - Ingresa: `Bearer TU_TOKEN_AQUI` (incluye la palabra "Bearer")
   - Haz clic en "Authorize"
6. **Prueba endpoints protegidos:** Como `/api/Auth/profile`

---

## 📝 **CONFIGURACIÓN DE MONGODB**

### **Colecciones que se crearán automáticamente:**

- `Users` - Usuarios del sistema
- `Properties` - Propiedades inmobiliarias
- `Owners` - Propietarios
- `PropertyImages` - Imágenes de propiedades
- `PropertyTraces` - Trazas de propiedades

### **Índices que se crean automáticamente:**

- `Users.email` (único)
- `Users.refreshToken`
- `Users.isActive + role`

---

## ⚙️ **CONFIGURACIÓN ACTUAL**

### **JWT Settings (`appsettings.json`)**

```json
{
  "JwtSettings": {
    "SecretKey": "tu-clave-secreta-super-segura-de-al-menos-32-caracteres-para-jwt-tokens",
    "Issuer": "RealEstateAPI",
    "Audience": "RealEstateAPI-Users",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

### **MongoDB Settings**

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RealEstateDB",
    "UsersCollectionName": "Users"
  }
}
```

---

## 🛡️ **CARACTERÍSTICAS DE SEGURIDAD IMPLEMENTADAS**

✅ **Autenticación JWT**
✅ **Refresh Tokens**
✅ **Hashing de contraseñas con BCrypt**
✅ **Validación de tokens**
✅ **Middleware de autorización**
✅ **Manejo de intentos fallidos de login**
✅ **Bloqueo de cuentas**
✅ **Índices únicos en email**
✅ **Swagger con autenticación**

---

## 🚨 **SOLUCIÓN DE PROBLEMAS**

### **Error: "MongoDB connection failed"**

- Verifica que MongoDB esté ejecutándose
- Comprueba la cadena de conexión en `appsettings.json`

### **Error: "JWT SecretKey is required"**

- Asegúrate de que `JwtSettings.SecretKey` esté configurado
- La clave debe tener al menos 32 caracteres

### **Error: "User with this email already exists"**

- El email ya está registrado
- Usa un email diferente o el endpoint de login

### **Error 401 en endpoints protegidos**

- Verifica que el token esté incluido en el header `Authorization`
- Formato correcto: `Bearer tu-token-aqui`
- Verifica que el token no haya expirado

---

## 📚 **PRÓXIMOS PASOS OPCIONALES**

1. **Verificación de email** - Implementar envío de emails
2. **Recuperación de contraseña** - Reset password flow
3. **Roles y permisos** - Autorización basada en roles
4. **OAuth2** - Login con Google/Facebook
5. **Rate limiting** - Limitar intentos de login
6. **Logging avanzado** - Auditoría de seguridad

---

## 🎯 **RESUMEN**

Tu API de Real Estate ya tiene un sistema de autenticación completo y funcional con:

- Registro e inicio de sesión
- Tokens JWT seguros
- Refresh tokens para renovación
- Integración con MongoDB
- Documentación en Swagger
- Middleware de seguridad

**¡Todo está listo para usar!** 🚀
