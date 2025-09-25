#!/bin/bash

# Script to start both projects in development mode
# Usage: ./start-dev.sh

set -e

echo "🚀 Starting Real Estate App in development mode..."
echo "======================================================="

# Verify Bun is installed
if ! command -v bun &> /dev/null; then
    echo "❌ Error: Bun is not installed. Please install it from https://bun.sh/"
    exit 1
fi

# Verify .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET is not installed. Please install it from https://dotnet.microsoft.com/"
    exit 1
fi

# Install frontend dependencies if needed
if [ ! -d "frontend/node_modules" ]; then
    echo "📦 Installing frontend dependencies..."
    cd frontend
    bun install
    cd ..
fi

# Install concurrently if not available
if ! bun pm ls -g | grep -q concurrently; then
    echo "📦 Installing concurrently globally..."
    bun add -g concurrently
fi

echo "🔧 Setting up environment variables..."

# Check .env file in backend
if [ ! -f "backend/.env" ]; then
    echo "⚠️  .env file not found in backend. Copying from example..."
    if [ -f "backend/env.example" ]; then
        cp backend/env.example backend/.env
        echo "📝 Please edit backend/.env with your real configurations"
    fi
fi

echo "🎯 Starting services..."
echo "  - Backend API: http://localhost:5260 (HTTP) / https://localhost:7190 (HTTPS)"
echo "  - Frontend: http://localhost:3000"
echo "  - Swagger UI: http://localhost:5260"
echo ""
echo "Press Ctrl+C to stop both services"
echo "======================================================="

# Run both projects using concurrently
bun run --bun concurrently \
    --prefix "{name}" \
    --names "BACKEND,FRONTEND" \
    --prefix-colors "blue,green" \
    "cd backend && dotnet run" \
    "cd frontend && bun run dev"
