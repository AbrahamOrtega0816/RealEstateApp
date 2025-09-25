# Makefile for Real Estate App
# Simplified commands to manage the project

.PHONY: help dev dev-backend dev-frontend build build-backend build-frontend install clean test docker-up docker-down docker-build setup

# Configuración por defecto
BACKEND_DIR = backend
FRONTEND_DIR = frontend

# Help - show available commands
help:
	@echo "🏠 Real Estate App - Available Commands"
	@echo "======================================"
	@echo ""
	@echo "📋 Development Commands:"
	@echo "  make dev          - Run backend and frontend together"
	@echo "  make dev-backend  - Run backend only"
	@echo "  make dev-frontend - Run frontend only"
	@echo ""
	@echo "🔨 Build Commands:"
	@echo "  make build        - Build backend and frontend"
	@echo "  make build-backend- Build backend only"
	@echo "  make build-frontend- Build frontend only"
	@echo ""
	@echo "📦 Installation Commands:"
	@echo "  make install      - Install all dependencies"
	@echo "  make setup        - Complete initial setup"
	@echo ""
	@echo "🧹 Cleanup Commands:"
	@echo "  make clean        - Clean build files"
	@echo ""
	@echo "🧪 Testing Commands:"
	@echo "  make test         - Run all tests"
	@echo ""
	@echo "🐳 Docker Commands:"
	@echo "  make docker-up    - Start services with Docker"
	@echo "  make docker-down  - Stop Docker services"
	@echo "  make docker-build - Build Docker images"
	@echo ""

# Development commands
dev:
	@echo "🚀 Starting full development..."
	@./start-dev.sh

dev-backend:
	@echo "🔵 Starting backend only..."
	@./start-backend.sh

dev-frontend:
	@echo "🟢 Starting frontend only..."
	@./start-frontend.sh

# Build commands
build: build-backend build-frontend
	@echo "✅ Complete build finished"

build-backend:
	@echo "🔨 Building backend..."
	@cd $(BACKEND_DIR) && dotnet build --configuration Release

build-frontend:
	@echo "🔨 Building frontend..."
	@cd $(FRONTEND_DIR) && bun run build

# Dependencies installation
install: install-frontend
	@echo "✅ All dependencies installed"

install-frontend:
	@echo "📦 Installing frontend dependencies..."
	@cd $(FRONTEND_DIR) && bun install

# Initial setup
setup: install
	@echo "⚙️  Initial setup..."
	@if [ ! -f "$(BACKEND_DIR)/.env" ]; then \
		if [ -f "$(BACKEND_DIR)/env.example" ]; then \
			cp $(BACKEND_DIR)/env.example $(BACKEND_DIR)/.env; \
			echo "📝 .env file created from example. Please edit it with your configurations."; \
		fi \
	fi
	@echo "✅ Initial setup completed"

# Cleanup
clean: clean-backend clean-frontend
	@echo "✅ Complete cleanup finished"

clean-backend:
	@echo "🧹 Cleaning backend..."
	@cd $(BACKEND_DIR) && dotnet clean

clean-frontend:
	@echo "🧹 Cleaning frontend..."
	@cd $(FRONTEND_DIR) && rm -rf .next node_modules
	@cd $(FRONTEND_DIR) && bun install

# Testing
test:
	@echo "🧪 Running tests..."
	@cd $(BACKEND_DIR) && dotnet test

# Docker commands
docker-up:
	@echo "🐳 Starting services with Docker..."
	@docker-compose up -d

docker-down:
	@echo "🐳 Stopping Docker services..."
	@docker-compose down

docker-build:
	@echo "🐳 Building Docker images..."
	@docker-compose build

docker-logs:
	@echo "📋 Showing Docker logs..."
	@docker-compose logs -f

# Production commands
start: build
	@echo "🚀 Starting in production mode..."
	@bun run --bun concurrently \
		--prefix "{name}" \
		--names "BACKEND,FRONTEND" \
		--prefix-colors "blue,green" \
		"cd $(BACKEND_DIR) && dotnet run --configuration Release" \
		"cd $(FRONTEND_DIR) && bun run start"

# Check services status
status:
	@echo "📊 Services status:"
	@echo ""
	@echo "Backend (.NET):"
	@if pgrep -f "dotnet.*run" > /dev/null || curl -s -f http://localhost:5260/api/diagnostic/health > /dev/null 2>&1; then \
		echo "  ✅ Backend running on http://localhost:5260"; \
	else \
		echo "  ❌ Backend not running"; \
	fi
	@echo ""
	@echo "Frontend (Next.js with Bun):"
	@if pgrep -f "next.*dev" > /dev/null || curl -s -f http://localhost:3000 > /dev/null 2>&1; then \
		echo "  ✅ Frontend running on http://localhost:3000"; \
	else \
		echo "  ❌ Frontend not running"; \
	fi
	@echo ""
	@echo "🔍 Active processes:"
	@ps aux | grep -E "(dotnet.*run|next.*dev|bun.*dev)" | grep -v grep || echo "  No development processes found"
