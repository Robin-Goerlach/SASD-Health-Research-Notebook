# 145 - Codex-Arbeitsauftrag

Projekt: SASD Health Research Notebook  
Stand: 2026-10-01
Dokumenttyp: Entwickler-/Codex-Leitfaden  
Status: aktiv – WinForms primär

## 1. Zweck

Dieses Dokument bringt Codex oder einen separaten Entwicklungs-/Debug-Chat schnell und reproduzierbar auf den tatsächlichen Repository-Stand.

Das unmittelbare Ziel ist nicht "alle Features bauen". Das Ziel ist ein **vorzeigbares, funktionierendes WinForms-Produkt**, das auf den bestehenden Domain/Application/Infrastructure-Schichten aufbaut und sich visuell an den Konzept-Screenshots orientiert.

WPF bleibt im Repository als buildbare Referenzoberfläche und Kompatibilitätscheck, ist aber derzeit nicht das primäre Frontend für neue Features.

## 2. Aktueller Repository-Stand

Bereits vorhanden:

- Domain / Application / Infrastructure;
- lokale JSON-Persistenz über `JsonHealthTopicRepository`;
- WPF-Referenzfrontend;
- WinForms-Frontend;
- gemeinsamer `HealthTopicService`;
- WinForms-Dashboard und HealthTopic-Grid;
- Navigation;
- Statusbereich;
- WinForms-Create-HealthTopic-Wizard;
- zentrale WinForms-Stylingklassen;
- Presenter;
- Deutsch/Englisch-Lokalisierung;
- Smoke Tests;
- Windows-GitHub-Actions-CI.

WPF und WinForms nutzen denselben lokalen JSON-Datenbestand.

## 3. Repository zuerst verstehen

Vor jeder größeren Änderung:

1. aktuellen `main` und offene PRs/Issues prüfen;
2. `AGENTS.md` lesen;
3. README und Baseline-2.1-Dokumente lesen;
4. Solution und Projektabhängigkeiten prüfen;
5. WinForms `MainForm`, Views, Controls, Presenter, Styling und Localization lesen;
6. WinForms-Wizard lesen;
7. WPF nur als Referenz und für Shared-Layer-Kompatibilität prüfen;
8. `HealthTopic`, `HealthTopicService`, Repository-Interface und JSON-Persistenz lesen;
9. Smoke Tests lesen;
10. Build ausführen.

Keine Architektur aus älteren Chats erraten, wenn der Repository-Stand etwas anderes zeigt.

## 4. Aktueller Codex-Auftrag: Measurement / Vitalwerte Slice 1

WinForms UI Baseline 2 ist abgeschlossen: Release-/Smoke-Prüfungen erfolgreich,
manuelle Nutzerakzeptanz bestätigt, PR #11 gemergt. Details stehen in Dokument 147.

Der abgeschlossene HealthEntry-Slice setzt FR-OBS-001 um: allgemeine Notiz, Beobachtung oder
Recherchenotiz mit Datum/Uhrzeit, Titel, Text und optional einem vorhandenen Thema.
Domain/Application bleiben UI-unabhängig. Der separate JSON-Store
`health-entries.json` liegt neben dem unveränderten `health-topics.json`.
HealthEntry wird direkt chronologisch angezeigt; kein dupliziertes TimelineEvent.

