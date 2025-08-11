# TalentHire Test Projects

This directory contains comprehensive unit tests for all the controllers in the TalentHire microservice project.

## Test Projects Overview

### 1. ApplicationsService.Tests
Tests for the ApplicationsService microservice controller endpoints.

**Tested Controller:** `ApplicationsController`

**Endpoints Covered:**
- `GET /api/applications` - Get all applications (Admin only)
- `GET /api/applications/{id}` - Get application by ID
- `POST /api/applications` - Create a new application
- `PUT /api/applications/{id}/status` - Update application status
- `GET /api/applications/job/{jobId}` - Get applications by job ID
- `GET /api/applications/user/{userId}` - Get applications by user ID
- `PUT /api/applications/{id}` - Update an existing application
- `DELETE /api/applications/{id}` - Delete an application
- `GET /api/applications/job/{jobId}/count` - Get application count by job ID
- `GET /api/applications/status/{status}/count` - Get application count by status
- `GET /api/applications/user/{userId}/job/{jobId}/exists` - Check if user applied to job

**Test Categories:**
- Authentication and authorization scenarios
- Input validation (invalid IDs, null values)
- Business logic validation (duplicate applications, ownership checks)
- Error handling and exception scenarios
- Success scenarios with various user roles

### 2. IdentityService.Tests
Tests for the IdentityService microservice controller endpoints.

**Tested Controller:** `AuthenticationController`

**Endpoints Covered:**
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/logout` - User logout (Authorized)

**Test Categories:**
- Valid and invalid credential scenarios
- Token generation and validation
- User registration with various input combinations
- Logout with different authentication states
- Error handling for authentication failures

### 3. JobService.Tests
Tests for the JobService microservice controller endpoints.

**Tested Controller:** `JobsController`

**Endpoints Covered:**
- `GET /api/jobs` - Get all jobs
- `GET /api/jobs/{id}` - Get job by ID
- `POST /api/jobs` - Create a new job
- `PUT /api/jobs/{id}` - Update a job
- `DELETE /api/jobs/{id}` - Delete a job

**Test Categories:**
- CRUD operations validation
- Input validation and error handling
- Repository exception handling
- Complete workflow testing (Create → Read → Update → Delete)

## Testing Framework and Tools

### Technologies Used:
- **xUnit** - Testing framework
- **Moq** - Mocking framework for dependencies
- **Microsoft.AspNetCore.Mvc.Testing** - ASP.NET Core testing utilities
- **AutoMapper** - For testing mapping scenarios (Applications service)

### Test Structure:
Each test class follows a consistent pattern:
1. **Arrange** - Set up test data and mock dependencies
2. **Act** - Execute the controller action
3. **Assert** - Verify the expected results

### Mock Objects:
- Repository interfaces are mocked to isolate controller logic
- External service dependencies are mocked
- Authentication context is mocked for authorization testing

## Running the Tests

### Prerequisites:
- .NET 9.0 SDK
- All project dependencies restored

### Commands:

Run all tests:
```bash
cd Tests
dotnet test TalentHire.Tests.sln
```

Run tests for a specific project:
```bash
# Applications Service tests
dotnet test ApplicationsService.Tests/

# Identity Service tests
dotnet test IdentityService.Tests/

# Job Service tests
dotnet test JobService.Tests/
```

Run tests with coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Coverage

The test suites provide comprehensive coverage including:

### ✅ Happy Path Scenarios
- Valid requests with expected responses
- Successful CRUD operations
- Proper authentication flows

### ✅ Edge Cases
- Invalid input validation
- Boundary conditions
- Empty or null data handling

### ✅ Error Scenarios
- Repository/database exceptions
- Authentication failures
- Authorization violations
- Business rule violations

### ✅ Security Testing
- Role-based access control
- User ownership validation
- JWT token claim verification

## Test Organization

Tests are organized using regions for better readability:
- Each endpoint has its own test region
- Related test scenarios are grouped together
- Integration-style workflow tests demonstrate complete user journeys

## Key Testing Patterns

### 1. Controller Context Setup
Tests set up authentication context with different user roles and claims to test authorization scenarios.

### 2. Mock Verification
Tests verify that repository methods are called with expected parameters and frequencies.

### 3. Exception Testing
Tests verify proper exception handling and error response generation.

### 4. Business Logic Validation
Tests verify business rules like preventing duplicate applications, ownership checks, etc.

## Contributing to Tests

When adding new endpoints or modifying existing ones:

1. **Add corresponding test methods** for all scenarios
2. **Follow the AAA pattern** (Arrange, Act, Assert)
3. **Include both positive and negative test cases**
4. **Test all authorization scenarios** if the endpoint is secured
5. **Verify mock interactions** where appropriate
6. **Add descriptive test method names** that clearly indicate what is being tested

## Notes

- The HealthController was skipped as requested (simple health check endpoints)
- Tests focus on controller logic rather than integration testing
- Repository implementations are mocked to isolate controller behavior
- Authentication middleware behavior is simulated through controller context setup
