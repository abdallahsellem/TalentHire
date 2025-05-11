# TalentHire

## Project Overview
TalentHire is a .NET-based web application for talent recruitment and job applications management. 

- Backend: .NET 9.0
- Database: PostgreSQL
- Docker containerization
- Entity Framework Core for ORM
- JWT Authentication

## Project Structure

### Core Components:

1. **Authentication System**
   - Authentication Controller for user management
   - JWT-based authentication using TokenService
   - Support for user roles and permissions

2. **Models**
   - User
   - Job
   - JobApplication
   - RecommendedApplicant
   - Credentials

3. **Data Layer**
   - Entity Framework Core with PostgreSQL
   - Repository pattern implementation
   - Migrations for database version control

4. **API Endpoints**
   - Authentication endpoints
   - Health check endpoint

### Docker Configuration
The project is containerized with Docker and includes:
- Backend service running on port 8080
- PostgreSQL database running on port 5432
- Health checks for both services
- Development environment configuration

### Key Features
1. User Authentication and Authorization
2. Job Posting Management
3. Job Application Processing
4. Applicant Recommendations


## Running the Project
The application can be started using Docker Compose:
```bash
docker compose up 
```

The services will be available at:
- Backend API: http://localhost:8080
- PostgreSQL Database: localhost:5432


