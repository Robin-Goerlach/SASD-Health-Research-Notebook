# 150 - Feature-Backlog und Anforderungskatalog

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Feature-Backlog / Anforderungskatalog  
Status: Entwurf  

## 1. Zweck

Dieses Dokument sammelt bewusst viele mögliche Funktionen, auch solche, die später wahrscheinlich gestrichen oder verschoben werden. Ziel ist, früh sichtbar zu machen, welche Funktionsbereiche denkbar sind, damit spätere Architekturentscheidungen nicht versehentlich wichtige Erweiterungen blockieren.

## 2. Prioritätsklassen

| Kürzel | Bedeutung |
|---|---|
| M | Muss für eine nützliche V1 |
| S | Soll nach V1 oder in V1, wenn klein |
| K | Kann später kommen |
| X | bewusst nicht für V1 / kritisch / nur nach Prüfung |

## 3. Gesundheitsthemen / Conditions

| Prio | Feature |
|---|---|
| M | Gesundheitsthema anlegen |
| M | Titel, Typ, Status, Priorität erfassen |
| M | Synonyme/Alternativbezeichnungen erfassen |
| M | Notizen und Zusammenfassung erfassen |
| M | Gesundheitsthema bearbeiten |
| M | Gesundheitsthema archivieren und wiederherstellen |
| S | ICD-10-Code optional erfassen |
| S | Organsystem/Kategorie erfassen |
| S | Farbmarkierung |
| S | Dublettenwarnung |
| K | LOINC/SNOMED/medizinische Klassifikationen |
| X | automatische Diagnosezuordnung |

## 4. Wizard

| Prio | Feature |
|---|---|
| M | 10-Schritte-Wizard für neue Gesundheitsthemen |
| M | Entwurf speichern |
| M | Wizard später fortsetzen |
| M | Grunddaten erfassen |
| M | Symptome im Wizard erfassen |
| M | Dokumente im Wizard anhängen |
| M | Quellen im Wizard erfassen |
| M | offene Fragen im Wizard erfassen |
| M | Zusammenfassung vor Abschluss |
| S | Ärzte/Ansprechpartner im Wizard |
| S | Medikamente/Maßnahmen im Wizard als Notiz |
| S | Messwerte/Laborwerte im Wizard |
| K | Wizard-Vorlagen für bestimmte Themenarten |
| K | Import-Wizard für vorhandene Dokumentensammlung |

## 5. Einträge und Notizen

| Prio | Feature |
|---|---|
| M | allgemeine Einträge erfassen |
| M | Eintrag einem Thema zuordnen |
| M | Eintrag typisieren: Notiz, Beobachtung, Recherche, Entscheidung |
| M | Eintrag mit Tags versehen |
| S | Eintrag mit Quellen verbinden |
| S | Eintrag mit Dokumenten verbinden |
| S | Status offen/in Prüfung/besprochen/erledigt |
| K | Versionierung von Einträgen |
| K | Vorlagen für Einträge |

## 6. Dokumente

| Prio | Feature |
|---|---|
| M | PDF/Bild/Datei importieren |
| M | Dokumentmetadaten erfassen |
| M | Dokument einem Thema zuordnen |
| M | Dokument per Hash prüfen |
| M | Dublettenhinweis |
| M | Dokumentliste |
| S | Dokumentvorschau |
| S | Dokument mehreren Themen zuordnen |
| S | Dokument exportieren |
| S | Dokument als sensibel markieren |
| K | OCR |
| K | DICOM-Unterstützung |
| K | automatische Metadatenextraktion |
| K | Webclipper |

## 7. Quellen

| Prio | Feature |
|---|---|
| M | Quelle erfassen |
| M | Quellentyp erfassen |
| M | URL/Abrufdatum erfassen |
| M | Quelle einem Thema zuordnen |
| S | Verlässlichkeit bewerten |
| S | Quelle mit Eintrag verbinden |
| S | Quellenliste pro Thema |
| K | DOI/ISBN |
| K | Literaturverzeichnis |
| K | Link-Rot-Erkennung |
| K | Zotero-Import |

## 8. Symptome und Verlauf

| Prio | Feature |
|---|---|
| M | Symptom erfassen |
| M | Symptom-Beobachtung mit Datum |
| M | Schweregrad 0-10 |
| S | Dauer erfassen |
| S | Kontext/Auslöser erfassen |
| S | Verlaufsliste |
| S | einfache Verlaufsgrafik |
| K | Korrelation mit Wetter/Ernährung/Medikament |
| X | automatische Ursachenbewertung |

## 9. Messwerte und Laborwerte

| Prio | Feature |
|---|---|
| S | einfachen Messwert erfassen |
| S | Laborwert erfassen |
| S | Einheit und Referenzbereich erfassen |
| S | Laborwert mit Dokument verbinden |
| K | CSV-Import |
| K | Diagramme |
| K | LOINC-Codes |
| K | Grenzwertvorlagen |
| X | automatische medizinische Interpretation |

## 10. Medikamente und Maßnahmen

