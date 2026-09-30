# 080 - Such- und Wissenskonzept

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Such- und Wissenskonzept  
Status: Entwurf  

## 1. Ziel

Die Anwendung soll nicht nur Daten speichern, sondern gespeicherte Informationen später zuverlässig auffindbar machen. Bei Gesundheitsthemen ist das besonders wichtig, weil Begriffe oft uneinheitlich sind: Umgangssprache, Fachsprache, lateinische Begriffe, ICD-Bezeichnungen, Synonyme, Abkürzungen und falsch erinnerte Begriffe stehen nebeneinander.

## 2. Suchprinzipien

| Prinzip | Beschreibung |
|---|---|
| Global suchen | Ein Suchfeld für alle Inhalte |
| Gefiltert verfeinern | Ergebnis nach Typ, Datum, Tag, Thema einschränken |
| Synonyme berücksichtigen | z. B. Gefäßverkalkung / Arteriosklerose / Atherosklerose |
| Quelle sichtbar machen | Ergebnisse zeigen Herkunft und Kontext |
| Nicht überinterpretieren | Suche findet Informationen, bewertet sie aber nicht medizinisch |
| Datenschutz beachten | Suchbegriffe nicht loggen |

## 3. Suchbereiche

Zu durchsuchen:

- Gesundheitsthemen
- Synonyme
- Notizen
- Dokumenttitel
- Dokumentbeschreibungen
- OCR-Text später
- Quellen
- Fragen
- Termine
- Symptome
- Messwerte/Laborwerte
- Tags

## 4. Suchergebnisgruppen

Die Ergebnisse sollen gruppiert werden:

```text
Gesundheitsthemen
Dokumente
Einträge und Notizen
Symptome und Verlauf
Messwerte / Laborwerte
Offene Fragen
Quellen
Termine
```

## 5. Facetten / Filter

| Filter | Zweck |
|---|---|
| Zeitraum | nur bestimmte Zeitspanne |
| Gesundheitsthema | Kontext einschränken |
| Dokumenttyp | z. B. Laborbericht |
| Quelle | z. B. Arzt, Webseite, Studie |
| Verlässlichkeit | offizielle/professionelle Quellen hervorheben |
| Tag | thematische Suche |
| Status | offen, erledigt, archiviert |
| Priorität | wichtige Fragen/Themen |
| Exportrelevanz | Material für Arztmappe |

## 6. Synonymkonzept

Synonyme sind für medizinische Informationen zentral.

Beispiele:

| Umgangssprache | Fachbegriff | Alternativen |
|---|---|---|
| Gefäßverkalkung | Arteriosklerose | Atherosklerose |
| Blutzucker | Glukose | Blutglukose |
| Vitamin D | 25-OH-Vitamin D | Calcidiol |
| Nasennebenhöhlenentzündung | Sinusitis | Rhinosinusitis |
| Bluthochdruck | Hypertonie | arterielle Hypertonie |

Funktionen:

- Synonyme pro Gesundheitsthema
- globale Synonyme
- Sprache markieren
- Synonym in Suche einbeziehen
- Dublettenwarnung bei ähnlichen Begriffen

## 7. Tag-Konzept

Tags sollen flexibel, aber nicht chaotisch werden.

Tag-Arten:

- Organsystem
- Symptomgruppe
- Lebensbereich
- Arzt/Fachrichtung
- Quelle
- Priorität
- Export
- Recherche

Beispiele:

```text
#diabetes
#gefaesse
#hno
#laborwert
#arztfrage
#ernaehrung
#beobachtung
#offen
```

## 8. Wissensbeziehungen

Später können Einträge nicht nur getaggt, sondern semantisch verbunden werden.

Beziehungstypen:

| Beziehung | Bedeutung |
|---|---|
| gehört zu | Eintrag gehört zu Thema |
| stützt | Quelle stützt Aussage |
| widerspricht | Quelle widerspricht Aussage |
| erklärt möglicherweise | Hypothese, keine Diagnose |
| wurde besprochen in | Arztterminbezug |
| basiert auf | Zusammenfassung basiert auf Dokumenten |
| ersetzt | neuer Eintrag ersetzt alten Stand |
| verworfen wegen | Hypothese wurde aufgegeben |

