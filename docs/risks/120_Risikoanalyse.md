# 120 - Risikoanalyse

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Risikoanalyse  
Status: Entwurf  

## 1. Ziel

Dieses Dokument sammelt technische, fachliche, rechtliche, organisatorische und produktbezogene Risiken. Ziel ist nicht, das Projekt zu verhindern, sondern teure Fehlentscheidungen früh sichtbar zu machen.

## 2. Bewertungsmodell

| Stufe | Eintrittswahrscheinlichkeit | Auswirkung |
|---|---|---|
| niedrig | unwahrscheinlich | kleiner Mehraufwand |
| mittel | realistisch | spürbarer Schaden/Aufwand |
| hoch | wahrscheinlich | großer Schaden/Aufwand |
| kritisch | sehr wahrscheinlich oder gravierend | Projektgefährdung oder erheblicher Schaden |

## 3. Risikoübersicht

| Risiko | Wahrscheinlichkeit | Auswirkung | Bewertung |
|---|---:|---:|---:|
| Datenschutzverletzung durch ungeschützte Daten | mittel | kritisch | hoch |
| falsche medizinische Interpretation durch Nutzer | mittel | hoch | hoch |
| Anwendung wirkt wie Diagnose-Software | mittel | hoch | hoch |
| Datenverlust durch fehlerhafte Backups | mittel | kritisch | hoch |
| Dokumentenablage wird chaotisch | hoch | mittel | hoch |
| Scope Creep | hoch | hoch | hoch |
| KI-Funktionen zu früh | mittel | hoch | hoch |
| Suchfunktion findet relevante Daten nicht | mittel | mittel | mittel |
| Datenmodell zu starr | mittel | hoch | hoch |
| UI zu komplex | mittel | mittel | mittel |
| Verschlüsselung zu spät | mittel | hoch | hoch |
| Migration zerstört Daten | niedrig-mittel | kritisch | hoch |
| Gesundheitsdaten in Logs | mittel | hoch | hoch |
| Git enthält versehentlich Testdaten | niedrig-mittel | hoch | mittel-hoch |

## 4. Datenschutzrisiken

### 4.1 Risiko: unverschlüsselte lokale Daten

Beschreibung: Laptop oder Backup-Datenträger geht verloren.

Gegenmaßnahmen:

- Verschlüsselung einplanen
- BitLocker als Mindestschutz empfehlen
- SQLCipher/verschlüsselten Dokumentenspeicher prüfen
- Backups verschlüsseln
- Warnung bei ungeschütztem Betrieb

### 4.2 Risiko: sensible Daten in Logs

Beschreibung: Logs enthalten Diagnosen, Dateinamen, Suchbegriffe oder Laborwerte.

Gegenmaßnahmen:

- Logging-Konzept
- technische Tests gegen sensible Inhalte
- keine Freitexte loggen
- Fehlercodes statt Inhalte

### 4.3 Risiko: Export in Cloud-Ordner

Beschreibung: Nutzer speichert Arztmappe in OneDrive/Dropbox/Google Drive ohne Bewusstsein.

Gegenmaßnahmen:

- Zielordnerwarnung, soweit technisch möglich
- Exporthinweis
- Passwortschutz anbieten

## 5. Medizinische Risiken

### 5.1 Risiko: Nutzer versteht Dokumentation als Diagnose

Gegenmaßnahmen:

- klare Zweckbeschreibung
- keine Diagnose-Engine
- UI-Text „Kenntnisstand“ statt „Diagnose stellen“
- Arztfragen statt Empfehlungen

### 5.2 Risiko: auffällige Werte werden falsch bewertet

Gegenmaßnahmen:

- Referenzbereich als dokumentierter Bereich, nicht Diagnose
- Hinweise: Werte mit Arzt besprechen
- keine automatische Dringlichkeitseinstufung

### 5.3 Risiko: Medikamentenmodul wird missverstanden

Gegenmaßnahmen:

- Medikamentenbereich als Dokumentation, nicht Planung
- keine Absetz-/Dosisempfehlungen
- Warnhinweis bei Änderungen

