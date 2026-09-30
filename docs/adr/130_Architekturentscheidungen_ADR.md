# 130 - Architekturentscheidungen (ADR)

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Architecture Decision Records  
Status: Entwurf  

## ADR-0021 - Explizite Entwicklungs- und Testdatenumleitung

Status: Akzeptiert
Datum: 2026-09-30

### Entscheidung

Infrastructure löst `SASD_HEALTHNOTEBOOK_DATA_PATH` als vollständig qualifizierten
Datenordner auf. Ohne Variable bleibt der vorhandene Produktpfad unverändert.
Ein ungültiger gesetzter Wert erzeugt einen Fehler, keinen Rückfall auf persönliche
Daten. Der Repository-Konstruktor erfasst den aufgelösten Dateipfad bei Erstellung;
beide unveränderten Frontend-Bootstrapper nutzen diesen gemeinsamen Mechanismus.

### Konsequenzen

Keine Formatänderung, Migration, Löschung oder neue Abhängigkeit. Der allgemeine
Override ist keine Sandbox; Entwickler wählen den Pfad ausdrücklich. Der sichere
Repository-Launcher verwendet ausschließlich `.codex/`, prüft Reparse Points und
setzt nur Prozessvariablen, einschließlich lokaler Tool-Caches und Tempordner.
Smoke-Tests prüfen den Produktpfad ohne Dateioperationen und schreiben anschließend
nur synthetische Daten in neue Repository-Unterordner. Sie lehnen externe Ziele
und Reparse-Point-Vorfahren ab und bereinigen keine bestehenden Daten.

## ADR-0001 - Lokal-first statt Cloud-first

Status: Vorgeschlagen / empfohlen

### Kontext

Die Anwendung verarbeitet persönliche Gesundheitsdaten. Cloud-Speicherung erhöht Datenschutz-, Sicherheits- und Compliance-Aufwand erheblich.

### Entscheidung

Die Anwendung wird zunächst lokal-first entwickelt. Alle Kernfunktionen müssen ohne Cloud funktionieren.

### Konsequenzen

Vorteile:

- geringerer Datenschutzaufwand
- offline nutzbar
- bessere Kontrolle über Daten
- einfacherer Start

Nachteile:

- kein Geräte-Sync in V1
- Nutzer muss Backup selbst ernst nehmen
- mobile Nutzung später aufwendiger

## ADR-0002 - Keine Diagnose- oder Therapie-Software in V1

Status: Vorgeschlagen / empfohlen

### Kontext

Software mit medizinischer Zweckbestimmung kann regulatorisch relevant werden. Außerdem besteht das Risiko, dass Nutzer automatische Ausgaben falsch interpretieren.

### Entscheidung

V1 dokumentiert, strukturiert, sucht und exportiert Informationen. V1 stellt keine Diagnosen und gibt keine Therapieempfehlungen.

### Konsequenzen

- UI-Texte müssen vorsichtig formuliert werden.
- Messwerte dürfen nicht automatisch medizinisch bewertet werden.
- KI-Funktionen werden nicht in V1 integriert.

## ADR-0003 - Desktop-first mit C#/.NET

Status: Vorgeschlagen / empfohlen

### Kontext

Der Nutzer arbeitet bereits mit C#/.NET-Desktopprojekten. Die Anwendung soll lokal, sicher und ohne Serverzwang starten.

### Entscheidung

Das Projekt startet als Windows-Desktop-App mit C#/.NET.

### Konsequenzen

- schnelle Integration in bestehende SASD-Erfahrung
- gute lokale Dateiverarbeitung
- später ggf. Portierung/Erweiterung nötig

## ADR-0004 - WPF bevorzugt gegenüber WinForms

Status: Vorgeschlagen

### Kontext

Das Projekt benötigt komplexere Layouts, Dashboard, Wizard, Detailansichten und langfristig gute UI-Struktur.

### Entscheidung

WPF wird als bevorzugte UI-Technologie empfohlen.

### Konsequenzen

Vorteile:

- bessere Layoutmöglichkeiten
- MVVM geeignet
- modernes UI leichter umsetzbar

Nachteile:

- etwas höhere Einstiegskomplexität als WinForms

## ADR-0005 - SQLite als lokale Datenbank

Status: Vorgeschlagen / empfohlen

### Kontext

Die Anwendung ist lokal-first, ein einzelner Nutzer arbeitet auf einem Desktop. Eine Serverdatenbank wäre für V1 unnötig.

### Entscheidung

SQLite wird als primäre lokale Datenbank verwendet.

### Konsequenzen

- einfache Installation
- gute Testbarkeit
- später muss Verschlüsselung separat geklärt werden

## ADR-0006 - Dokumente im Dateisystem statt als BLOBs

Status: Vorgeschlagen

### Kontext

Arztbriefe, PDFs, Bilder und Scans können groß werden. BLOB-Speicherung in SQLite macht Backup, Vorschau und externe Verarbeitung schwerer.

### Entscheidung

Dokumente werden in einem verwalteten Dokumentenspeicher im Dateisystem abgelegt. Die Datenbank speichert Metadaten, Pfad und Hash.