V1 kann Beziehungen über einfache Zuordnungen und Notizen abbilden.

## 9. Timeline als Wissensinstrument

Die Timeline beantwortet Fragen wie:

- Wann begann ein Symptom?
- Welche Dokumente kamen danach?
- Wann wurde ein Arzt gefragt?
- Welche Messwerte änderten sich?
- Welche Fragen sind noch offen?

Timeline-Ereignisse:

- Thema angelegt
- Dokument importiert
- Symptom beobachtet
- Messwert eingetragen
- Laborwert dokumentiert
- Frage angelegt/beantwortet
- Termin stattgefunden
- Zusammenfassung exportiert

## 10. Suchtechnik

Für V1 empfohlen:

- SQLite FTS5 für Volltextsuche
- separate Suchindex-Tabelle
- manuelle oder eventbasierte Aktualisierung
- normalisierte Begriffe
- keine Suchbegriff-Logs

Später denkbar:

- Lucene.NET
- semantische Suche
- lokale Embeddings
- medizinisches Vokabular
- OCR-Integration

## 11. Ranking

Ergebnisse sollten priorisiert werden nach:

1. exakter Titel-Treffer
2. Synonym-Treffer
3. aktueller Eintrag
4. professioneller/offizieller Quelle
5. zugeordnetem Gesundheitsthema
6. Tag-Treffer
7. Volltext-Treffer

## 12. Gespeicherte Suchen

Beispiele:

- „offene Arztfragen“
- „neue Dokumente letzte 30 Tage“
- „Laborwerte ohne Zuordnung“
- „Quellen unbewertet“
- „Themen mit hoher Priorität“
- „Dokumente ohne Backup seit Import“

## 13. Wissensqualität

Die Anwendung soll helfen, Unsicherheit sichtbar zu machen.

Felder:

- Quelle vorhanden?
- Quelle bewertet?
- mit Arzt besprochen?
- eigene Beobachtung oder externe Information?
- Stand der Information?
- offen/widersprüchlich/erledigt?

## 14. KI-Ausblick

KI kann später helfen, aber ist riskant.

Erlaubte spätere KI-Ideen:

- Notizen zusammenfassen
- Arztfragen aus Notizen vorschlagen
- Tags vorschlagen
- Dokumentinhalt extrahieren
- Widersprüche zwischen eigenen Notizen und Quellen markieren

Nicht erlaubt ohne tiefere Prüfung:

- Diagnosevorschläge
- Therapieempfehlungen
- Risikoeinstufungen mit Handlungsaufforderung
- automatische Medikamentenänderungen

## 15. Akzeptanzkriterien V1

- Suche findet Gesundheitsthemen über Titel.
- Suche findet Gesundheitsthemen über Synonyme.
- Suche findet Dokumente über Titel und Beschreibung.
- Suche findet Einträge über Inhalt.
- Nutzer kann Ergebnisse filtern.
- Suchbegriffe werden nicht im Log gespeichert.
- Archivierte Inhalte sind standardmäßig ausgeblendet, aber auffindbar, wenn Filter aktiv ist.


---

## 16. Sucherweiterung Baseline 2.1 (2026-09-30)

Spätere globale Suche und Filter sollen zusätzlich berücksichtigen:

- Ernährungseinträge;
- ContextSnapshots/Wetterkontext;
- Sessions und Folgemaßnahmen;
- HealthActions und Routinen;
- SourceLocations/EvidenceNotes;
- Medienmetadaten;
- Kontaktreferenzen.

Sensible Suchbegriffe bleiben weiterhin aus technischen Logs ausgeschlossen.

Für Beziehungen gilt besonders:

- "zeitlich zusammen beobachtet mit" ist nicht gleich "verursacht";
- "laut Quelle/Session" ist nicht gleich "medizinisch bestätigt";
- professionelle und eigene Aussagen müssen in Suchergebnissen unterscheidbar sein.
