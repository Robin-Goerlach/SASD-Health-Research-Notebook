# Lastenheft
# SASD Health Research Notebook

**Projekt:** SASD Health Research Notebook  
**Dokumenttyp:** Lastenheft / fachliche Anforderungsspezifikation aus Auftraggeber- und Anwendersicht  
**Version:** 0.1  
**Datum:** 2026-05-25  
**Auftraggeber / Produktidee:** SASD-GmbH / interne Produkt- und Eigenbedarfsentwicklung  
**Arbeitsname:** SASD Health Research Notebook  
**Abgeleitete Vorlagen:** Findings Lab Notebook, Feature-Sammlung für passende Vergleichsprogramme, vorheriges Pflichtenheft  

---

## 1. Zweck dieses Dokuments

Dieses Lastenheft beschreibt aus Sicht des Auftraggebers und der späteren Anwender, **was** das geplante Programm leisten soll. Es legt fachliche Ziele, gewünschte Funktionen, Qualitätsanforderungen, Abgrenzungen, Nutzungsszenarien und Prioritäten fest.

Das Dokument ist bewusst ausführlich formuliert. Ziel ist nicht, die erste Version klein zu halten, sondern **möglichst vollständig zu erfassen, welche Anforderungen grundsätzlich denkbar und wichtig sind**, damit spätere Streichungen, Verschiebungen und Priorisierungen bewusst erfolgen können.

Das zugehörige Pflichtenheft beschreibt später bzw. ergänzend, **wie** diese Anforderungen technisch umgesetzt werden sollen. Dieses Lastenheft bleibt dagegen auf der fachlichen Ebene.

---

## 2. Management Summary

Das SASD Health Research Notebook soll ein lokal nutzbares Programm werden, mit dem persönliche Gesundheitsinformationen, Erkrankungen, Symptome, Befunde, Arztbriefe, Laborwerte, Dokumente, Webseitenquellen, eigene Beobachtungen und offene Fragen strukturiert gesammelt, dokumentiert, wiedergefunden und für Arztgespräche oder eigene Recherche ausgewertet werden können.

Das Programm soll **kein Diagnose-, Therapie- oder Medizinprodukt im engeren Sinne** sein. Es soll keine ärztlichen Entscheidungen ersetzen und keine Behandlungsempfehlungen geben. Es soll stattdessen helfen, vorhandene Informationen nachvollziehbar zu ordnen, Quellen zu bewerten, Fragen vorzubereiten und die eigene Krankheits- und Informationslage besser zu dokumentieren.

Eine zentrale Funktion soll ein **Wizard zum Anlegen neuer Krankheiten bzw. Gesundheitsthemen** sein. Dieser Wizard soll es ermöglichen, beim Anlegen eines neuen Themas direkt Symptome, Dokumente, Quellen, Arztkontakte, Medikamente, Laborwerte, Fragen und erste Notizen zu erfassen und sauber zuzuordnen.

Das System soll von Anfang an Datenschutz, lokale Datenhaltung, Exportfähigkeit, Backup, Nachvollziehbarkeit und spätere Erweiterbarkeit berücksichtigen.

---

## 3. Ausgangssituation

Der spätere Anwender sammelt Informationen zu mehreren Erkrankungen, Symptomen und medizinischen Themen aus unterschiedlichen Quellen. Dazu gehören unter anderem:

- eigene Beobachtungen,
- ärztliche Befunde,
- Laborberichte,
- Arztbriefe,
- Medikationsinformationen,
- Webseiten,
- Studien,
- Notizen aus Gesprächen,
- Fotos,
- Screenshots,
- Fragen an Ärzte,
- Verlaufseinträge,
- Ernährungshinweise,
- Messwerte,
- Diagnosen und Verdachtsdiagnosen,
- Therapie- oder Untersuchungspläne, sofern sie von Ärzten stammen und nur dokumentiert werden.

Diese Informationen liegen typischerweise verstreut vor: in Papierordnern, PDFs, Browser-Lesezeichen, E-Mails, Notizen, Downloads, Chatverläufen, Patientenportalen, Krankenkassen-Apps, Laborportalen und persönlichen Erinnerungen.

Dadurch entstehen mehrere Probleme:

- Informationen werden später nicht wiedergefunden.
- Begriffe, Diagnosen und Symptome werden uneinheitlich notiert.
- Der zeitliche Verlauf ist schwer nachvollziehbar.
- Arzttermine werden nicht optimal vorbereitet.
- Fragen gehen verloren.
- Quellen werden nicht sauber dokumentiert.
- Wichtige Dokumente sind nicht mit Erkrankungen oder Symptomen verknüpft.
- Mehrere Erkrankungen beeinflussen sich möglicherweise gegenseitig, werden aber getrennt betrachtet.
- Es fehlt ein strukturierter Überblick über den eigenen Gesundheitskontext.
- Datenschutz und Datensouveränität sind bei Cloud-Lösungen problematisch.

Das geplante System soll diese Lücke schließen.

---

## 4. Zielsetzung

### 4.1 Hauptziel

Das Hauptziel ist die Entwicklung eines persönlichen Gesundheits- und Recherche-Notebooks, das Gesundheitsinformationen strukturiert, nachvollziehbar, lokal und datenschutzbewusst verwaltet.

### 4.2 Fachliche Einzelziele

Das Programm soll:

1. Krankheiten, Verdachtsdiagnosen und Gesundheitsthemen strukturiert erfassen.
2. Symptome und Beobachtungen zeitlich dokumentieren.
3. Dokumente, PDFs, Bilder und Webseitenquellen zuordnen.
4. Laborwerte und Messwerte sammeln und später auswertbar machen.
5. Arzttermine vorbereiten und nachbereiten.
6. Offene Fragen, Hypothesen und Recherchethemen verwalten.
7. Quellen, Aussagen und eigene Schlussfolgerungen sauber trennen.
8. Einen nachvollziehbaren Verlauf über Zeit erzeugen.
9. Informationen schnell wieder auffindbar machen.
10. Exportfähige Berichte für Ärzte, eigene Ablage und spätere Auswertungen erzeugen.
11. Daten lokal halten und vor unbefugtem Zugriff schützen.
12. Spätere Erweiterungen wie OCR, KI-Zusammenfassungen, FHIR-Export oder Geräteimport vorbereiten, aber nicht zwingend in Version 1 umsetzen.

### 4.3 Nicht-Ziele

Das Programm soll ausdrücklich nicht:

- Diagnosen stellen,
- Therapieempfehlungen geben,
- ärztliche Beratung ersetzen,
- Medikamente automatisch empfehlen,
- Medikamentendosierungen bewerten,
- medizinische Alarme ohne ärztliche Grundlage auslösen,
- Notfallmedizin leisten,
- externe Gesundheitsdaten ungefragt in Cloud-Dienste übertragen,
- rechtliche oder medizinische Verantwortung übernehmen.

---

## 5. Produktvision

Das SASD Health Research Notebook soll sich anfühlen wie eine Mischung aus:

- elektronischem Laborbuch,
- persönlicher Krankenakte,
- Quellenverwaltung,
- Wissensdatenbank,
- Symptomtagebuch,
- Dokumentenarchiv,
- Arzttermin-Vorbereitung,
- persönlichem Forschungsjournal.

Die Grundidee von Findings als Lab Notebook ist nützlich: strukturierte Einträge, Anhänge, Protokolle, Zeitstempel und nachvollziehbare Dokumentation. Für den Gesundheitskontext muss diese Idee jedoch erweitert werden: Statt Laborversuchen stehen Erkrankungen, Symptome, Befunde, Quellen, ärztliche Dokumente und persönliche Beobachtungen im Mittelpunkt.

---

## 6. Zielgruppen

### 6.1 Primäre Zielgruppe

Die primäre Zielgruppe sind Einzelpersonen mit mehreren oder chronischen gesundheitlichen Themen, die ihre Gesundheitsinformationen strukturiert sammeln und verstehen möchten.

Typische Nutzer:

- Menschen mit chronischen Erkrankungen,
- Menschen mit mehreren parallelen Diagnosen,
- Menschen mit vielen Arztbriefen, Befunden und Laborwerten,
- technisch interessierte Patienten,
- Angehörige, die Informationen für Familienmitglieder strukturieren,
- Personen, die Arzttermine gründlich vorbereiten möchten,
- Personen, die medizinische Informationen recherchieren und geordnet bewerten möchten.

### 6.2 Sekundäre Zielgruppen

Sekundäre Zielgruppen können später sein:

- kleine Pflege- oder Betreuungsstrukturen im privaten Umfeld,
- Selbsthilfegruppen,
- medizinisch interessierte Dokumentationsanwender,
- Forschungspersonen, die persönliche Gesundheitsrecherche dokumentieren,
- SASD interne Nutzung zur strukturierten Wissenssammlung.

### 6.3 Nicht-Zielgruppen

Nicht primär adressiert werden:

- Krankenhäuser als produktives Patientenaktensystem,
- Arztpraxen als Praxisverwaltungssystem,
- Krankenkassen als Abrechnungssystem,
- Hersteller medizinischer Diagnosesoftware,
- Notfalldienste,
- Nutzer, die eine automatische Diagnose-App erwarten.

