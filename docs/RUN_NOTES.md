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

For later milestones, the storage layer can be replaced or complemented by SQLite without changing the UI dramatically, because the UI talks to the application service and not directly to JSON.

## Manual smoke test

1. Start the WPF application.
2. Verify that the dashboard opens.
3. Click **New health topic**.
4. Enter a title, status, priority and a short note.
5. Click through the wizard until the topic is created.
6. Verify that the topic appears in the list.
7. Restart the application.
8. Verify that the topic is loaded from JSON.

## Automated smoke test

Run:

```powershell
dotnet run --project .\tests\Sasd.HealthNotebook.SmokeTests\Sasd.HealthNotebook.SmokeTests.csproj
```

The smoke test creates a temporary JSON repository, adds topics, reloads the repository and checks that the data survives a reload.
