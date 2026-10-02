# 151 – Session + Questions + Follow-up Slice 1

Stand: 2026-10-01. Implementiert und manuell akzeptiert.

## Modell und Abgrenzung

`Session` mit `DateTimeOffset ScheduledAt`, Titel (160 Zeichen), optionalem
HealthTopic, Kontaktfreitext (160), eigener Notiz (4000), Typ und Status.
Typen: DoctorVisit, Checkup, Coaching, Physiotherapy, NutritionConsultation,
OtherConsultation. Status: Planned, Completed, Cancelled. Keine automatischen Übergänge.
SessionQuestion und SessionFollowUp gehören jeweils genau einer Session.
Frage und Antwortnotiz sind getrennte Texte (je 4000); explizite SortOrder und
IsAnswered. Follow-up ist ein selbst eingegebener nächster Schritt (4000), Open/Done
mit optionaler `DateOnly DueDate` ohne Zeitzonenumrechnung oder Reminder.

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

## Nachweise

UT/IT/SEC-SES-001 im gemeinsamen Smoke-Testprojekt prüft Validierung, Beziehungen,
unabhängiges Reload, Sortierung, Statusänderungen, Store-Guards, Backup, Temp/Lock
und bytegenauen Erhalt der vier bestehenden Stores. SEC-SES-002 erweitert den echten
PowerShell-5.1-Launcher-Test um alle vier Session-Dateinamen.
UI-SES-001 prüft echte Dialoge, getrennte Antwortänderung, Follow-up-Status und
Fälligkeit, Themenbezug, frische Shell/Services und Wiederauswahl ohne Refresh
in Deutsch/English. WinForms nutzt die bestehenden Controls, Styling, Localization
und Dialogbasis. Termine links, Fragen/Nachbereitung in rechten Tabs, gemeinsame
proportionale untere Detailzeile. WPF erhält keine neue Session-Oberfläche.
Session-Metadaten sind im Slice create-only; Frageantwort und Follow-up-Status
sind gezielt änderbar. Die persönliche Session-Notiz wird beim Erstellen erfasst;
spätere Gesprächsantworten und nächste Schritte über die getrennten Kindobjekte.
PR #16 enthält den implementierten und manuell akzeptierten Slice; Merge bleibt ein separater Freigabeschritt.

Der vollständige Safe-Development-Lauf ist grün: Release einschließlich WPF,
0 Warnungen/0 Fehler, beide Smoke-Testprojekte und alle bestehenden Regressionen.
Synthetische Renderbilder bei Mindestgröße wurden geprüft; keine manuelle
Desktopakzeptanz wird daraus abgeleitet. Der frühe Backend-Checkpoint `8e38a3e`
wurde früh über Draft PR #16 gesichert; UI-Checkpoint `827bd1d` und manuelle Akzeptanz folgen auf demselben Branch.

## Manuelle Akzeptanz – bestätigt

Der Nutzer bestätigte am 2026-10-01: Navigation und verständlichen Empty State,
Sessions mit/ohne HealthTopic, neue Fragen und getrennte Antwortnotizen,
beantwortet/offen, Follow-ups mit optionaler Fälligkeit und erledigt/offen,
Sessionwechsel, Refresh, Erhalt nach Neustart, Deutsch/English, plausible Tooltips
und Tab-Reihenfolge sowie Layout ohne abgeschnittene wichtige Controls.

Computer Use bietet in dieser Sitzung keine native Desktopsteuerung. Automatisierte
Control-/Renderprüfungen ersetzen keine manuelle Nutzerakzeptanz.
Vom Repository aus nach grünem Validate-Lauf:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

1. Termine & Fragen / Sessions öffnen; Empty State oder ausschließlich synthetische Daten.
2. `CODEX TEST – Session` mit Datum/Uhrzeit, Typ/Status, optional synthetischem Thema
   und Kontakt/Notiz anlegen. Eine zweite Session ohne Thema anlegen.
3. Eigene synthetische Frage hinzufügen; Antwort separat dokumentieren, beantwortet
   markieren und wieder öffnen. Keine Aussage wird von der App bestätigt.
4. Synthetischen nächsten Schritt hinzufügen, optionales Datum prüfen, erledigen und
   wieder öffnen. Keine Benachrichtigung oder medizinischer Vorschlag.
5. Refresh und Wechsel zwischen den beiden Sessions ohne zusätzlichen Refresh prüfen.
6. Schließen und mit demselben Safe-Befehl neu starten; Fragen, Antwortnotiz und
   Follow-up/Status/Fälligkeit nach Wiederauswahl erhalten.
7. Deutsch/English, Tooltips, Tab/Enter/Escape sowie Mindestgröße ohne wichtiges Clipping.

Nur `.codex/synthetic-development-data/sessions.json` wird für neue Sessiondaten verwendet.
Benutzer-Wizard-`.resx` unverändert/untracked; `.codex/` nicht versioniert.

Keine medizinische Bewertung, Diagnose, Therapie-, Medikamenten-/Dosislogik,
Kalenderintegration, Benachrichtigung, Cloud-Sync oder generische Workflow-Engine.
