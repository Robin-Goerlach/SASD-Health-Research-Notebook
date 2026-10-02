# 152 – HealthAction + Routine + Progress Slice 1

Stand: 2026-10-02. Implementiert und automatisiert geprüft; manuelle Nutzerabnahme offen. Start von main
`6bd99137f4d529568bedb1c9e3ad004bc8df28e6`.

## Modell und begrenzter Umfang

HealthAction ist selbst erfasste Dokumentation: Titel (160), optionale HealthTopicId,
ActionType (Movement, Nutrition, Sleep, Relaxation, Organization, Other), Active/Inactive,
Beschreibung (4000), berichtete Herkunft (SelfDefined, Doctor, Therapist, Coach, Source,
Other), optionale Herkunftsnotiz (4000), SourceId/SessionId, Id und technische Zeitstempel.
Herkunft ist Nutzerdokumentation, keine professionelle Bestätigung. Inhalt/Herkunft
sind create-only; keine Bearbeitung professioneller Anweisungen ohne Auditkonzept.
Routine ist ein getrenntes Objekt mit genau einer HealthActionId, Titel (160),
Beschreibung (4000), optionalem ScheduleText (160), Active/Paused und Zeitstempeln.
ProgressEntry gehört genau einer Routine: OccurredAt mit Offset, Completion
Performed/NotPerformed/Skipped, optionale nichtnegative ganzzahlige Anzahl, eigene Note (4000), Id/CreatedAt/ModifiedAt.

FR-ACT-001/002/003/005 und Teile FR-ACT-004, FR-ROU-001/002/003/004/006/007.
Pausieren/Reaktivieren ändert nur Routinenstatus; historische Dokumentation bleibt
auch bei pausierten Routinen möglich und erhalten. Einträge überschreiben sich nicht.
ScheduleText beschreibt auch tägliche/wöchentliche/ausgewählte Tage, wird aber nicht
als Terminplan interpretiert. Die größere Baseline bleibt Zielmodell: eigenständige
Routinen ohne Action, strukturierte Frequenzen, Zielquoten, Mengen/Dauer/
Messaufgaben, Zeiträume, komplexe Modulbeziehungen, Bearbeiten/Audit, Reminder,
Export und allgemeine Timeline bleiben spätere Arbeit. Keine Habit-/Workflow-Engine,
kein Medication-/Treatment-Typ, keine Dosislogik oder Wirksamkeits-/Kausalitätsaussage.
Die engeren Slice-Akzeptanzkriterien werden in Dokument 155 konkretisiert; damit
werden die langfristigen Anforderungen nicht als vollständig umgesetzt behauptet.

## Application und JSON

HealthActionService erstellt/lädt Actions, Routinen und historischen Progress,
ändert Routinenstatus und liefert reine Dokumentationszahlen. Actions: CreatedAt
absteigend, Id. Routinen: CreatedAt/Id aufsteigend. History: OccurredAt absteigend,
CreatedAt absteigend, Id. DateTimeOffset bewahrt den fachlichen Offset; die Liste
zeigt lokale Zeit. Dialoge weisen ungültige/mehrdeutige DST-Uhrzeiten ab.

Themenbezug sowie optionale SourceId/SessionId werden bei Anlage gegen vorhandene
Datensätze geprüft. Aktuelle Titel werden nur für die Anzeige aufgelöst; später
fehlende Verweise verlieren keine Action. Archivierte Themen sind zulässig. Keine
Inhalte werden übernommen, keine Titel redundant gespeichert, keine anderen Stores
beschrieben und keine Cascading Deletes. Source-/Session-Verweise sind die minimale
Umsetzung von FR-ACT-003, ohne Entscheidungen aus Gesprächsinhalten abzuleiten.

Ein gemeinsamer Store `health-actions.json` enthält Actions, Routines, ProgressEntries,
Kennung `SASD.HealthNotebook.HealthActions`, Version 1. Die enge Eltern-/Kindbeziehung
wird in einem atomaren Austausch konsistent gehalten. Application und Repository
prüfen interne Elternreferenzen; alle IDs pro Sammlung sind eindeutig. Required-Felder,
Domainregeln, Kennung/Version und unbekannte Felder werden strikt geprüft. Fehlertexte
enthalten keine Inhalte. Bestehende fünf Stores bleiben bytegenau unverändert.

