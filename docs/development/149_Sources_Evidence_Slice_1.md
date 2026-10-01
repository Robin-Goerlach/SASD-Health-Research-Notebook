# 149 – Sources + SourceLocation + EvidenceNote Slice 1

Stand: 2026-10-01. Implementiert und manuell akzeptiert.
Anforderungen: FR-SRC-001/002/004/005/006, zusätzlich optionale externe ID (FR-SRC-003).

## Modell und Application

`Source` dokumentiert Typ (`WebPage`, `DocumentOrPdf`, `Book`, `Study`, `Conversation`,
`ProfessionalStatement`, `OwnObservation`), Titel, URL, Autor/Institution,
Publikations-/Abrufdatum und externe ID. Mindestens Titel, HTTP(S)-URL oder
Autor/Institution ist nötig. Titel/Autor/ID sind auf 160 Zeichen begrenzt, URL auf 2048.
Die App lädt URLs oder Dokumente nicht automatisch. Kalenderdaten sind `DateOnly?`,
damit keine unbeabsichtigte Zeitzonenumrechnung das Publikations-/Abrufdatum ändert;
technische Zeitstempel sind wie bisher `DateTimeOffset`.

`SourceLocation` enthält SourceId, Typ (`Page`, `Section`, `Chapter`, `Paragraph`,
`UrlAnchor`, `Other`), Pflicht-Locator (160 Zeichen), optionale Notiz (4000 Zeichen)
und CreatedAt. `EvidenceNote` enthält SourceId, optional SourceLocationId, Pflicht-
Aussage (4000 Zeichen), optionales Originalexzerpt (1000 Zeichen), Pflicht-
Eigeneinordnung (4000 Zeichen), CreatedAt und ModifiedAt. Freitexte werden nicht
abgeschnitten; Zitat und eigene Worte bleiben separate Felder. Kein Wahrheitsurteil,
keine Vertrauensbewertung und keine medizinische Interpretation.

`SourceService` mit `ISourceRepository` bietet Create/Load/List und sourcebezogene
Fundstellen/Notizen. Die Application weist fehlende Quellen und Fundstellen einer
anderen Quelle ab. Der Store prüft die Beziehungen zusätzlich beim Lesen und unter
dem Writer-Lock. SourceLocation/EvidenceNote enthalten keine Kopien des Quellentitels.
Eine optionale einzelne HealthTopicId liegt auf Source; der aktuelle Themenname
wird nur zur Anzeige aufgelöst. Ein fehlendes/archiviertes Thema entfernt keine Quelle.
Die Mehrfachzuordnung aus FR-GEN-001 bleibt Zielmodell; HealthEntry-Verknüpfungen
werden in diesem Slice nicht erzwungen. Keine Cascading Deletes.

## Persistenzentscheidung

Ein gemeinsamer `sources.json`-Store enthält drei getrennte Listen (`Sources`,
`Locations`, `Notes`), `Store = SASD.HealthNotebook.Sources`, `Version = 1`.
Gegenüber drei separaten Dateien vermeidet dies Teil-Commits und Dateitransaktionen
über abhängige Beziehungen. Keine redundante Persistenz und keine neue generische
Repository-Basis. Die Granularität ist für den kleinen create-only Slice ausreichend.

`JsonSourceRepository` folgt dem bestehenden Entry-Store: strikte Pflichtmetadaten,
Version/Kennung, Domaininvarianten, eindeutige IDs und Referenzintegrität. Fremde,
beschädigte, zukünftige Stores, unbekannte Felder und fremde Backups werden nicht
überschrieben. Fehlende Primärdatei bei vorhandenem Backup bricht sicher ab;
es gibt keine automatische Wiederherstellung. Eine fehlende Datei ohne Backup ist
ein neuer leerer Store; eine vorhandene leere Datei ist beschädigt.

Dateinamen: `sources.json`, `sources.json.tmp`, `sources.backup.json`,
`sources.json.lock`. Exklusive CreateNew-Anlage für Lock und Temp, Flush und atomarer
Austausch erhalten das letzte gültige Backup. Bestehende Temp-/Lock-Dateien werden
nicht bereinigt. Reparse Points werden abgewiesen; die bekannte Race-Condition bei
gleichzeitig böswillig manipulierten Links bleibt akzeptiert. JSON ist unverschlüsselt
wie die bestehende Persistenz. Keine Migration, keine Änderung an `health-topics.json`
oder `health-entries.json`, keine SQLite-Vorwegnahme.