### Konsequenzen

- einfacherer Dokumentenzugriff
- Datenbank bleibt kleiner
- Konsistenz zwischen DB und Dateisystem muss geprüft werden

## ADR-0007 - Markdown-Export zuerst, PDF später

Status: Vorgeschlagen

### Kontext

PDF-Generierung kann technische Komplexität erzeugen. Markdown ist transparent und einfach testbar.

### Entscheidung

V1 unterstützt Markdown-Export. PDF folgt später.

### Konsequenzen

- schneller funktionsfähiger Export
- gute Git-/Diff-Fähigkeit
- Anwender wünschen später wahrscheinlich PDF

## ADR-0008 - Archivieren statt stilles Löschen

Status: Vorgeschlagen / empfohlen

### Kontext

Gesundheitsdaten können später wieder relevant werden. Versehentliches Löschen ist kritisch.

### Entscheidung

Standardaktion ist Archivieren. Hartes Löschen wird eingeschränkt und bestätigt.

### Konsequenzen

- weniger Datenverlust
- UI muss Archivfilter haben
- Datenbestand wächst stärker

## ADR-0009 - Keine externe KI in V1

Status: Vorgeschlagen / empfohlen

### Kontext

KI kann helfen, aber bei Gesundheitsdaten sind Datenschutz und Fehlinterpretation besonders riskant.

### Entscheidung

V1 enthält keine externe KI-Funktion.

### Konsequenzen

- geringeres Datenschutzrisiko
- weniger Funktionsumfang
- spätere KI-Schnittstellen können vorbereitet werden

## ADR-0010 - SQLCipher oder alternative Verschlüsselung prüfen

Status: Offen

### Kontext

SQLite ist nicht automatisch verschlüsselt. Gesundheitsdaten benötigen hohen Schutz.

### Entscheidung

Für V1/V1.1 wird geprüft, ob SQLCipher oder eine andere Verschlüsselungsstrategie eingesetzt wird.

### Konsequenzen

- Sicherheitsentscheidung muss vor echter Nutzung mit Realdaten fallen
- Backup/Restore muss Verschlüsselung berücksichtigen

## ADR-0011 - HealthEntry als flexible Basistabelle

Status: Vorgeschlagen

### Kontext

Nicht alle Gesundheitsinformationen passen sauber in starre Spezialtabellen.

### Entscheidung

Eine allgemeine `HealthEntry`-Entität bildet Notizen, Beobachtungen, Recherchehinweise, Entscheidungen und Zusammenfassungen ab.

### Konsequenzen

- V1 bleibt flexibel
- spätere Spezialisierung möglich
- UI muss Eintragstypen klar darstellen

## ADR-0012 - Wizard-Drafts persistent speichern

Status: Vorgeschlagen

### Kontext

Das Anlegen eines Gesundheitsthemas kann länger dauern. Datenverlust während der Erfassung wäre ärgerlich.

### Entscheidung

Wizard-Eingaben werden als Entwurf gespeichert und können später fortgesetzt werden.

### Konsequenzen

- bessere UX
- zusätzliche Tabelle `wizard_drafts`
- Datenschutz: Entwürfe sind ebenfalls sensibel

## ADR-Template für spätere Entscheidungen

```markdown
# ADR-XXXX - Titel

Status: Vorgeschlagen | Akzeptiert | Abgelehnt | Ersetzt
Datum: YYYY-MM-DD

## Kontext

...

## Entscheidung

...

## Konsequenzen

### Vorteile

...

### Nachteile

...

## Alternativen

...
```


---

## ADR-0013 - Konzept-Screenshots als kurzfristiges UI-Ziel

Status: Akzeptiert  
Datum: 2026-09-30

### Entscheidung

Die vorhandenen Dashboard- und Wizard-Konzeptbilder definieren die kurzfristige visuelle Richtung. Die laufende WPF-App wird inkrementell dorthin entwickelt; statische Mock-ups gelten nicht als Umsetzung.

### Konsequenzen

- UI-Qualität wird vor zusätzlichen breiten Fachmodulen priorisiert.
- Bestehende HealthTopic-Funktion bleibt erhalten.
- Styles/Design Tokens werden wiederverwendbar aufgebaut.

## ADR-0014 - HealthAction und Routine werden getrennt

Status: Akzeptiert  
Datum: 2026-09-30

### Entscheidung

Eine `HealthAction` beschreibt eine konkrete Handlung inklusive Herkunft. Eine `Routine` beschreibt die wiederkehrende Durchführung einer vom Nutzer gewählten Aktion.

### Konsequenzen

- Die Anwendung kann ärztlich/therapeutisch/coach-seitig besprochene Handlungen dokumentieren, ohne selbst medizinische Handlungen zu erzeugen.
- Reminder hängen an Nutzerkonfiguration bzw. Routine, nicht automatisch an einer Erkrankung.

## ADR-0015 - Allgemeines Session-Modell statt separater Arzt-/Coach-Silos

Status: Akzeptiert  
Datum: 2026-09-30

### Entscheidung

