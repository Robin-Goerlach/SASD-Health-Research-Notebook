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

Die erste Abschluss-CI zeigte zusätzlich einen anzahlabhängigen bestehenden
Action-Navigationstest: dessen Lade-Erkennung akzeptierte nur bestimmte Datensatzzahlen.
Der Helfer wartet nun auf den lokalisierten Abschluss unabhängig von der Anzahl;
die fachlichen Lifecycle-Tests waren in beiden CI-Läufen bereits erfolgreich.

## Measurement-Dialog: Korrektur aus manueller Abnahme

FR-MEA-001/002, UI-MEA-002: Im Create-Modus konnte die Feldinitialisierung vor der
ComboBox-Bindung abbrechen. Das primäre Label blieb leer; die Blutdruckfelder behielten
ihre Standardsichtbarkeit. Das bisher gemeinsam verwendete Value-/Systolic-Control
erschwerte zudem die eindeutige Zuordnung. Die bisherigen Tests wechselten zuerst den
Typ und maskierten damit den fehlerhaften Startzustand.

Create und Edit initialisieren die Felder jetzt nach Bindung im Load-Ereignis.
`UpdateMeasurementFieldVisibility` steuert Labels, Sichtbarkeit, Enabled, TabStop und
Zeilenhöhe. BloodPressure verwendet drei eigene numerische Controls: Systolic,
Diastolic und optional Pulse. Das allgemeine Value-Control ist verborgen/deaktiviert.
Pulse, Temperature, BloodGlucose und Weight verwenden ausschließlich Value mit passender
Beschriftung/Einheit. Ein echter Typwechsel leert alle Zahlen; erneute Layoutanwendung
erhält relevante Eingaben. Save berücksichtigt ausschließlich typrelevante Controls.
Keine kombinierte Blutdruckeingabe, keine Änderung des strukturierten Domain-/JSON-Modells.

UI-MEA-002 prüft in DE/EN den frisch geöffneten Dialog, sichtbare Labels/Controls,
tatsächliche Tab-Traversierung, Create/Reload und Edit aller fünf Typen, exakt getrennte
Blutdruckwerte 130/60/60, No-op-Edit, optional leeren Puls und die vier angeforderten
Typwechsel. Bewusst ungültig befüllte versteckte Controls dürfen nicht gespeichert
werden. Renderbilder für alle Create-/Edit-Typen liegen ausschließlich im isolierten
synthetischen Testverzeichnis unter `.codex`. Die erneute manuelle Nutzerabnahme bleibt
offen; PR #18 bleibt Draft.

Korrekturprüfung: vollständiger Windows-PowerShell-5.1-Safe-Lauf grün, Release WinForms
und WPF ohne Warnungen/Fehler, Backend- und alle WinForms-Smoke-Tests in DE/EN inklusive
UI-MEA-002, Lifecycle, SourceLocation, Session, HealthAction und Dashboardcounts.
BloodPressure-Create-/Edit-Renderbilder in beiden Sprachen bei Mindestgröße visuell
geprüft: exakt drei korrekt beschriftete Felder, kein sichtbares Einzelwertfeld.

## Measurement-Type-Filter aus manueller Abnahme

Die korrigierte Blutdruckerfassung ist vom Nutzer manuell akzeptiert. FR-MEA-008 und
UI-MEA-003 ergänzen einen kompakten ComboBox-Filter direkt neben Edit/Delete über der
Measurement-Liste: Messart / Measurement type, Alle / All und die fünf bekannten Typen.
`MeasurementsView` behält die vollständige vom Presenter geladene Liste und projiziert
nur die sichtbaren Zeilen. Keine Domain-/Service-/JSON-Filterlogik, keine Schreiboperation,
keine neue Datei oder Settings-Persistenz; Neustart beginnt mit Alle.

Bei Filterwechsel bleibt eine noch sichtbare ID ausgewählt; andernfalls folgt die erste
sichtbare Zeile oder „Keine Messwerte dieser Art vorhanden.“ / „No measurements of this
type.“ mit deaktiviertem Edit/Delete. Edit, Delete und Refresh erhalten den Filter.
Ändert Edit die Messart, kann der Datensatz aus der aktuellen Ansicht verschwinden,
bleibt aber in der vollständigen Liste. Der Filter speichert den Enum-Wert; Sprachwechsel
erneuert ausschließlich die Beschriftungen, ohne zwischenzeitlichen All-Filter.

UI-MEA-003 prüft in DE/EN Default/alle fünf Typen, vollständige Daten nach Rückkehr zu
Alle, Auswahl, Edit inklusive Typänderung, Delete Cancel/Confirm, Refresh, Sprachwechsel,
Empty State, Neustart, Mindestgröße, Tab-Reihenfolge und Clipping. Store-Dateien bleiben
bei reinen Filter-/Refresh-/Sprachaktionen bytegenau erhalten; keine neue Store-Datei.
Manuell noch prüfen: Filter im Alltag wechseln, Empty State und Auswahl nach Edit/Delete
beobachten, DE/EN bei Mindestgröße prüfen. PR bleibt Draft, kein Merge.