---

## 7. Stakeholder

| Stakeholder | Interesse | Bedeutung |
|---|---|---|
| Hauptanwender | Informationen sammeln, verstehen, exportieren | sehr hoch |
| Angehörige | Unterstützung bei Dokumentation und Arztvorbereitung | mittel |
| Ärzte | strukturierte, knappe und relevante Informationen erhalten | hoch |
| SASD-GmbH | Entwicklung eines sauberen, datenschutzorientierten Produkts | sehr hoch |
| Datenschutz / Recht | Einhaltung sensibler Datenanforderungen | sehr hoch |
| Spätere Entwickler | saubere fachliche Grundlage und Erweiterbarkeit | hoch |
| Support / Wartung | nachvollziehbares Verhalten und stabile Datenhaltung | mittel |

---

## 8. Grundprinzipien

Das Produkt soll sich an folgenden Grundprinzipien orientieren:

1. **Lokal zuerst:** Gesundheitsdaten bleiben standardmäßig auf dem eigenen Gerät.
2. **Keine Diagnose:** Das System dokumentiert, bewertet aber nicht medizinisch verbindlich.
3. **Nachvollziehbarkeit:** Jede Information soll Quelle, Datum und Kontext haben können.
4. **Trennung von Fakten und Interpretation:** Befund, Quelle, eigene Notiz und Vermutung müssen unterscheidbar sein.
5. **Exportierbarkeit:** Der Nutzer muss seine Daten jederzeit exportieren können.
6. **Datensparsamkeit:** Es werden nur Daten erhoben, die der Nutzer bewusst erfasst.
7. **Sicherheit:** Gesundheitsdaten sind besonders schützenswert.
8. **Erweiterbarkeit:** V1 darf klein sein, aber das Zielbild muss langfristig tragfähig bleiben.
9. **Bedienbarkeit:** Ein medizinisches Dokumentationssystem darf nicht nur technisch korrekt sein, sondern muss im Alltag schnell nutzbar sein.
10. **Keine versteckte Cloud:** Externe Dienste dürfen nur bewusst, optional und transparent verwendet werden.

---

## 9. Begriffe und fachliche Definitionen

| Begriff | Bedeutung im Projekt |
|---|---|
| Krankheit / Erkrankung | Vom Nutzer angelegtes Gesundheitsthema, Diagnose oder Verdachtsdiagnose |
| Condition | Technischer und fachlicher Oberbegriff für Krankheit, Beschwerdebild, Verdacht oder Gesundheitsthema |
| Symptom | Vom Nutzer beobachtete Beschwerde oder Veränderung |
| Befund | Ergebnis einer Untersuchung, eines Arztbriefs, eines Labors oder einer Messung |
| Laborwert | Strukturierter Wert mit Name, Einheit, Datum und optionalem Referenzbereich |
| Messwert | Wert aus Selbstmessung, Gerät oder Dokumentation |
| Quelle | Ursprung einer Information, z. B. Arzt, Studie, Webseite, Buch, Gespräch |
| Dokument | Datei, PDF, Bild, Scan, Screenshot oder sonstiger Anhang |
| Notiz | Freitext des Nutzers |
| Hypothese | Eigene Vermutung oder zu prüfender Zusammenhang |
| Frage | Offene Frage für Arzt, Apotheke, Recherche oder eigene Klärung |
| Timeline | Chronologische Darstellung von Ereignissen |
| Wizard | Geführter Dialog zum strukturierten Anlegen einer neuen Condition |
| Arztmappe | Exportpaket oder Bericht für einen Arzttermin |

---

## 10. Produktkontext

Das SASD Health Research Notebook soll als eigenständige Desktop-Anwendung beginnen. Es soll Daten lokal speichern und Dokumente lokal verwalten. Eine spätere Synchronisierung oder mobile Ergänzung kann vorgesehen werden, darf aber den lokalen Grundcharakter nicht aufheben.

Das System steht fachlich zwischen folgenden Produktgruppen:

- Electronic Lab Notebooks,
- Personal Health Records,
- Symptomtagebüchern,
- Medikamenten-Trackern,
- Dokumentenmanagementsystemen,
- Wissensdatenbanken,
- Quellenverwaltungen,
- Patientenportalen,
- persönlichen Notizsystemen.

Es soll nicht versuchen, alle Spezialfunktionen dieser Produktgruppen sofort vollständig zu ersetzen. Es soll ihre wichtigsten Ideen in einem persönlichen, datenschutzbewussten Gesundheits-Notebook bündeln.

---

## 11. Muss-/Soll-/Kann-Klassifikation

Dieses Lastenheft verwendet folgende Prioritäten:

| Priorität | Bedeutung |
|---|---|
| Muss | Für eine sinnvolle erste produktive Version zwingend erforderlich |
| Soll | Sehr wichtig, aber notfalls nach V1 verschiebbar |
| Kann | Nützlich oder langfristig interessant, aber nicht zwingend |
| Später | Bewusst für spätere Versionen vorgesehen |
| Nicht vorgesehen | Soll aus fachlichen, rechtlichen oder Sicherheitsgründen nicht umgesetzt werden |

---

## 12. Hauptfunktionen im Überblick

| Bereich | Kurzbeschreibung | Priorität |
|---|---|---|
| Condition-Verwaltung | Krankheiten, Diagnosen, Verdachtsdiagnosen und Themen verwalten | Muss |
| Krankheits-Wizard | Geführtes Anlegen neuer Conditions mit Symptomen, Dokumenten und Quellen | Muss |
| Eintragsverwaltung | Notizen, Befunde, Quellen, Fragen, Beobachtungen erfassen | Muss |
| Dokumentenverwaltung | PDFs, Bilder, Scans und Anhänge verwalten und zuordnen | Muss |
| Symptomdokumentation | Symptome mit Datum, Intensität und Kontext erfassen | Muss |
| Quellenverwaltung | Herkunft und Qualität von Informationen dokumentieren | Muss |
| Suche und Filter | Informationen schnell wiederfinden | Muss |
| Timeline | Chronologischen Verlauf anzeigen | Soll |
| Arzttermin-Vorbereitung | Fragen, Zusammenfassungen und relevante Dokumente bündeln | Soll |
| Export | Markdown/PDF/CSV-Exporte für eigene Ablage und Arztgespräche | Soll |
| Laborwerte | Strukturierte Erfassung und Verlauf | Soll |
| Medikamente | Dokumentation von Medikamenteninformationen und Änderungen | Soll |
| Datenschutz und Sicherheit | lokale Datenhaltung, Schutz, Backups, optional Verschlüsselung | Muss |
| Audit/Änderungshistorie | Nachvollziehbarkeit von Änderungen | Soll |
| OCR | Texterkennung für Scans und PDFs | Später |
| KI-Unterstützung | Zusammenfassungen, Quellenanalyse, Widerspruchserkennung | Später |
| FHIR/Standardexport | Interoperabilität mit Gesundheitsstandards | Später |

---

## 13. Funktionale Anforderungen

### 13.1 Condition-Verwaltung

#### LH-F-001 – Conditions anlegen

Das System muss es ermöglichen, eine neue Condition anzulegen. Eine Condition kann eine gesicherte Diagnose, eine Verdachtsdiagnose, ein Symptomkomplex oder ein allgemeines Gesundheitsthema sein.

**Priorität:** Muss

Mögliche Felder:

- Titel,
- Kurzbeschreibung,
- Typ,
- Status,
- Beginn / erstes Auftreten,
- Datum der Diagnose,
- behandelnde Ärzte,
- relevante Fachrichtungen,
- Schweregrad aus Nutzersicht,
- eigene Priorität,
- Tags,
- Synonyme,
- Notizen,
- verknüpfte Symptome,
- verknüpfte Dokumente,
- verknüpfte Quellen,
- offene Fragen.

#### LH-F-002 – Typisierung von Conditions

Das System soll Conditions nach Typ unterscheiden können.

Beispiele:

- gesicherte Diagnose,
- Verdachtsdiagnose,
- Symptom ohne Diagnose,
- Risikofaktor,
- Laborwertthema,
- Medikamententhema,
- Ernährungsthema,
- Präventionsthema,
- Arzttermin-Thema,
- Recherchethema.

**Priorität:** Soll

#### LH-F-003 – Status von Conditions

Das System soll einen Status pro Condition unterstützen.

Beispiele:

- neu,
- aktiv,
- in Beobachtung,
- mit Arzt besprochen,
- abgeklärt,
- chronisch,
- verbessert,
- verschlechtert,
- unklar,
- archiviert,
- verworfen.

**Priorität:** Soll

#### LH-F-004 – Synonyme und alternative Begriffe

Das System soll pro Condition Synonyme und alternative Begriffe verwalten.

Beispiel:

- Gefäßverkalkung,
- Arteriosklerose,
- Atherosklerose,
- vaskuläre Verkalkung.

Diese Synonyme sollen Suche und Zuordnung unterstützen.

**Priorität:** Soll

#### LH-F-005 – Conditions archivieren statt löschen

