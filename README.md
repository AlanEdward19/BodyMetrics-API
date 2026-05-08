# BodyMetricsApi

BodyMetricsApi is a `.NET 10` ASP.NET Core Web API for athlete and sport management.

## Local dependencies
- MongoDB for primary persistence
- Azurite for local Azure Blob Storage emulation
- Firebase project configuration for JWT validation

## Run locally
```powershell
Set-Location "C:\Users\Alan-\Desktop\Projetos\ArqonTech\BodyMetricsApi"
docker compose up -d
dotnet build BodyMetricsApi.slnx
dotnet run --project .\BodyMetricsApi\BodyMetricsApi.csproj
```

## Authentication configuration
- Configure `BodyMetricsApi/appsettings*.json` with `FirebaseAuthentication:ProjectId`.
- The API now requires a bearer token for every endpoint.
- Athlete data is scoped by the authenticated user id from the token.

## Listing endpoints
- `GET /api/sports` supports `page`, `pageSize`, `name`, `sector`, and `category`.
- `GET /api/athletes` supports `page`, `pageSize`, `fullName`, `sportId`, `sector`, `category`, and `phase`.
- List responses return `items`, `page`, `pageSize`, `totalCount`, and `totalPages`.

## Run tests
```powershell
Set-Location "C:\Users\Alan-\Desktop\Projetos\ArqonTech\BodyMetricsApi"
dotnet test BodyMetricsApi.slnx
```

## Development URLs
- `http://localhost:5000`

## Notes
- API slices live under `BodyMetricsApi/Features/`.
- Context notes live under `docs/context/`.
- Profile photos use Azure Blob Storage in production and Azurite locally.
- Integration tests spin up MongoDB and Azurite through Testcontainers.

