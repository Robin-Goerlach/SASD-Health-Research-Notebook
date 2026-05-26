# 110 - Roadmap

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Roadmap  
Status: Entwurf  

## 1. Leitgedanke

Erst eine sichere, kleine und verständliche Grundlage schaffen. Danach Komfort, Dokumentenintelligenz, Messwerte, Exporte und spätere KI-Funktionen ergänzen.

Dieses Projekt darf nicht als riesige Gesundheitsplattform starten. Es muss zuerst zuverlässig persönliche Informationen strukturieren, schützen und wieder auffindbar machen.

## 2. Phasenübersicht

| Phase | Titel | Ziel |
|---:|---|---|
| 0 | Dokumentation und Repository | Planungsbasis, README, Screenshots, Lizenz |
| 1 | Anwendungsschale | Startbare Desktop-App mit Grundstruktur |
| 2 | Datenbank- und Infrastrukturgrundlage | SQLite, Repositories, Migrationen |
| 3 | Gesundheitsthemen-Verwaltung | Conditions anlegen/bearbeiten/archivieren |
| 4 | Wizard für neue Gesundheitsthemen | strukturierte Anlage mit Entwürfen |
| 5 | Dokumentenablage | Dateien importieren, zuordnen, suchen |
| 6 | Quellen und Informationen | Quellenverwaltung und Verlässlichkeit |
| 7 | Symptome und Verlauf | Symptomtagebuch, Timeline |
| 8 | Tags, Synonyme und Suche | Volltextsuche, Filter, Synonyme |
| 9 | Fragen und Arzttermin-Vorbereitung | offene Fragen, Arztmappe-Grundlage |
| 10 | Export und Backup | Markdown-Export, Arztmappe, Backup/Restore |
| 11 | Sicherheitsausbau | Verschlüsselung, Sperre, Datenschutztests |
| 12 | Messwerte und Laborwerte | strukturierte Werte, Verlaufsgrafiken |
| 13 | UI-Politur und Bedienkomfort | Dashboard, Kontextmenüs, Tastatur, UX |
| 14 | V1 Stabilisierung | Tests, Doku, Release-Vorbereitung |
| 15+ | Erweiterungen | OCR, KI, FHIR, Mobile, Sync |

## 3. Phase 0 - Dokumentation und Repository

Inhalte:

- Repository anlegen
- README mit Konzeptbildern
- MIT-Lizenz oder alternative Lizenz
- Lastenheft
- Pflichtenheft
- Architekturkonzept
- Datenmodell
- UI-/UX-Konzept
- Sicherheitskonzept
- Roadmap
- Risikoanalyse

Ergebnis:

- Projekt ist sauber dokumentiert.
- Scope und Nicht-Ziele sind klar.
- Entwicklung kann strukturiert starten.

## 4. Phase 1 - Anwendungsschale

Inhalte:

- .NET Solution
- WPF App
- Hauptfenster
- Navigation
- Dashboard-Platzhalter
- Konfigurationsordner
- Logging ohne Gesundheitsdaten
- Fehlerdialog
- Teststruktur

Akzeptanz:

- App startet.
- Build erfolgreich.
- Tests laufen.
- Keine echten Daten nötig.

## 5. Phase 2 - Datenbank- und Infrastrukturgrundlage

Inhalte:

- SQLite-Anbindung
- Schema-Migrationen
- Repository-Grundstruktur
- Datenordner
- Dokumentenordner
- Basis-Entitäten
- Audit-Grundlage

Akzeptanz:

- Datenbank wird angelegt.
- Migration wird angewendet.
- einfache Entität kann gespeichert und geladen werden.

## 6. Phase 3 - Gesundheitsthemen-Verwaltung

Inhalte:

- Conditions anlegen
- Conditions bearbeiten
- archivieren/wiederherstellen
- Liste und Detailansicht
- Priorität, Status, Typ, Synonyme

Akzeptanz:

- Gesundheitsthema bleibt nach Neustart erhalten.
- Archivierte Themen sind aus Standardliste ausgeblendet.
- Tests für Create/Update/Archive/Restore.

## 7. Phase 4 - Wizard

Inhalte:

- 10-Schritte-Wizard
- Entwürfe
- Autosave
- Dublettenwarnung
- Zusammenfassung
- Abschluss erzeugt Condition mit ersten Beziehungen

Akzeptanz:

- Wizard kann unterbrochen und fortgesetzt werden.
- Pflichtfelder werden validiert.
- erzeugtes Thema ist korrekt sichtbar.

## 8. Phase 5 - Dokumentenablage

Inhalte:

- Dateiauswahl/Drag & Drop
- Dokumentmetadaten
- Hashberechnung
- Dublettenhinweis
- Zuordnung zu Themen
- Dokumentliste

