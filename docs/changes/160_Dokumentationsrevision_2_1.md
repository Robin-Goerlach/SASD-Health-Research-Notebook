# 160 - Dokumentationsrevision 2.1

Projekt: SASD Health Research Notebook  
Stand: 2026-09-30  
Dokumenttyp: Änderungsprotokoll / konsolidierte Produktentscheidung  
Status: Baseline 2.1

## 1. Zweck

Diese Revision konsolidiert die Anforderungen, die seit der ersten Dokumentationsbaseline hinzugekommen oder deutlich präzisiert worden sind. Sie ersetzt nicht die historischen Lasten-/Pflichtenhefte, sondern erklärt verbindlich, wie deren offene oder allgemein formulierte Stellen ab Baseline 2.1 zu verstehen sind.

Die Revision wurde gegen den aktuellen Repository-Stand von `main` geprüft. Die bestehende Anwendungsschale, das HealthTopic-Modell, der lokale JSON-Speicher, das Dashboard und der erste Wizard bleiben eine sinnvolle technische Grundlage.

## 2. Kritische Gesamtentscheidung

Das Projekt soll **nicht** gleichzeitig alle geplanten Gesundheitsmodule implementieren. Zwei Ziele werden getrennt verfolgt:

1. **Kurzfristig sichtbares Produkt:** Die bestehende WPF-Anwendung wird gezielt an die Konzept-Screenshots im Repository herangeführt.
2. **Langfristig tragfähiges Fachmodell:** Neue Anforderungen werden so modelliert, dass spätere Module ohne fachlichen oder technischen Bruch ergänzt werden können.

Die Screenshots sind damit ein bewusstes UI-Zielbild; die Fachmodule werden schrittweise als vertikale Slices ergänzt.

## 3. Verbindliche Produktgrenzen

Die Anwendung ist Infrastruktur für persönliche Dokumentation und Recherche.

Sie darf:

- vom Nutzer eingegebene Informationen speichern;
- Quellen und genaue Fundstellen verwalten;
- Messwerte und Kontextdaten darstellen;
- vom Nutzer definierte Ziele, Routinen und Erinnerungen verwalten;
- dokumentieren, dass eine Handlungsanweisung laut Nutzer von Arzt, Therapeut, Coach oder einer anderen Quelle stammt;
- Arzt-/Coach-Sitzungen vorbereiten und nachbereiten;
- deskriptive Zusammenfassungen und Exporte erzeugen.

Sie darf nicht:

- aus einer Erkrankung automatisch Handlungen ableiten;
- Diagnosen stellen;
- Therapie- oder Dosisempfehlungen erzeugen;
- Messwerte automatisch medizinisch bewerten;
- Kausalität zwischen Wetter, Ernährung, Symptomen oder Messwerten behaupten;
- eine subjektive Quellenbewertung als medizinische Validierung darstellen.

## 4. Neue bzw. präzisierte Fachmodule

### 4.1 Beobachtungs- und Kontexttagebuch

Beobachtungen können mit Kontext verknüpft werden, z. B.:

- Ernährung und Getränke;
- Alkohol;
- Schlaf;
- Stress;
- Bewegung;
- Wetter;
- Medikamente;
- besondere Ereignisse.

Die Anwendung dokumentiert zeitliche Zusammenhänge. Sie beweist keine Ursache.

### 4.2 Ernährungstagebuch

Ernährung wird als eigenständiges Modul vorgesehen und nicht nur als Freitext-Kontext.

Erfassbar sollen sein:

- Mahlzeiten und Getränke;
- Zeitpunkte;
- grobe Mengen;
- besondere Lebensmittel;
- Alkohol;
- Essenspausen/Fasten als Nutzereintrag;
- Verträglichkeit und Beobachtungen;
- Verknüpfung mit Messwerten, Symptomen, Gewicht und Gesundheitsthemen.

Eine Kalorien- oder Diät-Engine ist für die frühe Version nicht erforderlich.

### 4.3 Vitalwerte und Messwerte

Messungen umfassen mindestens:

- Blutdruck;
- Puls;
- Gewicht;
- Blutzucker;
- Temperatur;
- optional SpO2 und weitere benutzerdefinierte Typen.

Rohwert und eigene Interpretation/Notiz bleiben getrennt.

### 4.4 Automatische Kontextanreicherung / Wetter

Zu einem Messwert oder einer Beobachtung kann optional ein unveränderlicher `WeatherSnapshot` gespeichert werden.

