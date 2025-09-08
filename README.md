# TalentHire - Talent Management Platform (Backend)
## Description 
TalentHire is a backend API for a talent management platform built using a microservices architecture with .NET Core. This backend-only solution implements modern distributed system patterns including event-driven communication via Apache Kafka and provides RESTful APIs for talent recruitment and job application management.

## 🏗️ Architecture Overview

The application follows a microservices architecture pattern with the following components:

### Core Services
- **API Gateway** (Port 8080) - Central entry point for all client requests
- **Identity Service** (Port 8082) - User authentication and authorization
- **Job Service** (Port 8081) - Job posting and management
- **Applications Service** (Port 8083) - Job application processing

### Infrastructure Components
- **Apache Kafka** - Event streaming platform for inter-service communication
- **Apache Zookeeper** - Kafka cluster coordination
- **PostgreSQL Databases** - Data persistence for each service
- **Docker & Docker Compose** - Containerization and orchestration

### Message Queuing
- **Kafka Producer/Consumer Services** - Asynchronous communication between services
- **Event-Driven Architecture** - Decoupled service interactions

## 📁 Project Structure

```
TalentHire/
├── Services/
│   ├── IdentityService/           # User authentication & authorization
│   ├── JobService/                # Job posting & management
│   └── ApplicationsService/       # Job application processing
├── GatewayAPI/                    # API Gateway & routing
├── Shared/
│   └── Kafka/                     # Shared Kafka utilities & models
├── Tests/                         # Unit & integration tests
├── docker-compose.yml             # Multi-container orchestration
└── TalentHire.sln                 # Visual Studio solution file
```

## 🚀 Getting Started

### Prerequisites

Before running the application, ensure you have the following installed:

- **Docker Desktop** (v4.0 or later)
- **Docker Compose** (v2.0 or later)
- **.NET 8.0 SDK** (for local development)
- **Git** (for version control)

### Installation & Setup

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd TalentHire
   ```

2. **Environment Setup**
   
   **⚠️ IMPORTANT - Security Configuration:**
   
   Before running the application, you should configure your own credentials:
   
   a) **Create Environment File** (Recommended):
   ```bash
   cp .env.example .env  # If available
   # Or create your own .env file
   ```
   
   b) **Configure Database Credentials**:
   ```bash
   # Add to .env file or export as environment variables
   POSTGRES_USER=your_db_username
   POSTGRES_PASSWORD=your_secure_password
   ```
   
   c) **Configure JWT Settings**:
   ```bash
   JWT_SECRET_KEY=your_very_secure_jwt_secret_key_at_least_32_characters
   JWT_ISSUER=your_application_issuer
   JWT_AUDIENCE=your_application_audience
   ```
   
   The application uses environment variables defined in the `docker-compose.yml` file. Default configurations are set for development environments only.

3. **Build and Start Services**
   ```bash
   # Build and start all services
   docker-compose up --build
   
   # Or run in detached mode
   docker-compose up --build -d
   ```

4. **Verify Installation**
   
   Once all containers are running, verify the services are accessible:
   
   - **API Gateway**: http://localhost:8080
   - **Job Service**: http://localhost:8081
   - **Identity Service**: http://localhost:8082
   - **Applications Service**: http://localhost:8083
   - **Kafka**: localhost:29092

### Service Startup Order

The services have dependencies and start in the following order:

1. **Infrastructure**: Zookeeper → Kafka → Databases
2. **Core Services**: Identity Service → Job Service → Applications Service
3. **Gateway**: API Gateway (depends on all core services)

## 🛠️ Development

### Local Development Setup

For local development without Docker:

1. **Start Infrastructure Services**
   ```bash
   # Start only infrastructure (databases, Kafka)
   docker-compose up zookeeper kafka job_db identity_db applications_db
   ```

2. **Run Services Locally**
   ```bash
   # Identity Service
   cd Services/IdentityService
   dotnet run
   
   # Job Service
   cd Services/JobService
   dotnet run
   
   # Applications Service
   cd Services/ApplicationsService
   dotnet run
   
   # API Gateway
   cd GatewayAPI
   dotnet run
   ```

### Database Management

Each service has its own PostgreSQL database:

- **job_db** (Port 5432): Job Service database
- **identity_db** (Port 6303): Identity Service database  
- **applications_db** (Port 6304): Applications Service database

**Connection Details:**
- Username: Configured via `POSTGRES_USER` environment variable
- Password: Configured via `POSTGRES_PASSWORD` environment variable
- Host: `localhost` (or service name in Docker network)

> ⚠️ **Security Note**: The default docker-compose.yml contains development credentials. For production, always use secure credentials and consider using Docker secrets or external secret management systems.

### Kafka Configuration

**Kafka Broker**: localhost:29092 (external) / kafka:9092 (internal)

**Topics** (Auto-created):
- Application events
- Job events
- User events

## 🧪 Testing

### Running Tests

The project includes comprehensive test suites for each service:

```bash
# Run all tests
dotnet test TalentHire.Tests.sln

