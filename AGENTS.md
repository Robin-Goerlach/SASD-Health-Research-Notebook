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

## Codex model and reasoning policy

This section is advisory for the human/operator. `AGENTS.md` cannot itself select or switch the Codex model or reasoning level.

Model selection and reasoning effort are separate decisions. Do not treat a higher reasoning level as the only escalation mechanism.

The task-role names below are intentionally stable. The concrete model mapping may be updated as Codex model availability changes.

### Current role-to-model mapping

Use this mapping when the listed models are available in the operator's Codex environment:

| Role | Preferred model | Reasoning | Purpose |
|---|---|---|---|
| **EFFICIENCY** | GPT-6 Luna | Low | mechanical, low-risk, high-volume work |
| **DEFAULT** | GPT-6.1 Sol | Medium | normal product development and most feature work |
| **DEEP** | GPT-6.1 Sol | High | technically difficult implementation/debugging |
| **CRITICAL** | GPT-6 Astra | Medium or High | expensive-to-reverse architecture/safety decisions and independent reviews |

Model availability depends on plan, workspace settings and Codex version. If a preferred model is unavailable, use the closest available model with the same task-role intent instead of weakening verification.

Do not use xHigh/Max by default. Reserve it for exceptional tasks where High has proved insufficient or repository-specific evaluation demonstrates a clear benefit.

### EFFICIENCY

Use for mechanical, low-risk and highly constrained work.

Typical tasks:

- spelling and Markdown cleanup;
- straightforward XML documentation;
- localized string additions or repetitive localization changes;
- simple renames;
- repetitive boilerplate;
- narrowly specified edits with strong existing tests.

EFFICIENCY is not appropriate merely because a task is small if that task changes persisted data, security/privacy boundaries, public interfaces or medical-safety behavior.

### DEFAULT

Use for normal product development.

This is the default role for the SASD Health Research Notebook.

Typical tasks:

- normal feature implementation;
- vertical slices across Domain, Application, Infrastructure and WinForms;
- unit and integration tests;
- routine bug fixing;
- normal refactoring;
- UI development with clear requirements.

Prefer DEFAULT over EFFICIENCY whenever the task contains meaningful design choices.

### DEEP

Use when the task is technically difficult but does not automatically require the strongest model.

Typical tasks:

- difficult debugging;
- complex failure paths;
- cross-cutting refactoring;
- concurrency or async problems;
- repeated failure to identify a root cause at DEFAULT;
- subtle test failures involving several layers.

### CRITICAL

Use for tasks where a wrong architectural, privacy, security or data-integrity decision would be expensive to reverse.

Typical tasks:

- architecture decisions;
- persistence-format changes;
- database schema and migrations;
- backup and restore design;
- encryption;
- privacy and security review;
- possible silent data-loss paths;
- public/plugin interfaces;
- medical-safety boundaries;
- release-candidate review.

For high-risk changes, CRITICAL is often best used for planning and independent review while bounded implementation is performed with DEFAULT or DEEP.

### Escalation rules

Do not increase model capability or reasoning merely because a result is wrong.

First determine whether the problem is caused by:

- unclear requirements;
- missing repository context;
- insufficient acceptance criteria;
- incorrect or incomplete tests;
- or genuinely difficult reasoning.

Improve requirements, context or tests before increasing reasoning when those are the root cause.

Escalate as follows:

1. **EFFICIENCY -> DEFAULT** when more than one substantive correction is required, the task stops being mechanical, or meaningful design choices appear.
2. **DEFAULT -> DEEP** when difficult reasoning is required or the root cause remains unresolved after a reasonable correction cycle.
3. **DEFAULT/DEEP -> CRITICAL immediately** when persisted data, migrations, security, privacy, encryption, public interfaces, medical-safety boundaries or possible silent data loss are involved.
4. Before a release candidate, perform a **CRITICAL read-only review** against the Pflichtenheft, Baseline 2.1 requirements, traceability matrix, architecture, security/privacy rules and release quality gates.

Do not use CRITICAL merely for volume. Large but mechanical work may still belong in EFFICIENCY or DEFAULT if verification is strong.

### Implementation versus review

A CRITICAL task does not imply that the strongest model must write all production code.

Preferred pattern for high-risk changes:

1. CRITICAL model plans and reviews the architecture and failure modes.
2. DEFAULT or DEEP model performs the bounded implementation.
3. Automated tests and acceptance criteria verify observable behavior.
4. CRITICAL model performs an independent review.

For release candidates:

1. run a read-only CRITICAL review against requirements, traceability, architecture, security/privacy and release gates;
2. remediate findings in bounded tasks;
3. perform a final CRITICAL verification.

### Verification over model confidence

A higher-capability model or higher reasoning level is not evidence that a requirement is implemented correctly.

Requirement IDs, acceptance criteria, automated tests, migration tests, security/privacy tests and documented manual UI checks remain authoritative.

A green build alone is not sufficient.

### Optimize for cost per successful task

Do not optimize for the cheapest individual Codex run.

Consider:

- number of correction loops;
- repository context that must be reread;
- build/test iterations;
- review effort;
- risk of rework;
- latency;
- total usage.

The preferred configuration is the lowest-cost model/reasoning combination that repeatedly completes the task to the required quality level.

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
