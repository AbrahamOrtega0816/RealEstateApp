# 📚 Documentación - Real Estate API

Bienvenido a la documentación completa del proyecto Real Estate API. Aquí encontrarás todas las guías y recursos necesarios para configurar, desarrollar y mantener la aplicación.

## 📋 **ÍNDICE DE DOCUMENTACIÓN**

### 🔐 **Autenticación y Seguridad**

- **[AUTHENTICATION_SETUP_GUIDE.md](./AUTHENTICATION_SETUP_GUIDE.md)**
  - Guía completa de configuración de autenticación JWT
  - Configuración de servicios de usuario
  - Pruebas de endpoints de autenticación
  - Swagger con autenticación
  - Características de seguridad implementadas

### 🌐 **Base de Datos**

- **[MONGODB_ATLAS_SETUP.md](./MONGODB_ATLAS_SETUP.md)**
  - Configuración de MongoDB Atlas
  - Manejo seguro de credenciales
  - Variables de entorno
  - Configuración para diferentes entornos
  - Solución de problemas comunes
  - Monitoreo y mejores prácticas

### 🏗️ **Arquitectura del Proyecto**

- **[REORGANIZATION_GUIDE.md](./REORGANIZATION_GUIDE.md)**
  - Estructura de carpetas por features
  - Guía de reorganización del código
  - Namespaces y patrones de arquitectura
  - Beneficios de la estructura modular

---

## 🚀 **GUÍA DE INICIO RÁPIDO**

### **1. Configurar MongoDB**

```bash
# Configurar variable de entorno
export MONGODB_PASSWORD="tu_password_de_mongodb_atlas"

# O usar el script automático
./setup-mongodb.sh
```

### **2. Ejecutar la API**

```bash
cd Backend/RealEstateAPI
dotnet run
```

### **3. Probar en Swagger**

- Ve a `https://localhost:7190`
- Registra un usuario
- Autorízate con el token
- Prueba los endpoints

---

## 📖 **ORDEN DE LECTURA RECOMENDADO**

Para nuevos desarrolladores en el proyecto:

1. **[REORGANIZATION_GUIDE.md](./REORGANIZATION_GUIDE.md)** - Entender la arquitectura
2. **[MONGODB_ATLAS_SETUP.md](./MONGODB_ATLAS_SETUP.md)** - Configurar la base de datos
3. **[AUTHENTICATION_SETUP_GUIDE.md](./AUTHENTICATION_SETUP_GUIDE.md)** - Configurar autenticación

---

## 🛠️ **RECURSOS ADICIONALES**

### **Archivos de Configuración**

- `env.example` - Plantilla de variables de entorno
- `setup-mongodb.sh` - Script de configuración automática
- `Properties/launchSettings.json` - Configuración de desarrollo

### **Endpoints Principales**

- **Autenticación:** `/api/Auth/*`
- **Propiedades:** `/api/Properties/*`
- **Propietarios:** `/api/Owners/*`
- **Imágenes:** `/api/PropertyImages/*`
- **Trazas:** `/api/PropertyTraces/*`

### **Tecnologías Utilizadas**

- **.NET 9.0** - Framework principal
- **MongoDB Atlas** - Base de datos NoSQL
- **JWT Bearer** - Autenticación
- **BCrypt** - Hashing de contraseñas
- **Swagger/OpenAPI** - Documentación de API

---

## 🔧 **COMANDOS ÚTILES**

```bash
# Compilar el proyecto
dotnet build

# Ejecutar tests
dotnet test

# Restaurar paquetes
dotnet restore

# Ejecutar con perfil específico
dotnet run --launch-profile https

# Ver logs en tiempo real
dotnet run --verbosity normal
```

---

## 📞 **SOPORTE**

Si tienes preguntas o encuentras problemas:

1. **Revisa la documentación** específica del tema
2. **Consulta la sección de troubleshooting** en cada guía
3. **Verifica los logs** de la aplicación
4. **Comprueba la configuración** de variables de entorno

---

## 🎯 **ESTADO DEL PROYECTO**

### ✅ **Completado**

- Sistema de autenticación JWT completo
- Integración con MongoDB Atlas
- Estructura modular por features
- Documentación completa
- Configuración de desarrollo y producción

### 🔄 **En Progreso**

- Tests unitarios
- Frontend (React/Angular)
- Deployment automatizado

### 📋 **Próximos Pasos**

- Verificación de email
- Recuperación de contraseña
- Roles y permisos avanzados
- Rate limiting
- Logging avanzado

---

**¡Bienvenido al proyecto Real Estate API!** 🏠✨
