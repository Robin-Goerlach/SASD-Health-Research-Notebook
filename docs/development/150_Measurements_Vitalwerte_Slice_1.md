# 150 – Measurement / Vitalwerte Slice 1

Stand: 2026-10-01. Implementiert; manuelle Nutzerakzeptanz noch offen.
Anforderungen: FR-MEA-001/002/004/005/007; FR-MEA-006 teilweise (Gesundheitsthema).

## Modellentscheidung

Eine kleine `Measurement`-Entität mit typabhängigen optionalen Zahlenfeldern ist
für fünf bekannte Kategorien verständlicher als eine generische Komponenten-Engine.
`BloodPressure` verlangt `Systolic` und `Diastolic`, erlaubt optional `Pulse` und
verlangt `Value = null`. `Pulse`, `Temperature`, `BloodGlucose`, `Weight` verlangen
genau `Value`; Blutdruckfelder sind dabei unzulässig. Zahlen sind `double`, keine
formatierten Strings. Die Anzeige folgt der gewählten UI-Sprache; Speicherung ist
kulturunabhängiges JSON. Kein zweiter Puls-Datensatz wird automatisch erzeugt.

`MeasurementUnit` bindet mmHg, /min, °C, mg/dL und kg eindeutig an den Typ.
Der optionale Blutdruck-Puls verwendet /min. Keine freie Einheit, mmol/L oder
stille Umrechnung. `OccurredAt`, `CreatedAt`, `ModifiedAt` sind wie bisher
`DateTimeOffset`; im create-only Slice sind die technischen Zeitstempel gleich.
Erfassungsart ist ausdrücklich `Manual`. Optionale selbst dokumentierte Messsituation
(500 Zeichen) und persönliche Notiz (4000 Zeichen) bleiben getrennt von Zahlen.
Texte werden nicht abgeschnitten oder medizinisch interpretiert.

Domainvalidierung verlangt gültige Metadaten, Pflichtkomponenten, typgerechte Felder
und endliche, nichtnegative Zahlen. Der Zahlenwert 0 und technisch darstellbare Extremwerte
sind zulässig; keine medizinischen Minima/Maxima, kein Vergleich zwischen systolisch
und diastolisch, keine Klassifikation, Warnung, Diagnose, Therapie oder Dosislogik.
Der Dialog parst ohne Gruppierungszeichen mit DE-Dezimalkomma bzw. EN-Dezimalpunkt;
ungültige oder mehrdeutige lokale Zeiten beim Sommerzeitwechsel werden abgewiesen.

`MeasurementService`/`IMeasurementRepository` erstellen und laden chronologisch
(OccurredAt absteigend, danach CreatedAt und stabile ID), optional nach Thema.
Ein gewähltes Thema muss beim Erstellen vorhanden sein. Zur Anzeige wird der aktuelle
Titel aufgelöst; archivierte/fehlende Themen entfernen keine Messung. Keine Kopie
des Themennamens, keine Cascading Deletes. Weitere Referenzen aus FR-MEA-006 bleiben
offen. Die bestehende Timeline ist an HealthEntry gebunden; Measurement erhält daher
eine eigene Ansicht. Gemeinsame Timeline-Projektion ist später möglich, ohne eine
Measurement-Kopie als HealthEntry oder persistiertes TimelineEvent anzulegen.

## Persistenz und Entwicklungssicherheit

Separater `measurements.json`-Store: `Store = SASD.HealthNotebook.Measurements`,
`Version = 1`, Liste `Measurements`. Kein neues Format für bestehende Stores und
keine Migration/SQLite. `JsonMeasurementRepository` folgt dem Entry-Store: strikte
Kennung/Version/Pflichtmetadaten, unbekannte Felder und Domaininvarianten prüfen,
eindeutige IDs, CreateNew-Writer-Lock und Tempdatei, Flush und atomarer Austausch
mit letztem gültigem Backup. Dateinamen:

- `measurements.json`
- `measurements.json.tmp`
- `measurements.backup.json`
- `measurements.json.lock`

Beschädigte, fremde oder zukünftige Primär-/Backup-Stores werden nicht überschrieben.
Fehlende Primärdatei ohne Backup ist ein leerer neuer Store; eine vorhandene leere
Datei ist beschädigt. Vorhandenes Backup ohne Primärdatei verlangt bewusste Wiederherstellung.
Bestehende Temp-/Lock-Dateien werden nicht bereinigt. Kein automatischer Recovery-Lauf.
Reparse Points werden abgewiesen; die akzeptierte Race-Condition bei gleichzeitig
böswillig manipulierten Dateisystemlinks bleibt bestehen. Keine neue generische
Repository-Basis. JSON bleibt wie die bestehende Persistenz unverschlüsselt.

