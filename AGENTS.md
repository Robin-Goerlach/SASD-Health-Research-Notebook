# AGENTS.md

## Project mission

SASD Health Research Notebook is a local-first Windows desktop application for personal health documentation, research, organization and preparation of conversations with healthcare professionals.

The product is **not** a diagnostic, treatment, triage or medication-dosing system. It must not derive medical instructions from symptoms, measurements, diseases or research content.

## Current technical baseline

- C# / .NET 8+
- layered projects:
  - `Sasd.HealthNotebook.Domain`
  - `Sasd.HealthNotebook.Application`
  - `Sasd.HealthNotebook.Infrastructure`
  - `Sasd.HealthNotebook.Wpf`
  - `Sasd.HealthNotebook.WinForms`
- **WinForms is the primary frontend for current feature development.**
- **WPF remains a buildable reference frontend and compatibility check.**
- both frontends use the same Application/Infrastructure layers and the same local JSON persistence.
- current persistence: local JSON repository.
- planned persistence: SQLite with migrations.
- tests currently include a dependency-light smoke-test project.
- local-first; no cloud requirement; no telemetry.
- WinForms currently provides German/English UI localization.

Before changing code:

1. read this `AGENTS.md`;
2. inspect the current branch, affected projects and existing tests;
3. read only the task-relevant documents from the source hierarchy below;
4. for large cross-cutting work, additionally read the current baseline revision and relevant ADRs.

The repository and the current branch are authoritative over older chat history.

## Repository guidance and source hierarchy

Do not preload every planning document for a small edit. Use progressive disclosure: start with the repository state and only open the documentation needed for the affected behavior, architecture area or quality gate.

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

## Frontend strategy

Do not develop the same new feature independently in WPF and WinForms.

Current rule:

1. WinForms receives new product features and UI improvements.
2. WPF remains buildable and may be used to verify the shared Application/Infrastructure contracts.
3. WPF receives only compatibility fixes or explicitly requested work while WinForms is primary.
4. Domain, Application and Infrastructure must remain UI-independent so a future frontend decision stays possible.

Do not delete or deliberately break the WPF project.

## Immediate implementation priority

The next UI goal is to refine the **running WinForms application** so that it increasingly fulfills the design promise shown in:

- `docs/screenshots/dashboard-concept.png`
- `docs/screenshots/condition-wizard-concept.png`

The WinForms baseline already contains:

- dark left navigation;
- Dashboard / Gesundheitsthemen navigation;
- dashboard cards;
- health-topic grid;
- refresh and new-topic actions;
- status strip;
- health-topic wizard;
- reusable navigation/card controls;
- central styling classes;
- presenter classes;
- German/English localization.

Preserve working behavior and improve this foundation incrementally.

### Screenshot-first implementation order

1. Polish the WinForms shell: proportions, spacing, typography and visual hierarchy.
2. Refine navigation selected/hover/focus behavior and keyboard usability.
3. Refine dashboard cards and the health-topic work area.
4. Refine empty states and grid readability.
5. Bring the WinForms wizard closer to the concept screenshot while preserving its current functional scope.
6. Keep German and English UI texts consistent.
7. Only then expand health-domain modules as small vertical slices.

Do not replace the application with a static mock-up. Every UI increment must leave the application runnable.

## Health-domain safety boundary

The application may:

- store user-entered observations;
- store measurements;
- store user-defined goals and routines;
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

Use language such as "documented", "user-entered", "observation", "possible relationship", "source", "discuss with doctor/pharmacy" and "personal goal".

## Domain design principles

Keep these concepts distinct:

- `HealthTopic`: condition, suspicion, symptom complex or research topic.
- `Observation`: something noticed or measured.
- `Session`: doctor visit, coaching, physiotherapy, consultation or similar meeting.
- `HealthAction`: an action/instruction with provenance and status.
- `Routine`: repeatable user-configured execution of an action.
- `Reminder`: notification schedule for a user-configured item.
- `Source` / `SourceLocation` / `EvidenceNote`: where information came from and the exact cited location.
- `MediaResource`: image/file used as documentation or instruction material.
- `ContactReference`: healthcare contact reference, designed for future integration with a shared SASD contacts application.
- `ContextSnapshot`: immutable contextual data captured at an event/measurement, e.g. weather.

A doctor's instruction and a routine are not the same object. A session may create or reference a HealthAction; a user may choose to turn that action into a Routine.

## Architecture rules

- Domain must not reference WinForms, WPF, SQLite or file-system implementation details.
- Application may depend on Domain and abstractions, not concrete UI controls.
- Infrastructure implements persistence, files and external adapters.
- WinForms and WPF must not contain database or file-system business logic.
- Do not duplicate Domain/Application/Infrastructure types for a frontend.
- Do not move business logic into Form event handlers or WPF code-behind merely to make UI work faster.
- Prefer small classes and explicit interfaces over broad service objects.
- Preserve backward compatibility with current local JSON data until a migration path exists.
- Both frontends must continue to see the same local HealthTopic data while JSON is the active persistence layer.
- Do not silently delete or overwrite user data.

## WinForms presentation guidance

- Prefer idiomatic WinForms controls and layout over mechanically translating WPF/XAML.
- Keep large Forms from accumulating business logic; use Views, presenters/controllers and reusable controls where they help.
- Reuse `Styling/UiColors.cs`, `UiFonts.cs` and `UiMetrics.cs` instead of scattering UI constants.
- Keep localization through the existing localization layer; do not hard-code new user-facing strings in random Forms.
- Avoid adding a UI framework/package unless there is a documented benefit.
- The Visual Studio designer may be used where helpful, but generated designer code must not become a place for business logic.

## Coding style

- Use nullable reference types correctly.
- Add XML documentation to public domain/application types and important members.
- Add explanatory `//` comments where behavior or safety boundaries are not obvious.
- Avoid comments that merely repeat the code.
- Use clear names; do not abbreviate domain concepts without reason.
- Avoid large unrelated refactors in a feature change.
- Do not add NuGet packages unless they provide clear value and are documented in the PR.

## Privacy and logging

Never put into logs:

- health-topic titles;
- symptoms;
- measurements;
- lab values;
- medication names;
- source quotations;
- document names if medically revealing;
- search terms;
- session notes.

Synthetic/demo data must never resemble real user health data copied from issues, chats, files or screenshots.

## Build and validation

Run from repository root:

```powershell
dotnet restore
dotnet build Sasd.HealthNotebook.sln --configuration Release
dotnet run --project tests/Sasd.HealthNotebook.SmokeTests --configuration Release
```

For current UI work, start WinForms manually:

```powershell
dotnet run --project src/Sasd.HealthNotebook.WinForms
```

When shared contracts or persistence change, also start WPF and verify compatibility:

```powershell
dotnet run --project src/Sasd.HealthNotebook.Wpf
```

## UI Definition of Done

A WinForms UI task is complete only when:

- the full solution builds;
- smoke tests pass;
- existing health-topic loading still works;
- the screenshot target is visibly closer than before when the task is visual;
- controls remain keyboard usable;
- the layout remains usable at the documented minimum window size;
- German and English text still render sensibly;
- no health data are added to logs;
- no static mock replaces working behavior;
- WPF is not broken by shared-layer changes;
- changed public/complex code is documented;
- the PR explains the visual/functional changes and remaining gaps.

For visual comparison, prioritize hierarchy, spacing, density and visual language before pixel-level perfection.

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

- `feat/winforms-`
- `feat/domain-`
- `fix/`
- `docs/`
- `refactor/`

Commit messages should describe the product change, e.g. `feat: refine WinForms dashboard shell`.

Do not force-push shared branches unless explicitly requested.