## 6. Technische Risiken

### 6.1 Datenmodell zu komplex

Gegenmaßnahmen:

- V1 mit wenigen Tabellen starten
- HealthEntry als flexible Basistabelle
- Erweiterungen vorbereiten, aber nicht überbauen

### 6.2 Dokumente und Datenbank laufen auseinander

Gegenmaßnahmen:

- Transaktionskonzept
- Hashes
- Konsistenzprüfung
- Recovery bei fehlenden Dateien

### 6.3 Backup nicht wiederherstellbar

Gegenmaßnahmen:

- Restore-Test als Pflicht
- Manifest
- Prüfsummen
- Versionierung

### 6.4 Migration schlägt fehl

Gegenmaßnahmen:

- Backup vor Migration
- Migrations-Tests
- Schema-Versionen
- Transaktionen

## 7. Produkt- und Scope-Risiken

### 7.1 Scope Creep

Beschreibung: Das Projekt könnte zu einer Mischung aus EHR, DMS, Laborsoftware, KI-Assistent, Statistiksystem und Patientenakte wachsen.

Gegenmaßnahmen:

- Roadmap streng befolgen
- V1 klein halten
- Backlog dokumentieren, aber nicht alles sofort bauen
- Nicht-Ziele sichtbar halten

### 7.2 UI wird zu komplex

Gegenmaßnahmen:

- Dashboard nur mit Kernkarten
- Wizard führt durch komplexe Anlage
- erweiterte Felder einklappbar
- gute Standardwerte

## 8. KI-Risiken

| Risiko | Gegenmaßnahme |
|---|---|
| Halluzination | KI nur als Hilfstool, Quellen anzeigen, keine Diagnose |
| Datenschutzverletzung | keine externe KI in V1, später explizite Zustimmung |
| falsche Zusammenfassung | Originalquelle immer verlinken |
| Nutzer vertraut KI zu stark | Warnhinweise, Arztfragen statt Empfehlungen |

## 9. Regulatorische Risiken

Risiko: Das Projekt wird später öffentlich als medizinische Entscheidungssoftware positioniert.

Gegenmaßnahmen:

- Zweckbestimmung sauber formulieren
- keine Diagnose-/Therapiefunktionen
- regulatorische Prüfung vor Veröffentlichung mit medizinischem Anspruch
- Begriffe wie „Research Notebook“ und „Dokumentation“ statt „Diagnose-Assistent“

## 10. Risikoreduzierende Architekturentscheidungen

| Entscheidung | Reduziertes Risiko |
|---|---|
| Lokal-first | Cloud-/Datenabflussrisiko |
| SQLite | Betriebsaufwand |
| Dokumente mit Hash | Dubletten/Integrität |
| Audit Trail | Nachvollziehbarkeit |
| Archiv statt Löschen | Datenverlust |
| Markdown-Export | Transparenz |
| keine KI in V1 | Datenschutz/Fehlinterpretation |
| Wizard-Entwürfe | Datenverlust bei Erfassung |

## 11. Frühwarnindikatoren

- README klingt nach Diagnose-App.
- Datenbank enthält echte Testdaten.
- Logs enthalten medizinische Begriffe.
- Wizard wächst auf mehr als 10 Pflichtschritte.
- V1 enthält bereits KI, OCR, FHIR und Sync gleichzeitig.
- Export ist einfacher als Import/Backup-Sicherheit.
- Backups werden erstellt, aber nie wiederhergestellt.

## 12. Empfehlung

Die größten Risiken sind Datenschutz, medizinische Fehlinterpretation, Datenverlust und Scope Creep. Deshalb sollte die Entwicklung in dieser Reihenfolge erfolgen:

1. Architektur- und Sicherheitsgrenzen fixieren.
2. Kleine lokale Basis bauen.
3. Datenintegrität und Backup ernst nehmen.
4. Wizard und Dokumentenablage sauber entwickeln.
5. Suche und Export ergänzen.
6. Erst danach Komfort- und KI-Themen prüfen.