# Run specific service tests
dotnet test Tests/JobService.Tests/
dotnet test Tests/IdentityService.Tests/
dotnet test Tests/ApplicationsService.Tests/
```

### Health Checks

All services implement health check endpoints:

- **Job Service**: http://localhost:8081/health
- **Identity Service**: http://localhost:8082/health  
- **Applications Service**: http://localhost:8083/health

## 🐳 Docker Operations

### Useful Docker Commands

```bash
# View running containers
docker-compose ps

# View service logs
docker-compose logs [service-name]
docker-compose logs -f api-gateway  # Follow logs

# Restart specific service
docker-compose restart [service-name]

# Stop all services
docker-compose down

# Stop and remove volumes (⚠️ Data loss)
docker-compose down -v

# Scale services (if needed)
docker-compose up --scale job-service=2
```

### Troubleshooting

**Service Won't Start:**
```bash
# Check container status
docker-compose ps

# View detailed logs
docker-compose logs [service-name]

# Rebuild specific service
docker-compose build [service-name]
docker-compose up [service-name]
```

**Database Connection Issues:**
```bash
# Check database container
docker-compose logs job_db

# Access database directly
docker-compose exec job_db psql -U postgres -d job_db
```

**Kafka Connection Issues:**
```bash
# Check Kafka logs
docker-compose logs kafka

# Verify Kafka topics
docker-compose exec kafka kafka-topics --list --bootstrap-server localhost:9092
```

## 📊 Monitoring & Observability

### Service Endpoints

Each service exposes the following endpoints:

- `/health` - Health check endpoint
- `/swagger` - API documentation (Development only)
- Service-specific endpoints as documented in individual Swagger UIs

### Logging

- **Development**: Console logging with detailed information
- **Production**: Structured logging (can be extended with Serilog/NLog)

## 🔧 Configuration

### Environment Variables

Key environment variables used across services:

```bash
# General
ASPNETCORE_ENVIRONMENT=Development
NODE_ENV=development

# Database Connections
ConnectionStrings__DBConnection=Host=localhost;...

# JWT Configuration
JwtSettings__SecretKey=your-secret-key
JwtSettings__Issuer=your-issuer
JwtSettings__Audience=your-audience

# Kafka Configuration
KAFKA_BOOTSTRAP_SERVERS=localhost:29092
```

### Service Configuration

Configuration files are located in each service directory:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production settings

## 🚀 Deployment

### Production Deployment

1. **Update Configuration**
   - Set `ASPNETCORE_ENVIRONMENT=Production`
   - Update database connection strings
   - Configure JWT settings with secure keys

2. **Build Production Images**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.prod.yml build
   ```

3. **Deploy**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
   ```

### Container Registry

For deployment to cloud platforms:

```bash
# Tag images
docker tag talenthire_job-service your-registry/job-service:latest

# Push to registry
docker push your-registry/job-service:latest
```
### Development Guidelines

- Follow C# coding conventions
- Write unit tests for new functionality
- Update documentation for API changes
- Ensure Docker builds succeed
- Test Kafka integration thoroughly


## 🛡️ Security

- JWT-based authentication
- Service-to-service communication secured within Docker network
- Database credentials managed via environment variables
- HTTPS enforced in production environments



