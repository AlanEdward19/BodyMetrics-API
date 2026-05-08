# AGENTS.md

## Current repository state
- The repo is a minimal ASP.NET Core Web API on `.NET 10` with a single project: `BodyMetricsApi/BodyMetricsApi.csproj`.
- `Program.cs` only wires `AddControllers()`, `AddOpenApi()`, `UseAuthorization()`, and `MapControllers()`.
- `Controllers/WeatherForecastController.cs` and `WeatherForecast.cs` are template scaffolding and should be removed/replaced when the real domain is introduced.
- There is currently no test project, no MongoDB wiring, no `docker-compose`, no Azure Blob integration, and no feature context markdowns yet.

## Non-negotiable coding rules
- All code identifiers must be in English: namespaces, folders, classes, records, enums, methods, properties, variables, DTOs, ViewModels, test names, and database field names.
- Build features using **TDD first** and **Spec-Driven Development**: write/update the feature spec/context markdown before or alongside code.
- Use **DDD concepts inside vertical slices**, but keep a **single application project** unless a hard boundary forces extraction.
- Apply **CQRS** in all slices: commands must not contain read concerns, and queries must not contain write concerns.
- Prefer feature folders such as `Features/Athletes/Create`, `Features/Athletes/GetById`, `Features/Sports/Update` instead of layering by controller/service/repository.
- Keep each slice self-contained: request/response contracts, validator, handler, endpoint/controller, tests, and slice-local mapping.
- All validations must use **FluentValidation** (`AbstractValidator<T>`). Do not create manual/custom validator classes outside FluentValidation.
- Use **one file per artifact type** in a slice: do not mix command/query, validator, and handler in the same file.
- Keep **contracts** split in dedicated files (for example one file per command/query/response); do not create "mega contracts" files.
- **Never put more than one type (class, record, enum, interface) in the same file.**

## DDD building blocks
### Aggregate Root
- The central entity that controls access to all objects in its cluster (aggregate).
- Only the aggregate root may be referenced by external code; internal entities are accessed through the root.
- Named after the main concept it protects (e.g., `Athlete`, `Sport`).
- Lives at the domain root folder — e.g., `Features/Athletes/Athlete.cs`.

### Aggregate
- A cluster of domain objects (the root plus any child entities and value objects) treated as a single unit of consistency.
- All changes to internal state go through the aggregate root's methods; external code never mutates child objects directly.

### Value Object
- An immutable object defined entirely by its values; two value objects with equal values are considered identical.
- Has no identity (`Id`); equality is by value, not reference.
- Named with a `ValueObject` suffix (e.g., `GeneralMeasurementsValueObject`, `ProfilePhotoReferenceValueObject`).
- Lives in a `ValueObjects/` folder under the domain or subdomain `Shared/`.

### DTO (Data Transfer Object)
- A plain, flat object used **solely to carry data between internal application layers** (e.g., between an infrastructure adapter and a handler, or a message bus payload).
- DTOs are **not** exposed directly to the HTTP client.
- Named with a `Dto` suffix (e.g., `AthleteImportDto`).
- Use a `Dtos/` folder inside the relevant layer when needed.

### ViewModel
- The **API response contract** returned to the HTTP client.
- Represents a read-only projection of domain data shaped for consumer needs.
- Named with a `ViewModel` suffix (e.g., `AthleteViewModel`, `PhysicalAssessmentViewModel`).
- Lives in a `ViewModels/` folder inside the domain or subdomain `Shared/`.
- Never expose raw domain entities or value objects directly as ViewModels.

## Folder and namespace rules
- Artifacts **shared across multiple slices of the same domain** live in `<Domain>/Shared/` (e.g., `Athletes/Shared/`).
- **Subdomains** of a domain live inside the domain folder (e.g., `Athletes/PhysicalAssessments/` is a subdomain of `Athletes`).
- A subdomain may have its own `Shared/` folder for artifacts shared across its own slices.
- A subdomain can contain further subdomains (nesting is allowed).
- Nothing specific to a subdomain should leak up to the parent domain's `Shared/`.
- Sub-folder structure inside `Shared/`: `Enums/`, `ValueObjects/`, `Commands/`, `ViewModels/`, `Validators/`, `Interfaces/`, `Persistence/`.
- Folder names are always **plural** (e.g., `ViewModels/`, `Validators/`, `Commands/`, `ValueObjects/`).

