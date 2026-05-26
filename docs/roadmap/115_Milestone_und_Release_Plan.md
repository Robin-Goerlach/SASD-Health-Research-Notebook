# 115 - Milestone- und Release-Plan

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Milestone-Plan / Umsetzungsfahrplan  
Status: Entwurf  

## 1. Zweck dieses Dokuments

Dieses Dokument konkretisiert die Roadmap in umsetzbare Entwicklungsmeilensteine. Es soll verhindern, dass das Projekt zu früh zu groß wird oder wichtige Grundlagen wie Datenschutz, Backup, Datenmodell, Tests und Export nach hinten rutschen.

Der Plan unterscheidet drei Zielstufen:

| Zielstufe | Bedeutung | Orientierung |
|---|---|---:|
| Technischer Prototyp | App startet, einfache Daten werden erfasst | Milestone 1 bis 5 |
| Gut nutzbare interne V1 | Für die persönliche tägliche Nutzung sinnvoll | Milestone 1 bis 15 |
| Rundes vorzeigbares SASD-Produkt | Stabil, dokumentiert, testbar, mit Sicherheitsbasis | Milestone 1 bis 24 |

Die Milestones sind bewusst klein gehalten. Jeder Meilenstein soll möglichst baubar, testbar, dokumentierbar und commit-fähig sein.

## 2. Leitprinzipien

Für alle Milestones gelten folgende Regeln:

1. Die Anwendung bleibt ein Dokumentations- und Recherchewerkzeug.
2. Es werden keine Diagnosen, Therapieempfehlungen oder Medikamentendosierungen generiert.
3. Gesundheitsdaten werden nicht in Logs geschrieben.
4. Speicherung, Export und Backup müssen bewusst und nachvollziehbar gestaltet sein.
5. Neue Funktionen werden klein implementiert, getestet und dokumentiert.
6. UI-Komfort darf die Datenintegrität nicht gefährden.
7. Jede Phase muss einen klaren Nutzen oder eine klare technische Grundlage liefern.

## 3. Milestone 1 - Repository Foundation

### Ziel

Ein sauberes GitHub-Repository mit technischer Grundstruktur, README, Lizenz, Screenshots, Dokumentationsordnern und initialer .NET-Solution.

### Inhalte

- Repository `SASD-Health-Research-Notebook` anlegen.
- README mit Projektziel, Nicht-Zielen und Konzeptbildern pflegen.
- Lizenzdatei aufnehmen.
- `.gitignore` für Visual Studio, .NET, Build-Ausgaben und lokale Daten ergänzen.
- Grundordner anlegen: `src/`, `tests/`, `docs/`, `assets/`, `db/`.
- Erste Solution-Datei vorbereiten.

### Akzeptanzkriterien

- Repository kann geklont werden.
- README erklärt den Zweck des Projekts.
- Screenshots werden im README angezeigt.
- Lizenz ist enthalten.
- Build-Artefakte und lokale Gesundheitsdaten werden nicht eingecheckt.

## 4. Milestone 2 - Application Shell

### Ziel

Eine startbare Desktop-App mit Hauptfenster, Navigation, Dashboard-Platzhaltern, Statusbereich und stabiler Fehlerbehandlung.

### Inhalte

- WPF- oder WinForms-App anlegen.
- Hauptfenster implementieren.
- Linke Navigation oder Menüstruktur anlegen.
- Dashboard-Platzhalter mit Karten anzeigen.
- Statusleiste für Anwendungsmeldungen ergänzen.
- Globalen Fehlerfänger einrichten.
- Logging einführen, aber ohne Gesundheitsdaten.

### Akzeptanzkriterien

- App startet auf einem Windows-System.
- App zeigt ein Dashboard.
- App kann sauber beendet werden.
- Fehler werden benutzerfreundlich angezeigt.
- Logdateien enthalten keine personenbezogenen Gesundheitsinhalte.

## 5. Milestone 3 - Domain Foundation

### Ziel

Die ersten Domänenklassen und Services werden erstellt, damit Fachlogik nicht in der UI landet.

### Inhalte

- `HealthTopic` als zentrales Thema/Erkrankung/Verdachtsobjekt.
- `HealthEntry` als generischer Eintrag.
- `DocumentReference` als Dateireferenz.
- `SourceReference` als Quelle.
- `Tag` und `TopicStatus`.
- Erste Application Services.
- Erste Unit Tests für Konstruktion, Validierung und einfache Regeln.

### Akzeptanzkriterien

- Domänenobjekte sind unabhängig von der UI nutzbar.
- Basisvalidierung funktioniert.
- Tests laufen erfolgreich.
- XML-Kommentare erklären die wichtigsten Modelle.

## 6. Milestone 4 - Lokale Speicherung V1

### Ziel

