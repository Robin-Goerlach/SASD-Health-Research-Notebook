# 100 - Testkonzept

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Testkonzept  
Status: Entwurf  

## 1. Ziel

Das Testkonzept beschreibt, wie die Qualität der Anwendung sichergestellt wird. Bei dieser Anwendung sind nicht nur fachliche Korrektheit und UI-Stabilität wichtig, sondern auch Datenschutz, Datenintegrität, Backup/Restore, Dokumentimport und Nicht-Verlust von Entwürfen.

## 2. Testgrundsätze

1. Keine echten Gesundheitsdaten in Tests.
2. Testdaten müssen künstlich und unverfänglich sein.
3. Fachlogik wird ohne UI getestet.
4. Datenbankmigrationen werden getestet.
5. Backup und Restore werden regelmäßig getestet.
6. Logs werden auf sensible Inhalte geprüft.
7. Fehlerfälle sind genauso wichtig wie Erfolgsfälle.

## 3. Testebenen

| Ebene | Zweck | Beispiele |
|---|---|---|
| Unit Tests | einzelne Klassen/Regeln | Validierung, Services, Value Objects |
| Integration Tests | Zusammenspiel mit SQLite/Dateien | Repositories, Dokumentimport |
| UI Tests | Bedienfluss | Wizard, Dashboard, Suchmaske |
| End-to-End Tests | vollständige Workflows | Thema anlegen, Dokument importieren, exportieren |
| Security Tests | Datenschutzregeln | Logs, Exportwarnungen, Pfade |
| Migration Tests | Schemaänderungen | v0.1 -> v0.2 |
| Backup/Restore Tests | Datenintegrität | Backup erzeugen und wiederherstellen |
| Manual Tests | visuelle/UX-Prüfung | Dashboard, Wizard, Fehlertexte |

## 4. Testdaten

Beispielthemen ohne reale Daten:

- Beispiel: Chronische Sinusitis
- Beispiel: Vitamin-D-Mangel
- Beispiel: Blutdruckbeobachtung
- Beispiel: Verdacht Histaminintoleranz

Diese Beispiele dürfen keine echten Personendaten enthalten.

## 5. Unit-Testbereiche

| Bereich | Tests |
|---|---|
| Condition | Pflichtfelder, Status, Archivierung |
| Wizard | Entwurf, Schrittwechsel, Abschluss |
| Symptoms | Schweregradvalidierung, Datum |
| Documents | Hash, Dateityp, Dublette |
| Sources | Quellenbewertung, URL optional |
| Questions | Statuswechsel, Terminbezug |
| Search | Synonyme, Tags, Archivfilter |
| Export | Inhaltsauswahl, Datenschutzwarnung |
| Audit | richtige Ereignisse, keine sensiblen Details |

## 6. Integrationstests

Wichtige Tests:

- SQLite-Datenbank anlegen
- Migration anwenden
- Condition speichern/laden
- Wizard-Draft speichern/fortsetzen
- Dokumentdatei importieren
- Datei-Hash berechnen
- Dublette erkennen
- Suchindex aktualisieren
- Exportdatei erzeugen
- Backup erzeugen
- Restore in neue Datenbank

## 7. Wizard-Testfälle

| Testfall | Erwartung |
|---|---|
| Wizard ohne Titel abschließen | Validierungsfehler |
| Wizard mit Titel und ohne optionale Angaben | Thema wird angelegt |
| Wizard mit Dokument | Dokument wird importiert und zugeordnet |
| Wizard abbrechen | Entwurf bleibt erhalten oder wird nach Bestätigung verworfen |
| App während Wizard schließen | Entwurf kann fortgesetzt werden |
| ähnlicher Titel | Dublettenwarnung |
| Schritt zurück/vor | Daten bleiben erhalten |
| Abschlussfehler beim Dokument | kein halb gespeicherter Zustand |

## 8. Dokumentimport-Testfälle

- kleine PDF-Datei importieren
- Bild importieren
- nicht existierende Datei
- gesperrte Datei
- Datei ohne Leserechte
- gleiche Datei zweimal importieren
- Datei mit sehr langem Namen
- Datei mit Umlauten/Sonderzeichen
- Import abbrechen
- Speicherort nicht verfügbar

## 9. Export-Testfälle

- Arztfragenliste als Markdown
- Arztmappe mit einem Thema
- Arztmappe mit mehreren Dokumenten
- Export ohne Dokumente
- Export mit sensiblen Dokumenten
- Export abbrechen
- Zielordner nicht verfügbar
- temporäre Dateien werden entfernt
- Exportprotokoll enthält keine sensiblen Inhalte

