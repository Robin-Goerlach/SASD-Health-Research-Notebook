# 010 - GitHub Repository Setup

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Repository-Setup-Checkliste  
Status: Entwurf  

## 1. Zweck

Dieses Dokument beschreibt die empfohlene Erststruktur für das GitHub-Repository. Es soll sicherstellen, dass Dokumentation, Screenshots, technische Grundlagen, lokale Daten und spätere Implementierung sauber getrennt sind.

## 2. Empfohlenes Repository

Empfohlener Name:

```text
SASD-Health-Research-Notebook
```

Empfohlene Beschreibung:

```text
Local-first desktop application for documenting personal health topics, symptoms, documents, sources, lab values and doctor questions. Built as a privacy-conscious research notebook for structured self-organization, not diagnosis or therapy.
```

## 3. Empfohlene Sichtbarkeit

Für die frühe Phase gibt es zwei sinnvolle Wege:

| Option | Bewertung |
|---|---|
| Öffentliches Repository | gut für Portfolio, aber vorsichtig mit Beispiel- und Testdaten |
| Privates Repository | sicherer während früher Entwicklung, später veröffentlichbar |

Wenn das Repository öffentlich wird, dürfen niemals echte Gesundheitsdaten, echte Arztberichte, echte Labordokumente oder personenbezogene Testdaten eingecheckt werden.

## 4. Initiale Ordnerstruktur

```text
SASD-Health-Research-Notebook/
├── README.md
├── LICENSE
├── .gitignore
├── docs/
│   ├── 000_Dokumentationsuebersicht.md
│   ├── architecture/
│   ├── database/
│   ├── ui-ux/
│   ├── security/
│   ├── knowledge/
│   ├── export/
│   ├── testing/
│   ├── roadmap/
│   ├── risks/
│   ├── adr/
│   ├── development/
│   └── screenshots/
├── db/
│   └── schema_v0_1_draft.sql
├── src/
│   ├── Sasd.HealthResearchNotebook.App/
│   ├── Sasd.HealthResearchNotebook.Application/
│   ├── Sasd.HealthResearchNotebook.Domain/
│   └── Sasd.HealthResearchNotebook.Infrastructure/
├── tests/
│   ├── Sasd.HealthResearchNotebook.Domain.Tests/
│   └── Sasd.HealthResearchNotebook.Application.Tests/
└── tools/
```

## 5. Was nicht ins Repository gehört

Folgende Inhalte dürfen nicht eingecheckt werden:

- echte Gesundheitsdaten;
- echte Arztbriefe;
- echte Laborberichte;
- echte Medikationspläne;
- lokale Datenbanken;
- lokale Backup-Dateien;
- lokale Logs;
- personenbezogene Screenshots;
- Secrets, Passwörter, Tokens;
- Build-Ausgaben.

## 6. `.gitignore`-Hinweise

Die `.gitignore` sollte mindestens abdecken:

```text
bin/
obj/
.vs/
*.user
*.suo
*.db
*.sqlite
*.sqlite3
*.log
*.bak
*.tmp
local-data/
exports/
backups/
```

Für echte Anwendungsexporte sollte ein separater lokaler Ordner außerhalb des Repositories verwendet werden.

## 7. Branching-Strategie

Für den Anfang reicht ein sehr einfacher Workflow:

- `main`: lauffähiger Stand;
- Feature-Branches nur bei größeren Änderungen;
- kleine Commits pro Milestone;
- jede Phase endet mit Build/Test und Dokumentationsupdate.

Beispiele:

```text
docs: add architecture and database concepts
chore: initialize solution structure
feat: add application shell
feat: add health topic wizard v1
fix: prevent health data from being logged
```

## 8. README-Mindestinhalt

Das README sollte enthalten:

1. Projektname;
2. kurze Beschreibung;
3. Nicht-Ziele: keine Diagnose, keine Therapie, keine Dosierung;
4. aktueller Status;
5. Screenshots als Konzeptbilder;
6. geplante Features;
7. Datenschutz-Hinweis;
8. Build-/Run-Hinweise;
9. Roadmap-Link;
10. Lizenz.

## 9. Beispielhafte Sicherheitswarnung im README

```text
This application is a personal documentation and research notebook. It is not a diagnostic system, not a therapy recommendation system and not a replacement for medical advice. Do not store real health data in public repositories or issue trackers.
```

## 10. GitHub Issues und echte Gesundheitsdaten

Bei einem öffentlichen Repository sollten Issues keine echten Gesundheitsdaten enthalten. Bugreports sollten mit anonymisierten Beispieldaten arbeiten.

Empfohlene Issue-Vorlage:

- Was wurde versucht?
- Was wurde erwartet?
- Was ist passiert?
- App-Version;
- Betriebssystem;
- anonymisierte Beispieldaten;
- keine echten Gesundheitsdokumente anhängen.

## 11. Erste Schritte nach Repository-Anlage

1. Repository erstellen.
2. Starterpaket entpacken.
3. README prüfen.
4. Screenshots prüfen.
5. Dokumentationspaket unter `docs/` übernehmen.
6. Initial commit erstellen.
7. GitHub-Seite prüfen.
8. Erst danach mit Milestone 1 Code beginnen.

## 12. Akzeptanzkriterien

Das Repository ist bereit, wenn:

- README sichtbar und verständlich ist;
- Screenshots korrekt angezeigt werden;
- Lizenz enthalten ist;
- Dokumente im Repository liegen;
- keine echten Gesundheitsdaten enthalten sind;
- lokale Daten durch `.gitignore` ausgeschlossen sind;
- die nächsten Milestones im Roadmap-Dokument erkennbar sind.