`LocalHealthNotebookPaths.MeasurementsFilePath` verwendet ausschließlich den gemeinsamen
`SASD_HEALTHNOTEBOOK_DATA_PATH`. Launcher und echte PowerShell-5.1-Dateiguard-Tests
berücksichtigen alle vier neuen Namen. Entwicklungsdaten liegen ausschließlich unter
`.codex/synthetic-development-data`; TEMP/TMP, NuGet und DOTNET_CLI_HOME bleiben
isoliert. Alle Testdaten sind eindeutig synthetisch. Die bestehenden Topic-/Entry-/
Source-Dateien werden in Integrations- und UI-Tests bytegenau auf Erhalt geprüft.

## UI und Nachweise

WinForms bietet Vitalwerte/Measurements, chronologische Liste mit Zeitpunkt,
Messart, Zahlen, Einheit, Thema und kurzer Notizvorschau. Der Dialog enthält Datum,
Uhrzeit, Typ, dynamische spezifisch beschriftete Zahlenfelder mit Einheit, optionales
Thema, Messsituation und eigene Notiz. Typwechsel leert Zahlen bewusst, um versteckte
Werte oder falsche Einheiten nicht zu übernehmen. Styling, Navigation, Localization
und Presenter werden wiederverwendet; Fachlogik bleibt Domain/Application.
WPF erhält keine Measurement-UI und bleibt Bestandteil des Release-Builds.

- UT-MEA-001: alle Typen, Struktur, Pflichtwerte, Endlichkeit, negative Zahlen,
  Textgrenzen und technisch darstellbare Extremwerte ohne medizinische Grenze.
- IT-MEA-001: JSON-Roundtrip, numerische Werte, Einheiten, unabhängiges Reload,
  chronologische Instants, optionale Themen, archivierte/fehlende Themen und Erhalt
  aller drei bestehenden Stores.
- SEC-MEA-001: beschädigte/fremde/zukünftige Stores, Metadaten, Backup und Temp/Lock.
- SEC-MEA-002: tatsächlicher Windows-PowerShell-5.1-Launcher mit vier Dateiguards.
- UI-MEA-001: echte Navigation/Dialoge DE/EN, alle fünf synthetischen Typen,
  optionale Themen/Puls, Dezimalzeichen, Einheiten, Empty State, Create/Refresh/Reload,
  TabOrder, Tooltips und Mindestgröße.

Der vollständige sichere Release-Lauf einschließlich WPF und beiden Smoke-Testprojekten
ist grün (0 Warnungen, 0 Fehler). Synthetische Renderbilder der Liste und Dialoge
wurden geprüft. Computer Use kann die native Windows-Pipe nicht verbinden (os error 2);
eine manuelle Desktopprüfung wurde daher nicht als bestanden gewertet.

## Manuelle Akzeptanz – ausstehend

Nach grünem vollständigem Validate-Lauf vom Repository aus starten:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

1. Vitalwerte/Measurements öffnen; Empty State oder ausschließlich synthetische Daten.
2. Dialog öffnen, Blutdruck 120 / 80 mmHg mit optionalem Puls 70 /min erfassen.
3. Puls 70 /min, Temperatur 36,5 °C (EN: 36.5), Blutzucker 123 mg/dL und Gewicht
   80,0 kg (EN: 80.0) erfassen. Dies sind ausschließlich synthetische Testwerte.
4. Mit/ohne synthetisches Thema sowie Notiz `CODEX TEST` prüfen; Einheiten und
   getrennte Blutdruckwerte müssen sichtbar sein. Keine medizinische Bewertung.
5. Sofortige Anzeige, neueste fachliche Zeitpunkte zuerst und Refresh prüfen.
6. Schließen, mit demselben Launcher neu starten; alle Messungen/Themenbezüge erhalten.
7. Deutsch/English, Tooltips, Tab/Enter/Escape und Mindestfenster ohne Clipping prüfen.

Bis zur manuellen Bestätigung kein Commit/Push/PR. Die untracked Benutzer-Wizard-
`.resx` bleibt unverändert; `.codex/` wird nicht versioniert.

Offen außerhalb Slice 1: Edit/Delete/Archive, freie Typen/Einheiten und Umrechnung,
Geräte-/Dokumentimport, Laborwerte, zusätzliche Referenzen, allgemeine Timeline,
automatische Recovery und SQLite. Nächster geplanter Slice: Session + Questions + Follow-up.
