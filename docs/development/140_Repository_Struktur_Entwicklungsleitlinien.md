# 140 - Repository-Struktur und Entwicklungsleitlinien

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Entwicklungsleitlinien  
Status: Entwurf  

## 1. Ziel

Dieses Dokument beschreibt, wie das GitHub-Repository aufgebaut werden soll und welche Entwicklungsregeln für das Projekt gelten. Es orientiert sich an den SASD Engineering Standards: kleine robuste Schritte, gute Dokumentation, klare Architektur, Tests und Sicherheitsbewusstsein.

## 2. Empfohlener Repository-Name

```text
SASD-Health-Research-Notebook
```

## 3. Empfohlene GitHub Description

```text
Local-first desktop application for documenting personal health topics, symptoms, documents, sources, lab values and doctor questions. Built as a privacy-conscious research notebook for structured self-organization, not diagnosis or therapy.
```

## 4. Empfohlene Repository-Struktur

```text
SASD-Health-Research-Notebook/
  README.md
  LICENSE
  .gitignore
  docs/
    000_Dokumentationsuebersicht.md
    requirements/
      010_Lastenheft.md
      020_Pflichtenheft.md
      150_Feature_Backlog_und_Anforderungskatalog.md
    architecture/
      030_Architekturkonzept.md
    database/
      040_Datenmodell_Datenbankdesign.md
    ui-ux/
      050_UI_UX_Konzept.md
    security/
      060_Sicherheits_Datenschutzkonzept.md
    knowledge/
      070_Dokumenten_Quellenkonzept.md
      080_Such_Wissenskonzept.md
    export/
      090_Import_Exportkonzept.md
    testing/
      100_Testkonzept.md
    roadmap/
      110_Roadmap.md
    risks/
      120_Risikoanalyse.md
    adr/
      130_Architekturentscheidungen_ADR.md
    development/
      140_Repository_Struktur_Entwicklungsleitlinien.md
    screenshots/
      dashboard-concept.png
      condition-wizard-concept.png
  db/
    schema_v0_1_draft.sql
  src/
    Sasd.HealthResearchNotebook.App/
    Sasd.HealthResearchNotebook.Domain/
    Sasd.HealthResearchNotebook.Application/
    Sasd.HealthResearchNotebook.Infrastructure/
    Sasd.HealthResearchNotebook.ImportExport/
    Sasd.HealthResearchNotebook.Security/
  tests/
    Sasd.HealthResearchNotebook.Domain.Tests/
    Sasd.HealthResearchNotebook.Application.Tests/
    Sasd.HealthResearchNotebook.Infrastructure.Tests/
```

## 5. Gitignore-Regeln

Nicht ins Repository:

```text
bin/
obj/
.vs/
*.user
*.suo
*.db
*.db-shm
*.db-wal
*.sqlite
*.bak
*.backup
logs/
data/
exports/
backups/
*.log
*.tmp
```

Besonders wichtig: keine echten Gesundheitsdaten, keine privaten Dokumente, keine echten Arztbriefe, keine realen Laborwerte.

## 6. Branching

Für den Anfang genügt ein einfacher Workflow:

- `main`: stabiler dokumentierter Stand
- Feature-Branches optional: `feature/phase-01-application-shell`

Keine komplizierte GitFlow-Struktur nötig.

## 7. Commit-Konvention

Empfohlene Commit-Typen:

```text
feat: neue Funktion
fix: Fehlerbehebung
docs: Dokumentation
test: Tests
refactor: interne Umstrukturierung ohne Funktionsänderung
chore: Projektpflege
security: Sicherheitsrelevante Änderung
```

Beispiele:

```text
docs: add architecture and database planning documents
feat: add condition domain model
feat: add condition wizard draft persistence
fix: prevent sensitive data in document import logs
test: add backup restore integration tests
```

## 8. Coding-Leitlinien

- kleine Klassen
- klare Verantwortlichkeiten
- keine Geschäftslogik in UI-Code
- Domain-Logik testbar halten
- Repositories über Interfaces kapseln
- Fehler verständlich behandeln
- XML-Dokumentationskommentare für zentrale Klassen
- keine sensiblen Daten loggen
- keine echten Gesundheitsdaten in Beispielen

## 9. Schichtregeln

| Schicht | Darf kennen | Darf nicht kennen |
|---|---|---|
| Domain | eigene Entitäten/Value Objects | UI, SQLite, Dateisystem |
| Application | Domain, Interfaces | WinForms-/WPF-Controls, konkrete DB-Details |
| Infrastructure | SQLite, Dateien | UI |
| App/UI | ViewModels, Application Services | SQL, Dateisystemdetails |
| Security | Crypto/Schutzservices | UI-Layout |
| ImportExport | Exportmodelle, Dateien | direkte UI-Abhängigkeiten |

