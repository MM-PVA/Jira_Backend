# Jira Backend

A layered ASP.NET Core Web API for managing workspaces, projects, and project tasks, with JWT authentication and Azure Cosmos DB storage.

## Tech stack

- .NET 10 (`net10.0`)
- ASP.NET Core Web API
- Entity Framework Core (Cosmos provider)
- JWT Bearer authentication
- FluentValidation
- API versioning (`v1`, `v2`)

## Solution structure

```text
src/
  Jira.Api             # HTTP API layer (controllers, middleware, app bootstrap)
  Jira.Application     # Use cases, DTOs, validators, service contracts
  Jira.Domain          # Domain entities, enums, exceptions
  Jira.Infrastructure  # Persistence, repositories, auth, logging implementations
```

## Prerequisites

- .NET 10 SDK
- Azure Cosmos DB account (Core/SQL API)

## Configuration

Update `src/Jira.Api/appsettings.json` (or environment-specific configuration):

```json
{
  "Jwt": {
    "SecretKey": "YOUR_LONG_RANDOM_SECRET_KEY",
    "Issuer": "Jira.Api",
    "Audience": "Jira.Client",
    "ExpirationMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "AccountEndpoint=...;AccountKey=...;"
  },
  "CosmosDb": {
    "DatabaseName": "JiraDb"
  },
  "LoggingSettings": {
    "LogDirectory": "C:/path/to/Jira/Logs"
  }
}
```

> Ensure the log directory exists before running the API.

## Run locally

From repository root:

```powershell
dotnet restore
dotnet run --project .\src\Jira.Api\Jira.Api.csproj
```

The API is versioned via URL segment: `api/v{version}/...` (for example `api/v1/...`).

## Authentication flow

1. Register a user: `POST /api/v1/auth/register`
2. Login: `POST /api/v1/auth/login`
3. Use returned bearer token in `Authorization: Bearer <token>` for protected endpoints.

## Main endpoints

### Auth

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me` (authorized)

### Workspaces (authorized)

- `POST /api/v1/workspace`
- `GET /api/v1/workspace`
- `GET /api/v1/workspace/{id}`
- `PUT /api/v1/workspace/{id}`
- `DELETE /api/v1/workspace/{id}`

### Projects (authorized)

Base route: `/api/v1/workspaces/{workspaceId}/projects`

- `POST /`
- `GET /`
- `GET /{projectId}`
- `PUT /{projectId}`
- `DELETE /{projectId}`

### Project tasks (authorized, v1 + v2)

Base route: `/api/v{version}/workspaces/{workspaceId}/projects/{projectId}/tasks`

- `POST /`
- `GET /`
- `GET /{taskId}`
- `PUT /{taskId}`
- `DELETE /{taskId}`
  - `v1`: direct delete
  - `v2`: requires `?confirm=true`

### Logs

- `GET /api/v1/logs`
- `POST /api/v1/logs`
- `GET /api/v1/logs/header-search`
- `GET /api/v1/logs/ip-group`

## Notes

- Request/response metadata is logged to daily `.jsonl` files under `LoggingSettings:LogDirectory`.