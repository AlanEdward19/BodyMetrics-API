# API bootstrap ADR

## Problem
- The repository started as the default ASP.NET Core template.
- The domain now needs athlete and sport management with MongoDB and blob storage.
- The team wants DDD concepts without splitting the solution into many application layers.

## Decision
- Keep a single Web API project and organize code by vertical slices under `Features/`.
- Put MongoDB repositories, JSON/BSON serialization, and blob adapters inside the same project.
- Keep each HTTP slice explicit with request, validator, handler, and endpoint classes together.
- Use `DateOnly` for domain dates and serialize as `yyyy-MM-dd`.

## Consequences
- The project stays small and easy to navigate while still isolating feature behavior.
- Shared infrastructure is available without introducing premature project boundaries.
- Template files should be removed so the repo reflects the real domain immediately.

