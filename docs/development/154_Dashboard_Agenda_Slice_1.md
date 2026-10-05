# 154 – Dashboard-Agenda Slice 1

Stand: 2026-10-05. Basis: `5e67c74d828c3f143a140faf699f0e876e00199a`.
Branch: `feat/winforms-dashboard-agenda-slice-1`.

## Anforderungen und Akzeptanz vor Implementierung

| Requirement | Semantik | Acceptance / Testspezifikation |
|---|---|---|
| FR-DASH-001 | Nicht archivierte Planned-Sessions mit ScheduledAt >= now; aufsteigend nach Instant, CreatedAt, Id. | UT-DASH-001: Zukunft/Gleichheit/Vergangenheit, Status, Archiv, deterministische Tie-Breaker bei umgekehrter Eingabe. |
| FR-DASH-002 | Open-Follow-ups nicht archivierter Sessions, auch Completed/Cancelled-Eltern; Gruppen Overdue, Today, Later, NoDate. | UT-DASH-002: vier Gruppen, Done/Archiv ausgeschlossen, Completed-Eltern erhalten; Datum, CreatedAt, Id als stabile Reihenfolge. |
| FR-DASH-003 | Explizites DateTimeOffset now. Termine vergleichen Instants; today = DateOnly.FromDateTime(now.DateTime), Kalenderdatum des gelieferten lokalen Offsets. | UT-DASH-003: feste Zeit, Mitternacht, Tageswechsel, verschiedene Offsets einschließlich gleicher Instants. |
| FR-DASH-004 | Read-only Projektion eines einzigen kohärenten SessionNotebook; kein Store, Cache oder Rewrite. Fehler weitergeben. | IT-DASH-001: Primärdatei/Backup und Dateiinventar unverändert; fehlender Store leer ohne Anlage; korrupter Store wirft statt leerer Agenda. |
| FR-DASH-005 | DE/EN, bestehende Counts/Themen, max. 5 Termine/6 Follow-ups, Empty/Loading/Error unterscheidbar, exakte Parent-/Child-Navigation, Show all, Refresh/Rückkehr und Stale-Schutz. | UI-DASH-001: reale Shell, Child-Tab/Selektion, vollständige Liste, Sprachwechsel, Return/Refresh, überlappende Loads/Fehler, No-write, 1120×740, Tastatur/Fokus/Clipping, synthetische Renderbilder. |

Bezug: FR-UI-001, FR-SES-002/007, PF-APT-006 und Pflichtenheft §13.2.
Vor UI-Implementierung werden diese Regeln in Application-Tests abgesichert.

## Architektur und Zeit

Domain und Infrastructure bleiben unverändert. SessionService liest genau einen
SessionNotebook-Snapshot; DashboardAgendaProjector projiziert ihn ohne Seiteneffekte.
Die UI liefert DateTimeOffset.Now am äußeren Ladebeginn, Tests feste Zeiten.
Die Terminanzeige verwendet Windows-Lokalzeit. DueDate ist ein Kalenderdatum,
keine Reminder-Zeit. Completed-Sessions dürfen offene Nachbereitung behalten.
Überfällig bedeutet ausschließlich Datum < today. Routine.ScheduleText wird nicht
geladen/geparst: Freitext ist keine strukturierte Wiederholung.

## UI und Navigation

Shell/Presenter bleiben erhalten. Kompaktere Kennzahlen, Themenliste in voller Breite,
darunter zwei gleich breite Agenda-Bereiche. Die Vorschau 5/6 begrenzt die Startseite;
vertikales Scrollen innerhalb der Listen ist bei Mindestgröße erlaubt, horizontales
Scrollen wird vermieden. Show all öffnet die vollständige read-only Agenda-Liste.
Ein Presenter verwirft stale Loads über Generationen. MainForm koordiniert Seiten
und sichere Fehleranzeige. SessionsPresenter erhält explizite Parent-/Child-Navigation;
Follow-up-Aktivierung öffnet den richtigen Tab und selektiert das konkrete Kind.
Keine Timer. Keine Dashboard-Fachlogik in Controls.

## Nicht-Ziele

Reminder, Notifications, Toasts, Hintergrundjobs, Wiederholungen, Snooze, Ruhezeiten,
automatische Statusänderungen, generische Task-/Zeitframeworks, ScheduleText-Parsing,
Nutrition, Weather, Media, Contacts, SQLite und Cloud. Keine Migration, neue
Persistenzbeziehung oder Cascading Deletes.

## Nachweise und manuelle Abnahme

Vor Änderung: Safe-Lauf grün, Release WinForms/WPF, Backend und DE/EN UI.
Erster Sandbox-Lauf: MSBuild-Tempzugriff verweigert; identischer eskalierter Lauf grün.
Geschützte Benutzerhashes unverändert. Weitere Nachweise folgen tatsächlichen Läufen.
Manuelle Abnahme offen. Ältere offene Nachweise aus Dokument 153 bleiben offen.

Application-Checkpoint: Release-Lösung 0 Warnungen/Fehler; Backend-Smoke einschließlich
UT-DASH-001/002/003 und IT-DASH-001 erfolgreich. WinForms-Agenda noch nicht implementiert.
Reale Persistenz wird nur gelesen; fehlender Store wird nicht angelegt, korrupter Store
wirft und bleibt unverändert. Bestehende Primär-/Backup-Dateien und Inventar bleiben gleich.