| Prio | Feature |
|---|---|
| S | Medikamentennotiz erfassen |
| S | Zeitraum dokumentieren |
| S | Dosierung als Freitext |
| S | Wirkung/Nebenwirkung als Beobachtung |
| K | Medikationsplan |
| K | Einnahmeerinnerung |
| K | Packungsbeilage speichern |
| X | Interaktionsprüfung ohne regulatorische Prüfung |
| X | Dosisempfehlung |

## 11. Arzttermine und Fragen

| Prio | Feature |
|---|---|
| M | offene Fragen erfassen |
| M | Frage Thema zuordnen |
| M | Status offen/gefragt/beantwortet |
| S | Termin erfassen |
| S | Fragen Termin zuordnen |
| S | Arztfragenliste exportieren |
| K | Kalenderintegration |
| K | Erinnerungen |
| K | Gesprächsprotokoll-Vorlage |

## 12. Suche und Wissen

| Prio | Feature |
|---|---|
| M | globale Suche |
| M | Suche in Titeln und Notizen |
| M | Suche nach Tags |
| M | Synonym-Suche |
| S | Suche in Dokumentbeschreibungen |
| S | Suche in OCR-Text später |
| S | Filter nach Zeitraum/Typ/Quelle |
| K | gespeicherte Suchanfragen |
| K | semantische Suche |
| K | lokaler KI-Index |

## 13. Timeline und Dashboard

| Prio | Feature |
|---|---|
| M | Dashboard mit Überblick |
| M | aktive Themen anzeigen |
| M | offene Fragen anzeigen |
| S | letzte Aktivitäten |
| S | nächste Termine |
| S | auffällige Messwerte dokumentarisch anzeigen |
| S | Timeline pro Thema |
| K | konfigurierbare Widgets |
| K | Druckansicht |

## 14. Export und Berichte

| Prio | Feature |
|---|---|
| M | Markdown-Zusammenfassung |
| M | Arztfragenliste |
| M | Arztmappe mit ausgewählten Dokumenten |
| M | Exportvorschau |
| M | Datenschutzwarnung |
| S | PDF-Export |
| S | ZIP-Export |
| S | Exportprotokoll |
| K | FHIR-Export |
| K | CSV-Export für Messwerte |
| K | LaTeX/Pandoc hochwertiger Bericht |

## 15. Backup und Restore

| Prio | Feature |
|---|---|
| M | Backup erstellen |
| M | Backup-Metadaten |
| M | Restore durchführen |
| M | Prüfsummen |
| S | verschlüsseltes Backup |
| S | Backup-Erinnerung |
| S | Backupstatus im Dashboard |
| K | automatische Backup-Pläne |
| K | mehrere Backup-Ziele |

## 16. Sicherheit und Datenschutz

| Prio | Feature |
|---|---|
| M | keine sensiblen Daten in Logs |
| M | Exportwarnungen |
| M | `.gitignore` für Daten/Logs/Backups |
| M | Datenschutzhinweise |
| S | Datenbankverschlüsselung |
| S | Dokumentenverschlüsselung |
| S | Master-Passwort |
| S | automatische Sperre |
| K | Recovery-Key |
| K | sichere Löschung temporärer Dateien |
| X | Cloud-Sync ohne Verschlüsselung |

## 17. KI und Automatisierung

| Prio | Feature |
|---|---|
| K | lokale Zusammenfassung von Notizen |
| K | Tag-Vorschläge |
| K | Fragenvorschläge für Arzttermin |
| K | OCR-Korrekturhilfe |
| K | Widersprüche zwischen Quellen markieren |
| X | externe KI ohne explizite Zustimmung |
| X | KI-Diagnose |
| X | KI-Therapieempfehlung |
| X | automatische medizinische Priorisierung |

## 18. Interoperabilität

| Prio | Feature |
|---|---|
| K | FHIR-Export vorbereiten |
| K | FHIR Observation Mapping |
| K | DocumentReference Mapping |
| K | CSV-Import/Export |
| K | Kalenderexport |
| X | direkte ePA-Integration in V1 |
| X | produktive Arztsystem-Schnittstelle in V1 |

## 19. Bedienkomfort

| Prio | Feature |
|---|---|
| M | verständliche Fehlermeldungen |
| M | leere Zustände |
| S | Kontextmenüs |
| S | Doppelklickaktionen |
| S | Tastaturkürzel |
| S | zuletzt geöffnet |
| K | Themes |
| K | Dashboard anpassen |
| K | mehrsprachige UI |

## 20. MVP-Empfehlung

Für V1 wirklich bauen:

1. Anwendungsschale
2. lokale SQLite-Datenbank
3. Gesundheitsthemen
4. Wizard
5. Dokumentimport
6. Quellen
7. Symptome einfach
8. Tags/Synonyme
9. Suche
10. offene Fragen
11. Markdown-Export/Arztfragenliste
12. Backup/Restore-Grundlage
13. Datenschutzfreundliches Logging

Nicht in V1:

- KI
- Cloud-Sync
- mobile App
- OCR
- FHIR
- Medikationsplan mit Erinnerungen
- automatische medizinische Bewertung
- komplexe Diagramme
