# 145 - Codex-Arbeitsauftrag

Projekt: SASD Health Research Notebook  
Stand: 2026-09-30  
Dokumenttyp: Entwickler-/Codex-Leitfaden  
Status: aktiv

## 1. Zweck

Dieses Dokument soll Codex oder einen separaten Entwicklungs-/Debug-Chat schnell und reproduzierbar in den aktuellen Stand bringen.

Das unmittelbare Ziel ist nicht "alle Features bauen", sondern ein **vorzeigbares, weiterhin funktionierendes WPF-Produkt**, dessen Dashboard und Wizard dem Konzept im Repository entsprechen.

## 2. Repository zuerst verstehen

Vor der ersten Änderung:

1. aktuellen Branch und offene PRs prüfen;
2. `AGENTS.md` lesen;
3. README und Baseline-2.1-Dokumente lesen;
4. Solution und Projektabhängigkeiten prüfen;
5. `MainWindow.xaml`, `MainWindowViewModel.cs`, `App.xaml` lesen;
6. Wizard-XAML und Code-behind lesen;
7. `HealthTopic`, `HealthTopicService`, Repository-Interface und JSON-Persistenz lesen;
8. Smoke Tests lesen;
9. Build ausführen.

Keine Architektur aus Chat-Erinnerung erraten, wenn der Repository-Stand etwas anderes zeigt.

## 3. Erster Codex-Auftrag: UI Foundation

### Ziel

Die laufende Anwendung soll dem Dashboard-Konzeptbild sichtbar näherkommen.

### Aufgaben

- bestehende Farben/Styles aus `App.xaml` in eine wartbare Design-System-Struktur entwickeln;
- wiederverwendbare WPF-Styles für Navigation, Cards und Buttons einführen;
- Sidebar als echte Navigation darstellen;
- Dashboard-Spacing, Typografie und Card-Hierarchie verbessern;
- HealthTopic-Liste visuell aufwerten;
- Empty State hinzufügen;
- bestehende Load-/Refresh-/Create-Funktion beibehalten;
- keine neuen fachlichen Module implementieren.

### Guardrails

- keine Gesundheitsdaten in Logs;
- keine externe UI-Bibliothek ohne klare Begründung;
- keine statischen Fake-Screens statt funktionierender Controls;
- kein großer MVVM-Framework-Wechsel im ersten Sprint;
- keine SQLite-Migration als Nebenarbeit;
- Kommentare/XML-Dokumentation erhalten oder verbessern.

### Prüfung

```powershell
dotnet restore
dotnet build Sasd.HealthNotebook.sln --configuration Release
dotnet run --project tests/Sasd.HealthNotebook.SmokeTests --configuration Release
dotnet run --project src/Sasd.HealthNotebook.Wpf
```

Manuell vergleichen mit `docs/screenshots/dashboard-concept.png`.

## 4. Zweiter Codex-Auftrag: Wizard Visual Pass

Nach Abschluss von UI Foundation:

- Wizard auf gemeinsame Design Tokens umstellen;
- klarer Schrittstatus;
- einheitliche Buttons/Formfelder;
- Zusammenfassungsschritt verbessern;
- Abbruch/Validierung prüfen;
- keine fachliche Erweiterung erzwingen.

## 5. Dritter Codex-Auftrag: Navigation Host

Danach:

- MainWindow von fest verdrahteter Seite zu einem sauberen Navigation Host entwickeln;
- Dashboard als erste View;
- Health Topics als zweite View oder Detailbereich;
- Interfaces/ViewModels so klein halten, dass kommende Seiten ergänzt werden können;
- kein Service-Locator in ViewModels;
- keine Businesslogik im Code-behind.

## 6. Danach: vertikale Fach-Slices

Reihenfolge der bevorzugten Slices:

1. HealthEntry/Timeline
2. Sources + SourceLocation + EvidenceNote
3. Measurement
4. Session + Questions + Follow-up
5. HealthAction + Routine + Progress
6. Reminder/Notification
7. Nutrition + Context
8. MediaResource
9. ContactReference
10. WeatherSnapshot adapter

Jeder Slice soll möglichst enthalten:

- Domain;
- Application contract/service;
- persistence;
- WPF UI;
- tests;
- migration/compatibility notes;
- docs update.

## 7. Prompt-Vorlage für Codex

```text
Arbeite im Repository Robin-Goerlach/SASD-Health-Research-Notebook.

Lies zuerst AGENTS.md und die dort genannten Dokumente. Prüfe den aktuellen Branch,
offene PRs, Solution, Tests und den tatsächlichen Code. Das Repository ist die
maßgebliche Quelle.

Ziel dieses Auftrags:
<genau ein klar abgegrenztes Ziel einsetzen>

Halte die medizinische Sicherheitsgrenze strikt ein: Dokumentation und
Selbstorganisation ja; Diagnose, Therapieempfehlung, Dosisentscheidung oder
automatisch aus Erkrankungen abgeleitete Handlungsanweisung nein.

Arbeite in kleinen, nachvollziehbaren Änderungen. Erhalte bestehende Funktionen.
Kommentiere öffentliche/komplexe C#-Teile ausführlich mit XML- und sinnvollen
//-Kommentaren. Keine echten Gesundheitsdaten in Testdaten oder Logs.

Vor Abschluss:
- dotnet restore
- dotnet build Sasd.HealthNotebook.sln --configuration Release
- Smoke Tests ausführen
- relevante UI manuell starten/pruefen
- Diff auf unbeabsichtigte Änderungen prüfen
- README/Dokumentation aktualisieren, wenn Verhalten geändert wurde

Für UI-Aufgaben ist docs/screenshots/dashboard-concept.png bzw.
condition-wizard-concept.png das visuelle Zielbild.
```

## 8. Pull-Request-Erwartung

Ein Codex-PR soll enthalten:

- kurze Problem-/Zielbeschreibung;
- geänderte Bereiche;
- Testnachweise;
- bei UI: Beschreibung des visuellen Fortschritts;
- verbleibende Abweichungen;
- keine Behauptung "fertig", wenn nur ein Teil des Screenshot-Ziels erreicht ist.

## 9. Anti-Scope-Creep-Regel

Wenn Codex beim UI-Sprint feststellt, dass ein späteres Feature "auch gleich" implementiert werden könnte, wird es dokumentiert, aber nicht automatisch mitgebaut.

Das Ziel ist schnelle sichtbare Qualität **plus** stabile Architektur, nicht maximale Featurezahl pro PR.
