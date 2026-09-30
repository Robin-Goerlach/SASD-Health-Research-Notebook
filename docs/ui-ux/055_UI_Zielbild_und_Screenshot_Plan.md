# 055 - UI-Zielbild und Screenshot-Implementierungsplan

Projekt: SASD Health Research Notebook  
Stand: 2026-09-30  
Dokumenttyp: UI-Zielbild / Umsetzungsplan  
Status: Baseline 2.1

## 1. Ziel

Die beiden Konzeptbilder im Repository sind keine bloße Dekoration. Sie definieren die visuelle Richtung der Anwendung:

- `docs/screenshots/dashboard-concept.png`
- `docs/screenshots/condition-wizard-concept.png`

Das kurzfristige Ziel lautet: **Die tatsächlich startende WPF-Anwendung soll sichtbar das Versprechen des Dashboard-Screenshots einlösen, ohne ihre bestehende Funktionalität durch eine statische Demo zu ersetzen.**

## 2. Kritische Bestandsaufnahme

Der aktuelle Code ist näher am Zielbild, als der alte README-Status "concept and planning phase" vermuten lässt.

`MainWindow.xaml` enthält bereits:

- ein zweispaltiges Desktop-Layout;
- eine dunkle linke Navigation;
- SASD/Health-Branding;
- Dashboard-Überschrift und Untertext;
- einen primären "New health topic"-Button;
- drei Dashboard-Karten;
- eine HealthTopic-Liste;
- eine Statusleiste.

`CreateHealthTopicWizardWindow.xaml` enthält bereits:

- Wizard-Seitenleiste;
- Schrittüberschrift und Beschreibung;
- Formularbereich;
- Navigation durch Schritte;
- Speicherung eines Health Topics.

Das bedeutet: Für einen sichtbaren Qualitätssprung ist kein UI-Neustart erforderlich.

## 3. Aktuelle Hauptlücken

### 3.1 Navigation

Aktuell sind mehrere Navigationseinträge reine `TextBlock`-Platzhalter.

Ziel:

- echte Navigationsitems;
- ausgewählter Zustand;
- Hover/Focus;
- Icon-Platzhalter ohne externe Icon-Abhängigkeit;
- Tastaturbedienung;
- spätere Erweiterbarkeit für Dashboard, Topics, Tagebuch, Messwerte, Sessions, Routinen, Quellen und Dokumente.

### 3.2 Design Tokens

Farben existieren bereits als Application Resources, aber das visuelle System soll zentraler werden.

Benötigt werden Tokens/Styles für:

- Hintergrund;
- Sidebar;
- Primary/Accent;
- Card;
- Border;
- Headline/Body/Muted Text;
- Spacing;
- CornerRadius;
- Button Primary/Secondary/Ghost;
- NavigationItem;
- Card;
- DataGrid/ListView;
- TextBox/ComboBox;
- Focus states.

Keine externen Theme-Pakete einführen, solange WPF-Bordmittel genügen.

### 3.3 Dashboard-Hierarchie

Das Dashboard soll den Konzeptcharakter behalten:

1. klare Seitentitel-Zone;
2. sichtbare Hauptaktion;
3. kompakte Statuskarten;
4. zentraler Arbeitsbereich;
5. ruhige Statusleiste.

Karten dürfen zunächst bestehende Daten anzeigen. Neue fachliche Kennzahlen werden erst eingeführt, wenn die zugrunde liegenden Module implementiert sind.

### 3.4 HealthTopic-Liste

Die Liste soll von einer technischen GridView-Darstellung zu einer ruhigeren Produktansicht entwickelt werden:

- bessere Zeilenhöhe;
- klarere Typografie;
- Status/Priorität als visuelle, aber nicht alarmistische Badges;
- sinnvolle Empty State;
- Doppelklick/Öffnen vorbereiten;
- keine erfundenen medizinischen Warnfarben.

### 3.5 Wizard

Der Wizard soll visuell zum Dashboard passen:

- gleiche Design Tokens;
- klarer aktueller Schritt;
- Fortschrittsanzeige;
- bessere Feldgruppen;
- gute Validierung;
- sicherer Abbruch;
- später Draft/Autosave.

Die fachlichen 10 Zielschritte bleiben dokumentiert; die aktuelle Implementierung darf zunächst weniger Schritte besitzen, solange die Erweiterungsstruktur sauber bleibt.

## 4. Zielnavigation für spätere Versionen

Die Navigation wird früh so vorbereitet, dass folgende Bereiche später ohne UI-Umbau ergänzt werden können:

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

Nicht implementierte Bereiche dürfen disabled/hidden sein. "Later"-Text im Produkt-UI soll schrittweise verschwinden.

## 5. Screenshot-first Arbeitsphasen

### UI-01 - Visual Foundation

- Resource Dictionaries bzw. zentrale Styles ordnen.
- Farben und Typografie vereinheitlichen.
- Basisstyles für Buttons, Cards, Navigation und Form Controls.
- Bestehende Funktion beibehalten.

**Done:** App baut und sieht bereits konsistenter aus.

### UI-02 - Application Shell

- Sidebar an Konzeptbild angleichen.
- Navigation als echte Elemente.
- Branding, Abstände, Header und Footer polieren.
- Fenstergrößen und Minimumgrößen prüfen.

**Done:** Der erste visuelle Eindruck entspricht klar dem Screenshot.

### UI-03 - Dashboard

- Kartenlayout feinjustieren.
- HealthTopic-Arbeitsbereich polieren.
- Empty State.
- Status-/Prioritätsdarstellung.
- sinnvolle Focus-/Hover-Zustände.

**Done:** Dashboard ist vorzeigbar, ohne Fake-Daten zu benötigen.

### UI-04 - Wizard

- gleiche Designsprache;
- Schrittanzeige;
- Formularspacing;
- Buttons und Validierung;
- Zusammenfassungsansicht.

**Done:** Wizard wirkt wie Bestandteil desselben Produkts.

### UI-05 - Navigation Host

- technische Navigation so vorbereiten, dass kommende Seiten als Views eingebunden werden können;
- kein Domain-Code im Navigation Host;
- bestehendes Dashboard als erste echte Seite.

**Done:** Neue Fachmodule können später als Seiten ergänzt werden, ohne MainWindow erneut grundlegend umzubauen.

## 6. Akzeptanzkriterien

Für das erste Screenshot-Ziel:

- WPF-App startet ohne Fehler.
- Bestehende HealthTopics werden weiterhin geladen.
- Neuer HealthTopic-Wizard funktioniert weiterhin.
- Sidebar, Header, Karten und Arbeitsbereich entsprechen in Hierarchie und Stil dem Konzeptbild.
- Keine statische Screenshot-Nachbildung.
- Keine echten Gesundheitsdaten in Demo/Tests.
- Mindestfenstergröße bleibt benutzbar.
- Tastatur-Fokus ist erkennbar.
- UI-Texte bleiben nicht-diagnostisch.

## 7. Visuelle Verifikation

Bei UI-PRs soll möglichst ein aktueller Screenshot der laufenden App mit dem Konzeptbild verglichen werden.

Prüfpunkte:

- Gesamtproportion;
- Sidebar-Breite;
- Headerhöhe;
- Kartenabstände;
- Typografiehierarchie;
- Buttongewicht;
- Listendichte;
- Border/CornerRadius;
- ruhiges Gesamtbild.

Pixel-Perfektion ist nicht das erste Ziel. Wichtig ist, dass Informationshierarchie und visuelle Sprache eindeutig zusammenpassen.

## 8. Was ausdrücklich nicht Teil des ersten UI-Sprints ist

- vollständige Datenbankmigration;
- Ernährungstagebuch;
- Wetter-API;
- Notification Service;
- OCR;
- FHIR;
- KI;
- Kontaktverwaltung;
- komplexe Diagramme.

Diese Funktionen sind dokumentiert, sollen aber nicht den Weg zu einem vorzeigbaren, funktionierenden UI blockieren.
