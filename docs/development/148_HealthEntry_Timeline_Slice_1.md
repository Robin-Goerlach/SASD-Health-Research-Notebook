# 148 – HealthEntry / Timeline Slice 1

Stand: 2026-10-01. Anforderung: FR-OBS-001. Implementiert und manuell akzeptiert.

## Architektur und Umfang

`HealthEntry` enthält Id, optional HealthTopicId, EntryType (`Note`, `Observation`,
`Research`), OccurredAt, Title, Content, CreatedAt und ModifiedAt. Zeitwerte sind
`DateTimeOffset` wie bei HealthTopic. Titel werden getrimmt und auf 160 Zeichen
begrenzt; Text bleibt unverändert und darf höchstens 4000 Zeichen lang sein.
Überlange Inhalte werden abgewiesen, nicht abgeschnitten. Keine medizinische
Interpretation, keine Messwerte, Sessions, Aktionen oder anderen Fachmodule.

`HealthEntryService` erstellt Einträge, validiert optionale Themen gegen das
gemeinsame Repository und liefert chronologische Projektionen mit aktuellem
Themennamen. Sortierung: OccurredAt absteigend, dann CreatedAt absteigend und Id
als stabiler Tie-Breaker. Ein optionaler Application-Filter nach HealthTopicId ist
vorhanden; eine Filter-UI ist nicht Bestandteil dieses Slices. Ein später fehlendes
Thema entfernt keinen Eintrag; die UI zeigt einen Hinweis auf die fehlende Referenz.

Die Timeline leitet sich direkt aus HealthEntry ab. Kein zweites persistiertes
TimelineEvent und keine neue generische Repository-/Mediator-Infrastruktur.
WPF bleibt funktional unverändert und buildbar. WinForms nutzt gemeinsame Services,
einen TimelinePresenter, eine TimelineView und einen kleinen CreateHealthEntryForm.
Die bestehende Hauptaktion wird auf der Timeline zu „Neuer Verlaufseintrag“;
auf Dashboard/Themenliste bleibt die HealthTopic-Anlage erhalten.

## Persistenz und Sicherheit

`LocalHealthNotebookPaths.HealthEntriesFilePath` ergänzt `health-entries.json`
im selben aufgelösten Datenordner. Ohne Override unveränderter Produktordner;
für alle Entwicklungsprüfungen ausschließlich `.codex/synthetic-development-data`.
`health-topics.json` und sein Format werden weder geändert noch migriert.

Der neue Store ist ein JSON-Objekt mit `Store = SASD.HealthNotebook.HealthEntries`,
`Version = 1` und `Entries`. Die Kennung, Version, Pflichtmetadaten, Domaininvarianten,
eindeutigen IDs und unbekannten Felder werden vor Schreibzugriffen geprüft. Fremde,
beschädigte oder neuere Stores und fremde Backups werden abgewiesen, nicht ersetzt.
Es findet keine automatische Wiederherstellung aus dem Backup statt.
Fehlt die primäre Datei bei vorhandenem Backup, wird auch kein leerer Store
angelegt oder angezeigt: Der Zugriff bricht mit einem Fehler ab.

Einträge werden create-only ergänzt. Ein exklusiv neu angelegtes `.json.lock`
verhindert gleichzeitige kooperierende Schreibzugriffe; Konflikte brechen sicher ab.
Vorhandene Lock-Dateien werden nicht automatisch entfernt. `health-entries.json.tmp`
wird exklusiv neu angelegt, geflusht und atomar ersetzt; die letzte gültige
Version bleibt in `health-entries.backup.json`. Nur eigene temporäre Dateien werden
bereinigt. Reparse-Point-Ziele/Vorfahren werden abgewiesen. Wie beim Launcher ist
dies kein Schutz gegen gleichzeitig böswillig manipulierte Dateisystemlinks.
Die Dateien sind lokal und unverschlüsselt wie die bestehende JSON-Persistenz.
Der Launcher prüft dieselben vier Dateinamen wie das Repository: primärer Store,
`.json.tmp`, `.backup.json` und `.json.lock`. Eine vorhandene temporäre Datei wird
nicht überschrieben oder gelöscht; nach einem abgebrochenen Schreibvorgang ist
gegebenenfalls eine bewusste manuelle Prüfung nötig. Eine fehlende primäre Datei
ohne Backup ist ein neuer leerer Store; eine vorhandene Datei ohne JSON-Inhalt
gilt als beschädigt und bleibt unverändert. Ein gültiger Store mit leerer Entries-
Liste ist zulässig.