## 10. Backup/Restore-Testfälle

- Backup einer leeren Datenbank
- Backup mit Dokumenten
- Backup nach Dokumentimport
- Restore in leeres Profil
- Restore mit fehlender Datei im Backup
- Restore mit falscher Prüfsumme
- Restore einer alten Schema-Version
- Restore-Abbruch vor Überschreiben

## 11. Sicherheits- und Datenschutztests

| Test | Erwartung |
|---|---|
| Log nach Thema anlegen | kein Gesundheitsthema im Log |
| Log nach Suche | Suchbegriff nicht im Log |
| Log nach Dokumentimport | kein sensibler Originaldateiname im Log |
| Exportwarnung | Warnung erscheint vor Export |
| Cloud-Ordner-Erkennung | Hinweis erscheint, falls erkennbar |
| Testdatenprüfung | keine echten Personendaten im Repository |

## 12. UI-/UX-Tests

Manuelle Checkliste:

- Wizard verständlich?
- Pflichtfelder klar?
- Abbrechen ohne Panik?
- Entwurf wieder auffindbar?
- Dashboard nicht überladen?
- Suche gut erreichbar?
- Dokumente leicht zuordbar?
- Arztmappe logisch?
- Warnungen verständlich, nicht alarmistisch?
- Schriftgröße angenehm?

## 13. Akzeptanzkriterien für Phase 1 Anwendungsschale

- App startet.
- Hauptfenster öffnet.
- Logging funktioniert ohne sensible Daten.
- Konfiguration wird geladen.
- Datenordner wird angelegt.
- Fehlerdialog zeigt verständliche Meldung.
- Testprojekt läuft.

## 14. Akzeptanzkriterien für Phase 2 Condition-Verwaltung

- Thema anlegen, bearbeiten, archivieren, wiederherstellen.
- Liste zeigt aktive Themen.
- Detailansicht öffnet.
- Daten bleiben nach Neustart erhalten.
- Tests decken Kernfälle ab.

## 15. Akzeptanzkriterien für Phase 3 Wizard

- Wizard mit allen Schritten vorhanden.
- Entwürfe speichern.
- Fortsetzen möglich.
- Thema wird korrekt angelegt.
- Dokumente/Quellen/Fragen werden zugeordnet.
- Abbruch führt nicht zu Datenverlust.

## 16. CI-Empfehlung

Für GitHub Actions später:

```yaml
- dotnet restore
- dotnet build --configuration Release
- dotnet test --configuration Release
```

Später ergänzen:

- Markdown-Linkprüfung
- Formatprüfung
- Dependency-Check
- Tests mit mehreren .NET-Versionen

## 17. Definition of Done

Eine Phase gilt erst als abgeschlossen, wenn:

- Code gebaut wird
- Tests erfolgreich sind
- manuelle Kurzprüfung durchgeführt wurde
- Dokumentation aktualisiert wurde
- relevante ADRs ergänzt wurden
- README/Roadmap bei Bedarf angepasst wurden
- Commit-Message sauber formuliert ist


---

## 18. CI-Status Baseline 2.1 (2026-09-30)

GitHub Actions ist nicht mehr nur eine Empfehlung, sondern im Repository unter `.github/workflows/dotnet.yml` eingerichtet.

Die Windows-CI führt aus:

1. Checkout;
2. .NET-8-Setup;
3. `dotnet restore Sasd.HealthNotebook.sln`;
4. Release-Build der Solution;
5. Ausführung des Smoke-Test-Projekts.

Die erste Aktivierung der CI hat einen vorhandenen WPF-Buildfehler sichtbar gemacht: Der Typname `Application` kollidierte mit dem Namespace `Sasd.HealthNotebook.Application`. Der WPF-Basistyp wurde daraufhin explizit als `System.Windows.Application` qualifiziert. Der anschließende CI-Lauf war erfolgreich.

Für UI-Änderungen bleibt zusätzlich eine manuelle Sichtprüfung notwendig. Ein grüner Build ersetzt nicht den Vergleich der laufenden Anwendung mit den Konzept-Screenshots.

Später zu ergänzen:

- echte Unit-Testprojekte;
- Integrationstests für SQLite/Migrationen;
- Backup-/Restore-Tests;
- optional UI-Automatisierung;
- Markdown-/Linkprüfung.


---

## 19. Requirements-Traceability und Release-Gates (2026-09-30)

Die verbindliche Zuordnung zwischen Anforderungs-IDs, Akzeptanzkriterien und Prüfungen ist in
`docs/testing/105_Akzeptanzkriterien_Traceability_Quality_Gates.md` beschrieben.

