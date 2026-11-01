---
name: focused-feature-flow
description: Implement or revise a feature in Education with a narrow scope, economical inspection, and proportionate validation instead of repeatedly running the full integration suite.
---

# Focused feature flow

Use this workflow for feature implementation, bug fixes, and review fixes in this repository.

## Work in one coherent pass

1. Read `AGENTS.md`, then only the requirement and files directly relevant to the feature. Do not scan all documentation or the entire solution without a concrete reason.
2. Check `git status`, identify the expected layers and write a short acceptance checklist. Treat other repositories and unrelated dirty files as out of scope.
3. Find one similar vertical slice with `rg`; inspect its endpoint, contract, application logic, persistence and tests. Stop exploring once the local pattern and affected call chain are clear.
4. Implement the production path as a coherent batch. Add tests after the behavior and contract have stabilized, unless a small regression test is needed to reproduce a bug first.
5. Test observable behavior and important invariants. Do not duplicate implementation details in tests and do not create broad test matrices without a requirement or demonstrated risk.

## Validation ladder

Use the cheapest check that can reveal the current class of error:

- During implementation: compile the affected project when useful; do not run all tests after every edit.
- After a focused change: run the relevant integration test class or filter once.
- If it fails: rerun only the failing test while fixing it.
- At the end of a meaningful backend feature: run `dotnet format Education.sln --no-restore`, then one build and one full test run if persistence, authorization, public API, migrations, or shared infrastructure changed. For a small isolated fix, targeted tests plus a build are enough unless the user asks for the full suite.

Formatting is a final step. Inspect its diff and keep mechanical formatting separate from functional changes when it touches unrelated files.

Do not start hosts, containers, migrations, code generation, network installs, commits, pushes, or cross-repository edits unless the task requires them. Stop processes and containers started for the task. Report exactly what was and was not verified.