Das System soll Conditions nicht ohne Warnung endgültig löschen. Stattdessen soll eine Archivierung möglich sein.

**Priorität:** Muss

#### LH-F-006 – Beziehungen zwischen Conditions

Das System soll Beziehungen zwischen Conditions abbilden können.

Beispiele:

- kann zusammenhängen mit,
- Folge von,
- Risikofaktor für,
- Differentialdiagnose zu,
- beeinflusst,
- wird beeinflusst von,
- gehört zu,
- Teilaspekt von.

**Priorität:** Kann

---

### 13.2 Krankheits-/Condition-Wizard

Der Wizard ist eine zentrale Wunschfunktion. Er soll das strukturierte Anlegen einer neuen Krankheit bzw. Condition erleichtern und verhindern, dass wichtige Informationen direkt am Anfang vergessen werden.

#### LH-F-010 – Wizard starten

Das System muss einen deutlich sichtbaren Einstieg „Neue Krankheit / neues Gesundheitsthema anlegen“ anbieten.

**Priorität:** Muss

#### LH-F-011 – Wizard-Schritt 1: Grunddaten

Der Wizard muss zunächst Grunddaten erfassen.

Felder:

- Name der Krankheit / des Themas,
- Typ,
- Status,
- Kurzbeschreibung,
- erste Beobachtung,
- Diagnose bekannt ja/nein/unklar,
- Diagnosedatum, falls vorhanden,
- behandelnder Arzt, falls bekannt,
- Fachrichtung,
- persönliche Wichtigkeit.

**Priorität:** Muss

#### LH-F-012 – Wizard-Schritt 2: Symptome

Der Wizard muss direkt Symptome zur neuen Condition erfassen können.

Pro Symptom sollen mindestens erfassbar sein:

- Name,
- Beschreibung,
- erstes Auftreten,
- Häufigkeit,
- Intensität,
- Verlauf,
- Auslöser,
- lindernde Faktoren,
- verschlechternde Faktoren,
- betroffene Körperregion,
- Notizen.

**Priorität:** Muss

#### LH-F-013 – Wizard-Schritt 3: Dokumente

Der Wizard muss es ermöglichen, direkt beim Anlegen Dokumente hinzuzufügen.

Dokumenttypen:

- Arztbrief,
- Laborbericht,
- Scan,
- Foto,
- PDF,
- Screenshot,
- Webseite als PDF,
- Rezept,
- Medikationsplan,
- Entlassbrief,
- Untersuchungsbericht,
- Bildgebungshinweis,
- sonstiger Anhang.

**Priorität:** Muss

#### LH-F-014 – Wizard-Schritt 4: Quellen und Informationen

Der Wizard soll Quellen und erste recherchierte Informationen erfassen können.

Mögliche Quellen:

- Arztgespräch,
- Webseite,
- medizinisches Portal,
- Studie,
- Buch,
- Video,
- Fachartikel,
- Patientenbroschüre,
- Krankenkasseninformation,
- eigene Beobachtung,
- Chat-/KI-Ausgabe,
- Familieninformation,
- sonstige Quelle.

Pro Quelle sollen festgehalten werden können:

- Titel,
- URL oder Herkunft,
- Abrufdatum,
- Autor/Institution,
- Vertrauensniveau aus Nutzersicht,
- Zusammenfassung,
- wichtigste Aussagen,
- offene Zweifel,
- zugehörige Dokumente.

**Priorität:** Soll

#### LH-F-015 – Wizard-Schritt 5: Ärzte und Ansprechpartner

Der Wizard soll behandelnde oder relevante Ärzte und Ansprechpartner zuordnen können.

Felder:

- Name,
- Fachrichtung,
- Praxis/Klinik,
- Rolle,
- Kontaktinformationen,
- letzter Kontakt,
- nächster Termin,
- Notizen.

**Priorität:** Soll

#### LH-F-016 – Wizard-Schritt 6: Medikamente und Maßnahmen

Der Wizard soll dokumentieren können, ob im Zusammenhang mit der Condition Medikamente, ärztliche Maßnahmen, Empfehlungen oder eigene Maßnahmen existieren.

Wichtig: Das System darf daraus keine eigene Therapieempfehlung ableiten.

Mögliche Felder:

- Medikament / Wirkstoff,
- vom Arzt verordnet ja/nein/unklar,
- Dosierung als Freitext,
- Einnahmezeitraum,
- Grund der Einnahme,
- beobachtete Wirkung,
- beobachtete Nebenwirkungen,
- Quelle der Information,
- ärztliche Rückfrage erforderlich ja/nein.

**Priorität:** Soll

#### LH-F-017 – Wizard-Schritt 7: Laborwerte und Messwerte

Der Wizard soll vorhandene Labor- oder Messwerte erfassen können.

Beispiele:

- Blutzucker,
- HbA1c,
- Cholesterinwerte,
- Blutdruck,
- Gewicht,
- Entzündungswerte,
- Leberwerte,
- Nierenwerte,
- Vitamine / Mangelwerte,
- sonstige Werte.

Felder:

- Name des Werts,
- Wert,
- Einheit,
- Datum,
- Referenzbereich,
- Quelle/Labor,
- Dokument,
- Notiz.

**Priorität:** Soll

#### LH-F-018 – Wizard-Schritt 8: Fragen und offene Punkte

Der Wizard muss offene Fragen erfassen können.

Fragetypen:

- Frage an Arzt,
- Frage an Apotheke,
- Frage für eigene Recherche,
- Frage an Krankenkasse,
- Frage an Labor,
- Frage an Angehörige,
- unklare Quelle,
- zu prüfender Zusammenhang.

**Priorität:** Muss

#### LH-F-019 – Wizard-Schritt 9: Zusammenfassung und Prüfung

Der Wizard muss vor dem Speichern eine Zusammenfassung anzeigen.

Die Zusammenfassung soll zeigen:

- neue Condition,
- erfasste Symptome,
- erfasste Dokumente,
- erfasste Quellen,
- erfasste Ärzte,
- erfasste Medikamente,
- erfasste Laborwerte,
- erfasste Fragen,
- fehlende optionale Informationen.

**Priorität:** Muss

#### LH-F-020 – Wizard später fortsetzen

Der Wizard soll abgebrochen oder zwischengespeichert werden können.

**Priorität:** Soll

#### LH-F-021 – Wizard-Vorlagen

Das System kann später Wizard-Vorlagen anbieten.

Beispiele:

- chronische Erkrankung,
- akutes Symptom,
- Laborwertthema,
- Medikamentenproblem,
- Arzttermin-Nachbereitung,
- Recherchethema,
- Diagnoseklärung.

**Priorität:** Kann

#### LH-F-022 – Wizard mit Plausibilitätsprüfung

Der Wizard soll offensichtliche Eingabefehler erkennen, ohne medizinisch zu urteilen.

Beispiele:

- Datum liegt in der Zukunft,
- Dokument ohne Titel,
- Symptom ohne Name,
- Laborwert ohne Einheit,
- Quelle ohne Herkunft,
- Arztfrage ohne Fragetext.

**Priorität:** Soll

#### LH-F-023 – Wizard erzeugt automatisch Startstruktur

Nach Abschluss soll der Wizard automatisch eine Startstruktur erzeugen:

- Condition-Hauptseite,
- Symptomliste,
- Dokumentenliste,
- Quellenliste,
- Fragenliste,
- Timeline-Einträge,
- optional eine erste Arztmappe.

**Priorität:** Muss

---

### 13.3 Einträge und Journal

#### LH-F-030 – Freie Einträge erfassen

Das System muss freie Einträge erfassen können.

Eintragstypen:

- Notiz,
- Beobachtung,
- Befund,
- Symptomtagebuch,
- Arztgespräch,
- Rechercheergebnis,
- Frage,
- Entscheidung,
- Hypothese,
- Dokumentationskorrektur,
- Verlaufsupdate.

**Priorität:** Muss

#### LH-F-031 – Einträge mehreren Conditions zuordnen

Ein Eintrag soll mehreren Conditions zugeordnet werden können.

Beispiel: Ein Laborwert oder ein Arztbrief betrifft Diabetes und Gefäßthemen gleichzeitig.

**Priorität:** Soll

#### LH-F-032 – Einträge datieren

Jeder Eintrag muss ein Erstellungsdatum haben und soll zusätzlich ein fachliches Ereignisdatum haben können.

**Priorität:** Muss

#### LH-F-033 – Einträge klassifizieren

Einträge sollen nach Typ, Quelle, Status, Tags und Relevanz klassifiziert werden können.

**Priorität:** Soll

#### LH-F-034 – Fakten und eigene Interpretation trennen

Das System soll ermöglichen, zwischen folgenden Inhalten zu unterscheiden:

- Originalaussage,
- Zusammenfassung,
- eigene Einschätzung,
- offene Frage,
- ärztliche Aussage,
- ungesicherte Vermutung.

**Priorität:** Soll

#### LH-F-035 – Wiedervorlage

Ein Eintrag soll eine Wiedervorlage oder Erinnerung erhalten können.

**Priorität:** Kann

---