Daten werden lokal gespeichert und beim nächsten Start wieder geladen. Für den Anfang ist JSON erlaubt, solange die spätere SQLite-Migration vorbereitet wird.

### Inhalte

- Lokales Datenverzeichnis unter `%LocalAppData%`.
- JSON-Datei für erste Health Topics.
- Atomare Speicherung über temporäre Datei und Rename.
- Fehlerbehandlung bei beschädigter Datei.
- Keine stillen Überschreibungen.
- Einfache manuelle Sicherungskopie vor Speichern.

### Akzeptanzkriterien

- Neue Daten bleiben nach Neustart erhalten.
- Beschädigte Daten führen zu einer verständlichen Fehlermeldung.
- Keine Gesundheitsdaten werden ungewollt in Logs geschrieben.

## 7. Milestone 5 - Gesundheitsthemen-Verwaltung V1

### Ziel

Gesundheitsthemen können angelegt, angezeigt, bearbeitet und archiviert werden.

### Inhalte

- Liste aller aktiven Gesundheitsthemen.
- Detailansicht.
- Anlegen, Bearbeiten, Archivieren.
- Status: `Beobachtung`, `Verdacht`, `Diagnostiziert`, `Abgeklärt`, `Archiviert`.
- Priorität/Vertraulichkeit optional vorbereiten.

### Akzeptanzkriterien

- Ein Gesundheitsthema kann vollständig angelegt werden.
- Änderungen bleiben nach Neustart erhalten.
- Archivierte Themen verschwinden nicht endgültig.

## 8. Milestone 6 - Wizard V1

### Ziel

Der Wizard zum Anlegen eines neuen Gesundheitsthemas wird als zentrale Eingabefunktion eingeführt.

### Inhalte

- Schritt 1: Grunddaten.
- Schritt 2: Status und kurze Einordnung.
- Schritt 3: erste Symptome oder Beobachtungen.
- Schritt 4: erste Notizen/Informationen.
- Schritt 5: Zusammenfassung und Speichern.
- Abbrechen mit Warnung bei eingegebenen Daten.
- Spätere Erweiterbarkeit auf Dokumente, Quellen, Ärzte, Messwerte.

### Akzeptanzkriterien

- Wizard erstellt ein neues Thema.
- Pflichtfelder werden validiert.
- Der Nutzer sieht vor dem Speichern eine Zusammenfassung.
- Wizard-Code ist nicht monolithisch, sondern erweiterbar.

## 9. Milestone 7 - Notizen und Einträge

### Ziel

Zu jedem Gesundheitsthema können verschiedene Einträge angelegt werden.

### Inhalte

- Eintragstypen: Beobachtung, Information, Arztfrage, Verlauf, allgemeine Notiz.
- Datum und optional Uhrzeit.
- Titel und Text.
- Zuordnung zum Gesundheitsthema.
- einfache Sortierung nach Datum.

### Akzeptanzkriterien

- Einträge können erstellt, bearbeitet und archiviert werden.
- Einträge erscheinen in der Detailansicht.
- Einträge erscheinen später in der Timeline.

## 10. Milestone 8 - Dokumentenablage V1

### Ziel

Dateien können sicher und nachvollziehbar zu Gesundheitsthemen abgelegt werden.

### Inhalte

- PDF, Bilder und allgemeine Dateien hinzufügen.
- Datei in ein lokales Dokumentenverzeichnis kopieren.
- Originaldateiname, Ablagedatum, Beschreibung und Dokumenttyp speichern.
- Datei öffnen.
- Dateikonflikte über interne IDs vermeiden.

### Akzeptanzkriterien

- Dokument kann einem Thema zugeordnet werden.
- Dokument bleibt nach Neustart auffindbar.
- Löschen wird zunächst als Archivieren umgesetzt.

## 11. Milestone 9 - Quellenverwaltung

### Ziel

Informationen werden mit Quellen verbunden, damit später nachvollziehbar bleibt, woher Aussagen stammen.

### Inhalte

- Quelle anlegen: Webseite, Buch, Arztgespräch, Studie, eigene Beobachtung, sonstige Quelle.
- URL, Titel, Autor/Institution, Abrufdatum.
- Quellenstatus: ungeprüft, plausibel, ärztlich besprochen, fragwürdig, verworfen.
- Zuordnung zu Themen und Einträgen.

### Akzeptanzkriterien

- Eine Notiz kann mit einer Quelle verbunden werden.
- Quellenstatus ist sichtbar.
- Ungeprüfte Quellen werden nicht als gesicherte Fakten dargestellt.

## 12. Milestone 10 - Tags und Synonyme

### Ziel

Wiederfinden und Gruppieren werden verbessert.

### Inhalte

