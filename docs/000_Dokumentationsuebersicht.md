# 000 - Dokumentationsübersicht

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Projekt- und Dokumentationslandkarte  
Status: Entwurf  

## 1. Zweck dieses Dokuments

Dieses Dokument beschreibt, welche Planungs-, Architektur- und Entwicklungsdokumente für das Projekt **SASD Health Research Notebook** geführt werden sollen. Es dient als Einstiegspunkt für Entwickler, spätere Mitwirkende und für die interne SASD-Dokumentation.

Das Projekt verarbeitet potenziell hochsensible Gesundheitsinformationen. Deshalb soll vor der Implementierung nicht nur ein grobes Feature-Set existieren, sondern eine nachvollziehbare Dokumentationsbasis mit klaren Grenzen, Sicherheitsannahmen und technischen Entscheidungen.

## 2. Projekteinordnung

Das SASD Health Research Notebook ist als lokal-first Desktop-Anwendung gedacht. Ziel ist es, persönliche Gesundheitsinformationen strukturiert zu sammeln, zu dokumentieren, zu verschlagworten und für Gespräche mit Ärzten oder eigene Recherchen vorzubereiten.

Es handelt sich ausdrücklich **nicht** um ein Diagnosesystem, keine Therapie-Software, keinen Ersatz für ärztliche Beratung und keinen automatisierten medizinischen Entscheidungsassistenten.

## 3. Dokumentenlandkarte

| Dokument | Kernfrage |
|---|---|
| Lastenheft | Was wird aus Anwender-/Auftraggebersicht benötigt? |
| Pflichtenheft | Wie soll das System die Anforderungen erfüllen? |
| Architekturkonzept | Wie wird das System grundsätzlich aufgebaut? |
| Datenmodell | Welche Datenobjekte gibt es und wie hängen sie zusammen? |
| UI-/UX-Konzept | Wie wird das System bedient? |
| Sicherheitskonzept | Wie werden Gesundheitsdaten geschützt? |
| Dokumenten-/Quellenkonzept | Wie werden Dateien, Quellen und Informationen verwaltet? |
| Such-/Wissenskonzept | Wie werden Informationen gefunden und verknüpft? |
| Import-/Exportkonzept | Wie kommen Daten hinein und wieder heraus? |
| Testkonzept | Wie wird Qualität nachgewiesen? |
| Roadmap | In welchen Phasen wird entwickelt? |
| Risikoanalyse | Was kann schiefgehen und wie wird gegengesteuert? |
| ADRs | Welche Architekturentscheidungen wurden warum getroffen? |

## 4. Dokumentationsprinzipien

1. **Nachvollziehbarkeit vor Geschwindigkeit**  
   Wichtige Entscheidungen werden schriftlich begründet.

2. **Datenschutz von Anfang an**  
   Gesundheitsdaten dürfen nicht erst nachträglich geschützt werden.

3. **MVP klein, Architektur erweiterbar**  
   V1 soll nutzbar, aber nicht überladen sein.

4. **Keine medizinische Übergriffigkeit**  
   Das System dokumentiert und strukturiert, es diagnostiziert nicht.

5. **Keine stillen Datenverluste**  
   Löschen, Archivieren, Exportieren und Wiederherstellen müssen nachvollziehbar sein.

6. **Quellenbewusstsein**  
   Medizinische Informationen müssen mit Quelle, Datum, Qualität und Unsicherheit dokumentiert werden.

7. **Offline-Fähigkeit**  
   Die Anwendung soll ohne Cloud-Zwang nutzbar sein.

8. **Späterer Ausbau möglich**  
   OCR, KI, FHIR, Geräteimport und mobile Begleit-Apps sollen später denkbar bleiben, aber V1 nicht verkomplizieren.

## 5. Empfohlene Repository-Dokumentstruktur

```text
/docs
  /requirements
  /architecture
  /database
  /ui-ux
  /security
  /knowledge
  /export
  /testing
  /roadmap
  /risks
  /adr
  /development
  /screenshots
/db
/src
/tests
```

## 6. Begriffe

| Begriff | Bedeutung im Projekt |
|---|---|
| Gesundheitsthema | Oberbegriff für Krankheit, Verdacht, Symptomkomplex, Risikothema oder Recherchethema |
| Condition | Technischer Begriff für ein Gesundheitsthema |
| Eintrag | Freie oder strukturierte Notiz mit Datum, Typ, Quelle und Zuordnung |
| Dokument | Datei wie PDF, Bild, Arztbrief, Laborbericht, Scan, Screenshot |
| Quelle | Ursprung einer Information: Arzt, Studie, Webseite, Buch, Gespräch, eigene Beobachtung |
| Beobachtung | Subjektiver oder gemessener Verlaufseintrag |
| Messwert | Strukturierter Wert mit Einheit, Datum und optionalem Referenzbereich |
| Arztmappe | Export-Paket für Arzttermine mit Zusammenfassung, Verlauf, Fragen und ausgewählten Dokumenten |
| Audit Trail | Änderungsprotokoll, das wichtige Änderungen nachvollziehbar macht |

## 7. Empfohlene Priorität der nächsten Arbeit

1. Repository anlegen und Starterpaket einspielen.
2. Dokumentationspaket einchecken.
3. Architekturentscheidungen prüfen und ggf. ADRs anpassen.
4. Datenmodell in ein erstes Domain-Modell überführen.
5. Anwendungsschale entwickeln.
6. Condition-Verwaltung entwickeln.
7. Wizard entwickeln.
8. Dokumentenablage und Suche ergänzen.

## 8. Offene Leitfragen

- Soll V1 mit WPF oder WinForms starten?
- Wird Verschlüsselung direkt in V1 umgesetzt oder als V1.1-Sicherheitsausbau?
- Soll ein Master-Passwort zwingend sein oder optional?
- Wird die erste Version ausschließlich lokal ohne Cloud entwickelt?
- Wie streng soll der Audit-Trail in V1 sein?
- Werden Laborwerte in V1 bereits strukturiert oder zunächst als einfache Messwerte erfasst?
