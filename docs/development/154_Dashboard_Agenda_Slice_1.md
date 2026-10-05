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
Ergänzung vor Umsetzung: FR-UI-002 / UI-NAV-001 fordert kurze DE/EN-Tooltips
für alle sieben Navigationseinträge, einschließlich Aktualisierung beim Sprachwechsel.
FR-UI-003 / UI-GRID-001 fordert lokale typgerechte Spaltensortierung
Ascending → Descending → Original mit stabilen Gleichständen, ID-Selektion und
nativen Pfeilen. Refresh ersetzt die Originalreihenfolge und erhält die aktive
Sortierung; Sprachwechsel erhält Spalte/Richtung und lokalisiert die Anzeige neu.
Tests prüfen Text, Datum, Zahl, Status, null/leer, Zyklus, Refresh, Sprache und
bytegenau unveränderte Stores in DE/EN. Manuelle Abnahme bleibt offen.
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

Die sechs bestehenden Counts stehen in einer einzigen proportionalen Reihe (132 px).
Zweizeilige Titel/Beschreibungen sind in DE/EN vorgesehen; Werte bleiben unverändert.
Der Themenbereich erhält 210 px für Überschrift, Lifecycle-Commands, Header und eine
vollständige umbrochene Zeile; übrige Fensterhöhe geht an die Agenda. 40-px-Agenda-Zeilen
zeigen Zeitpunkt bzw. Kalendergruppe/Datum und einen kurzen Text mit vollständigem
Tooltip. Bei 1120×740 bleibt mindestens eine vollständige Themenzeile sichtbar;
die Agenda-Vorschau kann vertikal gescrollt werden. Die Grenzen 5/6 halten sie auch
bei großen Datenbeständen klein; „Alle anzeigen“ öffnet eine eigene vollständige Liste.
Bei fehlendem Child oder zwischenzeitlich archiviertem Parent wird vor dem Binden
ein sicherer Konflikt ausgelöst; keine Auswahl eines zufälligen Ersatzkindes.

Generationen werden bereits zu Refresh-Beginn invalidiert, nicht erst nach anderen
Dashboard-Reads. Presenter schützen sowohl alte Ergebnisse als auch alte Fehler.
Session-Listenrefresh invalidiert auch laufende Child-Reads sofort. Keine Background-
Aktualisierung: Reload erfolgt beim Öffnen, explizitem Refresh, Sprachwechsel und
Rückkehr vom Arbeitsbereich. Eine über Mitternacht offen stehende Seite wird beim
nächsten dieser Vorgänge mit einer neuen Zeitbasis eingeordnet.

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
UT-DASH-001/002/003 und IT-DASH-001 erfolgreich. Zu diesem Checkpoint war die
WinForms-Agenda noch nicht implementiert. Commit `2aac233`, gepusht, Draft PR #19.
Reale Persistenz wird nur gelesen; fehlender Store wird nicht angelegt, korrupter Store
wirft und bleibt unverändert. Bestehende Primär-/Backup-Dateien und Inventar bleiben gleich.

## Offene manuelle Prüfliste

Start ausschließlich mit synthetischen Daten:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

1. DE/EN, 1120×740 und normale Größe: sechs bestehende Counts, Themenzeile und beide
   Agenda-Überschriften lesbar; keine horizontalen Scrollbars, ruhige Farben/Abstände.
2. Geplante kommende Termine sichtbar; archivierte, abgeschlossene, abgesagte und
   vergangene Termine fehlen. Kontakt/Institution steht optional im vollständigen Tooltip.
3. Open-Follow-ups in vier Kalendergruppen; Completed-Session darf offene Kinder zeigen.
   Done und archivierte Eltern fehlen. „Überfällig“ ist keine medizinische Warnung.
4. Klick und Enter öffnen exakt die Session; Follow-up öffnet Parent, Follow-up-Tab,
   konkretes Kind und Fokus. Tab erreicht die Listen und „Alle anzeigen“.
5. Mehr als 5/6 Einträge: begrenzte Vorschau, vertikales Scrollen, vollständige Liste
   über „Alle anzeigen“, Ziel außerhalb der Vorschau exakt öffnen; Escape schließt.
6. Follow-up im Session-Arbeitsbereich erledigen/bearbeiten; Rückkehr und Refresh
   zeigen aktuelle Daten. Sprachwechsel erhält Projektion/Identität und übersetzt Texte.
7. Leerer Bestand zeigt klare Empty States. Ladefehler zeigt Fehlerzustand und den
   vorhandenen sicheren Dialog, niemals einen erfolgreich leeren Bestand.
8. Neustart lädt vorhandene Daten weiter. Kein neuer Store/Cache/Reminder, keine
   Änderung vorhandener Dateien allein durch Lesen. DPI/Fokus bei realem Desktop prüfen.

Diese Liste ist keine Abnahmebestätigung. Automatische Controls und Renderbilder
ersetzen weder die Nutzerabnahme dieses Slices noch offene ältere Nachweise aus 153.

## Abschlussvalidierung vor manueller Abnahme (2026-10-05)