Akzeptanz:

- Dokument wird kontrolliert importiert.
- Dublette wird erkannt.
- Datei geht bei Fehler nicht verloren.

## 9. Phase 6 - Quellen und Informationen

Inhalte:

- Quellen anlegen
- Quellen bewerten
- Quellen mit Themen/Einträgen verbinden
- Quellenliste
- Quelle im Wizard erfassen

Akzeptanz:

- Quelle kann einem Thema zugeordnet werden.
- Verlässlichkeit wird angezeigt.

## 10. Phase 7 - Symptome und Verlauf

Inhalte:

- Symptomstammdaten
- Symptom-Beobachtungen
- Schweregrad
- Datum/Kontext
- Timeline

Akzeptanz:

- Symptomverlauf ist sichtbar.
- Beobachtungen sind einem Thema zugeordnet.

## 11. Phase 8 - Tags, Synonyme und Suche

Inhalte:

- Tag-Verwaltung
- Synonyme
- SQLite FTS5
- globale Suche
- Ergebnisfilter
- Archivfilter

Akzeptanz:

- Suche findet Titel, Synonyme und Notizen.
- Suchbegriffe werden nicht geloggt.

## 12. Phase 9 - Fragen und Arzttermin-Vorbereitung

Inhalte:

- offene Fragen
- Status offen/gefragt/beantwortet
- Ziel der Frage
- Terminbezug optional
- Fragenliste für Arzttermin

Akzeptanz:

- Fragen können pro Thema geführt werden.
- offene Fragen erscheinen im Dashboard.

## 13. Phase 10 - Export und Backup

Inhalte:

- Markdown-Export
- Arztmappe V1
- Exportvorschau
- Datenschutzwarnung
- Backup
- Restore-Test

Akzeptanz:

- Arztmappe wird erzeugt.
- Backup kann wiederhergestellt werden.

## 14. Phase 11 - Sicherheitsausbau

Inhalte:

- Verschlüsselungsentscheidung umsetzen
- Sperrmechanismus
- sichere Backups
- Log-Sicherheitsprüfung
- Exportkontrolle verbessern

Akzeptanz:

- sensible Daten sind besser geschützt.
- Sicherheitscheckliste erfüllt.

## 15. Phase 12 - Messwerte und Laborwerte

Inhalte:

- allgemeine Messwerte
- Laborwerte
- Referenzbereiche
- einfache Verlaufstabellen
- spätere Diagramme vorbereiten

Akzeptanz:

- Laborwert kann erfasst und Dokument zugeordnet werden.
- Keine automatische Diagnose.

## 16. Phase 13 - UI-Politur und Bedienkomfort

Inhalte:

- Dashboard finalisieren
- Kontextmenüs
- Doppelklickaktionen
- Tastaturkürzel
- leere Zustände
- bessere Fehlermeldungen

Akzeptanz:

- Anwendung wirkt rund und erwartbar bedienbar.

## 17. Phase 14 - V1 Stabilisierung

Inhalte:

- Tests erweitern
- Performance prüfen
- Doku aktualisieren
- Installationshinweise
- Release Notes
- Beispiel-Demodaten

Akzeptanz:

- V1 kann als interner Prototyp/Release genutzt werden.

## 18. Erweiterungen nach V1

| Erweiterung | Nutzen | Risiko |
|---|---|---|
| OCR | gescannte Dokumente durchsuchbar | Datenschutz, Fehlerkennung |
| KI-Zusammenfassung | schnellere Orientierung | Halluzination, Datenschutz |
| FHIR-Export | Interoperabilität | hoher Standardisierungsaufwand |
| mobile App | schnelle Symptom-Erfassung | Sync/Sicherheit |
| verschlüsselter Sync | Geräteübergreifung | komplexe Konflikte |
| Diagramme | Verlauf besser sichtbar | Fehlinterpretation |
| Literaturverwaltung | bessere Recherche | Scope-Wachstum |

## 19. Nicht vor V1 aufnehmen

- externe KI
- Cloud-Sync
- automatische Diagnose
- Medikamenteninteraktionsprüfung
- ePA-Integration
- Mehrbenutzerbetrieb
- mobile App
- komplexe Statistik

## 20. Empfohlener erster Commit nach Repository-Anlage

```text
docs: add initial project documentation and planning baseline
```

## 21. Definition of Ready für Coding-Start

- Repository existiert.
- README vorhanden.
- Lizenz vorhanden.
- Lastenheft vorhanden.
- Pflichtenheft vorhanden.
- Architekturkonzept vorhanden.
- Datenmodell vorhanden.
- Sicherheitsgrenzen dokumentiert.
- Roadmap Phase 1 festgelegt.