- Tags für Themen, Einträge, Dokumente und Quellen.
- Tag-Verwaltung.
- Synonym-Liste vorbereiten, z. B. Gefäßverkalkung / Arteriosklerose / Atherosklerose.
- Filter nach Tags.

### Akzeptanzkriterien

- Tags können hinzugefügt und entfernt werden.
- Filter funktionieren zuverlässig.
- Synonyme werden zunächst dokumentiert, später in Suche integriert.

## 13. Milestone 11 - Suche V1

### Ziel

Der Nutzer findet Informationen schnell wieder.

### Inhalte

- Suche über Themen, Einträge, Dokumenttitel und Quellen.
- Trefferliste mit Typ, Titel, Datum, Thema.
- Öffnen eines Treffers.
- Suchfeld im Dashboard.
- SQLite FTS5 für spätere Volltextsuche vorbereiten.

### Akzeptanzkriterien

- Suche findet Begriffe aus Titeln und Notizen.
- Treffer öffnen die passende Detailansicht.
- Keine versteckten gelöschten/archivierten Elemente in normalen Ergebnissen.

## 14. Milestone 12 - Timeline

### Ziel

Eine chronologische Sicht macht den Verlauf nachvollziehbar.

### Inhalte

- Timeline pro Gesundheitsthema.
- globale Timeline.
- Ereignistypen: Notiz, Symptom, Dokument, Quelle, Frage, Termin.
- Zeitraumfilter.
- Sortierung neueste/älteste zuerst.

### Akzeptanzkriterien

- Wichtige Ereignisse erscheinen chronologisch.
- Timeline kann nach Thema gefiltert werden.
- Export kann später auf Timeline-Daten zugreifen.

## 15. Milestone 13 - Symptome und Beobachtungen

### Ziel

Symptome werden strukturierter als normale Notizen dokumentiert.

### Inhalte

- Symptomnamen.
- Intensitätsskala, z. B. 0 bis 10.
- Datum/Uhrzeit.
- Dauer, Kontext und Bemerkung.
- Zuordnung zu einem Thema.
- einfache Verlaufsliste.

### Akzeptanzkriterien

- Symptome können wiederholt erfasst werden.
- Verlauf ist in Liste/Timeline sichtbar.
- Keine Interpretation oder Diagnose aus Symptomen.

## 16. Milestone 14 - Arztfragen und Termine

### Ziel

Die App unterstützt Vorbereitung und Nachbereitung von Arztgesprächen.

### Inhalte

- Fragen erfassen.
- Status: offen, geplant, gestellt, beantwortet, erledigt.
- Terminnotizen.
- Arzt/Institution als Freitext oder Kontaktobjekt.
- Gesprächsnotizen nach Termin.

### Akzeptanzkriterien

- Offene Fragen sind zentral sichtbar.
- Fragen können einem Thema zugeordnet werden.
- Fragen können später exportiert werden.

## 17. Milestone 15 - Export V1 / interne nutzbare V1

### Ziel

Die Anwendung wird für die persönliche Nutzung wirklich hilfreich, weil Informationen exportiert werden können.

### Inhalte

- Markdown-Export eines Gesundheitsthemas.
- Arztfragenliste.
- Dokumentenliste.
- Quellenliste.
- Timeline-Auszug.
- Exporthinweis: keine medizinische Bewertung.

### Akzeptanzkriterien

- Exportdatei ist lesbar und vollständig.
- Nutzer kann vor Export auswählen, welche Bereiche enthalten sind.
- Export enthält keine versteckten Daten.

Mit Abschluss dieses Milestones gilt die Anwendung als **gut intern nutzbare V1**.

## 18. Milestone 16 - Backup und Restore

### Ziel

Datenverlust wird deutlich unwahrscheinlicher.

### Inhalte

- Manuelles Backup.
- Backup-Metadaten.
- Restore-Dialog.
- Validierung vor Restore.
- Warnung vor Überschreibung.
- Backup des Datenbestands und Dokumentenverzeichnisses.

### Akzeptanzkriterien

- Backup kann erstellt werden.
- Backup kann auf Testdaten wiederhergestellt werden.
- Restore überschreibt nicht stillschweigend aktive Daten.

## 19. Milestone 17 - Audit und Änderungsverlauf

### Ziel

Wichtige Änderungen bleiben nachvollziehbar.

### Inhalte

- Technischer Änderungsverlauf für kritische Objekte.
- Zeitstempel für Erstellung und Änderung.
- Soft Delete / Archivierung statt stilles Löschen.
- Keine vollständigen Gesundheitsinhalte im Auditlog, wenn nicht zwingend nötig.

### Akzeptanzkriterien

- Änderungen an Themen und Dokumenten sind nachvollziehbar.
- Auditdaten enthalten keine unnötig sensiblen Inhalte.

