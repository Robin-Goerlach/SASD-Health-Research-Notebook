# 055 - UI-Zielbild und Screenshot-Implementierungsplan

Projekt: SASD Health Research Notebook  
Stand: 2026-09-30  
Dokumenttyp: UI-Zielbild / Umsetzungsplan  
Status: Baseline 2.1 – WinForms primär

## 1. Ziel

Die Konzeptbilder im Repository definieren die visuelle Richtung:

- `docs/screenshots/dashboard-concept.png`
- `docs/screenshots/condition-wizard-concept.png`

Das kurzfristige Ziel lautet:

> **Die tatsächlich startende WinForms-Anwendung soll das visuelle und funktionale Versprechen dieser Konzeptbilder zunehmend einlösen, ohne ihre bestehende Funktionalität durch eine statische Demo zu ersetzen.**

WPF bleibt eine buildbare Referenzoberfläche, ist derzeit aber nicht das primäre Ziel für UI-Politur oder neue Fachfeatures.

## 2. Kritische Bestandsaufnahme

Der aktuelle WinForms-Stand ist bereits eine echte funktionale Oberfläche und kein leeres Parallelprojekt.

Vorhanden sind unter anderem:

- `MainForm` als Application Shell;
- dunkle linke Navigation;
- Dashboard und Gesundheitsthemen als echte Navigation;
- Dashboardkarten;
- HealthTopic-Liste über `DataGridView`;
- Refresh und "Neues Gesundheitsthema";
- StatusStrip;
- Create-HealthTopic-Wizard;
- `DashboardCardControl`;
- `NavigationControl` und `NavigationButton`;
- Presenter für Dashboard/HealthTopics;
- zentrale `UiColors`, `UiFonts`, `UiMetrics`;
- Deutsch/Englisch-Lokalisierung;
- gemeinsame JSON-Persistenz mit WPF.

Damit lautet die Aufgabe nicht mehr "WinForms aufbauen", sondern **den vorhandenen WinForms-Stand gezielt zum Produkt-UI entwickeln**.

## 3. Frontendstrategie

### WinForms

- primäres Entwicklungsfrontend;
- neue UI-Funktionen und Fachfeatures werden hier zuerst integriert;
- Konzept-Screenshots dienen als visuelles Ziel.

### WPF

- bleibt buildbar;
- dient als Referenz und als Test, dass Application/Infrastructure UI-unabhängig bleiben;
- wird nicht automatisch mit jedem neuen WinForms-Feature erweitert.

Doppelentwicklung derselben Fachfunktion in beiden Oberflächen wird vermieden.

## 4. Aktuelle Hauptlücken

### 4.1 Application Shell

Zu prüfen und schrittweise zu verbessern:

- Sidebar-Breite und Abstände;
- Headerhöhe;
- klare Hierarchie von Seitentitel, Beschreibung und Hauptaktion;
- Verhalten bei Mindestgröße;
- sinnvolle DPI-/Font-Skalierung;
- ruhige Statusleiste.

### 4.2 Navigation

Bereits als echte WinForms-Komponente vorhanden.

Ziel:

- klarer Selected State;
- Hover/Focus;
- vollständige Tastaturbedienung;
- konsistente Lokalisierung;
- später ohne MainForm-Umbau erweiterbar.

### 4.3 Dashboardkarten

Die Karten müssen:

- vollständig sichtbar sein;
- gleiche Höhe/Abstände besitzen;
- Zahlen, Titel und Beschreibung klar hierarchisieren;
- keine Fake-Daten benötigen;
- aus echten Application-Service-Daten gespeist bleiben.

### 4.4 HealthTopic-Grid

Weiterentwickeln zu einer ruhigen Produktansicht:

- passende Zeilenhöhe;
- gute Spaltenbreiten;
- klare Typografie;
- Empty State;
- Auswahl/Fokus;
- später Doppelklick/Öffnen;
- keine medizinisch alarmistischen Farben.

Priorität ist organisatorische Nutzerpriorität, keine medizinische Dringlichkeit.

### 4.5 Wizard

Der WinForms-Wizard bildet den heute funktionalen WPF-Umfang ab und zeigt spätere Schritte teilweise als Roadmap.

Ziel:

- visuell mit Dashboard/Shell harmonisieren;
- aktuellen Schritt klar zeigen;
- Feldgruppen und Hilfetexte verbessern;
- Navigation Zurück/Weiter/Abbrechen eindeutig machen;
- lange Tooltips lesbar halten;
- Deutsch/Englisch konsistent halten;
- später fachliche Schritte als vertikale Slices implementieren.

Die sichtbaren zukünftigen Schritte dürfen nicht vortäuschen, bereits vollständig implementiert zu sein.

## 5. Zielnavigation für spätere Versionen

```text
Dashboard
Gesundheitsthemen
Tagebuch
Messwerte
Sitzungen & Termine
Routinen
Quellen & Recherche
Dokumente
Fragen

Berichte & Export

Einstellungen
Backup & Wiederherstellung
```

Nicht implementierte Bereiche können verborgen oder eindeutig deaktiviert bleiben.

## 6. Screenshot-first Arbeitsphasen

### WF-UI-01 - Shell Polish

- Sidebar, Header, Footer/Status harmonisieren;
- Abstände und Typografie prüfen;
- Mindestgröße/DPI berücksichtigen;
- bestehende Funktion beibehalten.

**Done:** Der erste Gesamteindruck entspricht klar der Designsprache des Konzeptbilds.

### WF-UI-02 - Dashboard

- Card-Layout feinjustieren;
- HealthTopic-Arbeitsbereich polieren;
- Empty State;
- Grid-Dichte und Auswahl;
- Focus-/Hover-Zustände.

**Done:** Dashboard ist ohne Fake-Daten vorzeigbar.

### WF-UI-03 - Wizard

- gleiche Designsprache;
- Step-Navigation;
- Formularspacing;
- Buttons/Validierung;
- Hilfetexte/Tooltips;
- spätere Zusammenfassungsansicht.

**Done:** Wizard wirkt wie Bestandteil desselben Produkts.

### WF-UI-04 - Navigation Host Stabilisierung

- MainForm bleibt Shell statt Fachlogik-Sammelstelle;
- Views/Presenter sauber trennen;
- neue Seiten ohne grundlegenden Shell-Umbau ergänzbar machen.

**Done:** Fachmodule können als Views/Slices ergänzt werden.

### WF-UI-05 - Fach-Slices

Danach werden die fachlichen Module in der priorisierten Reihenfolge aus `145_Codex_Arbeitsauftrag.md` umgesetzt.

## 7. Akzeptanzkriterien für das nächste Screenshot-Ziel

- komplette Solution baut;
- Smoke Tests laufen;
- WinForms-App startet ohne Fehler;
- bestehende HealthTopics werden weiterhin geladen;
- Neuer-HealthTopic-Wizard funktioniert;
- WPF bleibt buildbar;
- beide Frontends verwenden denselben JSON-Datenbestand;
- Sidebar, Header, Karten und Arbeitsbereich entsprechen in Hierarchie und Stil dem Konzeptbild;
- keine statische Screenshot-Nachbildung;
- keine echten Gesundheitsdaten in Demo/Tests/Screenshots;
- Mindestfenstergröße bleibt benutzbar;
- Tastatur-Fokus ist erkennbar;
- Deutsch und Englisch sind funktionsfähig;
- UI-Texte bleiben nicht-diagnostisch.

## 8. Visuelle Verifikation

Bei WinForms-UI-PRs soll die laufende Anwendung möglichst mit dem Konzeptbild verglichen werden.

Prüfpunkte:

- Gesamtproportion;
- Sidebar-Breite;
- Headerhöhe;
- Kartenabstände;
- Typografiehierarchie;
- Hauptbutton;
- Listendichte;
- Rahmen/Flächen;
- ruhiges Gesamtbild;
- Darstellung in Deutsch und Englisch.

Pixel-Perfektion ist nicht das erste Ziel. Informationshierarchie und visuelle Sprache sind wichtiger.

## 9. Was nicht Teil der reinen UI-Politur ist

- SQLite-Migration;
- Ernährungstagebuch;
- Wetter-API;
- Notification Service;
- OCR;
- FHIR;
- KI;
- zentrale Kontaktverwaltung;
- komplexe Diagramme.

Diese Funktionen bleiben in der Fachroadmap und werden später als vertikale Slices entwickelt.