Filterprüfung: vollständiger Safe-Development-Lauf unter Windows PowerShell 5.1 grün;
Release WinForms/WPF ohne Warnungen/Fehler, Backend und alle WinForms-Regressionen samt
UI-MEA-002/003, Lifecycle, Dashboard, Sessions, Sources, HealthAction und SourceLocation.
DE/EN-Renderbilder des Filters bei 1120×740 und des Empty States visuell geprüft.
`git diff --check` erfolgreich; geschützte `.gitignore`/`.resx` unverändert.

## Erweiterung: HealthTopic und Session-Kinder

Der explizite Folgeauftrag ersetzt die frühere pauschale Topic-Hard-Delete-Sperre:
HealthTopic erhält Edit, Archive/Reactivate und bestätigtes Delete nur ohne Referenzen.
Session-, Action-, Routine- und Source-Hard-Delete bleiben nicht verfügbar.
FR-LIF-004/005; IT-LIF-002 / UI-LIF-002 (`ParentLifecycleTests` / `ParentLifecycleUiTests`).

Topic-Delete hält fail-fast alle sechs Store-Writer-Locks in fester Reihenfolge. Es prüft
Entry, Measurement, Source, Session, Action und `HealthActionRevision.Previous` auf
Topic-Verweise, einschließlich archivierter Eltern und rein historischer Referenzen.
Alle referenzierenden Writer prüfen neue/gewechselte Topic-IDs erneut unter ihrem eigenen
Lock. Somit erzeugt ein früher geladener Service-Snapshot nach Delete keinen verwaisten
neuen Verweis. Kein Cascade, keine Veränderung der anderen Stores. Korrupte/unbekannte
Stores sowie Backup-/Temp-/Lock-Probleme brechen sicher ab. Alte Legacy-Schreiber dürfen
nicht parallel mit dieser Version schreiben.

Topic-JSON bleibt die vorhandene nackte Liste. Optionales `StatusBeforeArchive` erhält
bei neuen Archivierungen den vorherigen Status; Legacy-Archive ohne dieses Feld werden
als Observation reaktiviert. Lesen migriert nichts. Unbekannte Felder werden abgewiesen,
um stillen Datenverlust beim Rewrite zu verhindern. Topic-Schreiben nutzt nun ebenfalls
CreateNew-Lock/Temp und atomaren File.Replace mit validiertem Backup. `SaveAllAsync` ist
nur Initialisierung einer leeren Sammlung, kein Bulk-Delete-Ausweg. Der sichere Launcher
prüft zusätzlich `health-topics.json.lock`; Windows PowerShell 5.1 bleibt unterstützt.

SessionQuestion-Edit erlaubt Text, Reihenfolge und explizite Antwortkorrektur. Hard Delete
ist nur bei IsAnswered=false UND ohne nichtleere AnswerNote möglich. Eine vorhandene
Antwortnotiz bleibt auch bei unmarkiertem Checkbox-Status geschützt. Edit beantworteter
Fragen ist erlaubt; eine bewusste Antwortkorrektur ist getrennt von Delete. Verständliche
DE/EN-Meldungen erklären die Sperre. Kein globales Audit. SessionFollowUp erhält Edit
von Text/Status/Fälligkeit und einzeln bestätigtes Delete. IDs, CreatedAt, Eltern und
übrige Kinder bleiben erhalten; ModifiedAt steigt, veraltete Tokens werden abgewiesen.

WinForms bietet Topic-Commands/Archivfilter sowohl im Arbeitsbereich als auch in der
Dashboard-Liste. Fragen-/Follow-up-Tabs ergänzen Edit/Delete. Der Bestätigungsdialog
bleibt Cancel-default. Tests prüfen Referenzarten einzeln, nur historische Links,
Writer-/Delete-Reihenfolgen, partielle Locks, Corruption, Unknown Fields/Legacy-Bytes,
Stale-Tokens, Antworten-/Kindererhalt, DE/EN-Dialoge, Cancel/Confirm und Restart.

Manuell prüfen: Topic Edit; Archive/Ausblenden/Anzeigen/Reactivate mit vorherigem Status;
unreferenziertes Topic Delete Cancel/Confirm; referenziertes Topic verständlich blockiert.
Frage Edit; unbeantwortete Frage Delete Cancel/Confirm; beantwortete Frage bleibt bei
Delete erhalten. Follow-up Text/Status/Fälligkeit Edit und Delete Cancel/Confirm.
Neustart, DE/EN, Tab und Mindestgröße 1120×740 prüfen. PR #18 bleibt Draft; kein Merge.

Abschlussprüfung der Erweiterung (2026-10-04): vollständiger Windows-PowerShell-5.1-Safe-Lauf grün, Release WinForms/WPF ohne Warnungen/Fehler, Backend und alle DE/EN-UI-Smoke-Tests einschließlich bestehender Measurement-/SourceLocation-/Session-/HealthAction-/Dashboard-Regressionen. Beide Frontends isoliert gestartet und geordnet beendet. Neue DE/EN-Renderbilder bei Mindestgröße visuell geprüft. Unabhängige CRITICAL-Prüfung abgeschlossen; Reload-Generationen schützen auch den letzten Dashboard-Await. Manuelle Nutzerabnahme dieser Erweiterung bleibt offen.
