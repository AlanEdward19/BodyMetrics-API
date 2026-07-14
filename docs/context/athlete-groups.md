# Athlete Groups ADR

## Problem

- The platform allows registering sports, athletes, and physical assessments, but offers no way to group athletes in a custom, cross-cutting manner.
- Clients managing multiple teams or sub-groups within the same sport need a way to organize, filter, and compare athletes beyond the existing sport/sector/category/phase dimensions.
- The existing athlete listing and comparison features cannot be scoped to an arbitrary set of athletes without a group abstraction.

## Decision

- Introduce a new aggregate `AthleteGroup` stored in a dedicated MongoDB collection (`athleteGroups`).
- Groups are owned by `OwnerUserId` — the same tenant isolation used for athletes. Sports remain shared.
- A group stores `List<string> AthleteIds` as an embedded list (M:N via owned IDs), avoiding a junction collection.
- Cross-owner access to a group is treated as `404` (same as athletes), never `403`.
- Group names must be unique per owner (case-insensitive), validated in the command handler.
- The `GET /api/athletes` endpoint receives an optional `groupId` query parameter. When provided, the handler loads the group's `AthleteIds` and filters the athlete query with a `.Contains()` check in memory (consistent with the existing filtering strategy in `EfAthleteRepository`).
- A dedicated `GET /api/athlete-groups/comparison?groupIds=...` endpoint computes aggregated metrics (average, min, max, median) from each athlete's latest `PhysicalAssessment`, reusing the existing domain objects and value objects.
- Deletion is physical for both groups and membership — no soft-delete (consistent with the rest of the project).
- Removing a group does not delete athletes. Orphan `AthleteId` entries left after athlete deletion are silently ignored during comparison and listing.
- MongoDB indexes are created at startup via `MongoDbIndexesHostedService`: `OwnerUserId` (for listing) and `OwnerUserId + Name` (for uniqueness check).

## Consequences

- Clients can organize athletes into arbitrary named groups and combine group filters with all existing filters (sport, sector, category, phase, name search).
- Comparison across groups reuses existing `PhysicalAssessment` value objects — no new formulas introduced.
- Athlets without a group continue appearing in all existing listings and reports when no `groupId` filter is provided.
- A group becomes inaccessible if its owner's account is deactivated — consistent with how athletes work today.
- If `AthleteIds` in a group grows beyond thousands of entries, the in-memory contains filter may degrade. For the expected use case (sports teams) this is not a practical concern.