Vollständiger Windows-PowerShell-5.1-Safe-Lauf Exit 0: Release-Lösung einschließlich
WinForms/WPF 0 Warnungen/Fehler, Backend und sämtliche DE/EN-WinForms-Regressionen grün.
UT-DASH-001/002/003, IT-DASH-001 und UI-DASH-001 sind automatisiert nachgewiesen.
Zusätzliche Randfälle: fehlendes konkretes Child, Parent-Archivierung zwischen Listen-
und Detailread, überlappende Child-Navigation, veralteter Erfolg/Fehler und Navigation-
away-Invalidierung. Kein Ersatzkind wird bei fehlendem Ziel gebunden.
Zelltext-Clipping wird anhand Font, Textmaß und Padding geprüft; zweizeilige Datums-
angaben benötigen 40 px statt der zunächst versuchten 32 px. Deutsche kompakte
Themenkarte heißt „Themen“, um keinen Wortumbruch mitten im Titel zu erzwingen.

Synthetische Renderbilder: `.codex/synthetic-development-data/ui-tests/21930f945ee64815b9c3cb4494750907/agenda-English`
und `agenda-German`. Gefüllte/leere Mindestansichten, 1280×820 und vollständige
Listen wurden visuell geprüft. Diese Prüfung ist keine manuelle Nutzerabnahme und
kein vollständiger High-DPI-Nachweis. Keine Bilder mit echten Gesundheitsdaten.

`git diff --check` erfolgreich. Geschützte Benutzerdateien bleiben außerhalb des Index:
`.gitignore` SHA-256 `9A84AA3B5E4E9DF41A745F8BF9288C1059948047061503A8634C574DB788A3C6`;
`CreateHealthTopicWizardForm.resx` SHA-256
`4363CD7D5B8671C72442CE1A1BFC10D64EBD24B2D718B54BD4FCD025E4967298`.
`.codex/` bleibt ignoriert. Kein Ready for Review, Merge oder nächster Slice autorisiert.

## CI-Korrektur zur Mindestgröße

Beide CI-Läufe des UI-Commits `98a4b2d` scheiterten an der neuen vollständigen
Themenzeilenprüfung bei Mindestgröße, trotz lokal grünem Safe-Lauf. Build, Backend
und alle zuvor durchlaufenen UI-Regressionen waren grün. Die zunächst prozentuale
48/52-Aufteilung bot dem Runner nicht genug Höhe. Der Themenbereich reserviert nun
210 px, zusätzliche Höhe erhält die Agenda. Die Prüfung bleibt unverändert streng
und meldet Grid-/Header-/Zeilen-/Clienthöhe und DPI ohne Nutzerdaten. Keine
Anpassung der Persistenz oder Absenkung des Akzeptanzkriteriums.

Nach der Korrektur vollständiger Safe-Lauf erneut grün (Exit 0, Release WinForms/WPF
0 Warnungen/Fehler, Backend und sämtliche DE/EN-UI-Tests). Die neuen synthetischen
Mindestgrößen-Renderbilder in beiden Sprachen wurden visuell geprüft; Hashes bleiben
gleich. Die abschließende Remote-CI wird am neuen Head des weiterhin offenen Draft-
PR #19 geprüft, bevor die Arbeit zur manuellen Abnahme zurückgegeben wird.

## Ergänzung: Navigationstooltips und dreistufige Tabellen-Sortierung

FR-UI-002 / UI-NAV-001: Dashboard, Gesundheitsthemen, Verlauf, Quellen,
Messwerte, Sessions und Maßnahmen besitzen zentral in AppStrings.Navigation
lokalisierte Kurzbeschreibungen. NavigationControl verwaltet eine native ToolTip-
Komponente, aktualisiert sie gemeinsam mit den Beschriftungen und entsorgt sie.
Keine neuen Tooltips an sonstigen Controls, keine medizinische Bewertung.

FR-UI-003 / UI-GRID-001: ThreeStateGridSort ist eine ausschließlich WinForms-seitige
Hilfe. Jede View registriert ihre Datenspalten mit expliziten typisierten Schlüsseln.
Keine Reflection, Text-zu-Datum-Konvertierung, Persistenz- oder Presenteränderung.
Ein anderer Spaltenkopf beginnt mit Ascending. Wiederholte linke Klicks auf denselben
Kopf wechseln Ascending → Descending → Original; danach beginnt der Zyklus erneut.
Nur die aktive Spalte hat einen nativen SortGlyphDirection-Pfeil. Original hat keinen.

Die separat kopierte Originalreihenfolge wird niemals umsortiert. Bei Gleichständen
entscheidet der Originalindex, auch absteigend. Datum/Zeit verwendet DateTimeOffset-
Instants, reine Daten verwenden DateOnly; Zahlen werden numerisch verglichen.
Enums behalten ihre definierte Enum-Reihenfolge über Sprachwechsel hinweg;
archivierte Sessions/Maßnahmen folgen den nicht archivierten Statuswerten aufsteigend.
Fragen sortieren Open vor Answered. Text ist DE-/EN-kulturgerecht und ignoriert
Groß-/Kleinschreibung. null/leer sind bei Text gleich und behalten ihre Reihenfolge;
fehlende optionale Zahlen/Daten stehen aufsteigend vor vorhandenen, absteigend danach.

