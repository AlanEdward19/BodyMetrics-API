# Sports CRUD ADR

## Problem
- Athletes depend on sports for valid sector and category options.
- The API needs explicit CRUD endpoints instead of generic base controllers.
- Sector and category values must be unique inside a sport definition.

## Decision
- Model `Sport` as a feature root under `Features/Sports/`.
- Expose dedicated slices for create, list, get by id, update, and delete.
- Normalize and de-duplicate `Sectors` and `Categories` inside the aggregate.
- Validate sport options before data reaches MongoDB.
- Keep commands/queries, handlers, validators, and response contracts in separate files.
- Resolve slice handlers in `SportsController` per action with `[FromServices]`.

## Consequences
- Athlete validation can rely on persisted sport options.
- Each sport slice remains self-contained and easy to extend.
- Invalid duplicated options fail fast with validation errors instead of dirty data.


