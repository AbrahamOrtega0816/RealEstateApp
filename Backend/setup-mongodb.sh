#!/bin/bash

echo "🔐 Configuración de MongoDB Atlas para Real Estate API"
echo "=================================================="

# Solicitar la contraseña de MongoDB
echo ""
read -p "Ingresa tu contraseña de MongoDB Atlas: " -s MONGODB_PASS
echo ""

# Opción 1: Configurar variable de entorno
echo ""
echo "Configurando variable de entorno..."
export MONGODB_PASSWORD="$MONGODB_PASS"

# Opción 2: Crear archivo .env
echo "Creando archivo .env..."
cat > .env << EOF
# MongoDB Atlas Configuration
MONGODB_PASSWORD=$MONGODB_PASS

# JWT Configuration (opcional)
JWT_SECRET_KEY=tu-clave-secreta-super-segura-de-al-menos-32-caracteres-para-jwt-tokens
EOF

# Opción 3: Actualizar launchSettings.Development.json
echo "Actualizando launchSettings.Development.json..."
sed -i.bak "s/TU_PASSWORD_MONGODB_ATLAS_AQUI/$MONGODB_PASS/g" launchSettings.Development.json

echo ""
echo "✅ Configuración completada!"
echo ""
echo "Opciones configuradas:"
echo "1. ✅ Variable de entorno MONGODB_PASSWORD"
echo "2. ✅ Archivo .env creado"
echo "3. ✅ launchSettings.Development.json actualizado"
echo ""
echo "⚠️  IMPORTANTE:"
echo "- Asegúrate de agregar .env al .gitignore"
echo "- NO subas launchSettings.Development.json con la contraseña real"
echo "- En producción usa variables de entorno del sistema"
echo ""
echo "🚀 Ahora puedes ejecutar: dotnet run"
echo ""
echo "📚 Para más información, consulta la documentación en:"
echo "   - ../../Documentation/MONGODB_ATLAS_SETUP.md"
echo "   - ../../Documentation/AUTHENTICATION_SETUP_GUIDE.md"
