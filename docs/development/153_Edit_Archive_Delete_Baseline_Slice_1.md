# 153 – Edit / Archive / Delete Baseline Slice 1

Start: `87438eced2ba770c943a2ae87ff906354d07f584` (main = origin/main).
Branch: `feat/edit-archive-delete-baseline-slice-1`. PR bleibt Draft bis manueller Abnahme.
Anforderungen: FR-LIF-001/002/003, FR-ACT-006, FR-GEN-003/005/007, FR-DEV-001.

## Lebenszyklusentscheidung

| Objekt | Bearbeiten | Archivieren / Reaktivieren | Hard Delete |
|---|---|---|---|
| Measurement | Werte, Zeit, Typ, Thema, Kontext, Notiz | nein | einzeln nach Bestätigung |
| HealthEntry | Titel, Inhalt, Zeit, Typ, Thema | nein | einzeln nach Bestätigung |
| Session | Metadaten und eigene Notiz | separates IsArchived, Status bleibt | nicht angeboten, auch ohne Kinder |
| HealthAction | Inhalt, Status, Herkunft und explizite Verweise | separates IsArchived, Status bleibt | nicht angeboten, auch ohne Kinder |
| Routine | Titel, Beschreibung, Rhythmus, Status | Pause/Reaktivieren bleibt; kein zusätzliches Archiv | nicht angeboten, auch ohne Progress |
| ProgressEntry | Zeitpunkt, Durchführung, Anzahl, Notiz | nein | einzeln nach Bestätigung |
| HealthTopic | vorhandene Domain-UpdateDocumentation vorbereitet; UI unverändert | vorhandener Status Archived | kein Hard Delete |
| Source / SourceLocation / EvidenceNote | unverändert; eigene Folgearbeit | unverändert | nicht angeboten |

Routine-Pause ist gemäß Dokument 152 keine Archivierung. Der ausdrückliche Slice-Auftrag
erlaubt die begrenzte Pause-Baseline; eine echte Routine-Archivierung bleibt Folgearbeit.
Die historische Hard-Delete-Einschränkung in Datenmodell §7 wird durch die neueren
FR-LIF-003-Akzeptanzkriterien auf die drei aktuell kinderlosen Korrekturdatensätze präzisiert.
Es gibt keine Parent-Delete-API und somit keine mögliche Kaskade bei Fragen, Follow-ups,
Routinen, Progress, SourceLocations, EvidenceNotes oder Herkunftsreferenzen.
Ein „blockiertes Delete“ wird hier durch Nichtverfügbarkeit der Parent-Löschaktion erfüllt;
keine scheinbar verfügbare Aktion, die erst nach einer Bestätigung scheitert.

## FR-ACT-006 und Datenintegrität

`HealthActionRevision` enthält Id, HealthActionId, ChangedAt, den vollständigen vorherigen
HealthAction-Snapshot samt Herkunft/Verweisen und optional ChangeReason. Bei Änderungen
mit bisheriger oder neuer Herkunft Doctor/Therapist/Coach wird diese Revision unter
demselben Writer-Lock und im selben atomaren Dateiaustausch wie die Action gespeichert.
So ist auch das Entfernen professioneller Herkunft nachvollziehbar. Normale Actions
haben kein Audit; dies ist keine globale Audit-Plattform. Archive/Reactivate ändern
nur den separaten Archivzustand, keine Anweisung und keine automatische Routinepause.
Revisionen sind sensible Nutzerdokumentation im lokalen JSON, keine technischen Logs.

Updates erhalten Id und CreatedAt. Routine-/Progress-Eltern können nicht gewechselt
werden; optionale Verweise können bewusst geändert werden. Unveränderte, inzwischen
fehlende optionale Verweise werden erhalten. Neu gewählte Verweise müssen existieren.
ModifiedAt steigt bei Änderungen. Unverändertes Speichern schreibt weder Store noch
Backup. ExpectedModifiedAt verhindert das Überschreiben oder Löschen eines inzwischen
geänderten Datensatzes. Fehlende/veraltete Datensätze werden abgewiesen.

## JSON-Kompatibilität

Measurement-/Entry-Stores bleiben Version 1. Sessions und HealthActions lesen Version 1
und 2. In v1 bedeutet fehlendes IsArchived ausdrücklich false; fehlende Revisionsliste
bedeutet leer. In v2 ist die Revisionsliste erforderlich. Lesen schreibt nichts.
Erst erfolgreiche explizite Änderungen speichern Sessions/Actions als Version 2;
das atomare Backup behält die vorherige gültige Datei, auch v1, bytegenau.
Alte Programme können v2 nicht lesen; kein Downgrade. Keine SQLite-Migration.

Dateinamen bleiben unverändert: vorhandene Primär-/Backup-/Temp-/Lock-Dateien in denselben
sechs Stores. Daher sind keine neuen Safe-Launcher-Dateiguards nötig. Windows PowerShell
5.1 und ausschließlich synthetische Daten unter `.codex/` bleiben verbindlich.
Beschädigte/fremde/zukünftige Stores, fremde Backups und bestehende Temp-/Lock-Dateien
werden nicht überschrieben oder bereinigt. Kein automatisches Recovery.

## Nachweise und offene Arbeit

IT/SEC/MIG-LIF-001 (`LifecycleTests`) prüft Korrekturen/Metadaten, Löschung/Reload,
Archivroundtrip, Kind-/Referenzerhalt, professionelle Vorinhalte, fehlende Parent-Delete-APIs,
veraltete Editoren, No-op-Bytes, Backup, Temp/Lock und Corruption sowie v1-Lesen ohne
Rewrite/v2-Schreiben mit v1-Backup. Bestehende Regressionen bleiben Teil des Safe-Laufs.

Backend-Checkpoint und UI-Nachweise werden nach grünen Läufen ergänzt.
Manuelle Nutzerabnahme ist noch offen. Kein Ready, Merge oder weiterer Fachslice.

Offen: Source-Lebenszyklus, HealthTopic-Edit-UI, Routine-Archivierung, globale Audit-/Undo-/
Restore-Funktionen, Parent-Hard-Delete-Workflow und künftig neue Kindbeziehungen.
Bei neuen Beziehungen müssen die drei Löschregeln vor Erweiterung erneut geprüft werden.
