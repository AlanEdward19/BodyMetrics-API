# Firebase Authentication and Ownership ADR

## Problem
- The API must only accept authenticated requests.
- There is no local `User` table; the owner identifier must come from the token.
- Athlete data is tenant-scoped by user, while sports remain shared.

## Decision
- Configure ASP.NET Core authentication with Firebase-compatible JWT bearer validation.
- Use a fallback authorization policy so every controller action requires an authenticated user.
- Resolve the current user through an `ICurrentUserService` backed by `HttpContext` claims.
- Persist `OwnerUserId` only on `Athlete`; embedded assessments inherit ownership from the aggregate.
- Treat cross-user athlete access as `404` instead of exposing another user's resource.

## Consequences
- The API stays stateless and does not need a user persistence model.
- Firebase project configuration becomes mandatory outside the test host.
- Ownership rules live in handlers and repositories instead of controllers.

