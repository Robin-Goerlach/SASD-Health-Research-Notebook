# 152 – HealthAction + Routine + Progress Slice 1

Stand: 2026-10-02. In Entwicklung; manuelle Abnahme ausstehend. Start von main
`6bd99137f4d529568bedb1c9e3ad004bc8df28e6`.

## Modell und begrenzter Umfang

HealthAction ist selbst erfasste Dokumentation: Titel (160), optionale HealthTopicId,
ActionType (Movement, Nutrition, Sleep, Relaxation, Organization, Other), Active/Inactive,
Beschreibung (4000), berichtete Herkunft (SelfDefined, Doctor, Therapist, Coach, Source,
Other), optionale Herkunftsnotiz (4000), Id und technische Zeitstempel.
Herkunft ist Nutzerdokumentation, keine professionelle Bestätigung. Inhalt/Herkunft
sind create-only; keine Bearbeitung professioneller Anweisungen ohne Auditkonzept.
Routine ist ein getrenntes Objekt mit genau einer HealthActionId, Titel (160),
Beschreibung (4000), optionalem ScheduleText (160), Active/Paused und Zeitstempeln.
ProgressEntry gehört genau einer Routine: OccurredAt mit Offset, Completion
Performed/NotPerformed/Skipped, eigene Note (4000), Id/CreatedAt/ModifiedAt.
Keine automatische Fortschritts- oder Wirkungsbewertung. Pausieren/Reaktivieren
ändert nur Routinenstatus; historische Dokumentation bleibt möglich und erhalten.

FR-ACT-001/002/005 und Teile FR-ACT-004, FR-ROU-001/002/003/004/006/007.
ScheduleText beschreibt auch tägliche/wöchentliche/ausgewählte Tage, wird aber nicht
als Terminplan interpretiert. Die größere Baseline bleibt Zielmodell: eigenständige
Routinen, strukturierte Frequenzen, Zielquoten, Zähler/Mengen/Dauer/Messaufgaben,
Zeiträume, Session-/Source-IDs, Bearbeiten/Audit, Reminder, Export und allgemeine
Timeline bleiben spätere Arbeit. Kein Medication-/Treatment-Typ, keine Dosislogik.

## Application und JSON

HealthActionService erstellt/lädt Actions, Routinen und historischen Progress,
ändert Routinenstatus und liefert reine Dokumentationszahlen. Actions werden nach
CreatedAt absteigend und Id sortiert; Routinen nach CreatedAt/Id aufsteigend;
History nach OccurredAt absteigend, CreatedAt absteigend, Id. Themenbezug wird bei
Anlage geprüft, aktueller Titel nur zur Anzeige aufgelöst. Archivierte Themen sind
zulässig; später fehlende Themen verlieren keine Action. Kein Cascading Delete.

Ein gemeinsamer Store `health-actions.json` enthält Actions, Routines, ProgressEntries,
Kennung `SASD.HealthNotebook.HealthActions`, Version 1. Die enge Eltern-/Kindbeziehung
wird damit in einem atomaren Austausch konsistent gehalten. Application und Repository
prüfen Referenzen; alle IDs pro Sammlung sind eindeutig. Required-Felder, Domainregeln,
Kennung/Version und unbekannte Felder werden strikt geprüft. Fehlerdiagnostik enthält
keine Inhalte. Bestehende fünf Stores bleiben bytegenau unverändert.

Begleiter: `health-actions.json.tmp`, `health-actions.backup.json`,
`health-actions.json.lock`. Exklusive CreateNew-Lock/Temp-Anlage, Flush und atomarer
Austausch, letztes gültiges Backup. Beschädigte/fremde/zukünftige Stores, fremde
Backups und vorhandene Temp-/Lock-Dateien werden erhalten und Schreibversuche
abgewiesen. Fehlende Primärdatei mit Backup wird nicht als leerer Store ausgegeben;
keine stille Recovery. Reparse Points werden abgewiesen. Die bekannte Link-Race-
Einschränkung der bisherigen Stores bleibt; JSON ist weiterhin unverschlüsselt.
Alle vier Dateinamen werden vom PowerShell-5.1-Safe-Launcher geprüft.

## Geplante Oberfläche und Abnahme

WinForms bleibt primär, WPF buildbare Referenz. Die neue Seite verwendet vorhandene
Navigation, Styling, Karten und Dialogbasis. Drei kompakte Dokumentationszahlen,
Actions links, Routinen rechts oben und History rechts unten; proportionale Panels
mit gemeinsamer Abstand-/Rahmensprache. Keine medizinischen Vorschläge.
Automatisierte Nachweise und manuelle Prüfliste werden vor Nutzerprüfung ergänzt.
PR bleibt Draft; kein Merge und kein nächster Slice.