### 13.4 Symptome und Verlauf

#### LH-F-040 – Symptome erfassen

Das System muss Symptome erfassen und Conditions zuordnen können.

Felder:

- Name,
- Beschreibung,
- Beginn,
- Ende,
- Intensität,
- Häufigkeit,
- Verlauf,
- betroffene Körperregion,
- Auslöser,
- Kontext,
- lindernde Faktoren,
- verschlechternde Faktoren,
- Notizen.

**Priorität:** Muss

#### LH-F-041 – Symptomskalen

Das System soll Intensitäten über einfache Skalen erfassen können.

Beispiele:

- 0 bis 10,
- leicht/mittel/stark,
- selten/gelegentlich/häufig/dauerhaft,
- besser/gleich/schlechter.

**Priorität:** Soll

#### LH-F-042 – Symptomtagebuch

Das System soll wiederkehrende Symptomtagebuch-Einträge ermöglichen.

**Priorität:** Soll

#### LH-F-043 – Trendanzeige

Das System kann später Symptomtrends anzeigen.

**Priorität:** Kann

#### LH-F-044 – Korrelationen als Hinweis, nicht als Diagnose

Das System kann später mögliche Zusammenhänge zwischen Symptomen, Medikamenten, Ernährung, Schlaf, Stress oder Messwerten visualisieren. Diese Darstellung darf nicht als medizinische Diagnose formuliert werden.

**Priorität:** Später

---

### 13.5 Dokumentenverwaltung

#### LH-F-050 – Dokumente importieren

Das System muss Dokumente importieren und verwalten können.

Dateitypen:

- PDF,
- PNG,
- JPG/JPEG,
- TIFF,
- TXT,
- Markdown,
- DOCX optional,
- CSV optional,
- HTML optional.

**Priorität:** Muss für PDF/Bilder; Soll für weitere Formate

#### LH-F-051 – Dokumente beschreiben

Zu jedem Dokument sollen Metadaten erfasst werden können.

Felder:

- Titel,
- Dokumenttyp,
- Datum des Dokuments,
- Importdatum,
- Quelle,
- Autor/Institution,
- zugeordnete Conditions,
- Tags,
- Kurzbeschreibung,
- Vertraulichkeitsstufe,
- Notizen.

**Priorität:** Muss

#### LH-F-052 – Dokumente mehrfach zuordnen

Ein Dokument soll mehreren Conditions, Symptomen, Laborwerten oder Arztterminen zugeordnet werden können.

**Priorität:** Soll

#### LH-F-053 – Dokumentvorschau

Das System soll eine Vorschau für gängige Dateitypen anbieten.

**Priorität:** Soll

#### LH-F-054 – Originaldateien unverändert halten

Importierte Originaldokumente sollen nicht unbemerkt verändert werden. Ergänzende Notizen sollen separat gespeichert werden.

**Priorität:** Muss

#### LH-F-055 – OCR

Das System kann später OCR für gescannte Dokumente unterstützen.

**Priorität:** Später

#### LH-F-056 – Dokumentduplikate erkennen

Das System soll später doppelte Dateien anhand von Hashwerten erkennen können.

**Priorität:** Soll

---

### 13.6 Quellenverwaltung und Evidenznotizen

#### LH-F-060 – Quellen erfassen

Das System muss Quellen erfassen können.

Quellentypen:

- Arzt,
- Praxis,
- Klinik,
- Labor,
- Studie,
- medizinische Webseite,
- Buch,
- Video,
- Patientenbroschüre,
- Krankenkasse,
- Apotheke,
- eigene Beobachtung,
- KI-Ausgabe,
- persönliche Kommunikation.

**Priorität:** Muss

#### LH-F-061 – Quellenqualität dokumentieren

Das System soll eine einfache, subjektive Bewertung der Quellenqualität ermöglichen.

Beispiele:

- ärztlich bestätigt,
- offizielle Stelle,
- Fachquelle,
- populärwissenschaftlich,
- persönliche Erfahrung,
- unklar,
- widersprüchlich,
- noch zu prüfen.

**Priorität:** Soll

#### LH-F-062 – Abrufdatum speichern

Bei Onlinequellen soll das Abrufdatum gespeichert werden können.

**Priorität:** Soll

#### LH-F-063 – Kernaussagen extrahieren

Zu einer Quelle sollen Kernaussagen und eigene Notizen erfasst werden können.

**Priorität:** Soll

#### LH-F-064 – Widersprüche markieren

Das System soll widersprüchliche Quellen oder Aussagen markierbar machen.

**Priorität:** Kann

---

### 13.7 Arzttermine und Gesprächsvorbereitung

#### LH-F-070 – Arzttermine dokumentieren

Das System soll Arzttermine erfassen können.

Felder:

- Datum,
- Arzt / Praxis,
- Fachrichtung,
- Anlass,
- zugehörige Conditions,
- mitgenommene Dokumente,
- Fragen,
- Antworten,
- Ergebnisse,
- Folgemaßnahmen,
- neue Dokumente,
- Wiedervorlage.

**Priorität:** Soll

#### LH-F-071 – Fragenliste für Arzttermine

Das System muss offene Fragen sammeln und für Arzttermine ausgeben können.

**Priorität:** Muss

#### LH-F-072 – Arztmappe erzeugen

Das System soll eine kompakte Arztmappe erzeugen können.

Inhalt:

- Kurzprofil des Themas,
- relevante Symptome,
- Verlauf,
- wichtigste Befunde,
- wichtige Laborwerte,
- aktuelle Fragen,
- relevante Dokumentliste,
- Medikamente als dokumentierte Information,
- offene Punkte.

**Priorität:** Soll

#### LH-F-073 – Nachbereitung eines Arzttermins

Das System soll nach einem Arzttermin strukturierte Notizen und Ergebnisse erfassen können.

**Priorität:** Soll

---

### 13.8 Laborwerte und Messwerte

#### LH-F-080 – Laborwerte erfassen

Das System soll Laborwerte strukturiert erfassen können.

Felder:

- Wertname,
- Wert,
- Einheit,
- Datum,
- Referenzbereich,
- Labor/Quelle,
- zugehöriges Dokument,
- zugehörige Conditions,
- Notizen.

**Priorität:** Soll

#### LH-F-081 – Messwerte erfassen

Das System soll persönliche Messwerte erfassen können.

Beispiele:

- Blutdruck,
- Puls,
- Blutzucker,
- Gewicht,
- Temperatur,
- Schlafdauer,
- Aktivität,
- Schmerzen,
- sonstige eigene Skalen.

**Priorität:** Soll

#### LH-F-082 – Verlaufsgrafiken

Das System kann Verlaufsgrafiken anzeigen.

**Priorität:** Kann

#### LH-F-083 – Keine automatische Bewertung

Das System darf Labor- oder Messwerte nicht automatisch medizinisch bewerten, solange keine rechtliche und fachliche Grundlage dafür geschaffen wurde. Es darf Werte dokumentieren und anzeigen.

**Priorität:** Muss

---

### 13.9 Medikamente und Maßnahmen

#### LH-F-090 – Medikamente dokumentieren

Das System soll Medikamente dokumentieren können.

Felder:

- Handelsname,
- Wirkstoff,
- Dosierung als Freitext,
- Einnahmezeitraum,
- verordnet von,
- Grund,
- zugehörige Conditions,
- beobachtete Wirkung,
- beobachtete Nebenwirkungen,
- Notizen,
- Dokumente.

**Priorität:** Soll

#### LH-F-091 – Maßnahmen dokumentieren

Das System soll ärztliche oder eigene Maßnahmen dokumentieren können.

Beispiele:

- Untersuchung,
- Therapie,
- Ernährung,
- Bewegung,
- Schlaf,
- Kontrolltermin,
- Laborwiederholung,
- Verhaltensänderung.

**Priorität:** Soll

#### LH-F-092 – Keine Medikationsentscheidung

Das System darf keine Empfehlung ausgeben, ein Medikament zu beginnen, zu ändern oder abzusetzen.

**Priorität:** Muss

---

### 13.10 Suche, Filter und Navigation

#### LH-F-100 – Volltextsuche

Das System muss eine Suche über wichtige Inhalte ermöglichen.

Suchbereiche:

- Conditions,
- Symptome,
- Einträge,
- Dokumentmetadaten,
- Quellen,
- Fragen,
- Arzttermine,
- Tags,
- Notizen.

**Priorität:** Muss

#### LH-F-101 – Filter

Das System soll Filter anbieten.

Filterkriterien:

- Condition,
- Datum,
- Dokumenttyp,
- Eintragstyp,
- Quelle,
- Tag,
- Status,
- Priorität,
- Arzt,
- offene Fragen.

**Priorität:** Soll

#### LH-F-102 – Schnelle Navigation

Das System soll die wichtigsten Bereiche schnell erreichbar machen.

Bereiche:

- Dashboard,
- Conditions,
- Timeline,
- Dokumente,
- Fragen,
- Arzttermine,
- Quellen,
- Suche,
- Exporte,
- Einstellungen.

**Priorität:** Muss

#### LH-F-103 – Tag-System

