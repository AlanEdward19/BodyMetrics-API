# Athletes CRUD ADR

## Problem
- The main aggregate is `Athlete`, and it depends on `Sport` rules plus time-series physical assessments.
- Athlete creation also needs profile photo storage without writing raw image bytes to MongoDB.
- Physical assessments need nullable anthropometric fields but still need ordering in time.

## Decision
- Store athletes in MongoDB with embedded `PhysicalAssessment` documents.
- Require each assessment to carry an `AssessmentDate` so the list can behave as a time series.
- Upload profile photos through an `IAthletePhotoStorage` abstraction backed by Azure Blob Storage.
- Persist only photo metadata and blob path, then generate access URLs on reads.
- Keep each athlete slice with one artifact per file (command/query, handler, validator, and contract records separated).
- In `AthletesController`, resolve handlers per action with `[FromServices]` instead of constructor-wide injection.

## Consequences
- Athlete reads stay simple because assessments are loaded with the aggregate.
- Blob storage can be replaced in tests while production keeps Azure Blob semantics.
- Sector and category validation must check the referenced sport before saving athletes.