Messwert-Werte gruppieren zunächst nach Messart, dann nach unveränderten Zahlen:
Einzelwert bzw. systolisch, danach diastolisch und optionaler Puls. Unterschiedliche
Messarten/Einheiten werden nicht als vergleichbare gesundheitliche Größen behandelt.
Fundstellen bleiben Freitext (auch Seitenbereiche), ohne vermeintliche Zahlenauswertung.
Notizsortierung verwendet den vollständigen Text, nicht die abgeschnittene Vorschau.
Agenda-Datumsgruppen verwenden Gruppenum und DueDate, keine lokalisierten Strings.

Unterstützt werden alle bestehenden DataGridViews: Themen (auch Dashboard), Timeline,
Messwerte, Quellen, Fundstellen, EvidenceNotes, Sessions, Fragen, Follow-ups, Maßnahmen,
Routinen, Durchführungseinträge sowie beide Agenda-Vorschauen und vollständige Agenda-
Dialoge. Die Action-Historie ist RichTextBox-Dokumentation, keine Tabelle. IDs werden
nicht als Spalten angezeigt; keine Button-/Dekorationsspalte wird registriert.

Refresh ersetzt die Originalreihenfolge durch die neu gelieferte View-/Presenterfolge.
Die aktive semantische Spalte/Richtung wird erneut angewandt; Original zeigt die neue
Folge exakt. Sprachwechsel lokalisiert Anzeigezeilen neu und erhält Spalte/Richtung.
Sortierung einer Agenda-Vorschau betrifft ausschließlich die fachlich ausgewählten
5/6 Einträge; sie wählt keinen anderen Ausschnitt aus dem vollständigen Bestand.

Die Auswahl wird über stabile Datensatz-ID erhalten, nicht über den Zeilenindex.
Nicht mehr sichtbare Datensätze folgen der bisherigen View-Semantik (erster sichtbarer
Datensatz bzw. keine Auswahl). IsRebinding unterdrückt Zwischenereignisse bei Parent-
und Routinen-Selektion; Details werden erst nach Wiederherstellung der ID aktualisiert.
Damit startet visuelle Sortierung keinen falschen Parent-/Child-Reload. Die Hilfe kennt
weder Services noch Stores. Domain/Application/Infrastructure/WPF bleiben unverändert.

GridUxTests prüfen alle Grids in DE/EN mit realen Header-Ereignissen: sechs Klicks
einschließlich viertem Klick, exakt rückkehrende Originalfolge, Einzelpfeil, Auswahl-
ID. Zusätzlich konkrete Text-, Instant-/Offset-, Enum-/Bool-, Zahlen-, strukturierte
BloodPressure- und null/leer-Reihenfolgen; Refresh übernimmt neue Baseline, Sprache
erhält Sortierzustand, Parent-Sortierung löst keine Zwischenloads aus, vollständige
Agenda-Dialoge bleiben vollständig, Dateiinventar/Storebytes bleiben unverändert.
Synthetische Renderbilder für Themen, Messwerte, Sessions und Agenda werden erzeugt.

### Noch offene manuelle UX-Abnahme

- Alle sieben Navigationstooltips in DE/EN: tatsächliches Hover-Popup, Lesbarkeit,
  keine abgeschnittenen Texte; Sprachwechsel und Navigation.
- Themen, Messwerte, Sessions, Timeline, Quellen, Maßnahmen und Dashboard-Agenda:
  Klick 1 Asc, Klick 2 Desc, Klick 3 Original; passende Pfeile und sinnvolle Auswahl.
- Unterlisten/Alle-anzeigen-Dialoge, Sprachwechsel und Refresh mit aktiver Sortierung
  und mit Original; Mindestgröße und reale Maus-/Tastaturbedienung, High-DPI.
- Die zuvor offenen Agenda- sowie älteren Nachweise aus Dokument 153 bleiben offen.

PR #19 bleibt Draft. Keine Ready-for-Review-Umschaltung oder Merge.

Abschließender UX-Safe-Lauf: Exit 0, Release-Solution einschließlich WinForms/WPF
mit 0 Warnungen/Fehlern; Backend sowie sämtliche DE/EN-UI-Regressionen und neue
UI-NAV-001/UI-GRID-001-Prüfungen grün. `git diff --check` erfolgreich. Synthetische
Renderbilder mit aktiven Sortierpfeilen für Themen, Messwerte, Sessions und Agenda
in beiden Sprachen wurden visuell geprüft:
`.codex/synthetic-development-data/ui-tests/54fa780f01a94006875997e8cbb72c04/grid-*.png`.
Geschützte Hashes weiterhin identisch; keine geschützte Datei wird gestagt.
Remote-CI wird am gepushten Head geprüft, manuelle Abnahme bleibt offen.