Wichtig für den aktuellen Stand:

- der vorhandene Smoke-Test ist automatisiert und läuft in CI;
- er ist noch kein Ersatz für systematische Unit-, Integrations- und End-to-End-Tests;
- `dotnet test` wird erst zu einem belastbaren Quality Gate, sobald echte Testprojekte mit einem Testframework in der Solution vorhanden sind;
- neue oder wesentlich geänderte MUSS-Funktionalität soll künftig mit Requirement-ID und Testzuordnung entwickelt werden;
- ein grüner Build allein ist kein Nachweis der vollständigen fachlichen Umsetzung.

## 20. Sources Slice 1 – aktuelle Prüfungen

Die bestehenden beiden Smoke-Testprojekte prüfen FR-SRC-001/002/004/005/006
einschließlich optionaler externer ID (FR-SRC-003). `SourceTests` deckt Domaingrenzen,
Application-Referenzintegrität, alle Kategorien, Roundtrip und unabhängiges Reload ab.
Fremde/beschädigte/neue Store-Versionen, inkonsistente Beziehungen, vorhandene
Temp-/Lock-Dateien und fremde Backups werden abgewiesen und erhalten.
Topic-/Entry-JSON bleibt bytegenau unverändert. `SafeEntryLauncherTests` prüft
zusätzlich die vier Sources-Dateiguards mit echtem Windows PowerShell 5.1.

Die WinForms-Smoke-Tests verwenden echte Controls und Dialoge: Quelle, Fundstelle,
Quellen-Notiz, sofortige Anzeige, Reload, Sprachwechsel, getrennte Textanzeigen,
Tab-Reihenfolge, Tooltips und Mindestgrößen. Alle Daten sind ausdrücklich synthetisch
in frischen `.codex/`-Laufverzeichnissen. Keine neuen Testprojekte/Pakete.
Manuelle Desktopakzeptanz bleibt ein separates Gate; Stand und Prüfliste in Dokument 149.

SourceLocation-Neustartregression: IT-SRC-RELOAD-001 prüft einen frischen isolierten
Store über den produktiven Servicepfad, tatsächliche JSON-Felder und neue Repository-/
Service-Instanzen mit `GetSourceDetailsAsync`. UI-SRC-RELOAD-001 erstellt über echte
Dialoge eine Quelle/Fundstelle und eine zweite Quelle, disposed die ursprüngliche
Shell und wählt nach Neustart die ältere Quelle ohne zusätzlichen Refresh aus.
Die Auswahlbenachrichtigung muss bereits die neue CurrentRow melden; Fundstelle,
Original-IDs, Wieder-/Rückauswahl und Empty States sind in Deutsch/English abgesichert.
Der Test schlägt mit dem alten SelectionChanged-Leseweg fehl und besteht erst mit
CurrentCellChanged. Ein direkter Save/Load-Test oder ein Refresh nach Auswahl würde
diese UI-Regression verdecken; Dokument 149 hält Ursache und bisherigen Testblindspot fest.

## 21. Measurement / Vitalwerte Slice 1 – aktuelle Prüfungen

Die bestehenden Smoke-Testprojekte decken FR-MEA-001/002/004/005/007 und den
Themenbezug aus FR-MEA-006 ab. `MeasurementTests` prüft alle fünf Kategorien,
getrennte Blutdruckkomponenten/optionalen Puls, Pflichtwerte, NaN/Infinity,
negative Zahlen, technische Extremwerte ohne klinische Schranken, Textgrenzen,
feste Einheiten, Offset-Sortierung, unabhängiges Reload und fehlende/archivierte Themen.
Kennung/Version, beschädigte/fremde/zukünftige Stores, Backup, vorhandene Temp-/Lock-
Dateien und bytegenauer Erhalt von Topic-/Entry-/Source-Dateien werden abgesichert.
Der echte Windows-PowerShell-5.1-Launcher weist Links unter allen vier Messwertnamen ab.

`MeasurementUiTests` prüft echte Navigation und Dialoge in Deutsch/English, alle fünf
synthetischen Messarten mit/ohne Thema, sichtbare Einheiten, Dezimalzeichen gemäß
UI-Sprache ohne stille Tausender-/Einheitenumrechnung, dynamische Felder, sofortige
Anzeige, Refresh-Auswahl, Shell-/Repository-Neustart, Tooltips, Tab-Reihenfolge und
Mindestgrößen. Vorhandene Dashboard-/HealthTopic-/Timeline-/Sources-Prüfungen bleiben
Teil desselben Laufs. WPF ist im vollständigen Release-Build enthalten.