`LocalHealthNotebookPaths.SourcesFilePath` verwendet ausschließlich denselben
`SASD_HEALTHNOTEBOOK_DATA_PATH`. Der PowerShell-5.1-Launcher prüft alle vier neuen
Dateinamen. Für Entwicklung ausschließlich `.codex/synthetic-development-data`;
TEMP/TMP, NuGet und DOTNET_CLI_HOME bleiben ebenfalls unter `.codex/` isoliert.

## UI und Nachweise

WinForms bietet Quellen/Sources, eine Quellenliste mit Metadaten und rechts die
Tabs Fundstellen/Quellen-Notizen. Die getrennten Listen und vollständigen, scrollbar
lesbaren Details vermeiden eine komplexe Drei-Fenster-Oberfläche. Neue Quelle,
Neue Fundstelle und Neue Quellen-Notiz öffnen funktionierende Dialoge. Date-Checkboxen
lassen unbekannte Daten ausdrücklich unset. Styling, Navigation, Localization und
Presenter-Struktur werden wiederverwendet; die gemeinsame kleine Dialogbasis enthält
nur Layout, Accessibility und den Save-Lifecycle. Fachregeln bleiben UI-unabhängig.
WPF erhält keine neue UI und bleibt buildbar.

- UT-SRC-001: Mindest-/Längenvalidierung, Kategorien, Pflicht-Aussage/Eigeneinordnung.
- IT-SRC-001: alle Typen/Metadaten, mehrere Fundstellen, Notizen mit/ohne Fundstelle,
  fremde Referenzen abweisen, unabhängiges Reload, getrennte Texte, Backup und
  bytegenauer Erhalt von Topic-/Entry-Dateien.
- SEC-SRC-001: beschädigte/fremde/zukünftige/inkonsistente Stores, Temp/Lock und Backup.
- SEC-SRC-002: echte PowerShell-5.1-Dateiguards für alle vier Sources-Dateinamen.
- UI-SRC-001: echte WinForms-Navigation/Dialogs, DE/EN, Leerzustände, Create/Reload,
  Themenbezug, Referenzen, Texttrennung, TabOrder, Tooltips und Mindestgrößen.

Release-Build einschließlich WPF und beide Smoke-Testprojekte sind grün (0 Warnungen,
0 Fehler). Synthetische Renderbilder wurden geprüft; dies ersetzt nicht die manuelle
Desktopakzeptanz. Computer Use meldet eine nicht verfügbare native Pipe (os error 2).

## Manuelle Akzeptanz – bestätigt

Der Nutzer bestätigte Quellen/Sources, verständlichen Empty State, synthetische
Anlage von Quelle/Fundstelle/Notizen, klare Trennung von Aussage, Originalexzerpt
und eigener Einordnung, Notizen mit/ohne Fundstelle, sofortige Anzeige und Refresh.
Nach Neustart über denselben Safe-Launcher blieben alle Daten und Beziehungen
erhalten. Deutsch/English, Tooltips, Tab-Reihenfolge und Layout ohne abgeschnittene
wichtige Controls wurden ebenfalls bestätigt. Dies ist manuelle Nutzerakzeptanz;
Computer Use wurde nicht als bestandene Desktopprüfung gewertet.

Vom Repository aus nach grünem Validate-Lauf:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

1. Quellen/Sources öffnen, Empty State oder ausschließlich synthetische Daten prüfen.
2. `CODEX TEST – Synthetic Research Source` anlegen; optionale Metadaten und ggf.
   synthetisches Gesundheitsthema wählen.
3. Fundstelle `Page 17` hinzufügen.
4. Quellen-Notiz mit `Synthetic statement for automated verification.`,
   `Synthetic excerpt.` und `Synthetic personal summary for test purposes only.`
   in den drei getrennten Feldern erfassen; Fundstelle auswählen.
5. Sofortige Anzeige, vollständige Details und Refresh prüfen; eine Notiz ohne
   Fundstelle muss ebenfalls möglich sein.
6. Schließen, mit demselben Launcher neu starten; Beziehungen und Texte wiederfinden.
7. Deutsch/English, Tooltips, Tab/Enter/Escape und verkleinertes Fenster prüfen.