## Target domain and invariants
- Main aggregate: `Athlete`.
- `Athlete` fields: `FullName`, `SportId`/`Sport`, `Sector`, `Phase`, `Category`, `Sex`, `Ethnicity`, `BirthDate`, `ProfilePhoto` storage reference, and a time-series list of `PhysicalAssessment` items.
- Use `DateOnly` for `BirthDate` and serialize as `yyyy-MM-dd` with no timestamp.
- `Phase` enum: `Competitive`, `PreSeason`, `WeightLoss`, `WeightGain`, `Maintenance`.
- `Sex` enum: `Male`, `Female`.
- `Ethnicity` enum: `White`, `Black`, `Asian`.
- `Sport` owns valid `Sector` and `Category` options; `Sector` and `Category` are unique text values within the sport context.
- Required CRUD scope: `Athlete` and `Sport`.
- `Athlete` has many `PhysicalAssessment` entries.
- `PhysicalAssessment` sections: `GeneralMeasurements`, `Skinfolds`, `Circumferences`.
- `GeneralMeasurements`: `WeightKg`, `HeightCm`, `SittingHeightCm`.
- `Skinfolds` values are in `mm`; every field is nullable.
- `Circumferences` values are in `cm`; every field is nullable.

## Result Pattern
- All command and query handlers return `OperationResult` (no value) or `OperationResult<T>` (with value) from `Shared/Results/`.
- **Never throw exceptions** for expected business failures (not found, validation errors); always return a failed `OperationResult`.
- Factory methods available: `OperationResult.Success()`, `OperationResult<T>.Success(value, statusCode)`, `OperationResult.NotFound(message)`, `OperationResult.Validation(errors)`.
- Use `StatusCodes.Status201Created` as the `statusCode` argument on `Success()` for create operations; default is `200` for reads and `204` for void commands.
- Controllers convert results to `IActionResult` exclusively via the `ToActionResult()` extension method; never translate status codes manually in a controller.
- Validation errors are surfaced as `ValidationProblemDetails` (HTTP 400); not-found errors are surfaced as `ProblemDetails` (HTTP 404).

## Persistence and external integrations
- MongoDB is the primary database. Add a root `docker-compose.yml` when MongoDB is introduced so local development and integration tests have a predictable setup.
- Access MongoDB through **Entity Framework** for data access patterns in the application layer.
- Use Testcontainers for integration tests that need MongoDB or other external dependencies; do not fake infrastructure that can be containerized.
- Profile photos must be stored in Azure Blob Storage. Persist only the blob identifier/path plus metadata needed to generate an access URL on reads; never store raw image bytes in MongoDB.

## Testing standard
- Every feature must ship with tests before completion.
- Minimum: **5 tests per feature** (unit and/or integration depending on the slice).
- Use **xUnit** for all test projects.
- Prefer `[Theory]` over `[Fact]` whenever inputs/branches can be parameterized.
- After finishing a feature, run the full suite and confirm nothing else regressed.

## Spec/ADR workflow
- Every new context/feature must have a short markdown note (target: 15-20 lines) before or during implementation.
- Reuse and update the existing context file when a later iteration learns something important; brevity is preferred, but accuracy wins.
- Write these notes in ADR style: problem, decision, consequences.
- Recommended location: `docs/context/` with one file per context or slice, e.g. `docs/context/athletes-create.md`.

## Developer workflow verified from this repo
- Build from root: `dotnet build BodyMetricsApi.slnx`
- Current test command from root: `dotnet test BodyMetricsApi.slnx --no-build`
- Dev URLs currently configured in `Properties/launchSettings.json`: `http://localhost:5282` and `https://localhost:7156`

## Implementation notes for agents
- Remove template artifacts early so the repo reflects the real domain.
- Keep endpoints and contracts explicit; avoid generic CRUD base classes.
- Each domain must expose one general controller (for example, `AthletesController`, `SportsController`).
- Inside each domain controller flow, each feature must still keep explicit request parameters, handler, and FluentValidation validator.
- In controllers, inject handlers/validators **per action** with `[FromServices]`; do not inject all feature handlers in the controller constructor.
- Put business invariants close to the slice/domain code they protect.
- When adding a new slice, also add/update its context markdown and its tests in the same iteration.
- Repository implementations live in `<Domain>/Shared/Persistence/` (e.g., `Athletes/Shared/Persistence/EfAthleteRepository.cs`).
- Repository interfaces live in `<Domain>/Shared/Interfaces/`.
- Use `ViewModels/` (plural) as the folder name for all ViewModel files.
- All repository **read** methods (`GetAllAsync`, `GetByIdAsync`) must call `.AsNoTracking()` — query results are never mutated directly; write operations call `Update`/`Remove` explicitly and do not need prior EF tracking.

