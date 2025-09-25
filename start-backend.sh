#!/bin/bash

# Script to start backend only
# Usage: ./start-backend.sh

set -e

echo "🔵 Starting Backend API..."
echo "======================================================="

# Verify .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET is not installed. Please install it from https://dotnet.microsoft.com/"
    exit 1
fi

# Check .env file
if [ ! -f "backend/.env" ]; then
    echo "⚠️  .env file not found in backend. Copying from example..."
    if [ -f "backend/env.example" ]; then
        cp backend/env.example backend/.env
        echo "📝 Please edit backend/.env with your real configurations"
    fi
fi

echo "🎯 Available services:"
echo "  - API: http://localhost:5260 (HTTP) / https://localhost:7190 (HTTPS)"
echo "  - Swagger UI: http://localhost:5260"
echo ""
echo "Press Ctrl+C to stop the service"
echo "======================================================="

# Change to backend directory and run
cd backend
dotnet run
