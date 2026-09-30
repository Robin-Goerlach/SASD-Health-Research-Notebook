# AGENTS.md

## Project mission

SASD Health Research Notebook is a local-first Windows desktop application for personal health documentation, research, organization and preparation of conversations with healthcare professionals.

The product is **not** a diagnostic, treatment, triage or medication-dosing system. It must not derive medical instructions from symptoms, measurements, diseases or research content.

## Current technical baseline

- C# / .NET 8+
- WPF reference frontend plus WinForms baseline frontend
- layered projects:
  - `Sasd.HealthNotebook.Domain`
  - `Sasd.HealthNotebook.Application`
  - `Sasd.HealthNotebook.Infrastructure`
  - `Sasd.HealthNotebook.Wpf`
  - `Sasd.HealthNotebook.WinForms`
- current persistence: local JSON repository
- planned persistence: SQLite with migrations
- current automated verification: Windows CI build plus dependency-light smoke-test runner
- local-first; no cloud requirement; no telemetry

The repository and the current branch are authoritative over older chat history.

## Repository guidance and source hierarchy

Do not read every planning document for every small edit. Read only the documents relevant to the task.

Use these sources as the main map:

- product scope and current status: `README.md`
- current documentation baseline and precedence: `docs/changes/160_Dokumentationsrevision_2_1.md`
- original detailed requirements and acceptance criteria: `docs/SASD_Health_Research_Notebook_Pflichtenheft.md`
- newer stable requirement IDs: `docs/requirements/155_Fachmodule_Baseline_2_1.md`
- architecture changes: `docs/architecture/030_Architekturkonzept.md` and `docs/adr/130_Architekturentscheidungen_ADR.md`
- persistence/schema changes: `docs/database/040_Datenmodell_Datenbankdesign.md`
- UI work: `docs/ui-ux/050_UI_UX_Konzept.md` and `docs/ui-ux/055_UI_Zielbild_und_Screenshot_Plan.md`
- testing, acceptance and traceability: `docs/testing/100_Testkonzept.md` and `docs/testing/105_Akzeptanzkriterien_Traceability_Quality_Gates.md`
- current implementation sequence: `docs/development/145_Codex_Arbeitsauftrag.md`

If documents conflict, prefer the current branch, explicit Baseline 2.1 revisions and newer accepted ADRs over older planning text. Do not silently resolve a material contradiction; document the chosen interpretation.

## Codex model / reasoning guidance

This section is **advisory for the human/operator**. `AGENTS.md` can guide how Codex works, but it cannot itself switch the selected model or reasoning effort.

Use the following default:

- **Medium**: normal implementation work, feature slices, refactoring with clear scope, test creation, bug fixes and most repository work.
- **Instant**: low-risk mechanical work such as spelling/docs cleanup, straightforward XML comments, localized strings, simple renames, repetitive boilerplate or an already-specified change with strong tests.
- **High**: architecture decisions, cross-cutting refactors, persistence/schema migrations, security/privacy work, possible data-loss paths, difficult debugging, concurrency, release-candidate review and requirement-completeness audits.

Escalation rules:

1. If an Instant task needs more than one substantive correction loop, continue the work in Medium.
2. If a change crosses architectural boundaries, changes persisted data, affects privacy/security or can cause silent data loss, use High for planning/review even if implementation is performed in Medium.
3. If Medium repeatedly fails to explain or fix the root cause, escalate the investigation to High.
4. Before a release candidate, perform a High-effort review against the Pflichtenheft, Baseline 2.1 requirements, traceability matrix, security rules and release quality gates.

Do not use High merely for volume. Large but mechanical edits are usually better handled in Medium or Instant with verification.

## Requirement and acceptance discipline

For every new or changed user-visible behavior:

- identify the relevant `PF-*` or `FR-*` requirement ID, or add one before implementation if none exists;
- preserve a concrete acceptance criterion;
- add or update an automated test when the behavior is reasonably automatable;
- record unavoidable manual UI checks explicitly;
- update the traceability document when a MUSS requirement or release-critical behavior changes.

A green build is not evidence that all requirements are implemented.

## Health-domain safety boundary

The application may:

