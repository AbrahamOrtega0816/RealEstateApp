#!/bin/bash

# Script to start frontend only
# Usage: ./start-frontend.sh

set -e

echo "🟢 Starting Frontend Next.js..."
echo "======================================================="

# Verify Bun is installed
if ! command -v bun &> /dev/null; then
    echo "❌ Error: Bun is not installed. Please install it from https://bun.sh/"
    exit 1
fi

# Install dependencies if needed
if [ ! -d "frontend/node_modules" ]; then
    echo "📦 Installing frontend dependencies..."
    cd frontend
    bun install
    cd ..
fi

echo "🎯 Available services:"
echo "  - Frontend: http://localhost:3000"
echo ""
echo "⚠️  Note: Make sure the backend is running on http://localhost:5000"
echo "Press Ctrl+C to stop the service"
echo "======================================================="

# Change to frontend directory and run
cd frontend
bun run dev