## 10. Namenskonventionen

| Element | Konvention | Beispiel |
|---|---|---|
| Solution | PascalCase | `Sasd.HealthResearchNotebook.sln` |
| Projekte | PascalCase | `Sasd.HealthResearchNotebook.Domain` |
| Klassen | PascalCase | `ConditionService` |
| Interfaces | I-prefix | `IConditionRepository` |
| Tests | Verhalten benennen | `CreateCondition_WithValidTitle_SavesCondition` |
| DB-Tabellen | snake_case | `health_entries` |
| Dokumente | nummeriert | `030_Architekturkonzept.md` |

## 11. Dokumentationsregeln

- Jede Phase erhält ein kurzes Phasendokument.
- Architekturentscheidungen als ADR dokumentieren.
- README nur aktuell und nicht überladen halten.
- Lastenheft/Pflichtenheft nicht ständig umschreiben, sondern versionieren.
- Roadmap bei Scope-Änderungen aktualisieren.
- Sicherheitsentscheidungen nicht nur im Chat lassen.

## 12. Phasenabschluss

Am Ende jeder Phase:

1. `dotnet clean`
2. `dotnet restore`
3. `dotnet build`
4. `dotnet test`
5. App kurz starten
6. README/Docs prüfen
7. Commit erstellen
8. Push
9. Phase im Dokument vermerken

## 13. Beispiel für Phase-Log

```markdown
# Phase 1 - Application Shell

Status: abgeschlossen
Datum: YYYY-MM-DD
Commit: <hash>

## Inhalt
- Solution erstellt
- primäres WinForms-Frontend startet
- Logging-Grundlage
- Testprojekte

## Prüfung
- dotnet build: erfolgreich
- dotnet test: erfolgreich
- manueller Start: erfolgreich

## Offene Punkte
- Dashboard nur Platzhalter
```

## 14. Sicherheitsregeln für Entwickler

- niemals echte Gesundheitsdaten committen
- Screenshots nur mit Musterdaten
- Logs vor Commit prüfen
- Export-/Backup-Dateien nie committen
- lokale Testdaten künstlich erzeugen
- sensible Pfade nicht dokumentieren

## 15. Erste technische Arbeit nach Repository-Anlage

Empfohlene Reihenfolge:

1. Solution erstellen
2. Projekte anlegen
3. Testprojekte anlegen
4. zentrale Ordnerstruktur einrichten
5. Basis-README aktualisieren
6. Logging-Grundlage ohne sensible Daten
7. Konfigurationsservice
8. Datenordner-Service
9. erste Domain-Entität `Condition`
10. erster Unit-Test


---

## 16. Codex-/Agenten-Workflow Baseline 2.1 (2026-09-30)

Für Codex und andere Coding-Agenten ist `/AGENTS.md` verbindlicher Einstieg.

Zusätzlich gilt:

- zuerst tatsächlichen Repository-Stand prüfen;
- keine älteren Chatannahmen gegen neueren Code durchsetzen;
- einen klaren Scope pro PR;
- UI-Sprints dürfen keine versteckten Domain-/Persistenzumbauten enthalten;
- neue Fachmodule als vertikale Slices;
- Build und Smoke Tests vor Abschluss;
- bei UI-Arbeit Konzept-Screenshots als Referenz verwenden;
- keine realen Gesundheitsdaten als Beispiel/Testdata übernehmen.

Konkreter Ablauf: `docs/development/145_Codex_Arbeitsauftrag.md`.


---

## 17. Frontend-Entwicklungsregel 2.1a (2026-09-30)

Die Solution enthält aktuell WPF und WinForms.

Für neue Arbeiten gilt:

- WinForms ist das primäre Frontend;
- WPF bleibt buildbar und wird nicht gelöscht;
- Domain/Application/Infrastructure dürfen keine UI-Abhängigkeiten erhalten;
- neue Fachfeatures werden nicht standardmäßig in beiden UIs doppelt implementiert;
- WinForms nutzt die vorhandenen Presenter/Views/Controls sowie zentrale Styling- und Localization-Klassen;
- bei Änderungen an gemeinsamen Verträgen muss geprüft werden, ob WPF weiterhin baut;
- beide Frontends müssen während der JSON-Phase denselben lokalen Datenbestand lesen können.

Der konkrete Agent-/Codex-Workflow steht in `AGENTS.md` und `145_Codex_Arbeitsauftrag.md`.