Arztbesuche, Kontrollen, Coaching, Physiotherapie und Beratung verwenden ein gemeinsames Session-Grundmodell mit Vorbereitung, Protokoll und Nachbereitung.

### Konsequenzen

- Gemeinsame Fragen-, Dokument-, Quellen- und Follow-up-Mechanismen.
- TK-Coach-spezifische Workflows können später auf dem allgemeinen Modell aufsetzen.

## ADR-0016 - Kontakte als Referenz, kein eingebautes CRM

Status: Akzeptiert  
Datum: 2026-09-30

### Entscheidung

Das Health Notebook speichert nur die notwendigen Ansprechpartnerdaten und eine optionale externe Kontakt-ID.

### Konsequenzen

- Kein paralleles vollständiges CRM.
- Spätere Integration einer allgemeinen SASD-Kontakteverwaltung bleibt möglich.

## ADR-0017 - Wetter als optionaler unveränderlicher Kontext-Snapshot

Status: Akzeptiert  
Datum: 2026-09-30

### Entscheidung

Wetterdaten können opt-in zu Messungen/Beobachtungen gespeichert werden. Der gespeicherte Snapshot repräsentiert den Kontext zum fachlichen Zeitpunkt und wird nicht automatisch überschrieben.

### Konsequenzen

- Wetterdienst-Ausfall darf lokale Erfassung nie blockieren.
- Wetterdaten begründen keine medizinische Kausalität.
- Standortdaten werden datensparsam behandelt.

## ADR-0018 - Quellen benötigen konkrete Fundstellen

Status: Akzeptiert  
Datum: 2026-09-30

### Entscheidung

Neben `Source` wird ein Konzept für konkrete Fundstellen (`SourceLocation`) und Aussagen (`EvidenceNote`) vorgesehen.

### Konsequenzen

- Seite, Absatz, Kapitel, URL/Anker und Exzerpt können nachvollziehbar gespeichert werden.
- Vertrauensbewertung bleibt subjektive Recherchemetadaten.

## ADR-0019 - Medienressourcen im Dateisystem, Metadaten in Persistenz

Status: Akzeptiert  
Datum: 2026-09-30

### Entscheidung

Bilder und weitere Medien werden wie Dokumente im verwalteten lokalen Dateispeicher gehalten. Fachliche Metadaten, Herkunft, Tags und Beziehungen werden strukturiert gespeichert.

### Konsequenzen

- Geeignet für Übungsbilder und spätere Vorschauen.
- Keine unnötige BLOB-Aufblähung der SQLite-Datenbank.


---

## ADR-0020 - WinForms als primäres Entwicklungsfrontend

Status: Akzeptiert  
Datum: 2026-09-30

### Kontext

Die WPF-Anwendung hat die frühe Architektur und den ersten funktionalen UI-Stand bewiesen. Parallel wurde ein Windows-Forms-Frontend aufgebaut, das dieselben Domain/Application/Infrastructure-Schichten und dieselbe JSON-Persistenz verwendet.

Für die nächsten klassischen Desktop-Workflows des Health Research Notebook – Formulare, Wizards, Tabellen, Detailansichten und Dialoge – soll die Entwicklung möglichst schnell zu einer täglich nutzbaren Anwendung führen, ohne dieselben Features in zwei UIs doppelt zu entwickeln.

### Entscheidung

`Sasd.HealthNotebook.WinForms` ist bis auf Weiteres das primäre Entwicklungsfrontend.

`Sasd.HealthNotebook.Wpf` bleibt buildbar und wird als Referenz-/Kompatibilitätsfrontend erhalten.

Neue Fachfeatures werden standardmäßig nur in WinForms als Produkt-UI implementiert. Gemeinsame Fachlogik bleibt in Domain/Application/Infrastructure.

### Konsequenzen

Vorteile:

- schnellere UI-Iteration für klassische Desktop-Workflows;
- Nutzung des Visual-Studio-/WinForms-Ökosystems;
- kein Verlust der bisherigen WPF-Arbeit;
- zwei Frontends prüfen indirekt die UI-Unabhängigkeit der gemeinsamen Schichten;
- keine doppelte Featureentwicklung.

Nachteile:

- zwei Frontend-Projekte müssen weiterhin kompilieren;
- gemeinsame Verträge dürfen nicht unbedacht frontend-spezifisch werden;
- WPF erhält vorerst nicht automatisch neue Produktfeatures.

### Ersetzt / präzisiert

- ADR-0004 ("WPF bevorzugt gegenüber WinForms") wird für die aktive Entwicklungsphase **ersetzt**.
- ADR-0013 ("Konzept-Screenshots als kurzfristiges UI-Ziel") bleibt gültig, bezieht sich operativ aber jetzt primär auf WinForms.

### Revisit-Kriterium

Die Frontendentscheidung kann später neu bewertet werden, wenn:

- WinForms bei notwendigen Visualisierungen oder UX-Anforderungen erheblich einschränkt;
- WPF wieder strategisch priorisiert wird;
- ein anderes SASD-UI-Frontend reif genug wird;
- eine Cross-Platform-Strategie beschlossen wird.
