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
mit bisheriger oder neuer Herkunft Doctor/Therapist/Coach/Source oder SourceId-Verweis wird diese Revision unter
demselben Writer-Lock und im selben atomaren Dateiaustausch wie die Action gespeichert.
So ist auch das Entfernen professioneller Herkunft nachvollziehbar. Rein selbst definierte Actions ohne Quellenverweis
haben kein Audit; dies ist keine globale Audit-Plattform. Archive/Reactivate ändern
nur den separaten Archivzustand, keine Anweisung und keine automatische Routinepause.
Quellen können professionelle Aussagen dokumentieren. Deshalb werden quellenbasierte
Änderungen konservativ ebenfalls geschützt, auch bei fehlender Quelle; keine inhaltliche
Interpretation und kein Race zwischen Source-Lesen und Action-Schreiben. Die Revision
speichert nur Action-Daten, keine Kopie einer Source oder Session. Die Historie löst
vorherige Verweise zu aktuellen Namen auf oder zeigt „nicht verfügbar“; damalige
Quellen-/Sessiontitel werden nicht versioniert. Revisionen sind sensible
Nutzerdokumentation im lokalen JSON, keine technischen Logs.

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

Backend-Checkpoint `c4b6854122adc0e3b8a7662946c6fad12088c180`: vollständiger Safe-Lauf
grün (Release WinForms/WPF ohne Warnungen/Fehler, beide Smoke-Projekte); gepusht und
Draft PR #18 erstellt: https://github.com/Robin-Goerlach/SASD-Health-Research-Notebook/pull/18.
Backend-CI erfolgreich. Zwei unabhängige CRITICAL-Prüfungen fanden Randfälle bei
Clock-Rollback, Offset-Korrektur, quellenbasierter Herkunft, Historienanzeige,
veralteter Timeline-Löschung und überlappenden Session-Refreshes; diese wurden korrigiert
und durch gezielte Regressionen abgesichert.
Manuelle Nutzerabnahme ist noch offen. Kein Ready, Merge oder weiterer Fachslice.

Offen: Source-Lebenszyklus, HealthTopic-Edit-UI, Routine-Archivierung, globale Audit-/Undo-/
Restore-Funktionen, Parent-Hard-Delete-Workflow und künftig neue Kindbeziehungen.
Bei neuen Beziehungen müssen die drei Löschregeln vor Erweiterung erneut geprüft werden.


## WinForms und manuelle Abnahme

Die sechs vorhandenen Create-Dialoge besitzen einen einfachen Create/Edit-Modus mit
vorbefüllten Feldern, eindeutigem Titel und „Änderungen speichern“. Unveränderte
Zeitfelder erhalten Sekunden, Präzision und Originaloffset; fehlende optionale Verweise
werden sichtbar erhalten. Normale Arbeitslisten blenden archivierte Session-/Action-
Eltern aus; „Archivierte anzeigen“ und „Reaktivieren“ bleiben direkt im Arbeitsbereich.
„Historie“ zeigt frühere professionelle Inhalte, Herkunft und Änderungsgrund read-only.
Auswahl bleibt nach Bearbeitung nach ID erhalten; nach Ausblenden/Löschen folgt die
erste verbleibende Zeile oder ein deaktivierter Empty State. Keine Parent-Löschbuttons.

Delete zeigt Datensatzart plus Titel/Zeitpunkt und den Hinweis „Diese Aktion kann nicht
rückgängig gemacht werden“. Initialer Fokus, Enter und Escape wählen Abbrechen; nur
explizites „Löschen“ bestätigt. Ein inzwischen geänderter Datensatz wird mit einer
lokalisierten Refresh-Aufforderung abgewiesen; keine technischen IDs oder Inhalte im Log.

UI-LIF-001 verwendet tatsächliche Shell-Kommandos und Dialoge in DE/EN, prüft
No-op-Speichern/Präzision, Edit/Refresh/Restart, professionelle Historie samt Link-only-
Korrektur, Archivfilter/Reactivate, Cancel/Confirm, Default-Cancel, ID-Auswahl,
Mindestgröße/Button-Clipping, Sprachwechsel und Dashboardcounts. Ein verzögertes
Session-Repository-/Topic-Read simuliert überlappende Refreshes; die alte Antwort
darf den archivierten Datensatz nicht wieder einblenden. Alle bisherigen Regressionen
(insbesondere SourceLocation-/Session-Reload und Action-Sprachwechsel) bleiben aktiv.