Mögliche Felder:

- Temperatur;
- Luftdruck;
- Luftfeuchtigkeit;
- Niederschlag;
- Wetterlage;
- Wind;
- Datenquelle;
- Wetterzeitpunkt;
- grober Ort bzw. Ortsreferenz.

Das Feature ist opt-in. Ein Fehler beim Wetterabruf darf das Speichern des Messwerts niemals verhindern. Historische Snapshots werden nicht nachträglich mit aktuellen Wetterdaten überschrieben.

### 4.5 Handlungsanweisungen, Ziele, Routinen und Fortschritt

Die Anwendung benötigt ein allgemeines Konzept `HealthAction`.

Eine HealthAction kann z. B. sein:

- eigene Entscheidung;
- ärztlich besprochene Handlung;
- physiotherapeutische Übung;
- Coaching-Aufgabe;
- Recherche-/Klärungsaufgabe.

Gespeichert werden Herkunft, Datum, Quelle/Sitzung, Status, optional Zeitraum und Verknüpfungen.

Davon getrennt ist eine `Routine`: Sie beschreibt die wiederkehrende Durchführung.

Fortschrittstypen:

- Ja/Nein;
- Zähler, z. B. 2 von 4;
- Menge, z. B. 1,0 von 2,0 l;
- Dauer;
- Messaufgabe;
- Dokumentationsaufgabe.

Die Anwendung erzeugt keine Ziele automatisch aus Erkrankungen.

### 4.6 Notification Service

Benachrichtigungen erinnern ausschließlich an vom Nutzer konfigurierte Aufgaben/Routinen.

Geplant sind:

- feste Zeiten oder Zeitfenster;
- Wiederholungen;
- Snooze;
- "heute überspringen";
- "erledigt";
- diskreter Benachrichtigungsmodus;
- Ruhezeiten.

Beispiel: "Heute sind noch 2 von 4 Einheiten offen." Die App entscheidet nicht, dass ein Nutzer medizinisch trinken, laufen oder messen muss.

### 4.7 Quellen, Belegstellen und Vertrauensbewertung

Eine Quelle muss konkrete Fundstellen unterstützen:

- URL;
- PDF/Dokument;
- Buch;
- Seite;
- Absatz/Abschnitt;
- Kapitel;
- Zitat oder Exzerpt;
- Abrufdatum;
- DOI/ISBN soweit vorhanden.

Eine `EvidenceNote` trennt Aussage, Fundstelle und eigene Einordnung.

Vertrauensstufen sind subjektive Recherchemetadaten. Sterne können als UI-Darstellung verwendet werden, müssen aber durch eine Bedeutung/Begründung ergänzt werden.

### 4.8 Medien und Bilder

Bilder und Medien werden als allgemeine Ressourcen verwaltet und können verknüpft werden mit:

- Gesundheitsthemen;
- Übungen;
- Routinen;
- Sitzungen;
- Tagebucheinträgen;
- Quellen;
- Dokumenten.

Beispiel: bebilderte Dehnungsübungen.

Originaldatei und Metadaten bleiben getrennt; Vorschaubilder dürfen generiert werden. Quellen-/Urheberhinweise sollen speicherbar sein.

### 4.9 Sitzungen / Arztbesuche / Coaching

Das bisherige Terminmodell wird zu einem allgemeinen `Session`-Konzept weiterentwickelt.

Sitzungstypen können sein:

- Arztbesuch/Kontrolluntersuchung;
- Coaching;
- Physiotherapie;
- Ernährungsberatung;
- Apothekerberatung;
- sonstige Gesundheitsberatung.

Eine Sitzung besitzt drei fachliche Phasen:

1. Vorbereitung
2. Durchführung/Protokoll
3. Nachbereitung

Erfassbar sollen sein:

- Anlass;
- beteiligte Gesundheitsthemen;
- Fragen;
- mitzunehmende Dokumente/Messwerte;
- besprochene Punkte;
- mündliche Aussagen als eigene Gesprächsnotiz;
- Untersuchungsvorgänge;
- Entscheidungen/Vereinbarungen;
- daraus resultierende HealthActions;
- offene Punkte;
- erwartete Dokumente;
- Folgetermine.

Die Herkunft jeder Aussage soll unterscheidbar sein, z. B. "während Gespräch notiert", "aus Erinnerung", "Arztbrief", "Laborbericht".

### 4.10 Kontakte

Kontaktdaten von Ärzten und anderen Ansprechpartnern sind sinnvoll, aber das Health Notebook soll kein vollständiges CRM werden.

