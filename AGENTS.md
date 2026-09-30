# AGENTS.md

## Project mission

SASD Health Research Notebook is a local-first Windows desktop application for personal health documentation, research, organization and preparation of conversations with healthcare professionals.

The product is **not** a diagnostic, treatment, triage or medication-dosing system. It must not derive medical instructions from symptoms, measurements, diseases or research content.

## Current technical baseline

- C# / .NET 8+
- WPF desktop UI
- Layered projects:
  - `Sasd.HealthNotebook.Domain`
  - `Sasd.HealthNotebook.Application`
  - `Sasd.HealthNotebook.Infrastructure`
  - `Sasd.HealthNotebook.Wpf`
- current persistence: local JSON repository
- planned persistence: SQLite with migrations
- tests currently include a dependency-light smoke-test project
- local-first; no cloud requirement; no telemetry

Before changing code, read:

1. `README.md`
2. `docs/000_Dokumentationsuebersicht.md`
3. `docs/changes/160_Dokumentationsrevision_2_1.md`
4. `docs/requirements/155_Fachmodule_Baseline_2_1.md`
5. `docs/ui-ux/055_UI_Zielbild_und_Screenshot_Plan.md`
6. `docs/development/145_Codex_Arbeitsauftrag.md`
7. `docs/architecture/030_Architekturkonzept.md`
8. `docs/database/040_Datenmodell_Datenbankdesign.md`
9. `docs/adr/130_Architekturentscheidungen_ADR.md`

The repository and the current branch are authoritative over older chat history.

## Immediate implementation priority

The first Codex implementation goal is **not** to implement every planned health feature.

The priority is to make the running WPF application visibly approach the design promise shown in:

- `docs/screenshots/dashboard-concept.png`
- `docs/screenshots/condition-wizard-concept.png`

The current `MainWindow.xaml` already contains the structural foundation: dark left navigation, page header, dashboard cards, health-topic list and footer. Preserve working behavior and refine this foundation incrementally.

### Screenshot-first implementation order

1. Stabilize and centralize WPF visual resources.
2. Match application shell proportions, spacing, typography, cards and navigation to the concept screenshot.
3. Add selected/hover/focus states and make navigation a real reusable control/model rather than decorative TextBlocks.
4. Keep the dashboard functional with the existing `HealthTopicService`.
5. Refine the health-topic list and empty states.
6. Refine the health-topic wizard to match the concept screenshot.
7. Only then expand domain modules in small vertical slices.

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
- `Source` / `EvidenceNote`: where information came from and the exact cited location.
- `MediaResource`: image/file used as documentation or instruction material.
- `ContactReference`: healthcare contact reference, designed for future integration with a shared SASD contacts application.
- `ContextSnapshot`: immutable contextual data captured at an event/measurement, e.g. weather.

A doctor's instruction and a routine are not the same object. A session may create or reference a HealthAction; a user may choose to turn that action into a Routine.

## Architecture rules

- Domain must not reference WPF, SQLite or file-system implementation details.
- Application may depend on Domain and abstractions, not concrete WPF controls.
- Infrastructure implements persistence, files and external adapters.
- WPF must not contain database or file-system business logic.
- Do not move business logic into code-behind merely to make UI work faster.
- Prefer small classes and explicit interfaces over broad service objects.
- Preserve backward compatibility with current local JSON data until a migration path exists.
- Do not silently delete or overwrite user data.

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

For WPF work, also start the application manually and verify the relevant screen.

## UI Definition of Done for screenshot work

A UI task is complete only when:

- the application builds;
- existing health-topic loading still works;
- the screenshot target is visibly closer than before;
- controls remain keyboard usable;
- layout remains usable at the current minimum window size;
- no health data are added to logs;
- no static mock replaces working behavior;
- changed public code is documented;
- the PR explains the visual changes and remaining gaps.

For visual comparison, use the concept screenshot at the repository's intended dashboard size as the reference. Aim for hierarchy, spacing, density and visual language before pixel-level perfection.

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