Begleiter: `health-actions.json.tmp`, `health-actions.backup.json`,
`health-actions.json.lock`. Exklusive CreateNew-Lock/Temp-Anlage, Flush und atomarer
Austausch, letztes gültiges Backup. Beschädigte/fremde/zukünftige Stores, fremde
Backups und vorhandene Temp-/Lock-Dateien werden erhalten und Schreibversuche
abgewiesen. Fehlende Primärdatei mit Backup wird nicht als leerer Store ausgegeben;
keine stille Recovery. Reparse Points werden abgewiesen. Die bekannte Link-Race-
Einschränkung der bisherigen Stores bleibt; JSON ist weiterhin unverschlüsselt.
Alle vier Dateinamen werden vom Windows-PowerShell-5.1-Safe-Launcher geprüft.

## WinForms und visueller Fortschritt

WinForms bleibt primär, WPF buildbare Referenz. Neue Navigation „Maßnahmen & Routinen“ /
„Actions & Routines“, Seitentitel/Kurzbeschreibung und prominente „Neue Maßnahme“.
Drei kompakte Dokumentationszahlen: aktive Actions, aktive Routinen, alle heutigen
Einträge nach lokaler Kalenderzeit (auch nicht durchgeführt/übersprungen). Beide
Statuszählungen sind unabhängig; eine inaktive Action pausiert keine Routine automatisch.
Actions links (46%), Routinen rechts oben und History rechts unten (54%, je 50%).
Standardabstand 12, Panel-Innenrand 12, gleich ausgerichtete Oberkanten und native
ruhige Panelrahmen. Vollständige Texte sind in separaten lesbaren RichTextBox-Detailbereichen; native
Scrollbars erscheinen bei Bedarf. Empty States blenden Details aus und nutzen
deren Fläche für den Hinweis, ohne leere Scrollbars.
Routine-Auswahl filtert History; Action-Wechsel leert veraltete Kinder sofort.
CurrentCellChanged und Presenter-Generationen verhindern die bekannte alte-Auswahl-
Regression. Refresh/Sprachwechsel erhalten IDs, neue Kinder werden direkt ausgewählt.

Vorhandene Karten erhalten einen kleinen kompakten Modus mit 16-pt-Zahlenschrift;
keine neue Theme-Engine. Die Shell lässt die bereits aktive Seite beim Sprachwechsel im Host; dadurch
springt ihre Datenbindung nicht auf die erste Zeile zurück. Der zentrale Header schrumpft von 156 auf 144 Pixel bei
unveränderten Hauptaktionen. Dieselbe kompakte Kennzahlenzeile erscheint auf dem
Dashboard unter den bestehenden Themenkarten. Die Themenliste bleibt funktional;
die Themen-Seite lädt keine Action-Kennzahlen. Kein kompletter Shell-Umbau.
Gegenüber dem README-Zielbild verbessert dies Hierarchie, Kartendichte, strukturierte
Arbeitsbereiche und Abstandskonsistenz. Das größere Dashboard-Raster und weitere
Fachbereiche des Konzeptbilds bleiben offen; klinische Bewertungskarten werden
nicht übernommen. Die neue Seite ist funktional, keine statische Screenshot-Kopie.

## Automatisierte Nachweise / Checkpoints

UT/IT/SEC-ACT-001: Domain, Herkunft/Verweise, optionale/archivierte/fehlende Themen,
Referenzintegrität in Application/Repository, unabhängiges Reload, Statusroundtrip,
chronologische Offset-Sortierung und exakte Notizen. Kennung/Version, beschädigte/
fremde/zukünftige Stores, Duplikate/null-Kinder, Pflicht-/unbekannte Felder, Backup,
Temp/Lock und bytegenauer Erhalt aller fünf bisherigen Stores. SEC-ACT-002 prüft
alle vier Guards mit echtem Windows PowerShell 5.1. Keine neuen Pakete/Projekte.
UI-ACT-001/002: echte DE/EN-Dialoge, Empty State, mit/ohne Thema, Herkunftsverweise,
Routinenanlage, Durchführung, Pausieren/Reaktivieren, Auswahlwechsel, Refresh,
frische Shell/Services und Wiederauswahl OHNE Refresh. Layout-/Text-/Buttongrenzen,
Panelproportionen/Oberkanten, Tab-Reihenfolge/Feldhilfe bei Normal-/Mindestgröße.
DASH-ACT-001: 2 aktive Actions, 2 aktive Routinen, 1 heutiger synthetischer Eintrag.

