# Blob Storage Testing ADR

## Problem
- Profile photos must use Azure Blob Storage semantics, not an in-memory fake.
- Integration tests need a deterministic local blob endpoint.
- Reads must return usable URLs without storing raw image bytes in MongoDB.

## Decision
- Keep `AzureBlobAthletePhotoStorage` as the concrete runtime implementation.
- Run Azurite in Testcontainers for integration tests and point the app to that container.
- Upload athlete photos during tests and verify the generated access URL can be fetched.
- Keep only blob path and metadata in the aggregate; reads generate the URL on demand.

## Consequences
- Blob-related failures surface earlier because tests exercise the real Azure SDK.
- Test startup is slightly heavier because both MongoDB and Azurite containers are required.
- Production and test storage code paths stay aligned.

