# Run Notes – Milestone 1

## Purpose

This milestone establishes a small but usable application shell for the SASD Health Research Notebook.

It deliberately focuses on local-first storage and clean architecture instead of feature breadth.

## Privacy and safety assumptions

- No cloud connection is used.
- No telemetry is used.
- Health data is stored locally as JSON.
- The application does not create diagnoses.
- The application does not recommend therapies.
- The application does not recommend medication dosage.
- Error messages are deliberately generic and should not include health data.

## Local JSON storage

The JSON repository writes all health topics to one file. This is simple and understandable for the first milestone.

WPF and WinForms both use the same `HealthTopicService`, `JsonHealthTopicRepository` and `LocalHealthNotebookPaths.HealthTopicsFilePath`. They should therefore read and write the same local JSON data instead of creating separate frontend-specific data stores.

For later milestones, the storage layer can be replaced or complemented by SQLite without changing the UI dramatically, because the UI talks to the application service and not directly to JSON.

## UI language

The WinForms baseline supports German and English UI texts.

- On German Windows installations, the first start should use German.
- On other Windows installations, the first start should use English.
- The main form contains a language selector for switching between Deutsch and English during the session.
- Later milestones can persist the selected language in an application settings module.

## Start the applications

Run the WPF reference frontend:

```powershell
dotnet run --project .\src\Sasd.HealthNotebook.Wpf\Sasd.HealthNotebook.Wpf.csproj
```

Run the WinForms baseline frontend:

```powershell
dotnet run --project .\src\Sasd.HealthNotebook.WinForms\Sasd.HealthNotebook.WinForms.csproj
```

## Manual smoke test

1. Start the WinForms application.
2. Verify that the dashboard opens without clipped dashboard cards or unnecessary card scrollbars.
3. Verify that the navigation contains **Dashboard** and **Gesundheitsthemen** when German is active.
4. Switch the language to **English** and verify that main-window labels, buttons, grid headers and status texts change.
5. Switch the language back to **Deutsch**.
6. Click **Neues Gesundheitsthema**.
7. Enter a synthetic title, status, priority and a short note.
8. Click through the wizard until the topic is created.
9. Verify that the topic appears in the list.
10. Restart the WinForms application.
11. Verify that the topic is loaded from JSON.
12. Start the WPF application.
13. Verify that the same synthetic topic is visible there too.

Do not use real health data in screenshots, issues or pull-request descriptions.

## Automated smoke test

Run:

```powershell
dotnet run --project .\tests\Sasd.HealthNotebook.SmokeTests\Sasd.HealthNotebook.SmokeTests.csproj
```

The smoke test creates a temporary JSON repository, writes synthetic data through one `HealthTopicService`, reloads it through another `HealthTopicService` and checks that the data survives a reload. This verifies the shared persistence contract below the UI layer without opening WPF or WinForms windows.