Das System soll ein Tag-System unterstützen.

**Priorität:** Soll

---

### 13.11 Timeline und Verlauf

#### LH-F-110 – Timeline anzeigen

Das System soll eine chronologische Timeline anzeigen.

Timeline-Ereignisse:

- Symptombeginn,
- Arzttermin,
- Dokumentdatum,
- Laborwert,
- Medikationsänderung,
- Notiz,
- Frage,
- Entscheidung,
- Rechercheergebnis.

**Priorität:** Soll

#### LH-F-111 – Timeline filtern

Die Timeline soll nach Condition, Zeitraum, Ereignistyp und Relevanz filterbar sein.

**Priorität:** Soll

#### LH-F-112 – Ereignisse automatisch erzeugen

Das System soll aus neuen Einträgen automatisch Timeline-Ereignisse erzeugen können.

**Priorität:** Kann

---

### 13.12 Export und Berichte

#### LH-F-120 – Markdown-Export

Das System soll strukturierte Exporte als Markdown erzeugen können.

**Priorität:** Muss

#### LH-F-121 – PDF-Export

Das System soll Berichte als PDF erzeugen können.

**Priorität:** Soll

#### LH-F-122 – CSV-Export

Das System soll strukturierte Tabellen wie Laborwerte, Messwerte und Symptomtagebücher als CSV exportieren können.

**Priorität:** Soll

#### LH-F-123 – Arztbericht / Arztmappe

Das System soll eine kompakte Arztmappe exportieren können.

**Priorität:** Soll

#### LH-F-124 – Komplett-Export

Das System muss einen vollständigen Datenexport für Datensicherung, Migration und Unabhängigkeit ermöglichen.

**Priorität:** Muss

#### LH-F-125 – Selektiver Export

Das System soll selektive Exporte ermöglichen, damit der Nutzer nicht versehentlich zu viele private Informationen weitergibt.

**Priorität:** Soll

---

### 13.13 Dashboard

#### LH-F-130 – Übersichtsseite

Das System soll eine Übersichtsseite anbieten.

Mögliche Inhalte:

- aktive Conditions,
- offene Fragen,
- kommende Arzttermine,
- zuletzt importierte Dokumente,
- zuletzt bearbeitete Einträge,
- wichtige Wiedervorlagen,
- unzugeordnete Dokumente,
- offene Wizard-Entwürfe.

**Priorität:** Soll

#### LH-F-131 – Unzugeordnete Informationen anzeigen

Das System soll unzugeordnete Dokumente, Quellen oder Notizen sichtbar machen, damit sie nicht verloren gehen.

**Priorität:** Soll

---

### 13.14 Import und Erfassung

#### LH-F-140 – Dateiimport per Dialog

Das System muss Dateien über einen Importdialog hinzufügen können.

**Priorität:** Muss

#### LH-F-141 – Drag-and-Drop

Das System soll Drag-and-Drop für Dokumente unterstützen.

**Priorität:** Soll

#### LH-F-142 – Webquellen erfassen

Das System soll URLs und Webseiteninformationen erfassen können.

**Priorität:** Soll

#### LH-F-143 – Zwischenablage-Import

Das System kann Texte aus der Zwischenablage als Notiz oder Quelle erfassen.

**Priorität:** Kann

#### LH-F-144 – E-Mail-Import

Das System kann später E-Mails oder E-Mail-Anhänge erfassen.

**Priorität:** Später

---

### 13.15 Wissensmanagement

#### LH-F-150 – Themenartikel

Das System soll pro Condition einen zusammenfassenden Themenartikel ermöglichen.

Inhalt:

- Was weiß ich sicher?
- Was ist unklar?
- Welche Quellen gibt es?
- Was wurde ärztlich bestätigt?
- Welche Fragen sind offen?
- Welche Dokumente sind wichtig?

**Priorität:** Soll

#### LH-F-151 – Hypothesen verwalten

Das System soll Hypothesen oder Verdachtszusammenhänge erfassen können.

**Priorität:** Kann

#### LH-F-152 – Entscheidungen dokumentieren

Das System soll Entscheidungen dokumentieren können.

Beispiele:

- Frage mit Arzt besprochen,
- Thema zurückgestellt,
- Quelle als unsicher markiert,
- Dokument als besonders wichtig markiert,
- Verdacht verworfen.

**Priorität:** Soll

---

### 13.16 Erinnerungen und Aufgaben

#### LH-F-160 – Aufgaben erfassen

Das System soll Aufgaben erfassen können.

Beispiele:

- Arzttermin vereinbaren,
- Laborwert nachfragen,
- Dokument scannen,
- Quelle prüfen,
- Frage stellen,
- Bericht exportieren.

**Priorität:** Soll

#### LH-F-161 – Erinnerungen

Das System kann Erinnerungen anzeigen.

**Priorität:** Kann

#### LH-F-162 – Keine kritischen medizinischen Alarme

Das System soll in frühen Versionen keine sicherheitskritischen medizinischen Alarme übernehmen.

**Priorität:** Muss als Einschränkung

---

### 13.17 Mehrpersonenfähigkeit

#### LH-F-170 – Profile für mehrere Personen

Das System kann später mehrere Profile unterstützen.

Beispiel:

- Nutzer selbst,
- Ehepartner,
- Elternteil,
- Kind,
- betreute Person.

**Priorität:** Später

#### LH-F-171 – Strikte Trennung zwischen Profilen

Falls mehrere Personen unterstützt werden, müssen Daten sauber getrennt werden.

**Priorität:** Muss, sobald Mehrpersonenfähigkeit umgesetzt wird

---

### 13.18 KI-Unterstützung

#### LH-F-180 – KI-Zusammenfassungen

Das System kann später KI-Zusammenfassungen anbieten.

Mögliche Funktionen:

- Dokument zusammenfassen,
- Arztfragen vorschlagen,
- Timeline zusammenfassen,
- Quellen vergleichen,
- Widersprüche auflisten,
- Berichtsentwurf erzeugen.

**Priorität:** Später

#### LH-F-181 – KI nur mit Zustimmung

KI-Funktionen dürfen nur bewusst und transparent genutzt werden. Keine Gesundheitsdaten dürfen ungefragt an externe KI-Dienste gesendet werden.

**Priorität:** Muss, sobald KI umgesetzt wird

#### LH-F-182 – KI-Ergebnisse kennzeichnen

KI-generierte Inhalte müssen eindeutig als solche gekennzeichnet werden.

**Priorität:** Muss, sobald KI umgesetzt wird

#### LH-F-183 – KI darf nicht diagnostizieren

KI-Funktionen dürfen nicht als Diagnose- oder Therapieentscheidung auftreten.

**Priorität:** Muss

---

## 14. Nichtfunktionale Anforderungen

### 14.1 Datenschutz

#### LH-NF-001 – Lokale Speicherung

Das System muss standardmäßig lokal speichern. Eine Cloud-Synchronisation darf nicht erforderlich sein.

**Priorität:** Muss

#### LH-NF-002 – Keine Telemetrie ohne Zustimmung

Das System darf keine Nutzungs-, Gesundheits- oder Diagnosedaten ohne ausdrückliche Zustimmung übertragen.

**Priorität:** Muss

#### LH-NF-003 – Datenschutz durch Design

Das System soll nach dem Prinzip Datenschutz durch Design entwickelt werden.

**Priorität:** Muss

#### LH-NF-004 – Datenminimierung

Das System soll nur Daten speichern, die der Nutzer bewusst erfasst oder importiert.

**Priorität:** Muss

#### LH-NF-005 – Export- und Löschfähigkeit

Der Nutzer muss seine Daten exportieren und löschen können.

**Priorität:** Muss

---

### 14.2 Sicherheit

#### LH-NF-010 – Schutz sensibler Daten

Das System muss Gesundheitsdaten als besonders schützenswert behandeln.

**Priorität:** Muss

#### LH-NF-011 – Optionale Verschlüsselung

Das System soll Datenbank und Dokumentenablage verschlüsseln können.

**Priorität:** Soll

#### LH-NF-012 – Master-Passwort

Das System kann ein Master-Passwort für den Zugriff auf verschlüsselte Daten verwenden.

**Priorität:** Soll

#### LH-NF-013 – Automatische Sperre

Das System kann sich nach Inaktivität automatisch sperren.

**Priorität:** Kann

#### LH-NF-014 – Backup-Schutz

Backups sollen optional verschlüsselt werden können.

**Priorität:** Soll

#### LH-NF-015 – Keine stillen externen Verbindungen

Das System darf keine stillen externen Verbindungen aufbauen, die Gesundheitsdaten betreffen.

**Priorität:** Muss

---

### 14.3 Nachvollziehbarkeit

#### LH-NF-020 – Änderungshistorie

Das System soll wichtige Änderungen nachvollziehbar machen.

**Priorität:** Soll

#### LH-NF-021 – Originaldokumente bewahren

Originaldokumente sollen unverändert erhalten bleiben.

**Priorität:** Muss

#### LH-NF-022 – Korrekturen statt stiller Überschreibung

Bei wichtigen medizinischen Notizen soll erkennbar bleiben, wenn Informationen korrigiert oder ergänzt wurden.

