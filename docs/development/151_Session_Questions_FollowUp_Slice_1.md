# 151 – Session + Questions + Follow-up Slice 1

Stand: 2026-10-01. In Entwicklung; keine manuelle Akzeptanz und nicht mergefertig.

## Modell und Abgrenzung

`Session` mit `DateTimeOffset ScheduledAt`, Titel (160 Zeichen), optionalem
HealthTopic, Kontaktfreitext (160), eigener Notiz (4000), Typ und Status.
Typen: DoctorVisit, Checkup, Coaching, Physiotherapy, NutritionConsultation,
OtherConsultation. Status: Planned, Completed, Cancelled. Keine automatischen Übergänge.
SessionQuestion und SessionFollowUp gehören jeweils genau einer Session.
Frage und Antwortnotiz sind getrennte Texte (je 4000); explizite SortOrder und
IsAnswered. Follow-up ist ein selbst eingegebener nächster Schritt (4000), Open/Done.

FR-SES-001/002 sowie Teilumfang von FR-SES-003/004/007/008 sind betroffen.
Dokument-/Messwert-/Quellenverknüpfungen, Kontakte als eigenes Modul,
Herkunftsmarkierungen FR-SES-005, HealthActions FR-SES-006 und Export FR-SES-009
bleiben außerhalb dieses Slices. Unabhängiger Frageeingang aus PF-APT-002 ist
ebenfalls spätere Arbeit; hier sind Fragen ausdrücklich sessionbezogen.
PF-APT-006 nennt Fälligkeit für Follow-ups: eine optionale Kalenderfälligkeit wird
im Slice ergänzt, ohne Reminder oder automatische medizinische Handlungsableitung.

## Persistenz und Use Cases

Ein gemeinsamer versionierter `sessions.json`-Store enthält Sessions, Questions,
FollowUps. Kennung `SASD.HealthNotebook.Sessions`, Version 1. Er hält Eltern/Kinder
atomar konsistent; keine redundanten Titelkopien oder TimelineEvent-Duplikate.
Application und Repository prüfen Elternreferenzen. Antwort-/Statusupdates erfolgen
unter dem Writer-Lock an den neuesten Datensätzen und ändern nur diese Felder.
Bestehende Stores erhalten ihr Format und werden nicht beschrieben.

Analog zum Sources-Store: `sessions.json.tmp`, `sessions.backup.json`,
`sessions.json.lock`, exklusive Anlage, Flush und atomarer Austausch. Kennung,
Version, Pflichtfelder, Domainregeln, unbekannte Felder, Duplikate und verwaiste
Kinder werden geprüft. Beschädigte/fremde/zukünftige Stores werden nicht überschrieben.
Kein automatisches Recovery oder Löschen vorhandener Temp-/Lock-Dateien.
Gemeinsamer `LocalHealthNotebookPaths`/`SASD_HEALTHNOTEBOOK_DATA_PATH` und vier
PowerShell-5.1-Dateiguards. Nur synthetische Daten unter `.codex/`; bekannte
Dateisystemlink-Race-Condition bleibt akzeptierte Einschränkung. JSON unverschlüsselt.

SessionService: Erstellen, chronologisch laden, Details kohärent laden, Frage und
Follow-up hinzufügen, Antwort/offen ändern und Follow-up erledigen/wieder öffnen.
Optionaler Topic-Bezug; aktuelle Titel erst bei Anzeige auflösen. Kein Cascading Delete.
Fragen/Antworten/Nachbereitung sind Nutzerdokumentation, keine medizinisch bestätigten Fakten.

## Nachweise und offenes Gate

UT/IT/SEC-SES-001 im gemeinsamen Smoke-Testprojekt prüft Validierung, Beziehungen,
unabhängiges Reload, Sortierung, Statusänderungen, Store-Guards, Backup, Temp/Lock
und bytegenauen Erhalt der vier bestehenden Stores. SEC-SES-002 erweitert den echten
PowerShell-5.1-Launcher-Test um alle vier Session-Dateinamen.
WinForms und manuelle Akzeptanz werden im nächsten kohärenten Zwischenstand ergänzt.
Der Draft-PR dient Backup/CI; keine Freigabe zum Merge.

Keine medizinische Bewertung, Diagnose, Therapie-, Medikamenten-/Dosislogik,
Kalenderintegration, Benachrichtigung, Cloud-Sync oder generische Workflow-Engine.
