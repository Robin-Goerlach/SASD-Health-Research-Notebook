# 157 – HealthTopic Wizard Completion

Stand: 2026-10-07. Anforderungen: PF-CON-001, PF-WIZ-001/003/004/007, FR-UI-001.
Status: implementiert und automatisiert validiert; manuelle Abnahme offen.

## Ausgangslage und Problem

Ausgangspunkt: main / origin/main `fca6d60` (Merge PR #20, Timeline 2), Main-CI erfolgreich,
keine offenen PRs oder weiteren lokalen/Remote-Featurebranches. Working Tree enthielt ausschließlich
die geschützte geänderte `.gitignore` und untracked Wizard-.resx.
Der vorhandene CreateHealthTopicWizardForm hatte zehn navigierbare Schritte, alle fünf Eingaben
im ersten Schritt und neun Roadmap-Platzhalter. Keine echte Zusammenfassung, Titelprüfung erst
beim Finish, keine Sperre gegen parallele async Saves. Die bisherigen Tests sprangen per ListBox
zum letzten Schritt und prüften hauptsächlich Create/Restart; sie belegten keinen vollständigen Ablauf.
MainForm lud nach Erfolg bereits einmal über den bestehenden Reload-Flow nach. EditHealthTopicForm,
HealthTopicService und JSON-Lifecycle bleiben die Grundlage; Edit bleibt getrennt vom Wizard.

## Ziel, Scope und Nicht-Ziele

Den bestehenden Wizard fertigstellen, nicht ersetzen: Grunddaten → Einordnung → Notizen → Prüfen.
Alle fünf vorhandenen Felder, bestehende Enum-Werte, lokalisierte Review-Ansicht, sichere Navigation,
Abbruch und einmaliges Speichern. Kein allgemeines Wizard-Framework, keine Pakete, keine neuen
Fachmodule, medizinischen Regeln, Persistenzversionen oder Domain-/Application-/Infrastructure-Änderungen.
WinForms bleibt führend; WPF bleibt unverändert und wird mitgebaut.

## Was wurde geändert und warum?

### Gegenüberstellung der bisherigen und heutigen Schritte

Die bisherigen Schritte 2 bis 10 waren im WinForms-Code ausschließlich sichtbare
Roadmap-Platzhalter: Sie enthielten keine eigenen Eingabefelder und speicherten keine
zusätzlichen Fachobjekte. Die fünf tatsächlich speicherbaren HealthTopic-Felder lagen
alle im ersten Schritt. Der Slice entfernt diese Platzhalter aus dem **aktuellen
Create-Wizard**, nicht die zugehörigen Fachanforderungen oder vorhandenen Module.

| Bisheriger Unterpunkt | Änderung im fertigen Wizard | Begründung und fachlicher Verbleib |
|---|---|---|
| 1. Grunddaten | Beibehalten, auf Titel und Kurzbeschreibung konzentriert. | Ein klarer Einstieg; Status/Priorität und Notizen erhalten eigene Schritte. Kein Feld entfällt. |
| 2. Diagnose / Status | Durch den echten Schritt „Einordnung“ mit Status und Priorität ersetzt. | Der alte Schritt war ein Platzhalter; die Auswahlfelder lagen vorher in Grunddaten. „Einordnung“ beschreibt dokumentarischen Status und organisatorische Priorität ohne den Eindruck einer App-Diagnose. Alle fünf Status- und vier Prioritätswerte bleiben erhalten. |
| 3. Symptome | Als eigener Wizard-Schritt entfernt. | Kein Symptomeingabefeld und kein Speichern von Beobachtungsobjekten waren dort implementiert. Beobachtungen können im vorhandenen Timeline-Modul dokumentiert werden. Eine zusätzliche Erfassung samt Verknüpfung im Create-Wizard wäre ein weiterer Fach-Slice. |
| 4. Dokumente | Als eigener Wizard-Schritt entfernt. | Es gab keine Upload-/Dateiverknüpfungsfunktion in diesem Schritt. Dokument-/Medienverwaltung bleibt im Produktplan; deren Umsetzung war ausdrücklich nicht Teil dieses Auftrags. |
| 5. Quellen & Informationen | Als eigener Wizard-Schritt entfernt. | Der Platzhalter führte keine Quellenanlage aus. Das vorhandene Quellen-Modul bleibt verfügbar. Quellen während der Themenanlage mit zu erzeugen würde zusätzliche Services, Verknüpfungen und Abbruchregeln benötigen. |
| 6. Ärzte / Kontakte | Als eigener Wizard-Schritt entfernt. | Es gab keine Kontaktanlage oder -auswahl in diesem Schritt. ContactReference und die spätere SASD-Kontaktintegration bleiben geplant; der Auftrag schloss diese Erweiterung ausdrücklich aus. |
| 7. Medikamente / Maßnahmen | Als eigener Wizard-Schritt entfernt. | Es gab dort keine Medikamenten- oder Maßnahmenanlage. Das vorhandene Aktionen-/Routinen-Modul bleibt erhalten; eine Medikamentenverwaltung wird durch diesen Slice weder eingeführt noch ersetzt. Der Wizard erzeugt keine medizinischen Maßnahmen. |
| 8. Messwerte / Laborwerte | Als eigener Wizard-Schritt entfernt. | Der Platzhalter erfasste keine Werte. Das vorhandene Messwerte-Modul bleibt verfügbar; die Wizard-Integration und zusätzliche Laborfunktionen sind hier nicht implementiert. |
| 9. Offene Fragen | Als eigener Wizard-Schritt entfernt. | Es gab keinen eigenen Frageneditor oder Fragen-Datensatz in diesem Schritt. Fragen können als Freitext in Notizen festgehalten werden; strukturierte Terminfragen bleiben im vorhandenen Sitzungen-Modul. Eine automatische Überführung findet nicht statt. |
| 10. Zusammenfassung | Durch den echten vierten Schritt „Prüfen“ ersetzt. | Die alte Zusammenfassung war ebenfalls nur ein Platzhalter. Die neue Review zeigt Titel, lokalisierte Auswahlwerte, Kurzbeschreibung und vorhandene Notizen vor dem einzigen finalen Save. |

Der neue dritte Schritt „Notizen“ nimmt das bereits vorhandene Notes-Feld aus den
Grunddaten auf. Damit lautet der vollständige Ablauf: **Grunddaten → Einordnung →
Notizen → Prüfen**. Es wurden keine HealthTopic-Felder, Enum-Werte, bestehenden
Datensätze oder funktionierenden Fachmodule entfernt.

### Warum die Platzhalter nicht sichtbar geblieben sind

Der Auftrag verlangte einen vollständig fertiggestellten kleinen Create-Slice mit
logischer Reihenfolge, geringer kognitiver Belastung und ohne neue Fachmodule.
Sieben zusätzliche leere Fachschritte hätten weiterhin nicht verfügbare Funktionen
angekündigt und unnötige Weiter-Klicks erzeugt. Ihre Entfernung macht den tatsächlich
implementierten Umfang sichtbar. Die historischen Zehn-Schritte-Pläne bleiben als
langfristiges Zielbild erhalten; daraus folgt keine Zusage, alle Schritte später
unverändert wieder in denselben Wizard einzubauen.

Eine künftige Integration zusätzlicher Fachobjekte benötigt einen eigenen Auftrag
und konkrete Regeln für Verknüpfung, Validierung, Transaktionen und Abbruch. Sie wird
nicht nebenbei durch Platzhalter oder durch Freitextfelder als erfüllt erklärt.

### Weitere bewusst geänderte Bedienfunktionen

| Bisheriges Verhalten | Änderung | Warum? |
|---|---|---|
| Beliebiger Sprung über die Schrittliste | Fortschrittsliste informativ; Navigation über Zurück/Weiter. | Verhindert, dass Pflichtfeldprüfung und Review-Ablauf durch direkte Sprünge umgangen werden. Eingaben bleiben bei Rückwärtsnavigation erhalten. |
| Titelprüfung erst beim finalen Save per Dialog | Inline-Prüfung vor Weiter; erneute Prüfung vor Finish. | Der Benutzer sieht die konkrete Ursache direkt im betreffenden Schritt. Kein später generischer Validierungsdialog. |
| Fehlende Auswahl konnte auf einen Standardwert zurückfallen | Fehlende/ungültige Status-/Prioritätsauswahl blockiert Weiter. | Keine unbemerkte Abweichung zwischen gewählter Einordnung und gespeichertem Datensatz. Die initialen Standardauswahlen bleiben erhalten. |
| Ungeschützter asynchroner Create-Aufruf | Save-Guard und deaktivierte Navigation während Save. | Verhindert parallele Saves und einen als Cancel gemeldeten Abschluss während eines laufenden Schreibvorgangs. |
| Abbrechen schloss auch nach Eingaben unmittelbar | Verwerfbestätigung bei Änderungen; leerer Wizard schließt direkt. | Verhindert versehentlichen Eingabeverlust ohne einen unnötigen Dialog im unveränderten Zustand. |
| Erfolg führte nur zum Reload | Nach demselben Reload gezielte Auswahl der neuen ID. | Das Ergebnis wird sofort auffindbar, auch mit Archivstatus oder noch nicht gebundener Themenansicht. |

Automatisches Zwischenspeichern und „später fortsetzen“ wurden **nicht aus einer
funktionierenden WinForms-Implementierung entfernt**: Der untersuchte Wizard besaß
keine Entwurfspersistenz. Diese älteren Planungsanforderungen wurden für diesen
Auftrag ausdrücklich nicht umgesetzt, weil Cancel keinen Datensatz hinterlassen
soll. Die Vorrangentscheidung und offen bleibenden PF-WIZ-Anforderungen sind unten
dokumentiert.

## UX-Regeln und Lifecycle

- Fortschrittsliste ist informativ; Zurück/Weiter steuern den Ablauf und verhindern Validierungsumgehung.
- Zurück im ersten Schritt deaktiviert; Weiter bei ungültigem aktuellem Schritt deaktiviert.
- Optionale Beschreibung/Notizen dürfen leer sein; keine unnötige Eingabepflicht (PF-WIZ-003, begrenzter Umfang).
- Fertigstellen nur auf der Review-Seite. Lokalisierte Status-/Prioritätsnamen statt Enum-Bezeichner.
- Fokus: Titel beim Öffnen, erster Editor nach Wechsel, lesbare Review-Textbox im letzten Schritt.
  Vor Ausblenden wird der Fokus aus dem alten Panel genommen. Versteckte Panels werden nicht durchtabbt.
- Tab/Shift+Tab folgen der Feldreihenfolge; Enter aktiviert Weiter/Finish, im Notizfeld sind Zeilenumbrüche möglich.
- Escape, Abbrechen und Fensterschließen verwenden denselben Closing-Guard. Leerer Wizard schließt ohne Nachfrage.
  Veränderte Eingaben einschließlich geänderter Auswahl benötigen eine Verwerfbestätigung, Standardantwort Nein.
- Während eines Schreibvorgangs sind Navigation/Abbrechen/Schließen gesperrt. Das ist notwendig, weil das
  bestehende Repository keinen garantierten Rollback eines laufenden AddAsync anbietet. Vor Finish ist Abbruch jederzeit möglich.
- Vor/nach erfolgreichem Save schützt ein expliziter Guard gegen parallele und wiederholte Finish-Aufrufe.
- Fehler bleiben als lokalisierte, inhaltsfreie Inline-Meldung sichtbar; Eingaben bleiben für Korrektur/Retry erhalten.

## Validierung und Persistenz

Titel erforderlich, Whitespace ungültig, Titel maximal 160, Beschreibung 500, Notizen 4000 Zeichen.
UI-MaxLength plus Prüfung vor Navigation/Finish verhindern die bestehende Domain-Trunkierung optionaler Texte.
Status/Priorität müssen vorhandene gültige Auswahlen sein; kein stiller Fallback.
Es gibt keine medizinische Bewertung. Die Zusammenfassung ist nur eine aus Eingaben gebildete Review-Ansicht.

Nur der finale HealthTopicService.CreateTopicAsync schreibt. Domain erzeugt Guid und gleiche initiale
CreatedAt/ModifiedAt; UI übernimmt die zurückgegebene ID. Keine temporären Datensätze/Entwürfe.
Nach Erfolg einmaliger bestehender Shell-Reload, danach Auswahl der neuen ID in beiden Themenlisten.
Explizit archivierte neue Themen werden über den vorhandenen Archivfilter sichtbar gemacht.
Die Auswahl läuft unter dem bestehenden Rebinding-Guard des Sorters; Commands werden erst danach aktualisiert,
um Focus/CurrentCell-Reentrancy zu verhindern. Noch nicht eingeblendete Themenansichten merken sich die
neue ID bis zur Aktivierung; ein per BeginInvoke verschobener Zugriff wartet den nativen Binding-/OnEnter-
Übergang ab. Der neue Integrationstest hat den zuvor fehlenden Fall ohne gebundene Zeilen aufgedeckt.
Auch ein später eintreffender überlappender Refresh übernimmt eine vorgemerkte Create-ID erst nach
BindTopics; ein eigener Control-Test belegt diesen Fall. Sortierzustand und Refresh-Auswahl bleiben erhalten.

### Dokumentationskonflikt / gewählte Interpretation

Die älteren UI-Pläne und PF-WIZ-002 verlangen automatisch gespeicherte/fortsetzbare Entwürfe und mehr
Fachschritte. Der explizite Auftrag für diesen Slice hat Vorrang: keine Persistenz vor Finish, Cancel ohne
Datensatz, nur die fünf vorhandenen Felder. PF-WIZ-002 und PF-WIZ-005/006 werden hier nicht als erfüllt erklärt.
Die alten Planungsdokumente bleiben unverändert. PF-WIZ-007 wird durch den nicht-diagnostischen Starthinweis belegt.

## Tests und Nachweise

UI-WIZARD-002 läuft im bestehenden echten STA-WinForms-Smoke-Runner in DE/EN mit synthetischen Daten:

- leerer/Whitespace-Titel, blockiertes Weiter, gültige/fehlende Enum-Auswahl;
- Öffnungs-/Schrittfokus, Tab/Shift+Tab, Enter/Escape über WinForms-Dialogkey-Verarbeitung;
- vollständiger Happy Path, Back/Forward ohne Werteverlust, lokalisierte Review aller Eingaben;
- leerer Abbruch ohne Save; geänderter Abbruch mit Nein/Ja am echten nativen Dialog, ohne Save;
- verzögertes AddAsync, mehrfacher Finish-Aufruf und Close während Save: exakt ein Add;
- Guid, initiale Zeitstempel, inhaltsfreie lokalisierte Save-Fehlermeldung und Retry;
- echter modaler Create über MainForm und JSON, Auswahl in beiden Listen, archiviertes Create, Refresh;
- Normal-/Mindestgrößen und PNG-Renderartefakte in .codex; vorhandene Lifecycle-, Dashboard-,
  Grid-Sortier-, Edit-/Focus-/Reentrancy- und Backend-Tests laufen unverändert weiter.

## Manuelle Abnahmeliste (offen)

1. Über sicheren Launcher starten, Neues Gesundheitsthema öffnen: Titel fokussiert, vier Schritte.
2. Leerer/Whitespace-Titel blockiert Weiter; gültigen synthetischen Titel und Beschreibung eingeben.
3. Weiter, Status/Priorität ändern, Zurück/Weiter: alle Werte kontrollieren.
4. Notizen mit mehreren Zeilen, Tab/Shift+Tab und Enter prüfen.
5. Review prüfen, Zurück korrigieren, Fertigstellen schnell mehrfach aktivieren: genau ein Thema.
6. Neue Auswahl in Dashboard und Themenliste, Sortierung/Refresh, archiviertes neues Thema mit Filter prüfen.
7. Leerer Abbruch ohne Nachfrage; geänderter Abbruch/Escape/Fenster-X: Nein erhält Eingaben, Ja verwirft sie.
8. DE/EN, Mindestgröße 960 × 660, 100/125/150 % DPI, lange Labels und maximal lange Eingaben prüfen.
9. Edit/Archive/Reactivate/Delete Guards, Dashboardcounts und Grid-Fokus unverändert prüfen.

## Bekannte Grenzen

Echte Hardware-Tastatur, Tooltip-Anzeige, OS-DPI 125/150 % und subjektive UX bleiben manuell offen.
Keine Entwurfswiederaufnahme; keine zusätzlichen Objekte. Während des begonnenen finalen Schreibens ist
Abbruch gesperrt. Crash-/Commit-Ambiguität und transaktionale Idempotenz über Prozessgrenzen sind kein neuer
Vertrag dieses UI-Slices; bestehende Repository-Fehlersemantik bleibt unverändert.

## Geschützte Dateien

Nicht gelesen als Änderungsgrundlage, nicht verändert, rekonstruiert, gelöscht, gestagt oder committed:

- `.gitignore`: SHA-256 `9A84AA3B5E4E9DF41A745F8BF9288C1059948047061503A8634C574DB788A3C6`
- `src/Sasd.HealthNotebook.WinForms/Forms/CreateHealthTopicWizardForm.resx`:
  SHA-256 `4363CD7D5B8671C72442CE1A1BFC10D64EBD24B2D718B54BD4FCD025E4967298`

.codex bleibt ignoriert. Die .resx wird nicht zur Implementierung verwendet.

## Geänderte Dateien

- `src/Sasd.HealthNotebook.WinForms/Forms/CreateHealthTopicWizardForm.cs`: bestehender Wizard, Schritte, Review, Validierung und Save/Cancel-Lifecycle.
- `src/Sasd.HealthNotebook.WinForms/Forms/MainForm.cs`: nach erfolgreichem Create neue ID auswählen.
- `src/Sasd.HealthNotebook.WinForms/Views/HealthTopicsView.cs`: Archivfilter und verzögerte ID-Auswahl.
- `src/Sasd.HealthNotebook.WinForms/Controls/ThreeStateGridSort.cs`: gezielte Auswahl unter bestehendem Rebinding-Guard.
- `src/Sasd.HealthNotebook.WinForms/Localization/AppStrings.cs`: vier echte lokalisierte Schritte.
- `src/Sasd.HealthNotebook.WinForms/Localization/AppStrings.Wizard.cs`: neue Wizard-Texte DE/EN.
- `tests/Sasd.HealthNotebook.WinForms.SmokeTests/Program.cs`: alten schmalen Wizard-Test durch den neuen Test ersetzen.
- `tests/Sasd.HealthNotebook.WinForms.SmokeTests/WizardUiTests.cs`: vollständige Control-/Lifecycle-/Integrationsregression.
- `docs/development/157_HealthTopic_Wizard_Completion.md`: Scope, Entscheidungen und Nachweise.
- `docs/testing/105_Akzeptanzkriterien_Traceability_Quality_Gates.md`: tatsächlich implementierte Wizard-Anforderungen.
- `README.md`: aktuellen Wizard-Umfang und offene manuelle Abnahme verlinken.

## Abschließende automatisierte Validierung

Am 2026-10-07 vollständig erfolgreich:

- `powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1"`, Exit 0.
- Vollständiger Release-Build einschließlich WinForms/WPF: 0 Warnungen, 0 Fehler.
- Alle bestehenden Backend-Smoke-Tests und DE/EN-WinForms-Smoke-/Regressionstests grün.
- UI-WIZARD-002 grün in Englisch und Deutsch, einschließlich Create aus Dashboard und Themenliste,
  verzögerter/überlappender Auswahl, Cancel/Escape/Close und Mindestgrößen-/Textpassungsprüfungen.
- `git diff --check`: erfolgreich. Die bestehende CRLF-Hinweismeldung zur geschützten .gitignore ist
  kein Whitespace-Fehler und wurde nicht durch Änderung dieser Datei behoben.
- Beide geschützten Hashes erneut exakt bestätigt; Index vor Staging leer; .codex weiterhin ignoriert.
- Finale UI-Renderartefakte: `.codex/synthetic-development-data/ui-tests/b02f93cfd93b40878b1d781d7088d4a4/`.
  Grunddaten, Einordnung, Notizen und Review wurden mit dem vorhandenen Rendermechanismus erzeugt;
  DE/EN-Normal-/Mindestgrößenansichten visuell geprüft. Keine manuelle Desktop-Abnahme behauptet.

Anfängliche Korrekturläufe: beim Verschieben des alten Tests vorübergehend fehlender Testname,
anschließend korrigierte Reflection-Typbeschränkung; ein neuer Integrationstest fand die fehlende
Auswahl in einer noch nicht eingeblendeten Liste. Diese Lücke wurde durch vorgemerkte ID-Auswahl
behoben. Zwei neue Nullable-Warnungen in Enum-Assertions wurden beseitigt. Ein Sandbox-Temp-Zugriffsfehler
wurde mit dem autorisierten Safe-Lauf außerhalb der Sandbox behoben; der Datenpfad blieb repository-lokal.