**Priorität:** Soll

---

### 14.4 Bedienbarkeit

#### LH-NF-030 – Einfache Erfassung

Das System muss auch dann nutzbar sein, wenn der Nutzer gesundheitlich belastet ist und nicht lange strukturieren möchte.

**Priorität:** Muss

#### LH-NF-031 – Schnelle Notiz

Das System soll eine schnelle Notizfunktion bieten.

**Priorität:** Soll

#### LH-NF-032 – Verständliche Sprache

Die Oberfläche soll verständliche deutsche Begriffe verwenden. Fachbegriffe dürfen genutzt werden, sollten aber nicht die Bedienung dominieren.

**Priorität:** Muss

#### LH-NF-033 – Barrierearme Bedienung

Das System soll große Schrift, gute Kontraste und Tastaturbedienbarkeit berücksichtigen.

**Priorität:** Soll

#### LH-NF-034 – Kein überladener Startbildschirm

Trotz vieler Funktionen soll die erste Oberfläche ruhig und übersichtlich bleiben.

**Priorität:** Soll

---

### 14.5 Performance

#### LH-NF-040 – Schneller Start

Das System soll auch bei vielen Dokumenten und Einträgen in angemessener Zeit starten.

**Priorität:** Soll

#### LH-NF-041 – Schnelle Suche

Die Suche soll auch bei vielen Einträgen und Dokumentmetadaten schnell reagieren.

**Priorität:** Muss

#### LH-NF-042 – Große Dokumentensammlung

Das System soll perspektivisch tausende Dokumente verwalten können.

**Priorität:** Soll

---

### 14.6 Wartbarkeit und Erweiterbarkeit

#### LH-NF-050 – Erweiterbare Datenstruktur

Die Datenstruktur soll spätere Erweiterungen ermöglichen.

**Priorität:** Muss

#### LH-NF-051 – Saubere Dokumentation

Das Projekt soll fachlich und technisch gut dokumentiert werden.

**Priorität:** Muss

#### LH-NF-052 – Testbarkeit

Das System soll testbar entwickelt werden.

**Priorität:** Soll

---

## 15. Regulatorische und rechtliche Abgrenzung

### 15.1 Medizinprodukt-Abgrenzung

Das System soll in seiner ersten Zielsetzung keine Diagnose-, Therapie- oder Behandlungsentscheidungen treffen. Es soll Informationen verwalten, ordnen und ausgeben.

Funktionen, die in Richtung Diagnose, Therapieempfehlung, medizinische Bewertung, automatische Risikobewertung oder Behandlungssteuerung gehen, müssen vor Umsetzung gesondert rechtlich und fachlich geprüft werden.

### 15.2 Datenschutz

Gesundheitsdaten sind besonders sensibel. Das System muss Datenschutz nicht als späteres Zusatzfeature, sondern als Grundanforderung behandeln.

### 15.3 Haftung und Hinweise

Das System soll klar darauf hinweisen, dass es keine medizinische Beratung ersetzt. Exportberichte sollen ebenfalls einen Hinweis enthalten, dass sie aus Nutzerdokumentation stammen und ärztlich geprüft werden müssen.

### 15.4 Keine Notfallfunktion

Das System ist nicht für Notfälle vorgesehen. Es soll keine Notfallentscheidung unterstützen.

---

## 16. Datenkategorien

Das System soll folgende Datenkategorien unterstützen oder perspektivisch vorbereiten:

1. Personenprofil,
2. Conditions,
3. Symptome,
4. Einträge / Journal,
5. Dokumente,
6. Quellen,
7. Ärzte / Kontakte,
8. Arzttermine,
9. Fragen,
10. Laborwerte,
11. Messwerte,
12. Medikamente,
13. Maßnahmen,
14. Aufgaben,
15. Tags,
16. Synonyme,
17. Timeline-Ereignisse,
18. Exporte,
19. Backups,
20. Einstellungen.

---

## 17. Rollen und Berechtigungen

Für eine erste lokale Einzelplatzversion genügt eine einfache Nutzerrolle. Trotzdem sollen spätere Berechtigungsmodelle vorbereitet werden.

Mögliche Rollen für spätere Versionen:

| Rolle | Beschreibung | Priorität |
|---|---|---|
| Eigentümer | Vollzugriff auf alle Daten | Muss |
| Lesender Angehöriger | Nur bestimmte Berichte oder Themen | Später |
| Bearbeitender Angehöriger | Kann im Auftrag dokumentieren | Später |
| Arzt-Export-Empfänger | Kein App-Zugriff, nur exportierte Berichte | Soll über Export |
| Administrator | Technische Verwaltung bei Mehrbenutzerbetrieb | Später |

---

## 18. Benutzeroberfläche – gewünschte Bereiche

### 18.1 Hauptnavigation

Die Anwendung soll folgende Hauptbereiche anbieten:

- Dashboard,
- Conditions,
- Neue Condition / Wizard,
- Journal,
- Symptome,
- Dokumente,
- Quellen,
- Arzttermine,
- Fragen,
- Laborwerte / Messwerte,
- Medikamente / Maßnahmen,
- Timeline,
- Suche,
- Exporte,
- Einstellungen.

### 18.2 Condition-Detailseite

Eine Condition-Detailseite soll zusammenführen:

- Übersicht,
- Beschreibung,
- Status,
- Symptome,
- Verlauf,
- Dokumente,
- Quellen,
- Fragen,
- Laborwerte,
- Medikamente,
- Arztkontakte,
- Timeline,
- Notizen,
- Exporte.

### 18.3 Dokumentansicht

Die Dokumentansicht soll zeigen:

- Dokumentvorschau,
- Metadaten,
- Zuordnungen,
- Notizen,
- Quellenbezug,
- Tags,
- Importdatum,
- Originalpfad oder interner Ablageort.

### 18.4 Fragenansicht

Die Fragenansicht soll offene Fragen sichtbar machen.

Filter:

- pro Condition,
- pro Arzttermin,
- nach Dringlichkeit,
- nach Status,
- nach Zielperson,
- beantwortet / offen / zurückgestellt.

---

## 19. Zentrale Anwendungsfälle

### UC-001 – Neue Krankheit mit Wizard anlegen

**Ziel:** Der Nutzer legt eine neue Krankheit oder ein neues Gesundheitsthema an und erfasst direkt Symptome, Dokumente und Fragen.

**Ablauf:**

1. Nutzer startet den Wizard.
2. Nutzer gibt Namen und Grunddaten ein.
3. Nutzer erfasst erste Symptome.
4. Nutzer fügt vorhandene Dokumente hinzu.
5. Nutzer erfasst Quellen oder erste Informationen.
6. Nutzer ordnet Ärzte oder Ansprechpartner zu.
7. Nutzer notiert offene Fragen.
8. Nutzer prüft die Zusammenfassung.
9. Nutzer speichert.
10. System erzeugt Condition, Einträge, Dokumentverknüpfungen und Timeline.

**Ergebnis:** Eine strukturierte Condition ist angelegt und sofort nutzbar.

### UC-002 – Arzttermin vorbereiten

**Ziel:** Der Nutzer bereitet einen Arzttermin vor.

**Ablauf:**

1. Nutzer wählt Arzttermin oder legt Termin an.
2. Nutzer ordnet relevante Conditions zu.
3. System zeigt offene Fragen.
4. Nutzer wählt relevante Dokumente und Befunde.
5. Nutzer erstellt eine kompakte Arztmappe.
6. Nutzer exportiert die Unterlagen.

**Ergebnis:** Der Nutzer hat eine strukturierte Gesprächsgrundlage.

### UC-003 – Neues Dokument importieren

**Ziel:** Der Nutzer importiert einen Arztbrief oder Laborbericht.

**Ablauf:**

1. Nutzer importiert Datei.
2. System fragt Metadaten ab.
3. Nutzer ordnet Dokument einer oder mehreren Conditions zu.
4. Nutzer ergänzt Notizen und Tags.
5. System speichert Dokument und erzeugt optional Timeline-Eintrag.

**Ergebnis:** Das Dokument ist auffindbar und fachlich eingeordnet.

### UC-004 – Symptomverlauf dokumentieren

**Ziel:** Der Nutzer dokumentiert wiederkehrende Symptome.

**Ablauf:**

1. Nutzer öffnet Symptomtagebuch.
2. Nutzer wählt Symptom oder legt neues Symptom an.
3. Nutzer gibt Datum, Intensität, Kontext und Notiz ein.
4. System speichert den Verlauf.

**Ergebnis:** Der Verlauf kann später angezeigt oder exportiert werden.

### UC-005 – Quelle dokumentieren

**Ziel:** Der Nutzer speichert eine gefundene medizinische Information.

**Ablauf:**

1. Nutzer erstellt neue Quelle.
2. Nutzer gibt Titel, URL, Autor/Institution und Abrufdatum ein.
3. Nutzer notiert Kernaussagen.
4. Nutzer ordnet Quelle einer Condition zu.
5. Nutzer bewertet die Quelle als gesichert, nützlich, unklar oder zu prüfen.

