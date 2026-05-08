# BodyMetricsApi

BodyMetricsApi is a `.NET 10` ASP.NET Core Web API for athlete and sport management.

## Local dependencies
- MongoDB for primary persistence
- Azurite for local Azure Blob Storage emulation

## Run locally
```powershell
Set-Location "C:\Users\Alan-\Desktop\Projetos\ArqonTech\BodyMetricsApi"
docker compose up -d
dotnet build BodyMetricsApi.slnx
dotnet run --project .\BodyMetricsApi\BodyMetricsApi.csproj
```

## Run tests
```powershell
Set-Location "C:\Users\Alan-\Desktop\Projetos\ArqonTech\BodyMetricsApi"
dotnet test BodyMetricsApi.slnx
```

## Development URLs
- `http://localhost:5282`
- `https://localhost:7156`

## Notes
- API slices live under `BodyMetricsApi/Features/`.
- Context notes live under `docs/context/`.
- Profile photos use Azure Blob Storage in production and Azurite locally.