Backend-Checkpoint `8b700321389372d4a2f4f960464b65f503c012ea`: vollständiger
Safe-Lauf grün, früh gepusht, Draft PR #17 und Backend-CI grün. UI und abschließende
Dokumentation folgen auf demselben Branch. Endprüfung: vollständiger Safe-Lauf unter Windows PowerShell 5.1 grün,
Release einschließlich WPF mit 0 Warnungen/0 Fehlern, beide Smoke-Testprojekte und
alle bestehenden Regressionen bestanden. Beide Frontends zusätzlich über den Safe
Launcher mit synthetischem Datenpfad gestartet: eigenes antwortendes Hauptfenster,
normales Schließen und Launcher-Exit 0. Dies ist eine Startprüfung, keine manuelle
Desktopabnahme. `git diff --check` grün; alle geänderten Quellen/Dokumente sind UTF-8.

Finale Renderbilder: `.codex/synthetic-development-data/ui-tests/1873b688332844ddb066f37abd8c67d4/`,
Unterordner actions-German / actions-English. Jeweils leer/gefüllt/Neustart bei
1280×820 und 1120×740 sowie drei Dialoge und Dashboard. Visuell gesichtet: gemeinsame
Oberkanten, gewollte Panelproportionen, kompakte Karten, sichtbare Header/Buttons und
keine leeren Detail-Scrollbars. Lange Details bleiben nativ scrollbar; das größere
Konzept-Dashboard bleibt weitere Arbeit. Nur synthetische Daten; Bilder bleiben
wie bisher unter ignoriertem .codex und werden nicht versioniert. Die frühe Dokumentationskodierung wurde auf UTF-8 normalisiert.
Die geschützte Wizard-resx bleibt unverändert/untracked; .codex ist ignoriert.

## Manuelle Abnahme – offen

Native Desktopsteuerung steht in dieser Sitzung nicht zur Verfügung. Echte Control-
Tests und synthetische Renderbilder ersetzen keine manuelle Nutzerakzeptanz.
Nach dem grünen Safe-Lauf vom Repository aus starten:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

1. Maßnahmen & Routinen öffnen; Empty State / ausschließlich synthetische Daten prüfen.
2. `CODEX TEST – Action` mit Kategorie, Beschreibung, Status und berichteter Herkunft
   erfassen, mit/ohne synthetisches Thema; optional vorhandene synthetische Quelle/Session.
3. `CODEX TEST – Routine` mit eigener Beschreibung und Rhythmus anlegen; pausieren und
   reaktivieren. Eine zweite Routine anlegen; Auswahlwechsel muss den richtigen Verlauf zeigen.
4. Durchführung mit Zeitpunkt, durchgeführt/nicht durchgeführt/übersprungen und
   eigener synthetischer Notiz sowie optionaler Anzahl dokumentieren. Keine Empfehlung oder Wirkungsbewertung.
5. Zwischen zwei Actions wechseln, Refresh und DE/EN prüfen; nur passende Kinder sichtbar.
6. Schließen und über denselben Launcher neu starten; ältere Action ohne extra Refresh
   wählen und Routine/History/Notiz/Herkunft wiederfinden. Dashboardzahlen vergleichen.
7. Normale Größe und Mindestgröße 1120×740, sichtbare Haupt-/Kindaktionen, sinnvolle
   Oberkanten/Abstände, Tab/Enter/Escape, Navigation Up/Down/Home/End und Feldhilfen prüfen.

Entwicklungsdaten ausschließlich unter .codex/synthetic-development-data. Session
Slice 1 bleibt abgeschlossen. PR #17 bleibt Draft; kein Ready, kein Merge, kein
nächster Slice vor gesondertem Auftrag.

Interpretation FR-UI-001: Der Satz „keine zusätzlichen Fachmodule“ begrenzte den
abgeschlossenen UI-Baseline-2-Auftrag. Dieser ausdrücklich beauftragte fachliche
Slice übernimmt dessen UI-Qualitätsregeln und erweitert die Navigation; Dokument
155 präzisiert den historischen Scope, statt daraus ein dauerhaftes Modulverbot
abzuleiten. Traceability und offene Teilanforderungen stehen in Dokument 105.