**Ergebnis:** Die Information ist später nachvollziehbar.

### UC-006 – Komplette Daten sichern

**Ziel:** Der Nutzer erstellt ein Backup.

**Ablauf:**

1. Nutzer startet Backup-Funktion.
2. System bietet Zielordner und Verschlüsselungsoption an.
3. System erstellt Backup.
4. System prüft Backup auf Vollständigkeit.
5. System meldet Ergebnis.

**Ergebnis:** Die Daten sind gesichert.

---

## 20. Gewünschte Berichte und Exporte

### 20.1 Arztmappe

Eine Arztmappe soll möglichst kurz, klar und druckbar sein.

Inhalte:

- Thema / Condition,
- Kurzbeschreibung,
- wichtige Symptome,
- Verlauf,
- aktuelle Fragen,
- wichtige Dokumente,
- relevante Laborwerte,
- aktuelle Medikamente als Dokumentation,
- offene Punkte.

### 20.2 Verlaufsbericht

Ein Verlaufsbericht soll Ereignisse chronologisch darstellen.

### 20.3 Quellenbericht

Ein Quellenbericht soll zeigen, welche Informationen aus welchen Quellen stammen.

### 20.4 Fragenliste

Eine Fragenliste soll für Arzttermine exportiert werden können.

### 20.5 Dokumentenindex

Ein Dokumentenindex soll alle relevanten Dokumente mit Datum, Typ und Zuordnung auflisten.

### 20.6 Laborwerttabelle

Laborwerte sollen tabellarisch exportierbar sein.

### 20.7 Gesamtexport

Ein Gesamtexport soll alle Daten in einem offenen oder zumindest gut dokumentierten Format bereitstellen.

---

## 21. Qualitätsanforderungen an Inhalte

Das System soll den Nutzer unterstützen, gute Dokumentation zu erstellen. Dazu gehören:

- klare Trennung von Quelle und eigener Meinung,
- Datum und Kontext,
- Erfassung offener Fragen,
- Statusangaben,
- Verknüpfung mit Dokumenten,
- Vermeidung unzugeordneter Informationen,
- Hinweise auf fehlende Metadaten,
- strukturierte Zusammenfassungen.

---

## 22. Datenhaltung und Backup aus Anwendersicht

Der Nutzer erwartet:

1. Daten bleiben lokal.
2. Dokumente gehen nicht verloren.
3. Backups sind einfach erstellbar.
4. Backups sind wiederherstellbar.
5. Daten sind exportierbar.
6. Kein Hersteller-Lock-in.
7. Optionaler Schutz durch Verschlüsselung.
8. Keine ungefragte Cloud.

---

## 23. Importquellen – langfristige Wunschliste

Langfristig sollen folgende Quellen zumindest fachlich bedacht werden:

- manuelle Eingabe,
- PDF-Import,
- Bildimport,
- Scan,
- Webclipper,
- Browser-Lesezeichen,
- CSV-Dateien,
- Laborwerttabellen,
- Patientenportal-Downloads,
- Krankenkassenunterlagen,
- Medikationspläne,
- Arztbrief-Archive,
- E-Mail-Anhänge,
- Geräteexporte,
- Smartphone-Health-Exporte,
- CGM-/Diabetesdaten,
- Blutdruckmessgeräte,
- Waagen,
- Wearables,
- FHIR-Daten.

Nicht alle diese Importe gehören in V1. Sie sollen aber im Zielbild nicht vergessen werden.

---

## 24. Abgrenzung zu Vergleichsprogrammen

### 24.1 Findings

Findings dient als Inspiration für das Konzept eines strukturierten Notebooks mit Protokollen, Anhängen, Zeitstempeln und nachvollziehbaren Einträgen. Das SASD Health Research Notebook soll jedoch nicht primär Laborversuche, sondern persönliche Gesundheitsinformationen strukturieren.

### 24.2 Electronic Lab Notebooks

Aus ELN-Systemen sollen übernommen werden:

- strukturierte Einträge,
- Templates,
- Anhänge,
- Audit-Idee,
- Nachvollziehbarkeit,
- Export,
- klare Trennung von Rohdaten und Auswertung.

Nicht übernommen werden müssen in V1:

- Laborgeräteintegration,
- Teamfreigaben,
- elektronische Signaturen,
- Compliance-Workflows für Forschungslabore.

### 24.3 Symptomtracker

Aus Symptomtrackern sollen übernommen werden:

- einfache tägliche Erfassung,
- Intensitätsskalen,
- Verlauf,
- Tags,
- Zusammenfassungen.

Nicht primär übernommen werden soll:

- starke Mobile-First-Ausrichtung,
- Gamification,
- Cloudzwang.

### 24.4 Dokumentenmanagement

Aus Dokumentenmanagementsystemen sollen übernommen werden:

- Dokumentenimport,
- Metadaten,
- Suche,
- Tags,
- Versionierung,
- Export.

Nicht zwingend in V1:

- komplexe Workflows,
- Mehrbenutzer-Freigaben,
- Enterprise-Berechtigungen.

### 24.5 Wissensmanagement-Tools

Aus Obsidian, Logseq, Joplin und ähnlichen Systemen sollen übernommen werden:

- Verlinkungen,
- Markdown-Export,
- Notizen,
- Tags,
- Backlinks als Idee.

Nicht zwingend übernommen werden soll:

- reine Freitextstruktur ohne Fachmodell.

---

## 25. Priorisierte V1-Anforderungen

Eine sinnvolle erste Version sollte mindestens leisten:

1. Conditions anlegen, bearbeiten, archivieren.
2. Krankheits-Wizard mit Grunddaten, Symptomen, Dokumenten, Fragen.
3. Einträge/Notizen erfassen.
4. Dokumente importieren und zuordnen.
5. Quellen erfassen.
6. Symptome dokumentieren.
7. Fragen verwalten.
8. Suche über zentrale Inhalte.
9. Markdown-Export.
10. Backup und Restore.
11. Lokale Datenhaltung.
12. Datenschutz-Hinweise und klare medizinische Abgrenzung.

---

## 26. V1 bewusst noch nicht zwingend

Folgende Funktionen sind wichtig, aber nicht zwingend in der ersten produktiven Version:

- OCR,
- KI-Unterstützung,
- FHIR-Export,
- Smartphone-App,
- Cloud-Sync,
- Mehrbenutzerbetrieb,
- elektronische Signatur,
- automatische medizinische Bewertung,
- Geräteintegration,
- komplexe Diagramme,
- Medikations-Wechselwirkungsprüfung,
- automatische Laborwertinterpretation,
- automatische Diagnosevorschläge.

---

## 27. Risiken

| Risiko | Beschreibung | Gegenmaßnahme |
|---|---|---|
| Funktionsumfang wird zu groß | Zu viele Ideen verzögern V1 | V1 klein schneiden, Backlog erhalten |
| Medizinprodukt-Risiko | App könnte als Diagnose-/Therapiesoftware interpretiert werden | klare Abgrenzung, keine Empfehlungen, Rechtsprüfung |
| Datenschutzrisiko | Gesundheitsdaten sind sensibel | lokal-first, Verschlüsselung, keine Telemetrie |
| Datenverlust | Dokumente oder Datenbank könnten beschädigt werden | Backup/Restore früh einbauen |
| Unklare Datenstruktur | Spätere Erweiterungen werden schwer | sauberes Datenmodell und Migrationen |
| Überladene Oberfläche | Nutzer wird von Funktionen erschlagen | Wizard, Dashboard, klare Navigation |
| Falsche Sicherheit durch App | Nutzer vertraut App mehr als Arzt | Warnhinweise und klare Formulierungen |
| Quellenvermischung | Eigene Meinung und Quelle werden verwechselt | Quellentypen und klare Felder |
| Vendor Lock-in | Daten nur in App nutzbar | Export in offene Formate |

---

## 28. Akzeptanzkriterien für eine erste Version

Eine erste Version gilt fachlich als akzeptabel, wenn:

1. der Nutzer eine neue Condition über den Wizard anlegen kann,
2. Symptome direkt im Wizard erfasst werden können,
3. Dokumente direkt im Wizard hinzugefügt werden können,
4. Quellen oder Informationen direkt zugeordnet werden können,
5. offene Fragen direkt angelegt werden können,
6. die Condition danach übersichtlich angezeigt wird,
7. Dokumente und Einträge später wiedergefunden werden,
8. eine Suche über zentrale Inhalte funktioniert,
9. ein Markdown-Export erstellt werden kann,
10. Daten lokal gespeichert werden,
11. ein Backup erstellt und wiederhergestellt werden kann,
12. das System keine medizinischen Empfehlungen gibt,
13. die wichtigsten Datenschutz- und Sicherheitshinweise sichtbar dokumentiert sind.

---

## 29. Grobe Roadmap aus Lastenheft-Sicht

### Phase 0 – Konzept und Dokumentation

- Lastenheft,
- Pflichtenheft,
- Datenmodell,
- UI-Konzept,
- Sicherheitskonzept,
- Roadmap.

### Phase 1 – Anwendungsschale