Test-IDs: UT-MEA-001, IT-MEA-001, SEC-MEA-001, SEC-MEA-002, UI-MEA-001.
Manuelle Desktopakzeptanz ist bestätigt; genaue Abdeckung, Prüfliste und Abgrenzungen in Dokument 150.
Kein weiteres Testprojekt, keine zusätzlichen Pakete, ausschließlich synthetische
Daten unter `.codex/`. Ein grüner automatisierter Lauf ersetzt dieses manuelle Gate nicht.

## 22. Session + Questions + Follow-up Slice 1

UT/IT/SEC-SES-001 prüft Session-/Kindvalidierung, optionale Themen, chronologische
Sortierung, getrennte Antwortnotizen, beantwortet/offen und Follow-up-Status,
Kalenderfälligkeit, unabhängiges Reload und Referenzintegrität auch im Repository.
Version/Kennung, beschädigte/fremde/zukünftige Stores, Backup, Temp/Lock und
bytegenauer Erhalt von Topic-/Entry-/Source-/Measurement-Dateien sind abgesichert.
SEC-SES-002 ergänzt vier Session-Dateiguards mit echtem Windows PowerShell 5.1.
UI-SES-001 nutzt echte Session-/Frage-/Follow-up-Dialoge, Antwortänderung,
Statuswechsel, Themenbezug, Neustart mit frischen Services und Wiederauswahl ohne
Refresh, DE/EN sowie Mindestgröße. Alle vorhandenen Regressionen bleiben aktiv.
Manuelle Desktopakzeptanz als eigenes Gate am 2026-10-01 vom Nutzer bestätigt; Prüfumfang und Grenzen in Dokument 151.

## 23. HealthAction + Routine + Progress Slice 1

UT/IT/SEC-ACT-001 (`HealthActionTests`) prüft Pflicht-/Längenregeln, Kategorien,
berichtete Herkunft, optionale/archivierte/fehlende Themen, getrennte Routinen,
Active/Paused-Roundtrip und unveränderte Historie. Progress-Identität, Offset,
Completion, optionale Anzahl, exakter Text, unabhängiges Reload und chronologische Sortierung sind
abgesichert. Verwaiste Routine/History wird in Application und Repository abgewiesen.
Kennung/Version, beschädigte/fremde/zukünftige Stores, Pflicht-/unbekannte Felder,
Duplikate/null-Kinder, Backup, Temp/Lock und bytegenauer Erhalt aller fünf bestehenden
Stores werden geprüft. SEC-ACT-002: vier echte PowerShell-5.1-Dateiguards.
UI-ACT-001/002: echte Dialoge, Navigation DE/EN, Leerzustände, mit/ohne Thema,
Auswahlwechsel von Actions/Routinen, Refresh, neue Shell/Services und Wiederauswahl
OHNE Refresh. Kompakte Karten, Header/Button-Clipping, proportionale Panels,
Oberkanten, TabOrder/Feldhilfe und keine Workspace-Scrollbars bei Normal-/Mindestgröße.
DASH-ACT-001: Dashboardzahlen aus synthetischen Datensätzen.
Renderbilder für beide Sprachen, Normal-/Mindestgröße, leer/gefüllt und Dialoge unter
frischen .codex-Laufverzeichnissen. Manuelle Desktopakzeptanz bleibt separates Gate;
Prüfliste in Dokument 152. Keine neuen Pakete/Projekte, alle Regressionen bleiben aktiv.


## Edit / Archive / Delete Baseline Slice 1

IT/SEC/MIG-LIF-001 (LifecycleTests): Updates aller sechs Entitäten, Id/CreatedAt,
ModifiedAt, Offset-only-Korrekturen, Archive/Reactivate, Referenz-/Kindschutz, professionelle
und Source-basierte Revisionen, Delete/Reload, No-op/Concurrency, v1/v2, Backup/Temp/Lock,
Corruption und bytegenauer Erhalt fremder Stores. Parent Delete ist absichtlich nicht
verfügbar. UI-LIF-001: echte Edit-/History-/Delete-Dialoge und Archive-Workspace DE/EN,
Enter=Cancel, Cancel/Confirm, Refresh/Restart/Sprachwechsel/Auswahl und Mindestgrößen;
veraltete Timeline-Version und überlappender Session-Refresh separat abgesichert.
Keine neuen Pakete/Projekte; Safe Launcher mit Windows PowerShell 5.1 und allen
bestehenden Regressionen. Manuelles Gate offen; konkrete Prüfliste in Dokument 153.