- store user-entered observations and measurements;
- document user-defined goals and routines;
- document instructions that a user says came from a doctor, therapist, coach or source;
- store sources and exact source locations;
- show descriptive history, counts and trends;
- remind the user about tasks the user explicitly configured;
- prepare questions and summaries.

The application must not:

- diagnose a disease;
- infer a treatment;
- tell the user to change or stop medication;
- generate medication doses;
- automatically decide that a measurement is dangerous;
- infer that one observation caused another;
- create health goals because a condition exists;
- treat a source reliability rating as medical validation.

Use wording such as "documented", "user-entered", "observation", "possible relationship", "source", "discuss with doctor/pharmacy" and "personal goal".

## Domain design principles

Keep these concepts distinct:

- `HealthTopic`: condition, suspicion, symptom complex or research topic.
- `Observation`: something noticed or measured.
- `Session`: doctor visit, coaching, physiotherapy, consultation or similar meeting.
- `HealthAction`: an action/instruction with provenance and status.
- `Routine`: repeatable user-configured execution of an action.
- `Reminder`: notification schedule for a user-configured item.
- `Source` / `EvidenceNote`: where information came from and the exact cited location.
- `MediaResource`: image/file used as documentation or instruction material.
- `ContactReference`: healthcare contact reference prepared for later shared-contact integration.
- `ContextSnapshot`: immutable contextual data captured at an event/measurement, for example weather.

A doctor's instruction and a routine are not the same object. A session may create or reference a HealthAction; a user may choose to turn that action into a Routine.

## Architecture rules

- Domain must not reference WPF, WinForms, SQLite or file-system implementation details.
- Application may depend on Domain and abstractions, not concrete desktop controls.
- Infrastructure implements persistence, files and external adapters.
- WPF and WinForms must not contain database or file-system business logic.
- Do not move business logic into code-behind merely to make UI work faster.
- Prefer small classes and explicit interfaces over broad service objects.
- Preserve backward compatibility with current local JSON data until a migration path exists.
- Do not silently delete or overwrite user data.
- Keep both frontends on the same application/domain behavior; do not create separate business-rule implementations.

## Coding style

- Use nullable reference types correctly.
- Add XML documentation to public domain/application types and important members.
- Add explanatory `//` comments where behavior, trade-offs or safety boundaries are not obvious.
- Avoid comments that merely repeat the code.
- Use clear names; do not abbreviate domain concepts without reason.
- Avoid large unrelated refactors in a feature change.
- Do not add NuGet packages unless they provide clear value and the dependency is documented.

## Privacy and logging

Never put into logs:

- health-topic titles;
- symptoms;
- measurements or lab values;
- medication names;
- source quotations;
- medically revealing document names;
- search terms;
- session notes.

Synthetic/demo data must never be copied from real user health data in chats, files, screenshots or issues.

## Build and validation

Run from repository root:

```powershell
dotnet restore Sasd.HealthNotebook.sln
dotnet build Sasd.HealthNotebook.sln --configuration Release --no-restore
dotnet run --project tests/Sasd.HealthNotebook.SmokeTests --configuration Release --no-build
```

When real test projects are present, also run:

```powershell
dotnet test Sasd.HealthNotebook.sln --configuration Release --no-build
```

For WPF or WinForms UI work, start the affected application manually and verify the relevant workflow. A successful smoke test does not replace UI inspection.

## Definition of Done

A change is complete only when, as applicable:

- the solution builds in Release;
- all existing automated tests/smoke tests pass;
- new or changed behavior has appropriate tests;
- no sensitive health data are added to logs or fixtures;
- persistence changes include migration/backward-compatibility handling;
- relevant requirement IDs and acceptance criteria remain satisfied;
- documentation/ADRs are updated when behavior or architecture changes;
- manual UI checks are recorded for behavior that cannot yet be automated;
- the PR explains remaining known gaps instead of hiding them.

## Git workflow

Prefer a focused branch and PR per coherent change.

Suggested prefixes:

- `feat/ui-`
- `feat/domain-`
- `fix/`
- `docs/`
- `refactor/`

Commit messages should follow the existing convention, e.g. `feat: refine dashboard shell`.

Do not force-push shared branches unless explicitly requested.
