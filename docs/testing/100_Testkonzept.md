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
Manuelle Desktopakzeptanz steht noch aus; Prüfliste und Abgrenzungen in Dokument 150.
Kein weiteres Testprojekt, keine zusätzlichen Pakete, ausschließlich synthetische
Daten unter `.codex/`. Ein grüner automatisierter Lauf ersetzt dieses manuelle Gate nicht.