Alle neuen Entwicklungsdaten liegen ausschließlich in
`.codex/synthetic-development-data/sources.json`. Die vorhandene untracked Wizard-
`.resx` bleibt unangetastet; `.codex/` ist ignoriert. Die lokale Commit-Freigabe
erfolgte nach dieser Bestätigung; Push/PR/Merge sind nicht beauftragt.

Offen außerhalb Slice 1: Edit/Delete/Archive, Dateiimport, Vertrauens-/Prüfstatus,
Many-to-many-/HealthEntry-Verknüpfungen, automatische Store-Wiederherstellung.
Nächster geplanter Slice: Measurement / Vitalwerte.

## Regression: Fundstelle nach Neustart/erneuter Auswahl scheinbar verschwunden

Der Fehler liegt in der WinForms-Auswahlbenachrichtigung, nicht in Persistenz oder
Application. `SourcesView` verwendete `DataGridView.SelectionChanged`, las aber
`CurrentRow`. Beim Wechsel der aktuellen Zeile feuert dieses Ereignis, bevor
`CurrentRow` aktualisiert ist. Der Presenter leert zunächst die abhängigen Listen,
lädt die vorherige Quelle und verwirft deren Ergebnis anschließend wegen der nun
abweichenden Auswahl. Die neu gewählte Quelle bleibt dadurch ohne Fundstellenanzeige.
Ihre Fundstellen sind weiterhin unverändert in `sources.json` gespeichert.

Die Reparatur verwendet `CurrentCellChanged` für die Quelle: `SourceSelected` meldet
damit die tatsächlich neue aktuelle Quelle. Binding-Unterdrückung und der vorhandene
Generation-/ID-Schutz des Presenters bleiben erhalten. Empty States unterscheiden
in DE/EN zwischen fehlender Auswahl und einer ausgewählten Quelle ohne Fundstellen
bzw. Notizen. Keine Store-/Formatänderung, Migration oder zweite Persistenzquelle.

Der alte UI-Test erzeugte eine neue, standardmäßig ausgewählte Quelle und rief nach
dem Neustart explizit `WaitForReload` auf. Dieser zusätzliche Refresh lud die richtige
Quelle und verdeckte den fehlerhaften Auswahlweg. Er prüfte nicht die Auswahl einer
älteren Quelle unter mehreren Quellen ohne Refresh.

- IT-SRC-RELOAD-001: frischer isolierter Store, Source/Location über den produktiven
  Service erstellen, tatsächliche JSON-Felder prüfen, ursprüngliche Instanzen verwerfen,
  neue Repository-/Service-Instanzen und `GetSourceDetailsAsync`; Id, SourceId, Typ,
  Locator, Notiz und CreatedAt vergleichen. Andere Stores bleiben bytegenau erhalten.
- UI-SRC-RELOAD-001: echte Source-/Location-Dialoge, zweite neuere Quelle, ursprüngliche
  MainForm schließen und disposen; neue Shell/Services, Sources öffnen und ältere
  Quelle ohne expliziten Refresh auswählen. Originalfundstelle, sichtbare Liste,
  Rück-/Wiederauswahl und korrekte Empty States werden in DE/EN geprüft.

Vor der Reparatur besteht der JSON-/Service-Test, während der neue UI-Test mit
fehlender Fundstellenanzeige fehlschlägt. Eine zusätzliche Assertion bestätigt,
dass `SourceSelected` die vorherige `CurrentRow` meldet. Nach der Reparatur muss der
vollständige sichere Release-/Smoke-Lauf einschließlich WPF erfolgreich sein.
Der anschließende vollständige Lauf ist erfolgreich: 0 Warnungen/0 Fehler,
beide Smoke-Testprojekte grün, Neustart-/Wiederauswahlregression in DE/EN bestanden.
Measurement-Checkpoint und Draft PR #14 bleiben von diesem separaten Bugfix unberührt.

Visuelle Ergänzung: Beide unteren Detailfelder liegen in derselben proportionalen
40%-Zeile eines gemeinsamen TableLayoutPanel; die oberen Listen/Tabs nutzen 60%.
Der rechte Detailtext folgt weiterhin dem gewählten Fundstellen-/Notizen-Tab.
UI-SRC-LAYOUT-001 prüft gleiche Höhe und ausgerichtete Oberkante (maximal ein Pixel
Rundungsabweichung), beide Tabs und normale/Mindestfenstergröße in DE/EN.
Keine feste Detailhöhe, Änderung der Mindestgröße, Fachlogik oder Persistenz.
