---
name: education-development
description: Develop Education backend features across Domain, Application, Contracts, Infrastructure, Web, Kafka, and PostgreSQL while preserving the platform and external-module contracts.
---

# Education development

Read `AGENTS.md` before changing code. Use `Readme.Md` for setup and API context. Read `../PLATFORM.md` only for a cross-service or platform-integration task.

## Route the change

- `src/Education.Domain`: entities and invariants; no dependency on outer layers.
- `src/Education.Application`: use cases and interfaces for persistence or integrations.
- `src/Education.Contracts`: public DTOs, enums, routes and Kafka contracts.
- `src/Education.Infrastructure`: EF Core, repositories, storage and migrations.
- `src/Education.Web`: Minimal API endpoints, mappings, validators, authorization and composition.
- `src/Education.Kafka`: Kafka transport and consumers/producers.
- `tests/Education.Tests`: integration tests against real PostgreSQL Testcontainers.

Do not modify the legacy `Education/` project unless the user explicitly asks; use it only to compare behavior.

## Preserve contracts

Keep routes in `Education.Contracts.ApiRoutes`. Add Russian XML documentation to public contracts and APIs. Use `Guid` for domain identifiers. Expected failures should use the established API error/result mechanism and structured status codes, not arbitrary exceptions.

For platform-module changes, trace the complete contract before editing: Education session lifecycle, Identity token exchange, external module HTTP calls, Kafka events, idempotency and frontend consumers. Do not infer permission to edit another repository.

## Data and generated artifacts

Use EF migrations for schema changes; never replace migration-driven setup with `EnsureCreated`. Generate migrations and API artifacts with the `unsandboxed-codegen` skill outside the sandbox. Do not hand-edit generated clients.

Follow `$focused-feature-flow` for analysis and validation. Prefer one targeted integration-test pass while iterating and one justified final suite, not repeated full runs.
