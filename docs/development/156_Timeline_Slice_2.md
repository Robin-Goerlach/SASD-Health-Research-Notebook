# Timeline 2 – gemeinsame Chronik

Stand: 2026-10-06. Grundlage: main c793248 (PR #19), frisch gegen origin/main geprüft.

## Umfang und Akzeptanz

PF-TIM-001/003/005: WinForms zeigt alle HealthEntries und Measurements in einer
gemeinsamen Chronik, neueste fachliche Zeitpunkte zuerst. UTC-Offsets werden als
absolute Zeitpunkte verglichen; Gleichstände: CreatedAt absteigend, Quellart, Id.
Notiz/Beobachtung/Recherche bleiben unterscheidbar; Messwerte zeigen Art, Zahlen,
feste Einheit und persönliche Notiz. Keine medizinische Bewertung oder Kausalität.

Die Application-Projektion liest bestehende Services. Keine neue Persistenz,
Migration, HealthEntry-Kopie oder TimelineEvent-Kopie. Ein Ladefehler eines Stores
bricht die gesamte Aktualisierung ab; keine scheinbar vollständige Teilchronik.
Die beiden Stores sind getrennte Snapshots, keine transaktionale Momentaufnahme.

Bearbeiten und Löschen gehen an den bestehenden Use Case des ausgewählten
Quellrecords. Bei Messwerten wird der angezeigte ModifiedAt-Token weitergereicht;
Löschen verlangt die vorhandene Bestätigung. Identität ist Quellart plus Id,
auch bei gleicher Guid in beiden Stores. Sortierung, Refresh und Sprachwechsel
erhalten die Auswahl. Die Hauptaktion bleibt Neuer Verlaufseintrag.
WPF erhält keine neue Oberfläche; gemeinsame Verträge bleiben kompatibel.

PF-TIM-002 (Themen-/Typ-/Zeitraumfilter), Sessions, Actions, Routinen, Quellen,
Export und eine eigenständige Detailansicht bleiben Folgearbeit.
Die älteren Dokumente 145/148/150 beschreiben historische Slices; dieser Auftrag
und der aktuelle main bestimmen den Umfang. WinForms bleibt gemäß AGENTS/ADR primär.

## Nachweise

- IT-TIM-002: echte isolierte JSON-Stores, Offset-Reihenfolge, identische Quell-IDs,
  Reload, unveränderte Dateibytes und propagierter beschädigter Measurement-Store.
- UI-TIM-002: gemischte echte WinForms-Controls in DE/EN, Auswahl bei identischen IDs,
  Sortierung/Sprachaktualisierung, ModifiedAt und Blutdruckeinheit; Renderbilder.
  Reale Shell-Dialogtests bearbeiten und löschen Messwerte direkt aus der Chronik,
  inklusive Cancel/Confirm, Refresh, Quellschutz und Neustart in beiden Sprachen.
- Bestehende Timeline-/Grid-/Lifecycle-Tests bleiben Regressionen.

## Manuelle Abnahme (erfolgreich, 2026-10-06)

Der Nutzer hat die manuelle Abnahme von Timeline 2 am 2026-10-06 erfolgreich
bestätigt. Die Bestätigung bezieht sich auf die folgende Slice-Prüfliste.
Sie erweitert den Scope nicht auf offene Filter oder weitere Module.
Die vorgesehenen Prüfungen verwenden ausschließlich sichere synthetische Daten
über Invoke-SafeDevelopment:

1. Notiz und Messwert mit unterschiedlichen Uhrzeiten anlegen; Verlauf enthält beide.
2. Messwertzeile bearbeiten; nur der Messwert ändert sich. Zurückkehr/Refresh prüfen.
3. Löschen abbrechen, danach explizit bestätigen; nur das ausgewählte Quellrecord entfällt.
4. Blutdruck mit optionalem Puls: mmHg an Druckwerten, /min am Puls.
5. DE/EN, Tastatur, 1120×740, Sortierung und Auswahl nach Refresh prüfen.
6. Neustart: Chronik und bestehende Themen laden; WPF lädt die Themen weiterhin.

Automatisierte Renderbilder ersetzen keine manuelle Nutzerabnahme.

## Abschlussprüfung 2026-10-06

Sicherer Launcher erfolgreich: Restore, vollständiger Release-Build (0 Warnungen,
0 Fehler), Backend- und WinForms-Smoke-Tests inkl. neuer Timeline-Prüfungen.
Gemischte Renderbilder DE/EN visuell geprüft; Blutdruck und Puls mit getrennten
Einheiten. WinForms und WPF über den sicheren Launcher gestartet, Prozesse liefen
ohne gemeldeten Startfehler. Eine native Desktop-/Nutzerabnahme wurde damit nicht
bestätigt. Die anschließende erfolgreiche Nutzerabnahme ist oben dokumentiert.
Keine Pakete oder Store-Änderungen.

## Abnahmeabschluss und Draft-PR (2026-10-06)

Nach bestätigter manueller Abnahme erneuter vollständiger Safe-Lauf erfolgreich
(Exit 0): Restore, Release-Lösung inkl. WPF/WinForms mit 0 Warnungen/0 Fehlern,
alle Backend-Smoke-Tests und DE/EN-WinForms-Smoke-/Regressionstests.
Synthetischer UI-Lauf: .codex/synthetic-development-data/ui-tests/801900af7db84c68bfc97062d9e2a7ff.
git diff --check erfolgreich. Geschützte Benutzerdateien bleiben unverändert
und außerhalb des Index:

- .gitignore SHA-256: 9A84AA3B5E4E9DF41A745F8BF9288C1059948047061503A8634C574DB788A3C6
- CreateHealthTopicWizardForm.resx SHA-256: 4363CD7D5B8671C72442CE1A1BFC10D64EBD24B2D718B54BD4FCD025E4967298

.codex bleibt ignoriert und enthält keine getrackten Dateien. Die Abnahmedokumentation
wird als separater Commit auf feat/winforms-timeline-2 ergänzt; der Implementierungscommit
5eb5672 bleibt erhalten. Push und Draft-PR gegen main sind autorisiert.
CI wird am gepushten Head vollständig abgewartet und im PR berichtet.
Ready for Review und Merge sind ausdrücklich nicht autorisiert.
Die oben genannten fachlichen Folgepunkte und Gates anderer Module bleiben offen.