Der Dialog erfasst lokale Datum-/Uhrzeitwerte minutengenau. Ungültige oder
mehrdeutige Uhrzeiten bei Sommerzeitwechseln werden abgewiesen statt still
umgerechnet; die gewählte lokale Uhrzeit erhält ihren UTC-Offset. Datumformat
folgt der gewählten UI-Sprache. Die Liste zeigt einen maximal 160 Zeichen langen
Textauszug; der vollständige Inhalt bleibt im Store erhalten. Bearbeiten,
Löschen und eine separate Detailansicht sind nicht Bestandteil von Slice 1.

## Nachweise

Bestehende Smoke-Testprojekte erweitert, keine neuen Projekte oder Pakete:

- UT-ENTRY-001: gültige Eingabe, Titel-/Textgrenzen, Typ, Metadaten, kein Abschneiden.
- IT-ENTRY-001: isolierter Roundtrip, Offset-Sortierung, optionale Themen/Filter,
  aktuelle/archivierte/fehlende Themen, keine persistierten Themenname-Kopien,
  letzter gültiger Backup-Inhalt und bytegenauer Erhalt des Themen-JSON.
- SEC-ENTRY-001: fremde/beschädigte Stores, fremdes Backup und vorhandene Temp-/Lock-Datei
  bleiben bei abgewiesenem Schreibversuch erhalten.
- SEC-ENTRY-002: tatsächlicher Windows-PowerShell-5.1-Launcher weist für alle vier
  HealthEntry-Dateinamen Junctions ab. Pro Fall wird ein neues synthetisches
  Repository unter `.codex/launcher-guards` verwendet; Link und Ziel liegen darin.
- UI-ENTRY-001: echte Timeline-Navigation/Hauptaktion/Dialog mit STA-Message-Loop,
  Deutsch/English, Leerzustand, Titelvalidierung, Mindestgröße, synthetisches Speichern
  mit/ohne Thema, sofortige Aktualisierung, Sprachwechsel und Wiederladen.

Review: Die zuvor GUID-basierten Temp-Dateinamen wurden auf `health-entries.json.tmp`
vereinheitlicht, mit exklusiver Anlage unter dem vorhandenen Writer-Lock. Es wurde
keine generische Persistenzabstraktion ergänzt. `Note`, `Observation` und `Research`
reichen für Slice 1 aus; keine weiteren Kategorien oder medizinischen Regeln.
Die Dialogbeschriftung lautet explizit „Titel des Verlaufseintrags“; fehlende Themen
werden neutral als „Gesundheitsthema nicht verfügbar“ angezeigt. Die bestehende
HealthTopic-Persistenzimplementierung bleibt unverändert.

Computer Use meldet eine nicht verfügbare native Pipe (os error 2). Automatisierte
Control-Tests und Sichtung synthetischer Renderbilder sind keine manuelle
Desktopakzeptanz. Der Nutzer hat diese am 2026-10-01 erfolgreich bestätigt:
Navigation, Empty State, Dialog, synthetische Anlage mit/ohne Thema, sofortige
Anzeige, Persistenz nach Neustart, Themenname, Deutsch/English, Tooltips,
Tab-Reihenfolge und keine abgeschnittenen wichtigen Controls.

Validierung unter Windows PowerShell 5.1 mit dem sicheren Launcher: vollständiger
Release-Build inklusive WPF erfolgreich, 0 Warnungen/0 Fehler; beide Smoke-Testprojekte
einschließlich FR-OBS-001 erfolgreich. `git diff --check` erfolgreich. Nur synthetische
Lauf-Unterordner wurden verwendet; Themenformat und Benutzer-`.resx` bleiben unverändert.

## Manuelle Akzeptanz

Vom Repository aus nach grünem Validate-Lauf:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

Verlauf öffnen, Leerzustand bzw. ausschließlich synthetische Daten prüfen, neuen
Eintrag `CODEX TEST – Timeline Entry` mit Datum, Uhrzeit, Typ, Text und optionalem
synthetischem Thema speichern. Sofortige Anzeige, Refresh, Neustart mit demselben
Befehl und Deutsch/English prüfen; Fenster verkleinern und Tastatur/Tab bedienen.
Der Eintrag liegt ausschließlich in `.codex/synthetic-development-data/health-entries.json`.
Die vorhandene Benutzer-Wizard-`.resx` bleibt unverändert und untracked.

Nächster geplanter fachlicher Slice: Sources + SourceLocation + EvidenceNote.
