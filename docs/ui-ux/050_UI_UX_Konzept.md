# 050 - UI-/UX-Konzept

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: UI-/UX-Konzept  
Status: Entwurf  

## 1. Ziel des UI-/UX-Konzepts

Die Anwendung soll sensible, komplexe und teilweise emotionale Informationen einfach, ruhig und nachvollziehbar erfassbar machen. Der Nutzer soll nicht das Gefühl haben, in einer medizinischen Fachsoftware zu arbeiten, sondern in einem gut strukturierten persönlichen Recherche- und Dokumentationssystem.

Die UI muss Ordnung schaffen, ohne zu bevormunden. Sie darf keine Diagnose suggerieren und keine medizinische Dringlichkeit behaupten, die nicht fachlich abgesichert ist.

## 2. UX-Leitprinzipien

1. **Ruhig und sachlich**  
   Keine alarmistische Darstellung.

2. **Schnelle Erfassung**  
   Symptome, Notizen, Dokumente und Fragen müssen schnell hinzugefügt werden können.

3. **Struktur ohne Zwang**  
   Der Nutzer soll mit wenigen Pflichtfeldern starten können.

4. **Wizard für komplexe Anlage**  
   Neue Gesundheitsthemen werden Schritt für Schritt angelegt.

5. **Nichts verlieren**  
   Entwürfe automatisch speichern.

6. **Alles wiederfinden**  
   Suche, Tags, Timeline und Filter sind zentrale UI-Bestandteile.

7. **Arztgespräch vorbereiten**  
   Fragen, Zusammenfassung und ausgewählte Dokumente müssen leicht exportierbar sein.

8. **Datenschutz sichtbar machen**  
   Backup- und Sicherheitsstatus sollen verständlich angezeigt werden.

## 3. Hauptnavigation

Empfohlene Hauptnavigation links:

```text
Dashboard
Gesundheitsthemen
Symptome & Verlauf
Dokumente
Messwerte & Labor
Medikamente
Quellen & Informationen
Termine
Offene Fragen
Notizen
Suche

Berichte & Export
  Zusammenfassungen
  Arztmappe erstellen
  Exportieren

System
  Einstellungen
  Backup & Wiederherstellung
  Hilfe
```

## 4. Dashboard

Das Dashboard soll Überblick geben, aber nicht überladen.

### 4.1 Dashboard-Karten

| Karte | Inhalt |
|---|---|
| Gesundheitsthemen | Anzahl aktiv, Anzahl archiviert |
| Symptome letzte 7 Tage | Anzahl Einträge, neue Symptome |
| Dokumente | Gesamtzahl, neu importiert |
| Messwerte | Anzahl, auffällige Werte als dokumentierte Abweichung, keine Diagnose |
| Offene Fragen | offen, überfällig, für nächsten Termin |
| Backupstatus | letztes Backup, Warnhinweis, wenn lange her |

### 4.2 Dashboard-Listen

- aktive Gesundheitsthemen
- letzte Aktivitäten
- nächste Termine
- zuletzt geöffnete Dokumente
- offene Fragen
- auffällige Messwerte
- persönliche Notizen
- Schnellzugriff

### 4.3 Dashboard-Schnellaktionen

- Neues Gesundheitsthema
- Neuer Eintrag
- Symptom eintragen
- Dokument hinzufügen
- Messwert eintragen
- Arztmappe erstellen
- Backup jetzt erstellen

## 5. Gesundheitsthemen-Ansicht

### 5.1 Liste

Die Liste zeigt:

- Titel
- Status
- Priorität
- letzte Änderung
- Anzahl Dokumente
- Anzahl offene Fragen
- letzte Symptomaktivität
- Tags

### 5.2 Detailansicht

Tabs oder Seitenbereiche:

| Bereich | Inhalt |
|---|---|
| Übersicht | Zusammenfassung, Status, wichtige Hinweise |
| Verlauf | Timeline |
| Symptome | Symptomliste und Verlauf |
| Dokumente | zugeordnete Dateien |
| Quellen | Webseiten, Studien, Arztinformationen |
| Messwerte | relevante Messwerte |
| Fragen | offene und beantwortete Fragen |
| Termine | zugehörige Termine |
| Notizen | freie Notizen |
| Export | Zusammenfassung/Arztmappe |

## 6. Wizard: Neues Gesundheitsthema anlegen

Der Wizard ist eine Kernfunktion. Er darf nicht nur eine einfache Eingabemaske sein. Er soll den Nutzer dabei unterstützen, ein neues Thema strukturiert anzulegen und sofort die wichtigsten Informationen zu erfassen.

### 6.1 Wizard-Grundregeln

- Jeder Schritt kann übersprungen werden, außer Pflichtfelder der Grunddaten.
- Der Wizard speichert automatisch Entwürfe.
- Der Nutzer kann später fortsetzen.
- Dokumente werden zunächst vorgemerkt und erst beim Abschluss endgültig übernommen.
- Am Ende gibt es eine Zusammenfassung.
- Nach Abschluss wird automatisch die Detailansicht geöffnet.