- lokale Desktop-App,
- Navigation,
- Datenbank,
- Dokumentenablage,
- Einstellungen,
- Backup-Grundlage.

### Phase 2 – Conditions und Wizard

- Conditions,
- Wizard,
- Symptome,
- Dokumentzuordnung,
- Fragen.

### Phase 3 – Dokumente, Quellen und Suche

- Dokumentenverwaltung,
- Quellenverwaltung,
- Volltextsuche,
- Tags.

### Phase 4 – Timeline und Arztmappe

- Timeline,
- Arzttermine,
- Fragenlisten,
- Arztbericht/Markdown-Export.

### Phase 5 – Laborwerte, Messwerte und Medikamente

- strukturierte Werte,
- einfache Tabellen,
- Verlauf,
- Medikationsdokumentation ohne Empfehlung.

### Phase 6 – Sicherheit und Komfort

- Verschlüsselung,
- automatische Sperre,
- verbesserte Backups,
- Importkomfort.

### Phase 7 – Spätere intelligente Funktionen

- OCR,
- KI-Zusammenfassungen,
- Quellenvergleich,
- FHIR,
- optionale Synchronisation.

---

## 30. Offene Fragen

Folgende Punkte müssen vor oder während der Umsetzung geklärt werden:

1. Soll die erste Version WinForms oder WPF verwenden?
2. Soll Verschlüsselung bereits in V1 Pflicht sein oder als Phase 2 kommen?
3. Soll die App nur für Eigenbedarf oder später öffentlich bereitgestellt werden?
4. Welche Exportformate sind zuerst notwendig: Markdown, PDF, CSV?
5. Wie streng soll die Trennung zwischen Diagnose, Verdacht und Thema umgesetzt werden?
6. Sollen mehrere Personenprofile schon früh vorgesehen werden?
7. Soll es eine mobile Ergänzung geben?
8. Soll ein Webclipper langfristig eingeplant werden?
9. Wie werden Datenmigrationen versioniert?
10. Wie wird ein sicherer Recovery-Prozess für verschlüsselte Daten gestaltet?
11. Welche rechtliche Prüfung ist vor Veröffentlichung erforderlich?
12. Wie soll das Projekt öffentlich beschrieben werden, ohne medizinische Leistungsversprechen zu machen?

---

## 31. Abnahmekriterien je Hauptbereich

### 31.1 Condition-Wizard

Der Wizard ist abnahmefähig, wenn:

- eine neue Condition vollständig angelegt werden kann,
- Symptome erfasst werden können,
- Dokumente zugeordnet werden können,
- Fragen erfasst werden können,
- der Nutzer eine Zusammenfassung vor dem Speichern sieht,
- unvollständige Pflichtfelder erkannt werden,
- der gespeicherte Datensatz danach korrekt in der Condition-Ansicht erscheint.

### 31.2 Dokumentenverwaltung

Die Dokumentenverwaltung ist abnahmefähig, wenn:

- PDFs und Bilder importiert werden können,
- Metadaten erfasst werden können,
- Dokumente Conditions zugeordnet werden können,
- Dokumente wiedergefunden werden,
- Originaldateien nicht unkontrolliert verändert werden,
- ein Backup die Dokumente einschließt.

### 31.3 Suche

Die Suche ist abnahmefähig, wenn:

- Conditions gefunden werden,
- Dokumentmetadaten gefunden werden,
- Einträge gefunden werden,
- Quellen gefunden werden,
- Tags berücksichtigt werden,
- Ergebnisse verständlich dargestellt werden.

### 31.4 Export

Der Export ist abnahmefähig, wenn:

- eine Condition als Markdown exportiert werden kann,
- offene Fragen exportiert werden können,
- Dokumentenlisten exportiert werden können,
- sensible Zusatzdaten bewusst ein- oder ausgeschlossen werden können.

---

## 32. Feature-Backlog – bewusst breit gesammelt

Die folgenden Features sind als langfristiger Ideenvorrat dokumentiert. Sie sind nicht alle Bestandteil von V1.

### 32.1 Erfassung

- Schnellnotiz,
- strukturierte Notiz,
- Sprachnotiz,
- Fotoaufnahme,
- Scanimport,
- PDF-Import,
- Webclipper,
- E-Mail-Import,
- CSV-Import,
- Drag-and-Drop,
- Clipboard-Import,
- Import aus Download-Ordner,
- Import-Assistent für Patientenportale,
- Import aus Krankenkassenunterlagen,
- Import aus Laborportalen.

### 32.2 Organisation

- Conditions,
- Kategorien,
- Tags,
- Synonyme,
- Status,
- Prioritäten,
- Favoriten,
- Archiv,
- verknüpfte Themen,
- Sammlungen,
- Projekte,
- Ordner,
- intelligente Ansichten.

### 32.3 Medizinische Dokumentation ohne Diagnosefunktion

- Symptome,
- Befunde,
- Laborwerte,
- Messwerte,
- Medikamente,
- Nebenwirkungen,
- Maßnahmen,
- Arzttermine,
- Fragen,
- Verlauf,
- familiäre Hinweise,
- Risikofaktoren,
- Ernährung,
- Bewegung,
- Schlaf,
- Stress,
- Schmerzskalen.

### 32.4 Wissensmanagement

- Quellenverwaltung,
- Kernaussagen,
- Zusammenfassungen,
- Widersprüche,
- eigene Hypothesen,
- Entscheidungsnotizen,
- Recherchelisten,
- offene Punkte,
- Glossar,
- Begriffssynonyme,
- Literaturverweise,
- Studiennotizen.

### 32.5 Auswertung

- Timeline,
- Trenddiagramme,
- Tabellen,
- Filterberichte,
- Exportberichte,
- Arztmappe,
- Jahresbericht,
- Symptomverlauf,
- Laborwertverlauf,
- Dokumentenindex,
- Quellenbericht,
- Fragenbericht.

### 32.6 Sicherheit

- lokale Datenhaltung,
- Verschlüsselung,
- Master-Passwort,
- automatische Sperre,
- verschlüsselte Backups,
- Wiederherstellungstest,
- Datenexport,
- Löschfunktion,
- Änderungsprotokoll,
- keine Telemetrie,
- explizite Cloud-Zustimmung,
- Audit-Historie.

### 32.7 Komfort

- Wizard,
- Vorlagen,
- Autovervollständigung,
- zuletzt verwendet,
- Favoriten,
- Schnellerfassung,
- Importwarteschlange,
- unzugeordnete Dokumente,
- Erinnerungen,
- Aufgaben,
- Druckansicht,
- helle/dunkle Oberfläche,
- größere Schrift.

### 32.8 Spätere intelligente Funktionen

- OCR,
- lokale KI,
- optionale externe KI,
- Zusammenfassungen,
- Arztfragen-Vorschläge,
- Quellenvergleich,
- Widerspruchssuche,
- Duplikaterkennung,
- automatische Verschlagwortung,
- semantische Suche,
- Chat über eigene Dokumente,
- Risiko: keine Diagnosefunktion.

### 32.9 Interoperabilität

- Markdown,
- PDF,
- CSV,
- JSON,
- ZIP-Komplettexport,
- FHIR später,
- Kalenderexport später,
- Kontaktexport später,
- Import/Export für andere Notizsysteme,
- strukturierte Backup-Dateien.

---

## 33. Vorläufige MVP-Abgrenzung

Das MVP soll nicht die vollständige Vision abbilden. Es soll beweisen, dass der Kernnutzen funktioniert:

> Eine neue Condition kann sauber angelegt, mit Symptomen, Dokumenten, Quellen und Fragen verknüpft, später wiedergefunden und als verständlicher Bericht exportiert werden.

Daraus ergibt sich für das MVP:

### MVP-Muss

- lokale App,
- lokale Datenbank,
- Condition-Verwaltung,
- Wizard,
- Symptome,
- Dokumente,
- Quellen,
- Fragen,
- Suche,
- Markdown-Export,
- Backup,
- klare medizinische Abgrenzung.

### MVP-Soll

- Timeline,
- Arztmappe,
- Tag-System,
- Dokumentvorschau,
- einfache Laborwerte,
- einfache Medikamentendokumentation.

### Nicht MVP

- OCR,
- KI,
- Cloud-Sync,
- mobile App,
- FHIR,
- automatische medizinische Bewertung,
- komplexe Diagramme.

---

## 34. Schlussbemerkung

Dieses Lastenheft beschreibt bewusst ein großes Zielbild, damit wichtige Anforderungen nicht verloren gehen. Die spätere Umsetzung muss trotzdem phasenweise, klein und robust erfolgen.

Der wichtigste Kern ist nicht eine möglichst große Funktionsliste, sondern ein vertrauenswürdiges System, das dem Nutzer hilft, Gesundheitsinformationen strukturiert zu sammeln, Quellen und eigene Beobachtungen sauber zu trennen, Arztgespräche besser vorzubereiten und die eigenen Daten sicher unter Kontrolle zu behalten.

Der Krankheits-/Condition-Wizard ist dabei der zentrale Einstiegspunkt, weil er aus einer unsortierten Sammlung von Informationen direkt eine geordnete Struktur erzeugt.
