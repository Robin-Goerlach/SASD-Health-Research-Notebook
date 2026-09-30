# 147 – WinForms UI Baseline 2

Stand: 2026-09-30. Anforderung: FR-UI-001.

## Umfang

Die bestehende Oberfläche erhält einen zweizeiligen Header mit ausreichend Platz
für deutsche und englische Aktionen, gleichmäßig verteilte Dashboardkarten und
eine ruhigere Statuszeile. Die Startgröße bleibt 1280 × 820, die Mindestgröße
1120 × 740. Navigation zeigt Auswahl, Hover und Fokus getrennt; Pfeiltasten sowie
Home/End bewegen den Fokus, Enter/Leertaste aktivieren die Seite.

Die Themenliste verwendet proportionale Spalten, Textumbruch und einen eigenen
Leerzustand. Refresh erhält die Auswahl anhand der Topic-ID. Dashboard und Liste
werden beim Navigieren wiederverwendet und beim Schließen freigegeben.

Der Wizard bleibt funktional unverändert: Grunddatenfelder nutzen den verfügbaren
Platz, Schritte umbrechen, die Buttonzeile hat ausreichend Höhe. Tab-Reihenfolge,
Enter/Escape, vorhandene Tooltips und zugängliche Feldbeschreibungen unterstützen
die Bedienung. Mindestgröße: 960 × 660. DPI-Skalierung ist explizit aktiviert.

Keine neuen Fachmodule, Pakete, Persistenzformate oder WPF-Designänderungen.
Fachlogik und JSON-Persistenz bleiben in den gemeinsamen Schichten.

## Prüfungen

Der sichere Launcher führt zusätzlich das dependency-light Projekt
`tests/Sasd.HealthNotebook.WinForms.SmokeTests` aus. Es referenziert das normale
WinForms-Projekt; produktive Quelldateien werden nicht in Tests kopiert.
Ein echter STA-Message-Loop prüft Forms und Controls außerhalb des sichtbaren
Bildschirms. Reflection auf private Controls vermeidet zusätzliche Produkt-APIs;
Control-Umbenennungen erfordern entsprechende Testanpassungen.

- UI-LAYOUT-001: Start-/Mindestgröße, Karten-/Buttongrenzen, Textpassung,
  Leerzustand, Spaltenbreiten und stabile Auswahl nach Refresh, Deutsch/English.
- UI-WIZARD-001: Editorgrenzen, Tab-Reihenfolge, Enter/Escape, synthetisches
  Thema über den Wizard speichern, Repository und MainForm neu erstellen und laden.
- Navigation und Sprachwechsel werden an echten Controls geprüft.

Release-Build einschließlich WPF: erfolgreich, 0 Warnungen und 0 Fehler.
Gemeinsame und WinForms-Smoke-Tests: erfolgreich. Gerenderte Dashboard-, Listen-
und Wizardansichten wurden visuell geprüft. Daten und PNG-Artefakte liegen nur in
frischen GUID-Unterordnern von `.codex/synthetic-development-data/ui-tests`.
Die Tests verlangen einen expliziten Override unter der Repository-`.codex` und
prüfen Link-Vorfahren vor Schreibzugriffen; persönliche Daten werden nicht geladen.

## Noch offene manuelle Desktopprüfung

Ein separater WinForms-Start über den sicheren Launcher wurde ausgeführt; die
Desktopverbindung war nicht verfügbar und lieferte kein bedienbares Fenster.
Das ist kein bestandener manueller UI-Test. Automatisierte Control-Prüfungen und
Rendervergleich ersetzen nicht die abschließende Bedienprüfung am Benutzerdesktop.
Insbesondere Tooltip-Anzeige, echte Tab-/Tastatureingaben und Skalierung bei
125/150 Prozent müssen dort noch geprüft werden.

Vom Repository aus nach erfolgreicher Validierung starten:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

Dashboard, Sprachwechsel und Verkleinern prüfen; ausschließlich ein eindeutig
synthetisches Thema anlegen, normal schließen und mit demselben Befehl wieder
starten. Datenpfad: `.codex/synthetic-development-data/health-topics.json`.

Die vorhandene untracked Benutzerdatei `CreateHealthTopicWizardForm.resx` wurde
nicht geändert und gehört nicht zum Commit. `.codex/` bleibt ignoriert.