### 6.2 Wizard-Schritte

| Schritt | Name | Zweck |
|---:|---|---|
| 1 | Grunddaten | Titel, Typ, Kategorie, Beginn, Priorität |
| 2 | Diagnose / Status | bestätigt, Verdacht, Beobachtung, ausgeschlossen, unbekannt |
| 3 | Symptome | erste Symptome und Beobachtungen erfassen |
| 4 | Dokumente | Arztbriefe, Laborwerte, Bilder, PDFs anhängen |
| 5 | Quellen & Informationen | Webseiten, Bücher, Studien, Arztinfos speichern |
| 6 | Ärzte / Ansprechpartner | behandelnde Stellen oder Ansprechpartner erfassen |
| 7 | Medikamente / Maßnahmen | relevante Medikamente/Maßnahmen als Notiz dokumentieren |
| 8 | Messwerte / Laborwerte | erste Messwerte oder Laborwerte aufnehmen |
| 9 | Offene Fragen | Fragen für Arzt oder Recherche erfassen |
| 10 | Zusammenfassung | prüfen, speichern, erste Struktur erzeugen |

### 6.3 Schritt 1: Grunddaten

Felder:

| Feld | Pflicht | Beschreibung |
|---|---:|---|
| Titel | Ja | Name des Gesundheitsthemas |
| Synonyme | Nein | alternative Begriffe |
| Kategorie/Organsystem | Nein | Auswahl oder Freitext |
| ICD-10-Code | Nein | optionaler Code |
| erstmalig bemerkt am | Nein | Datum |
| Thema-Typ | Ja | Erkrankung, Verdacht, Symptom, Recherchethema |
| Priorität | Nein | niedrig, mittel, hoch |
| Farbe/Markierung | Nein | visuelle Kennzeichnung |
| Notizen | Nein | erste Gedanken |

Validierung:

- Titel darf nicht leer sein.
- Dublettenwarnung bei ähnlichem Titel oder Synonym.
- ICD-10 nicht validieren erzwingen, nur optional prüfen.

### 6.4 Schritt 2: Diagnose / Status

Ziel: Unsicherheit sichtbar machen.

Felder:

- Status: bestätigt, Verdacht, in Beobachtung, ausgeschlossen, unbekannt
- diagnostiziert am
- diagnostiziert durch
- Sicherheit/Verlässlichkeit der Information
- Kurznotiz

Wichtig: UI darf nicht suggerieren, dass der Nutzer eine Diagnose stellen soll. Besser: „Wie ist der aktuelle Kenntnisstand?“

### 6.5 Schritt 3: Symptome

Funktionen:

- Symptom hinzufügen
- Schweregrad erfassen
- Startdatum oder Beobachtungszeitpunkt
- Dauer
- Auslöser/Kontext
- freie Notiz
- mehrere Symptome in einer Tabelle erfassen

Beispiel-Spalten:

```text
Symptom | Stärke 0-10 | seit/bemerkt am | Häufigkeit | Kontext | Notiz
```

### 6.6 Schritt 4: Dokumente

Funktionen:

- Dateien per Drag & Drop hinzufügen
- Dateityp erkennen
- Dokumentdatum erfassen
- Titel vergeben
- Dokumenttyp auswählen
- Beschreibung ergänzen
- sensible Dokumente markieren
- Dublettenprüfung per Hash

Dokumenttypen:

- Arztbrief
- Laborbericht
- Bild/Foto
- Scan
- Rezept/Medikationshinweis
- Studie/Artikel
- Webseite/PDF
- Rechnung/Versicherung
- Sonstiges

### 6.7 Schritt 5: Quellen & Informationen

Felder:

- Quellentyp
- Titel
- URL
- Autor/Anbieter
- Abrufdatum
- Veröffentlichungsdatum
- Verlässlichkeitseinschätzung
- Notiz
- zugehörige Dokumente

Quellenqualität:

| Stufe | Bedeutung |
|---|---|
| unbekannt | noch nicht bewertet |
| niedrig | Forum, unklare Herkunft |
| mittel | allgemeine Webseite, Magazin |
| hoch | Fachartikel, Leitlinie, Klinikseite |
| professionell | Arzt, Labor, offizielle medizinische Stelle |
| offiziell | Behörde, Fachgesellschaft, offizieller Standard |

### 6.8 Schritt 6: Ärzte / Ansprechpartner

V1 kann zunächst Freitext verwenden. Später eigenes Kontaktmodell.

Felder:

- Name/Praxis/Klinik
- Fachrichtung
- Rolle im Thema
- Kontaktinfo als Notiz
- nächster Termin
- offene Punkte

### 6.9 Schritt 7: Medikamente / Maßnahmen

Wichtig: Keine Therapieempfehlung, nur Dokumentation.

Felder:

- Name der Maßnahme/des Medikaments
- Zeitraum
- Dosierung/Anwendung als Freitext
- verordnet durch
- beobachtete Wirkung/Nebenwirkung
- Notiz

Warntext:

> Änderungen an Medikamenten oder Maßnahmen dürfen nur mit medizinischem Fachpersonal abgestimmt werden.

### 6.10 Schritt 8: Messwerte / Laborwerte

Felder:

- Typ des Messwerts
- Datum
- Wert
- Einheit
- Referenzbereich als Freitext
- Quelle/Dokument
- Notiz

V1 sollte einfache Messwerte erfassen. Komplexe Laborwertinterpretation wird verschoben.

### 6.11 Schritt 9: Offene Fragen

Funktionen:

- mehrere Fragen erfassen
- Ziel der Frage auswählen: Arzt, eigene Recherche, Apotheke, Labor, Versicherung
- Priorität setzen
- Terminbezug herstellen

Beispiele:

- „Welche Ursachen kommen für diesen Wert infrage?“
- „Muss dieser Laborwert kontrolliert werden?“
- „Welche Unterlagen soll ich zum Termin mitbringen?“

### 6.12 Schritt 10: Zusammenfassung

Die Zusammenfassung zeigt:

- Gesundheitsthema
- Status
- Symptome
- Dokumente
- Quellen
- Fragen
- Warnungen/fehlende Angaben
- was nach dem Speichern erzeugt wird

Aktionen:

- Speichern und öffnen
- Zurück zu Schritt X
- Als Entwurf speichern
- Abbrechen

## 7. Autosave und Entwürfe

Der Wizard speichert automatisch:

- nach Schrittwechsel
- nach Dokumentauswahl
- alle 30-60 Sekunden bei Änderungen
- vor Schließen des Fensters

Beim Neustart:

- aktive Entwürfe anzeigen
- Fortsetzen oder verwerfen
- klare Warnung vor Verwerfen

## 8. Suche und Filter in der UI

Globale Suche oben im Hauptfenster.

Filter:

- Gesundheitsthema
- Dokumenttyp
- Datum
- Tag
- Status
- Quelle
- offene Fragen
- archiviert/nicht archiviert

Suchergebnisgruppen:

- Gesundheitsthemen
- Dokumente
- Einträge
- Symptome
- Fragen
- Quellen
- Termine

## 9. Timeline-UI

Die Timeline soll Verlauf und Zusammenhang sichtbar machen.

Darstellung:

```text
Datum      Typ          Kurztext
23.05.25   Dokument     Laborwerte importiert
24.05.25   Symptom      Kopfschmerzen Stärke 6
27.05.25   Termin       HNO-Arzt
```

Filter:

- alle Ereignisse
- nur Dokumente
- nur Symptome
- nur Termine
- nur Messwerte
- nur Fragen/Antworten

## 10. Dokumentenansicht

Funktionen:

- Dokumentliste
- Vorschau, soweit möglich
- Metadaten bearbeiten
- Themen zuordnen
- Tags setzen
- Quelle zuordnen
- Hash/Dublettenhinweis anzeigen
- Exportfreigabe setzen

## 11. Arztmappe-UI

Schritte:

1. Zweck wählen: Arzttermin, Zweitmeinung, eigene Ablage
2. Gesundheitsthemen auswählen
3. Zeitraum auswählen
4. Dokumente auswählen
5. Fragen auswählen
6. Zusammenfassung prüfen
7. Exportformat wählen
8. Datenschutzwarnung bestätigen

## 12. Barrierefreiheit und Lesbarkeit

- skalierbare Schrift
- klare Kontraste
- Tastaturbedienung
- keine reine Farbcodierung ohne Text
- verständliche Fehlermeldungen
- große Klickflächen
- optional kompakter und komfortabler Modus

## 13. Fehlermeldungen

Gute Fehlermeldungen:

- sagen, was passiert ist
- sagen, was der Nutzer tun kann
- vermeiden medizinische Bewertung
- enthalten technische Fehler-ID

Beispiel:

```text
Das Dokument konnte nicht gespeichert werden.
Bitte prüfen Sie, ob der Speicherort verfügbar ist. Ihre bisherigen Angaben im Wizard wurden als Entwurf gesichert.
Fehler-ID: DOC-STORE-003
```

## 14. Kontextmenüs und Doppelklick

Erwartbare Aktionen sollen funktionieren:

| Bereich | Doppelklick | Kontextmenü |
|---|---|---|
| Gesundheitsthemen | Detail öffnen | bearbeiten, archivieren, exportieren |
| Dokumente | Vorschau öffnen | zuordnen, Metadaten, exportieren |
| Fragen | bearbeiten | als erledigt markieren, Termin zuordnen |
| Timeline | Detail öffnen | Eintrag öffnen, Dokument öffnen |
| Tags | filtern | umbenennen, zusammenführen |

## 15. UI-Konzeptbilder

Die Konzeptbilder liegen im Repository unter:

```text
docs/screenshots/dashboard-concept.png
docs/screenshots/condition-wizard-concept.png
```

Sie sind nicht als fertige App-Screenshots zu verstehen, sondern als Orientierung für Layout, Informationsdichte und Bedienlogik.
