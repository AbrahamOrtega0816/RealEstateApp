# 🏠 Real Estate App Execution Guide

This guide will show you all available ways to run the backend (.NET) and frontend (Next.js) projects of the real estate application.

## 📋 Prerequisites

### Required Software

- **Node.js** (version 18 or higher) - [Download here](https://nodejs.org/)
- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/)
- **MongoDB** (local or MongoDB Atlas) - [Setup instructions](./documentation/MONGODB_ATLAS_SETUP.md)
- **Docker** (optional, for containers) - [Download here](https://docker.com/)

### Initial Setup

1. Clone or download the project
2. Run initial setup:
   ```bash
   make setup
   ```
   Or manually:
   ```bash
   cd frontend && npm install
   cp backend/env.example backend/.env
   ```

## 🚀 Execution Methods

### 1. NPM Method (Recommended for development)

#### Run both projects together:

```bash
npm run dev
```

#### Run projects individually:

```bash
# Backend only
npm run dev:backend

# Frontend only
npm run dev:frontend
```

#### Other useful commands:

```bash
# Build both projects
npm run build

# Install dependencies
npm run install:all

# Clean and reinstall
npm run clean
```

### 2. Shell Scripts

#### Run both projects:

```bash
./start-dev.sh
```

#### Run projects individually:

```bash
# Backend only
./start-backend.sh

# Frontend only
./start-frontend.sh
```

### 3. Makefile (Simplified commands)

#### View all available commands:

```bash
make help
```

#### Main commands:

```bash
# Run both projects
make dev

# Backend only
make dev-backend

# Frontend only
make dev-frontend

# Build everything
make build

# Initial setup
make setup

# Check status
make status
```

### 4. Docker Compose (For containers)

#### First time - setup environment variables:

```bash
cp .env.example .env
# Edit .env with your configurations
```

#### Run with Docker:

```bash
# Start all services (includes MongoDB)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

#### With Makefile:

```bash
make docker-up    # Start services
make docker-logs  # View logs
make docker-down  # Stop services
```

### 5. Manual Execution

#### Backend (.NET):

```bash
cd backend
dotnet run
```

#### Frontend (Next.js):

```bash
cd frontend
npm run dev
```

## 🌐 Access URLs

Once the services are running, they will be available at:

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000 (HTTP) / https://localhost:5001 (HTTPS)
- **Swagger UI**: http://localhost:5000
- **MongoDB** (if using Docker): localhost:27017

## 🔧 Environment Variables Configuration

### Backend (.env in backend/ folder)

```env
MONGODB_PASSWORD=your_password_here
JWT_SECRET_KEY=your_jwt_secret_key
JWT_ISSUER=RealEstateAPI
JWT_AUDIENCE=RealEstateApp
```

### Docker (.env in project root)

```env
MONGODB_PASSWORD=your_mongodb_password
NODE_ENV=development
ASPNETCORE_ENVIRONMENT=Development
```

## 🛠️ Troubleshooting

### Error: "Port already in use"

```bash
# Check which process is using the port
lsof -i :3000  # For frontend
lsof -i :5000  # For backend

# Kill process if necessary
kill -9 <PID>
```

### Error: "Dependencies not found"

```bash
# Reinstall frontend dependencies
cd frontend
rm -rf node_modules package-lock.json
npm install

# Clean and restore backend
cd backend
dotnet clean
dotnet restore
```

### MongoDB connection error

1. Verify MongoDB is running
2. Check environment variables in `.env`
3. Consult the [MongoDB setup guide](./documentation/MONGODB_ATLAS_SETUP.md)

## 📊 Check Services Status

```bash
# With Makefile
make status

# Manually check processes
ps aux | grep dotnet     # Backend
ps aux | grep next       # Frontend
ps aux | grep mongod     # MongoDB
```

## 🔄 Useful Development Commands

```bash
# Reinstall everything from scratch
make clean && make setup

# Build for production
make build

# Run tests
make test

# View Docker logs
make docker-logs
```

## 📝 Important Notes

1. **Startup order**: The backend should start before the frontend to avoid connection errors
2. **Environment variables**: Make sure to properly configure the `.env` files
3. **Ports**: Default ports are 3000 (frontend) and 5000/5001 (backend)
4. **MongoDB**: You need a MongoDB instance running (local or Atlas)

## 🆘 Getting Help

If you encounter problems:

1. Check the service logs
2. Verify all requirements are installed
3. Consult the specific documentation in the `documentation/` folder
4. Use `make help` to see all available commands