## 20. Milestone 18 - SQLite-Migration / Datenbankstabilisierung

### Ziel

Persistenz wird auf ein belastbares relationales Modell gehoben, falls V1 mit JSON gestartet wurde.

### Inhalte

- SQLite-Datenbank.
- Migrationsmechanismus.
- Tabellen für Themen, Einträge, Dokumente, Quellen, Tags.
- Datenmigration aus JSON.
- Integritätstests.

### Akzeptanzkriterien

- Migration übernimmt bestehende Daten.
- Datenbanktests laufen.
- Datenmodell ist dokumentiert.

## 21. Milestone 19 - Sicherheitsbasis

### Ziel

Das Produkt wird sicherer im Umgang mit Gesundheitsdaten.

### Inhalte

- Überprüfung der Logdateien.
- Datenverzeichnis dokumentieren.
- Exportwarnungen und Bestätigung.
- App-Sperre vorbereiten.
- Verschlüsselung technisch bewerten.
- SQLCipher/Dateiverschlüsselung als ADR ergänzen.

### Akzeptanzkriterien

- Keine Gesundheitsdaten in Logs.
- Export ist bewusst und nachvollziehbar.
- Sicherheitsgrenzen sind dokumentiert.

## 22. Milestone 20 - UI/UX-Politur

### Ziel

Die App wird angenehmer und schneller bedienbar.

### Inhalte

- Verbesserte Empty States.
- Kontextmenüs.
- Doppelklick-Aktionen.
- bessere Filter.
- Tastaturkürzel.
- Layoutverbesserungen.
- Wizard-Politur.

### Akzeptanzkriterien

- Häufige Aufgaben benötigen weniger Klicks.
- Neue Nutzer verstehen die Grundlogik schneller.
- UI bleibt ruhig und nicht medizinisch alarmistisch.

## 23. Milestone 21 - Reports und Übersichten

### Ziel

Aus den gespeicherten Informationen werden nützliche Übersichten generiert.

### Inhalte

- Themenreport.
- offene Fragen.
- Dokumentenreport.
- Quellenreport.
- Verlaufsübersicht.
- Exportprofile.

### Akzeptanzkriterien

- Reports sind reproduzierbar.
- Reports unterscheiden klar zwischen Notizen, Quellen und gesicherten Angaben.

## 24. Milestone 22 - Tests und Qualitätssicherung

### Ziel

Stabilität und Vertrauen erhöhen.

### Inhalte

- Unit Tests ausbauen.
- Repository Tests.
- Exporttests.
- Backup-/Restore-Tests.
- Migrationstests.
- Fehlerfalltests.

### Akzeptanzkriterien

- Kritische Funktionen sind automatisiert getestet.
- Build und Tests laufen reproduzierbar.

## 25. Milestone 23 - Installer / Release-Paket

### Ziel

Die Anwendung wird installierbar bzw. als Release nutzbar.

### Inhalte

- Release-Build.
- ZIP-Release oder Installer.
- Versionsnummer.
- Release Notes.
- Installationshinweise.
- Datenverzeichnis und Backup-Hinweise.

### Akzeptanzkriterien

- Release kann auf einem Zielsystem gestartet werden.
- Installation und Deinstallation sind dokumentiert.
- Keine Entwicklungsdateien im Release.

## 26. Milestone 24 - V1-Abschluss

### Ziel

V1 wird fachlich und technisch abgeschlossen.

### Inhalte

- Dokumentation aktualisieren.
- Screenshots aktualisieren.
- bekannte Grenzen dokumentieren.
- Abschluss-Checkliste.
- GitHub Release.
- Planung V1.1/V2.0.

### Akzeptanzkriterien

- V1 ist nutzbar, dokumentiert und versioniert.
- Risiken und Grenzen sind offen beschrieben.
- Nächste Erweiterungen sind priorisiert.

## 27. Erweiterungen nach V1

Nach V1 können folgende Erweiterungen geplant werden:

- OCR für gescannte Dokumente.
- KI-Zusammenfassungen mit strenger Quellenbindung.
- FHIR-Export oder FHIR-Mapping.
- Laborwertdiagramme.
- komplexere Messwerttypen.
- Verschlüsselung als Pflichtfunktion.
- optionaler Sync.
- Mobile Companion App.
- Import aus Patientenportalen.
- erweiterte Quellenbewertung.
- Plausibilitätswarnungen ohne Diagnosefunktion.

## 28. Zusammenfassung

Für ein gut intern nutzbares Produkt werden etwa 15 Milestones benötigt. Für ein rundes, vorzeigbares SASD-Produkt sind etwa 24 Milestones realistisch. Alles darüber hinaus sollte als V2/V3 geplant werden, damit das Projekt nicht zu früh überladen wird.