Vor manueller Abnahme den sicheren Release-Launcher aus dem Repository starten:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

Nur synthetische Datensätze mit Kennzeichnung `CODEX TEST` verwenden:

1. Measurement mit Wert/Zeit/Thema/Notiz bearbeiten; Refresh und Auswahl prüfen.
   Löschen erst abbrechen (Datensatz bleibt), dann ausdrücklich bestätigen (verschwindet).
2. HealthEntry-Titel und Inhalt bearbeiten; Refresh prüfen; Cancel/Confirm beim Löschen.
3. Session mit Frage und Follow-up bearbeiten; archivieren, standardmäßig ausgeblendet;
   „Archivierte anzeigen“, reaktivieren; Fragen/Antworten/Follow-ups und Status bleiben.
4. HealthAction mit Routine/Progress bearbeiten, archivieren/reaktivieren; Kinder bleiben.
5. Synthetische Doctor/Therapist/Coach- oder quellenbasierte Maßnahme korrigieren,
   optional Herkunftsverweis entfernen; „Historie“ zeigt alten Inhalt, alte Herkunft,
   vorherige Verweise und optionalen Änderungsgrund. Keine medizinische Neubewertung.
6. Routine-Titel/Rhythmus bearbeiten, pausieren/reaktivieren; Progress bleibt erhalten.
7. Progress-Notiz/Status/Anzahl bearbeiten; Löschung abbrechen und bestätigen.
8. Abhängigkeitsschutz: Session mit Fragen/Follow-ups, Action mit Routine/Progress,
   Routine mit Progress und Source mit Fundstellen haben keinen Hard-Delete-Befehl.
   Es ist daher keine blockierte Parent-Delete-Meldung zu provozieren. Keine Kaskade.
9. Neustart über denselben Launcher: Korrekturen/Archive/Historie erhalten, Gelöschtes weg.
10. DE/EN wechseln: korrekte Texte, gewählte Datensätze und Kinder erhalten.
11. Dashboard: archivierte Actions nicht aktiv, pausierte Routinen nicht aktiv,
    gelöschter heutiger Progress nicht mehr gezählt; keine medizinische Interpretation.
12. Mindestfenster 1120×740, Tab-Reihenfolge und Fokus prüfen; alle Buttons erreichbar,
    Enter im Delete-Dialog bricht ab, keine wichtigen Texte/Buttons abgeschnitten.

Manuelle Nutzerabnahme ist ausdrücklich ausstehend. Automatisierte Controls und
synthetische Renderbilder ersetzen sie nicht. PR bleibt Draft; kein Merge.

## Abschließende automatisierte Prüfung (2026-10-04)

Vollständiger `Invoke-SafeDevelopment.ps1`-Lauf erfolgreich: Release WinForms/WPF,
0 Warnungen/Fehler, Backend-Smoke-Tests und sämtliche WinForms-Smoke-Tests in DE/EN.
Der sichere Launcher startete beide Release-Frontends mit isoliertem synthetischem
Datenpfad; beide Fenster reagierten und wurden mit erfolgreichem Launcher-Exit beendet.
Synthetische Edit-/Historienbilder und die Arbeitsfläche bei 1120×740 wurden geprüft.
Die abschließende unabhängige read-only CRITICAL-Verifikation fand keine verbleibenden
Fehler in den korrigierten Randfällen. `git diff --check` erfolgreich.

Geschützte Benutzerdateien bleiben unverändert und außerhalb der Commits:
`.gitignore` SHA-256 `9A84AA3B5E4E9DF41A745F8BF9288C1059948047061503A8634C574DB788A3C6`,
`CreateHealthTopicWizardForm.resx` SHA-256
`4363CD7D5B8671C72442CE1A1BFC10D64EBD24B2D718B54BD4FCD025E4967298`.
`.codex` bleibt ignoriert, ohne getrackte Dateien. Ausschließlich synthetische Testdaten.
