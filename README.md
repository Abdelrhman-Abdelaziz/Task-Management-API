# Task Management API

A small, scalable backend for managing projects and tasks, built for a .NET backend technical assessment. The API follows Clean Architecture and uses CQRS with MediatR.

## Overview

This API lets users register, authenticate, and manage their own projects and tasks. All write and read operations flow through MediatR handlers, and the API surface is versioned under `/api/v1`.

## Architecture

The solution is split into four projects:

- **TaskManagement.Domain**: Entities and enums only (no framework dependencies).
- **TaskManagement.Application**: CQRS handlers, DTOs, validation, and interfaces.
- **TaskManagement.Infrastructure**: EF Core persistence, JWT, BCrypt, and concrete services.
- **TaskManagement.API**: Controllers, middleware, Swagger, and composition root.

Dependency rule: `API -> Application -> Domain` and `Infrastructure -> Application -> Domain`. The application layer never references infrastructure types directly.

## Tech stack

- .NET 9, ASP.NET Core Web API
- Entity Framework Core + SQL Server
- JWT authentication (custom user table + BCrypt)
- MediatR + CQRS, FluentValidation
- Swagger + API versioning
- xUnit, Moq, FluentAssertions
- Docker (multi-stage API image, compose with SQL Server)

## Requirements

- .NET 9 SDK
- SQL Server (local instance) or Docker
- Optional: `dotnet-ef` tool for applying migrations

Install EF tools if needed:

```bash
dotnet tool install --global dotnet-ef
```

## Configuration

Defaults live in `src/TaskManagement.API/appsettings.json`. You can override them with environment variables.

Key settings:

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JwtSettings__ExpirationInMinutes`

Example environment overrides:

```bash
setx ConnectionStrings__DefaultConnection "Server=localhost;Database=TaskManagementDb;Trusted_Connection=true;TrustServerCertificate=true;"
setx JwtSettings__Secret "CHANGE-ME-use-a-long-secret-key-at-least-32-characters!"
```

## Local development

Restore and build:

```bash
dotnet restore
dotnet build
```

Apply migrations (from repo root):

```bash
dotnet ef database update -p src/TaskManagement.Infrastructure -s src/TaskManagement.API
```

Run the API:

```bash
dotnet run --project src/TaskManagement.API
```

Swagger UI:

- http://localhost:5245/swagger
- https://localhost:7258/swagger

## Docker

Build and run the API + SQL Server with Docker Compose:

```bash
docker compose up --build
```

Swagger UI:

- http://localhost:8080/swagger

Set a stronger password for SQL Server by exporting `SA_PASSWORD` before running compose:

```bash
setx SA_PASSWORD "YourStrong!Passw0rd"
```

## API basics

All endpoints are under `/api/v1`. Auth endpoints are public, everything else requires a bearer token.

### Response shape

All responses are wrapped in a consistent envelope:

```json
{
	"success": true,
	"data": { },
	"message": "Optional message",
	"errors": null
}
```

### Enum values

Enums are serialized as integers by default.

- `TaskPriority`: `0=Low`, `1=Medium`, `2=High`, `3=Critical`
- `TaskItemStatus`: `0=Todo`, `1=InProgress`, `2=Done`, `3=Cancelled`

### Endpoints

**Auth**

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`

**Projects** (requires auth)

- `GET /api/v1/projects`
- `GET /api/v1/projects/{id}`
- `POST /api/v1/projects`
- `PUT /api/v1/projects/{id}`
- `DELETE /api/v1/projects/{id}`

**Tasks** (requires auth)

- `GET /api/v1/projects/{projectId}/tasks`
- `POST /api/v1/projects/{projectId}/tasks`
- `PATCH /api/v1/projects/{projectId}/tasks/{taskId}/status`
- `DELETE /api/v1/projects/{projectId}/tasks/{taskId}`

## Postman

Import the collection at the repo root:

```
TaskManagement.postman_collection.json
```

Workflow:

1. Run `Auth - Register` (optional) to create a user.
2. Run `Auth - Login` to store `{{token}}` from the response.
3. Call any project or task endpoint with the bearer token already configured at the collection level.

## Testing

```bash
dotnet test
```