HealthEntry / Timeline Slice 1 ist abgeschlossen (PR #12 gemergt):
Am 2026-10-01 bestätigte der Nutzer Anlage mit/ohne Thema, sofortige Anzeige,
Wiederladen nach Neustart, Themenzuordnung, Deutsch/English, Tooltips, Tab-Reihenfolge
und Layout. Der Slice ist auf `main` enthalten.
Prüfumfang, Sicherheitsentscheidungen und Akzeptanzschritte: Dokument 148.
FR-OBS-002 bis FR-OBS-005, SQLite und neue Spezialmodule bleiben außerhalb des Scopes.

Alle Build-/Testläufe verwenden ausschließlich den sicheren Launcher:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

Sources + SourceLocation + EvidenceNote Slice 1 ist abgeschlossen (PR #13 gemergt);
manuelle Nutzerakzeptanz ist bestätigt und der Slice ist auf `main` enthalten.
FR-SRC-001/002/004/005/006 werden durch getrennte Entitäten,
gemeinsame Application-Verträge und einen atomar aktualisierten `sources.json`-Store
umgesetzt. Originalzitat und eigene Einordnung bleiben getrennt. Details und
Prüfliste: Dokument 149. Keine Vertrauensbewertung, kein Import und keine SQLite-Migration.

Measurement / Vitalwerte Slice 1 ist der aktuelle implementierte Slice auf
`feat/measurements-vitals-slice-1`; die manuelle Nutzerakzeptanz ist bestätigt (2026-10-01). Der Sources-Regressionsfix aus PR #15 ist integriert.
Blutdruck wird mit getrennten systolischen/diastolischen Zahlen und optionalem Puls
dokumentiert. Puls, Körpertemperatur, Blutzucker und Körpergewicht verwenden
typgebundene Einheiten ohne Umrechnung. Persönliche Notiz und Messsituation bleiben
getrennt von Zahlen. Ein separater `measurements.json`-Store lässt die drei bestehenden
Stores unverändert. Keine medizinische Bewertung und keine duplizierte Timeline-Persistenz.
FR-MEA-001/002/004/005/007 sowie der Themenbezug aus FR-MEA-006 werden abgedeckt;
weitere Verknüpfungen bleiben offen. Details und manuelle Prüfliste: Dokument 150.

WPF bleibt buildbar und erhält weder Sources- noch Measurement-Oberfläche.
Nächster geplanter Slice: **Session + Questions + Follow-up**.

## 5. Frontend-Regel

Neue Fachfeatures werden derzeit zuerst im WinForms-Frontend umgesetzt.

Nicht automatisch tun:

- dasselbe neue Feature parallel in WPF implementieren;
- WPF löschen;
- gemeinsame Fachlogik in das WinForms-Projekt verschieben.

Wenn eine WinForms-Funktion fachliche Logik benötigt, gehört diese in Domain/Application/Infrastructure und wird aus WinForms aufgerufen.

## 6. Danach: vertikale Fach-Slices

Bevorzugte Reihenfolge nach der UI-Baseline:

1. HealthEntry / Timeline
2. Sources + SourceLocation + EvidenceNote
3. Measurement / Vitalwerte
4. Session + Questions + Follow-up
5. HealthAction + Routine + Progress
6. Reminder / Notification
7. Nutrition + Context
8. MediaResource
9. ContactReference
10. WeatherSnapshot + Weather Adapter

Jeder Slice soll möglichst enthalten:

- Domain;
- Application contract/service;
- persistence;
- WinForms UI;
- tests;
- migration/compatibility notes;
- documentation update.

WPF wird nur angepasst, wenn eine gemeinsame Vertragsänderung sonst den Build oder die bestehende Referenzfunktion bricht.

## 7. Gemeinsame Persistenz als Architekturtest

Solange JSON aktiv ist, muss gelten:

```text
WinForms erstellt HealthTopic
        ↓
gemeinsamer JSON-Datenbestand
        ↓
WPF kann denselben HealthTopic lesen
```

und umgekehrt.

Ein Frontend darf keinen eigenen HealthTopic-Typ, kein eigenes Repository und keinen separaten Datenpfad einführen.

## 8. Prompt-Vorlage für Codex

```text
Arbeite im Repository Robin-Goerlach/SASD-Health-Research-Notebook.

Lies zuerst AGENTS.md und die dort genannten Dokumente. Prüfe aktuellen main,
offene PRs/Issues, Solution, Tests und den tatsächlichen Code. Das Repository ist
die maßgebliche Quelle.

WinForms ist derzeit das primäre Entwicklungsfrontend. WPF bleibt buildbare
Referenz und wird nicht parallel mit denselben neuen Features ausgebaut.

Ziel dieses Auftrags:
<genau ein klar abgegrenztes Ziel einsetzen>

Halte die medizinische Sicherheitsgrenze strikt ein: Dokumentation und
Selbstorganisation ja; Diagnose, Therapieempfehlung, Dosisentscheidung oder
automatisch aus Erkrankungen abgeleitete Handlungsanweisung nein.

Arbeite in kleinen, nachvollziehbaren Änderungen. Fachlogik gehört nicht in
Forms. Verwende gemeinsame Domain/Application/Infrastructure-Schichten.
Erhalte die gemeinsame Persistenz. Kommentiere öffentliche/komplexe C#-Teile
mit XML- und sinnvollen //-Kommentaren. Keine echten Gesundheitsdaten in
Testdaten, Screenshots oder Logs.

Vor Abschluss:
- dotnet restore
- dotnet build Sasd.HealthNotebook.sln --configuration Release
- Smoke Tests ausführen
- WinForms manuell starten/pruefen
- bei Shared-Layer-Aenderungen WPF ebenfalls pruefen
- Diff auf unbeabsichtigte Änderungen prüfen
- README/Dokumentation aktualisieren, wenn Verhalten geändert wurde

Für UI-Aufgaben sind docs/screenshots/dashboard-concept.png und
condition-wizard-concept.png die visuellen Zielbilder.
```

## 9. Pull-Request-Erwartung

Ein Codex-PR soll enthalten:

- kurze Problem-/Zielbeschreibung;
- geänderte Bereiche;
- Testnachweise;
- bei UI: Beschreibung des visuellen Fortschritts;
- bei Shared-Layer-Änderungen: WPF-Kompatibilitätsstatus;
- verbleibende Abweichungen;
- keine Behauptung "fertig", wenn nur ein Teil des Zielbilds erreicht ist.

## 10. Anti-Scope-Creep-Regel

Wenn Codex bei einem UI-Sprint feststellt, dass ein späteres Fachfeature "auch gleich" implementiert werden könnte, wird es dokumentiert, aber nicht automatisch mitgebaut.

Das Ziel ist schnelle sichtbare Qualität **plus** stabile Architektur, nicht maximale Featurezahl pro PR.
