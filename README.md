# SASD Health Research Notebook

> Local-first Windows desktop application for documenting personal health topics, observations, sources, measurements, sessions, documents and user-defined routines — designed for personal organization and research, not diagnosis or therapy.

![Dashboard concept screenshot](docs/screenshots/dashboard-concept.png)

## Project status

The repository is in the **early implementation phase**.

A working .NET desktop application baseline already exists with:

- layered Domain / Application / Infrastructure projects;
- a local JSON-backed HealthTopic repository;
- a functional WPF application shell;
- a first WinForms frontend baseline using the same application services and JSON persistence;
- a first "new health topic" wizard;
- smoke tests;
- concept screenshots that define the intended visual direction.

The current implementation is not yet feature-complete. The immediate development goal is to keep the application small, runnable and close to the concept screenshots while preserving working behavior and data safety.

## Purpose

SASD Health Research Notebook is a personal, local-first health documentation and research workspace.

It is intended to help users:

- organize health topics, conditions, suspicions and long-term observations;
- document symptoms, measurements, nutrition/context and personal observations;
- collect PDFs, images, screenshots and other supporting material;
- keep research sources with exact citation locations and trust metadata;
- prepare and follow up doctor, therapy, coaching or consultation sessions;
- manage user-defined actions, routines, progress and reminders;
- collect questions and unresolved points;
- build a searchable timeline and export selected information.

## Important medical boundary

This application is **not** intended to diagnose, treat, prevent or cure disease.

It must not:

- create diagnoses;
- generate treatment recommendations;
- decide medication doses;
- tell users to start, stop or change medication;
- derive health actions automatically from a disease or measurement;
- claim that weather, nutrition or another context factor caused a symptom;
- treat a source rating as medical validation.

The application may document what the user entered, what a professional reportedly said, what a source states, and what remains open for clarification.

## UI target

The visual target is documented by the repository screenshots:

- [Dashboard concept](docs/screenshots/dashboard-concept.png)
- [Condition wizard concept](docs/screenshots/condition-wizard-concept.png)

The implementation plan is described in:

- [UI target and screenshot plan](docs/ui-ux/055_UI_Zielbild_und_Screenshot_Plan.md)
- [Codex implementation guide](docs/development/145_Codex_Arbeitsauftrag.md)

## Documentation Baseline 2.1

The September 2026 revision consolidates the requirements developed since the initial planning baseline.

Start here:

- [Documentation map](docs/000_Dokumentationsuebersicht.md)
- [Documentation revision 2.1](docs/changes/160_Dokumentationsrevision_2_1.md)
- [Health modules requirements baseline 2.1](docs/requirements/155_Fachmodule_Baseline_2_1.md)
- [Architecture](docs/architecture/030_Architekturkonzept.md)
- [Database model](docs/database/040_Datenmodell_Datenbankdesign.md)
- [UI/UX concept](docs/ui-ux/050_UI_UX_Konzept.md)
- [Milestone plan](docs/roadmap/115_Milestone_und_Release_Plan.md)
- [ADRs](docs/adr/130_Architekturentscheidungen_ADR.md)

## Planned health modules

The target model includes, implemented incrementally:

- health topics;
- observations and symptom/context diary;
- measurements and lab values;
- nutrition diary;
- optional immutable weather snapshots linked to measurements/observations;
- doctor visits, coaching and other sessions with preparation/follow-up;
- questions and open points;
- sourced HealthActions;
- user-defined routines and progress counters;
- local notifications/reminders;
- documents, images and media resources;
- research sources, exact source locations and evidence notes;
- contact references prepared for later integration with a shared SASD contacts application.

These are a roadmap, not a promise that every module is already implemented.

## Privacy and security principles

- local-first by default;
- no mandatory cloud connection;
- no telemetry;
- no silent upload of health data;
- no health contents in technical logs;
- explicit export control;
- archive/restore instead of silent deletion;
- backups and migration safety;
- later encrypted storage after explicit architecture decision;
- discreet notification mode for health-related reminders.

## Technical baseline

- **Language:** C#
- **Runtime:** .NET 8+
- **Desktop UI:** WinForms primary frontend; WPF buildable reference frontend
- **Current persistence:** local JSON
- **Planned persistence:** SQLite with migrations
- **Search:** SQLite FTS5 planned
- **Documents/media:** managed local file store with metadata
- **Architecture:** Domain / Application / Infrastructure / WPF / WinForms
- **Testing:** current smoke-test runner, later broader unit/integration tests

## Build and run

```powershell
dotnet restore
dotnet build Sasd.HealthNotebook.sln --configuration Release
dotnet run --project tests/Sasd.HealthNotebook.SmokeTests --configuration Release
dotnet run --project src/Sasd.HealthNotebook.Wpf
dotnet run --project src/Sasd.HealthNotebook.WinForms
```

A GitHub Actions workflow validates restore, build and smoke tests on Windows.

## Frontend status

- **WinForms** is the current primary development frontend. It provides the functional baseline: dashboard, navigation, topic list, refresh, status line, health-topic wizard and German/English localization.
- **WPF** remains buildable as a reference frontend and compatibility check for the shared Application/Infrastructure layers.
- Both frontends use the same `HealthTopicService`, `JsonHealthTopicRepository` and local JSON file, so they do not create separate data worlds.
- The WinForms baseline contains a small localization foundation for German and English. It starts in German on German Windows installations, otherwise in English, and offers a language selector in the main window.

## Codex

Repository-level instructions for Codex are in [AGENTS.md](AGENTS.md).

The preferred near-term sequence is:

1. polish the existing WinForms application shell;
2. bring the WinForms dashboard closer to the concept screenshot;
3. refine the WinForms wizard;
4. stabilize the WinForms navigation host;
5. keep WPF buildable as a reference;
6. then implement new health features as small vertical slices in WinForms first.

## License

See [LICENSE](LICENSE).