Frühe Version:

- Name;
- Praxis/Organisation;
- Fachrichtung/Rolle;
- Telefon;
- E-Mail;
- Webseite;
- Anschrift;
- Notiz.

Das Datenmodell muss eine optionale `ExternalContactId` bzw. `ContactReference` unterstützen, damit später eine zentrale SASD-Kontakteverwaltung angebunden werden kann.

### 4.11 Wechselwirkungsnotizen

Die Anwendung kann mögliche Medikament-/Lebensmittel-/Supplement-Zusammenhänge als Recherche- oder Klärungsnotizen verwalten.

Keine automatische medizinische Interaktionsprüfung ohne gesonderte validierte Datenbasis und regulatorische Bewertung.

## 5. Fachliches Zielbild

Die wichtigsten Aggregate/Module werden langfristig so gedacht:

```text
HealthTopic
  ├─ Observation
  │   ├─ SymptomObservation
  │   ├─ Measurement
  │   ├─ NutritionEntry
  │   └─ ContextSnapshot
  ├─ Session
  │   ├─ Preparation
  │   ├─ Notes
  │   └─ FollowUp
  ├─ HealthAction
  │   ├─ Routine
  │   ├─ Progress
  │   └─ Reminder
  ├─ Knowledge
  │   ├─ Source
  │   ├─ SourceLocation
  │   └─ EvidenceNote
  ├─ MediaResource
  └─ ContactReference
```

Dies ist ein Zielmodell, keine Aufforderung, alle Entitäten sofort zu implementieren.

## 6. Priorisierung

### Jetzt

- WPF-Oberfläche an Konzept-Screenshot annähern;
- bestehende HealthTopic-Funktion stabil halten;
- Codex-Arbeitsgrundlage und CI schaffen;
- Baseline 2.1 dokumentieren.

### Frühe vertikale Slices

1. HealthTopic Detail + Timeline/Einträge
2. Quellen + genaue Fundstelle
3. Messwerte
4. Sitzungen/Arztbesuche + Fragen
5. HealthAction/Routine + Fortschritt
6. Notification Service
7. Ernährung/Context
8. Medien
9. Kontakte-Adapter

### Später

- automatische Wetteranreicherung;
- OCR;
- Geräteimporte;
- FHIR;
- lokale/optionale KI;
- zentrale SASD-Kontakteintegration;
- validierte medizinische Datenbanken nur nach gesonderter Bewertung.

## 7. Auswirkungen auf bestehende Dokumente

Diese Revision ergänzt bzw. präzisiert:

- Lastenheft;
- Pflichtenheft;
- Feature-Backlog;
- Datenmodell;
- UI-/UX-Konzept;
- Roadmap;
- ADRs;
- Sicherheits-/Datenschutzkonzept;
- Such-/Wissenskonzept;
- Import-/Exportkonzept.

Bei Widersprüchen zu älteren Entwurfsformulierungen gilt für die oben beschriebenen Themen Baseline 2.1.

## 8. Nächster technischer Fokus

Der unmittelbare Codex-Auftrag ist in `docs/development/145_Codex_Arbeitsauftrag.md` beschrieben. Das visuelle Ziel wird in `docs/ui-ux/055_UI_Zielbild_und_Screenshot_Plan.md` festgelegt.


---

## 9. Nachtrag 2.1a - Frontendstrategie (2026-09-30)

Nach Erstellung der Baseline 2.1 wurde ein funktionsfähiges Windows-Forms-Frontend implementiert und in `main` übernommen.

Damit gilt ab sofort:

- **WinForms ist das primäre Frontend für neue UI- und Fachentwicklung.**
- **WPF bleibt buildbar als Referenz- und Kompatibilitätsfrontend.**
- Neue Fachfeatures werden nicht automatisch parallel in beiden Oberflächen implementiert.
- Domain, Application und Infrastructure bleiben UI-unabhängig.
- Beide Frontends verwenden solange die JSON-Persistenz aktiv ist denselben lokalen Datenbestand.
- Die vorhandenen Konzept-Screenshots bleiben visuelles Ziel; die kurzfristige Screenshot-Arbeit bezieht sich primär auf WinForms.

Dieser Nachtrag ersetzt die frühere operative Annahme, dass die WPF-Oberfläche zuerst bis zum Screenshot-Ziel ausgebaut wird. Die fachlichen Entscheidungen der Baseline 2.1 bleiben unverändert.
