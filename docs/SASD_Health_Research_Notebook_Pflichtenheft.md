# SASD Health Research Notebook – Pflichtenheft

> **Revision 2.1 (2026-09-30):** Dieses Dokument bleibt als ursprüngliche ausführliche Spezifikation erhalten. Für seit Mai 2026 konkretisierte Anforderungen zu Ernährung, Messwert-/Wetterkontext, Sessions/Arztbesuchen/Coaching, HealthActions, Routinen, Benachrichtigungen, Quellenfundstellen, Medien und Kontaktreferenzen gelten ergänzend und bei Widerspruch vorrangig [160_Dokumentationsrevision_2_1.md](changes/160_Dokumentationsrevision_2_1.md) und [155_Fachmodule_Baseline_2_1.md](requirements/155_Fachmodule_Baseline_2_1.md). Die Anwendung bleibt ein Dokumentations- und Organisationswerkzeug und erzeugt keine Diagnose- oder Therapieanweisungen.

**Dokumenttyp:** Pflichtenheft / technische Produktspezifikation  
**Projekt:** SASD Health Research Notebook  
**Arbeitstitel:** `SASD Health Research Notebook`  
**Alternative Projektnamen:** `SASD Health Notebook`, `SASD CareNotes`, `SASD Evidence Notebook`, `SASD Medical Research Journal`  
**Status:** Entwurf V0.1  
**Stand:** 2026-05-25  
**Grundlagen:** Findings Lab Notebook, SASD Feature-Sammlung vom 2026-05-25, Produktvergleich ELN/PHR/DMS/Tracker/Wissensmanagement  
**Ziel:** Entwicklungsfähige Spezifikation für ein lokal-first Programm zur persönlichen medizinischen Recherche-, Befund- und Verlaufsdokumentation.

---

## 0. Kurzfassung

Das **SASD Health Research Notebook** ist eine lokale Desktop-Anwendung zur strukturierten Sammlung, Dokumentation, Verschlagwortung, Suche und Auswertung persönlicher Gesundheitsinformationen. Das System soll Erkrankungen, Symptome, Befunde, Laborwerte, Arztbriefe, Quellen, Dokumente, Arzttermine, offene Fragen und eigene Beobachtungen in einem nachvollziehbaren, revisionsfreundlichen System zusammenführen.

Das System soll **keine Diagnosen stellen**, **keine Therapieentscheidungen treffen**, **keine ärztliche Beratung ersetzen** und in der ersten Ausbaustufe **keine automatisierten medizinischen Handlungsempfehlungen** geben. Es ist zuerst ein **persönliches Organisations-, Recherche- und Dokumentationswerkzeug**.

Ein zentrales Element ist ein **Wizard zum Anlegen neuer Erkrankungen bzw. Gesundheitsthemen**. Dieser Wizard soll direkt beim Anlegen eines Themas Symptome, Dokumente, Quellen, Laborwerte, Medikamente/Maßnahmen, Arztfragen und Notizen erfassen und zuordnen können.

Die Anwendung soll lokal und datenschutzorientiert entwickelt werden: **lokale SQLite-Datenbank**, **lokale Dokumentenablage**, **keine Cloud-Pflicht**, **Exportfähigkeit**, **Backup-Konzept**, **Audit-Trail**, **Soft Delete/Archivierung** und später **optionale Verschlüsselung**.

---

## 1. Dokumentzweck

Dieses Pflichtenheft beschreibt, wie das geplante System aus technischer und funktionaler Sicht umgesetzt werden soll. Es übersetzt die vorherige Feature-Sammlung in eine entwicklungsfähige Spezifikation.

Das Dokument soll:

- die fachliche Zielrichtung stabilisieren,
- wichtige Funktionen früh sichtbar machen,
- teure spätere Nacharbeiten vermeiden,
- bewusst zwischen V1, späteren Erweiterungen und ausgeschlossenen Funktionen unterscheiden,
- eine Grundlage für Repository, Architektur, Datenmodell, Tests und Roadmap bilden,
- spätere Lastenheft-, Pflichtenheft- und Roadmap-Dokumente konsistent halten.

---

## 2. Ausgangslage und Problemstellung

Persönliche Gesundheitsinformationen liegen oft verstreut vor:

- Arztbriefe als PDF oder Papier,
- Laborwerte in Tabellen oder Patientenportalen,
- eigene Notizen in verschiedenen Apps,
- Webseiten und Studien als Browser-Lesezeichen,
- Medikamenteninformationen auf Packungen, Beipackzetteln oder Webseiten,
- Symptome im Kopf, auf Zetteln oder in Kalendern,
- Fragen an Ärzte in separaten Notizen,
- Verlaufserinnerungen ohne Zeitstempel.

Dadurch entstehen typische Probleme:

- Informationen werden nicht wiedergefunden.
- Befunde sind nicht mit Symptomen oder Arztterminen verbunden.
- Quellen sind nicht bewertet oder nicht mehr nachvollziehbar.
- Die eigene Krankheitsgeschichte ist vor Arztterminen schwer zusammenzufassen.
- Wichtige offene Fragen gehen verloren.
- Dokumente werden kopiert, umbenannt oder verändert, ohne dass klar bleibt, was das Original war.
- Spätere Auswertungen sind schwierig, weil Daten nicht strukturiert vorliegen.

Das geplante System soll diese Probleme lösen, ohne in den Bereich medizinischer Diagnostik oder Therapieempfehlung vorzudringen.

---

## 3. Produktvision

Das System soll wie ein persönliches, medizinisches Forschungstagebuch funktionieren:

> „Alles, was ich zu meinen Erkrankungen, Symptomen, Befunden, Arztgesprächen und Quellen finde oder beobachte, kann ich an einer Stelle sicher sammeln, strukturieren, wiederfinden und für Arztgespräche verständlich aufbereiten.“

Die Anwendung verbindet drei Welten:

1. **Lab Notebook / ELN-Denken**  
   Nachvollziehbare Einträge, Protokolle, Zeitstempel, Quellen, Versionen, Audit-Trail.

2. **Personal Health Record / Gesundheitsakte**  
   Erkrankungen, Befunde, Laborwerte, Medikamente, Arztkontakte, Verlauf.

3. **Wissensmanagement / Dokumentenmanagement**  
   Volltextsuche, Tags, Synonyme, Dokumente, Quellen, Beziehungen, Export.

---

## 4. Abgrenzung und regulatorische Leitplanken

### 4.1 Positiver Zweck

Das System darf in V1:

- Informationen sammeln,
- Dokumente archivieren,
- Symptome manuell protokollieren,
- Laborwerte manuell dokumentieren,
- Quellen speichern und bewerten,
- offene Fragen verwalten,
- Arzttermine vorbereiten,
- Verlauf und Timeline anzeigen,
- Berichte exportieren,
- eigene Notizen und Entscheidungen nachvollziehbar speichern.

### 4.2 Explizite Nicht-Ziele in V1

Das System darf in V1 nicht:

- Diagnosen stellen,
- Symptome automatisch medizinisch interpretieren,
- Therapieempfehlungen geben,
- Medikamente empfehlen, ändern oder absetzen,
- Wechselwirkungen verbindlich bewerten,
- Notfälle erkennen und triagieren,
- ärztliche Beratung ersetzen,
- Daten ungefragt an externe Dienste übertragen,
- KI-generierte medizinische Aussagen als Wahrheit darstellen.

### 4.3 Medizinprodukt-Grenze

Das System ist zunächst als **persönliches Dokumentations- und Recherchewerkzeug** zu konzipieren. Funktionen, die eine medizinische Zweckbestimmung nahelegen könnten, müssen in V1 vermieden oder streng begrenzt werden.

Besonders kritisch sind:

- Diagnosevorschläge,
- Risikoscores,
- Therapieempfehlungen,
- automatische Interpretation von Laborwerten,
- automatische Warnungen aus Vitalwerten,
- KI-gestützte medizinische Empfehlungen,
- automatisierte Entscheidungsunterstützung.

Solche Funktionen dürfen nur als spätere, gesondert geprüfte Erweiterungen betrachtet werden.

### 4.4 Datenschutz-Grenze

Gesundheitsdaten sind besonders sensible personenbezogene Daten. Das System muss daher von Beginn an datensparsam, lokal, nachvollziehbar und sicher entworfen werden.

Die Anwendung soll standardmäßig:

- keine Cloud benötigen,
- keine Telemetrie senden,
- keine personenbezogenen Daten an Dritte übertragen,
- Exporte bewusst und manuell auslösen,
- Backups und Exportpakete klar kennzeichnen,
- sensible Dokumente markieren können.

---

## 5. Zielgruppen und Rollen

### 5.1 Primäre Zielgruppe

Die primäre Zielgruppe ist eine einzelne Person, die eigene Gesundheitsinformationen strukturiert sammeln möchte.

Typische Nutzungssituationen:

- chronische Erkrankung dokumentieren,
- Befunde sammeln,
- Symptome im Verlauf verstehen,
- Arztgespräche vorbereiten,
- Quellen und Studien sammeln,
- Laborwerte und Vitalwerte nachvollziehen,
- eigene Fragen und Entscheidungen dokumentieren.

### 5.2 Sekundäre Zielgruppen

Spätere oder indirekte Zielgruppen:

- Angehörige oder Vertrauenspersonen, die beim Organisieren helfen,
- Ärzte als Empfänger von exportierten Zusammenfassungen,
- Pflegepersonen als Empfänger ausgewählter Informationen,
- Entwickler/Administrator im lokalen privaten Umfeld.

### 5.3 Rollen im System

| Rolle | Beschreibung | V1-Relevanz |
|---|---|---:|
| Hauptnutzer | Erfasst, verwaltet und exportiert eigene Informationen. | Muss |
| Lokaler Administrator | Installiert, sichert und aktualisiert die Anwendung. In V1 meist identisch mit Hauptnutzer. | Muss |
| Vertrauensperson | Könnte später lesend oder unterstützend Zugriff erhalten. | Später |
| Arzt/Empfänger | Erhält exportierte Berichte, hat aber keinen direkten Systemzugriff. | Muss als Exportempfänger |
| KI-Dienst | Optionaler späterer externer Dienst. Standardmäßig deaktiviert. | Später |

---

## 6. Produktumfang

### 6.1 Muss-Umfang für V1

V1 muss eine stabile, lokal nutzbare Grundlage liefern:

- lokale Desktop-Anwendung,
- Gesundheitsprofil / Themenverwaltung,
- Wizard zum Anlegen neuer Erkrankungen/Gesundheitsthemen,
- strukturierte Einträge,
- Dokumentenimport mit Anhängen,
- Symptome und Beobachtungen,
- Quellenverwaltung,
- Arztfragen und Terminvorbereitung,
- manuelle Labor- und Vitalwertnotizen,
- Tags und Synonyme,
- Volltextsuche über Metadaten und Notizen,
- Timeline,
- Export als Markdown und später PDF,
- Backup/Restore-Grundlage,
- Audit-/Änderungshistorie für wichtige Objekte,
- Soft Delete/Archivierung,
- klare Sicherheits- und Datenschutz-Hinweise.

### 6.2 Soll-Umfang für frühe Folgeversionen

- OCR für gescannte Dokumente,
- PDF-Vorschau und Annotationslayer,
- Diagramme für Laborwerte/Vitalwerte,
- Medikamentenmodul mit Einnahmenotizen,
- Importordner,
- Dublettenprüfung,
- verschlüsselte Datenablage,
- Vorlagen für häufige Gesundheitsbereiche,
- Report-Generator für Arzttermine,
- Exportpakete mit Dokumentmanifest und Prüfsummen.

### 6.3 Späterer Kann-Umfang

- KI-gestützte Zusammenfassungen,
- FHIR-Import/-Export,
- Apple Health / Health Connect Import,
- CGM-/Diabetesgeräteimport,
- Körperkarte,
- Web Clipper,
- Multi-User/Shared Vault,
- mobile Begleit-App,
- Termin-/Kalenderintegration,
- Zotero-Integration,
- lokale LLM-Unterstützung,
- Regel-/Expertensystem für Quellenprüfung ohne medizinische Empfehlung.

---

## 7. Qualitätsziele

| Qualitätsziel | Bedeutung |
|---|---|
| Vertrauenswürdigkeit | Daten dürfen nicht unbemerkt verloren gehen, verändert werden oder falsch zugeordnet werden. |
| Datenschutz | Gesundheitsdaten bleiben lokal, kontrollierbar und exportierbar. |
| Nachvollziehbarkeit | Einträge, Dokumente, Quellen und Entscheidungen bleiben zeitlich und inhaltlich nachvollziehbar. |
| Bedienbarkeit | Häufige Erfassung soll schnell gehen; komplexe Erfassung soll durch Wizard und Vorlagen geführt werden. |
| Erweiterbarkeit | V1 soll spätere Module wie OCR, KI, FHIR und Diagramme nicht blockieren. |
| Wartbarkeit | Klare Schichten, Tests, Dokumentation und verständliche Datenstrukturen. |
| Medizinische Vorsicht | Keine automatische Diagnose, keine Therapieempfehlung, keine Scheinsicherheit. |
| Exportfähigkeit | Nutzer darf nicht in einem proprietären Datengefängnis landen. |

---

## 8. Systemkontext

### 8.1 Systemgrenzen

Das System besteht in V1 aus:

- einer lokalen Desktop-Anwendung,
- einer lokalen SQLite-Datenbank,
- einer lokalen Dokumentenablage,
- einer lokalen Konfigurationsdatei,
- lokalen Logdateien,
- Exportdateien und Backups.

Nicht Bestandteil von V1:

- Cloud-Backend,
- Benutzerkonto bei SASD,
- automatische Synchronisation,
- mobile App,
- direkter Zugriff durch Ärzte,
- externe KI-Schnittstelle.

### 8.2 Externe Systeme

| Externes System | V1 | Später |
|---|---:|---:|
| Dateisystem | Muss | Muss |
| PDF-Reader / Systemvorschau | Soll | Muss |
| Browser | Manuelle URL-Erfassung | Web Clipper möglich |
| Scanner | Dateiimport | Scan-Workflow/OCR |
| E-Mail | Manuelle Ablage | Import möglich |
| Patientenportale | Manuelle PDF-Exporte | strukturierter Import möglich |
| Apple Health / Health Connect | Nein | optional |
| FHIR-Systeme | Nein | optional |
| KI-Dienste | Nein | optional und deaktiviert |

---

## 9. Fachliches Informationsmodell

### 9.1 Grundprinzip

Alle Informationen werden in einem gemeinsamen Wissensmodell gespeichert. Einträge können miteinander verknüpft werden. Ein Dokument kann mehreren Erkrankungen zugeordnet sein. Eine Quelle kann mehrere Aussagen stützen. Eine Arztfrage kann aus einem Symptom, Laborwert, Dokument oder einer Quelle entstehen.

### 9.2 Zentrale Objekte

| Objekt | Zweck |
|---|---|
| HealthProfile | Lokales Profil des Nutzers; in V1 nur ein Profil. |
| Condition | Erkrankung, Verdacht, Gesundheitsthema oder Problemkomplex. |
| HealthEntry | Allgemeiner Basiseintrag für Notizen, Beobachtungen, Entscheidungen, Hypothesen. |
| SymptomDefinition | Beschreibung eines wiederkehrenden Symptoms. |
| SymptomObservation | Konkretes Auftreten eines Symptoms zu einem Zeitpunkt. |
| HealthDocument | Importierte Datei mit Metadaten, Hash, Typ und Zuordnungen. |
| HealthSource | Quelle: Webseite, Studie, Arztgespräch, Buch, Artikel, Dokument. |
| Measurement | Vitalwert, Laborwert oder sonstige Messung. |
| MedicationNote | Medikamenten- oder Wirkstoffnotiz; keine Therapieempfehlung. |
| TreatmentMeasure | Maßnahme, Ernährung, Bewegung, ärztliche Handlung oder eigene Beobachtungsmaßnahme. |
| Appointment | Arzttermin, Laborbesuch, Untersuchung oder sonstiger medizinischer Kontakt. |
| Question | Offene Frage, meist für Arzttermin, Recherche oder Selbstklärung. |
| DecisionNote | Entscheidung oder verworfene Option mit Begründung. |
| Tag | Freies Schlagwort. |
| Synonym | Such- und Begriffsunterstützung. |
| Relationship | Beziehung zwischen beliebigen Objekten. |
| AuditEvent | Änderungs-, Import-, Export- oder Löschereignis. |
| ExportPackage | Exportierte Zusammenstellung für Arzt, Archiv oder Backup. |

### 9.3 Typische Beziehungen

- Condition hat viele Symptome.
- Condition hat viele Dokumente.
- Condition hat viele Quellen.
- Condition hat viele Fragen.
- Document gehört zu Arzttermin, Laborwert oder Condition.
- Measurement kann zu Dokument und Condition gehören.
- SymptomObservation kann mit Triggern, Maßnahmen und Notizen verbunden sein.
- Question kann aus Quelle, Dokument, Laborwert oder Symptom entstehen.
- DecisionNote referenziert Quellen und Begründungen.

---

## 10. Funktionale Anforderungen – Überblick

Prioritäten:

- **MUSS**: Für V1 erforderlich.
- **SOLL**: Für V1 wünschenswert oder frühe Folgeversion.
- **KANN**: Erweiterung, wenn Aufwand vertretbar ist.
- **SPÄTER**: bewusst nicht für V1.
- **AUSGESCHLOSSEN V1**: bewusst nicht implementieren.

| Bereich | Priorität V1 |
|---|---:|
| Lokale Anwendungsschale | MUSS |
| Gesundheitsprofil | MUSS |
| Condition-/Krankheitsverwaltung | MUSS |
| Krankheits-Wizard | MUSS |
| Dokumentenimport | MUSS |
| Quellenverwaltung | MUSS |
| Symptom- und Beobachtungseinträge | MUSS |
| Arztfragen und Terminvorbereitung | MUSS |
| Suche, Tags, Synonyme | MUSS |
| Timeline | MUSS |
| Export Markdown | MUSS |
| PDF-Export | SOLL |
| Audit-Trail | MUSS |
| Backup/Restore | MUSS |
| Verschlüsselung | SOLL, Architektur MUSS vorbereiten |
| OCR | SPÄTER |
| KI | SPÄTER |
| FHIR | SPÄTER |
| Geräteimport | SPÄTER |

---

## 11. Funktionale Anforderungen im Detail

### 11.1 Anwendungsschale

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-APP-001 | Die Anwendung muss als lokale Desktop-Anwendung starten. | MUSS | Anwendung startet ohne Cloud-Konto und zeigt Startdashboard. |
| PF-APP-002 | Die Anwendung muss eine zentrale Navigation besitzen. | MUSS | Hauptbereiche sind über Seitenleiste oder Menü erreichbar. |
| PF-APP-003 | Die Anwendung muss eine klare Trennung zwischen Navigation, Liste und Detailansicht unterstützen. | MUSS | Nutzer kann links navigieren, mittig suchen/listen, rechts Details bearbeiten. |
| PF-APP-004 | Die Anwendung muss einen sicheren Beenden-/Speichern-Mechanismus haben. | MUSS | Geänderte Daten gehen beim Schließen nicht still verloren. |
| PF-APP-005 | Die Anwendung soll zuletzt geöffnete Ansicht und Fensterposition merken. | SOLL | Neustart zeigt sinnvollen letzten Zustand. |
| PF-APP-006 | Die Anwendung muss eine Info-/Hinweisseite zur Nicht-Diagnose enthalten. | MUSS | Hinweis ist im Programm sichtbar und im Export optional ausgebbar. |

### 11.2 Gesundheitsprofil

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-PRO-001 | Das System muss mindestens ein lokales Gesundheitsprofil verwalten. | MUSS | Profil kann angelegt, angezeigt und bearbeitet werden. |
| PF-PRO-002 | Das Profil darf in V1 keine unnötigen personenbezogenen Pflichtdaten erzwingen. | MUSS | Name, Geburtsdatum etc. sind optional oder bewusst leerbar. |
| PF-PRO-003 | Das Profil soll Stammdaten für Arzt-Exporte enthalten können. | SOLL | Export kann Name/Geburtsdatum optional einfügen. |
| PF-PRO-004 | Das Profil muss sensible Felder als optional behandeln. | MUSS | Keine Zwangserfassung von Geschlecht, Adresse, Diagnosen. |
| PF-PRO-005 | Mehrere Profile sollen später möglich sein, dürfen V1 aber nicht blockieren. | SPÄTER | Datenmodell enthält vorbereitende ProfileId. |

### 11.3 Conditions / Erkrankungen / Gesundheitsthemen

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-CON-001 | Das System muss Erkrankungen und Gesundheitsthemen als `Condition` verwalten. | MUSS | Nutzer kann Condition anlegen, ändern, archivieren. |
| PF-CON-002 | Eine Condition muss Name, Kurzbeschreibung, Status und Startdatum unterstützen. | MUSS | Felder sind speicherbar und suchbar. |
| PF-CON-003 | Eine Condition muss Synonyme und alternative Begriffe unterstützen. | MUSS | Suche nach Synonym findet Condition. |
| PF-CON-004 | Eine Condition muss Tags besitzen können. | MUSS | Tags sind filterbar. |
| PF-CON-005 | Eine Condition muss mehreren Kategorien/Organsystemen zugeordnet werden können. | SOLL | z. B. Stoffwechsel, Herz/Kreislauf, Mund/Zunge. |
| PF-CON-006 | Eine Condition muss mit Dokumenten, Symptomen, Quellen, Fragen, Terminen und Messwerten verknüpfbar sein. | MUSS | Detailansicht zeigt verknüpfte Objekte. |
| PF-CON-007 | Eine Condition darf gelöscht werden nur über Archiv/Papierkorb, nicht still hart. | MUSS | Archivierte Conditions verschwinden aus Standardlisten, bleiben wiederherstellbar. |
| PF-CON-008 | Das System soll Verdachtsdiagnosen und gesicherte Diagnosen unterscheiden. | SOLL | Statusfeld enthält z. B. Verdacht, ärztlich bestätigt, verworfen. |
| PF-CON-009 | Das System muss eigene Notizen klar von ärztlich bestätigten Informationen trennen. | MUSS | Eintragstyp/Quelle macht Herkunft sichtbar. |

### 11.4 Krankheits-Wizard / Condition-Wizard

Der Wizard ist eine Kernfunktion. Er soll das strukturierte Anlegen eines neuen Gesundheitsthemas ermöglichen und sofort relevante Informationen zuordnen.

#### 11.4.1 Grundverhalten

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-WIZ-001 | Das System muss einen Wizard zum Anlegen neuer Conditions bereitstellen. | MUSS | Button „Neue Erkrankung / neues Gesundheitsthema“ öffnet Wizard. |
| PF-WIZ-002 | Der Wizard muss schrittweise funktionieren und Zwischenspeichern erlauben. | MUSS | Nutzer kann abbrechen und als Entwurf fortsetzen. |
| PF-WIZ-003 | Der Wizard muss Schritte überspringen können. | MUSS | Kein Schritt außer Basisdaten blockiert unnötig. |
| PF-WIZ-004 | Der Wizard muss vor dem Speichern eine Zusammenfassung anzeigen. | MUSS | Nutzer sieht alle zu erstellenden Objekte. |
| PF-WIZ-005 | Der Wizard muss alle erfassten Objekte korrekt mit der neuen Condition verknüpfen. | MUSS | Nach Abschluss sind Symptome, Dokumente, Quellen usw. in der Detailansicht sichtbar. |
| PF-WIZ-006 | Der Wizard soll Vorlagen für typische Themen bereitstellen. | SOLL | Vorlagen erzeugen sinnvolle Startfelder, aber keine medizinische Bewertung. |
| PF-WIZ-007 | Der Wizard muss deutlich machen, dass keine Diagnose gestellt wird. | MUSS | Hinweis im Start- oder Zusammenfassungsschritt. |

#### 11.4.2 Wizard-Schritte

| Schritt | Name | Zweck | Priorität |
|---:|---|---|---:|
| 1 | Start und Zweck | Erklärung, dass ein Thema dokumentiert wird, keine Diagnose. | MUSS |
| 2 | Basisdaten | Name, Synonyme, Status, Kategorie, Startdatum, Beschreibung. | MUSS |
| 3 | Symptome | Wiederkehrende Symptome definieren und erste Beobachtungen erfassen. | MUSS |
| 4 | Dokumente | PDFs, Bilder, Arztbriefe, Laborberichte zuordnen. | MUSS |
| 5 | Quellen & Informationen | Webseiten, Studien, Gesprächsnotizen, Bücher, Artikel erfassen. | MUSS |
| 6 | Messwerte & Labor | vorhandene Werte als strukturierte Notiz erfassen. | SOLL |
| 7 | Medikamente & Maßnahmen | aktuelle oder historische Medikamente/Maßnahmen dokumentieren. | SOLL |
| 8 | Arztkontakte | beteiligte Ärzte, Kliniken, Labore, Termine erfassen. | SOLL |
| 9 | Offene Fragen | Fragen für Arzt, Recherche oder spätere Klärung sammeln. | MUSS |
| 10 | Risiken/Datenschutz | sensible Inhalte markieren, Exporthinweise setzen. | SOLL |
| 11 | Zusammenfassung | Prüfen, speichern, Entwurf oder vollständige Anlage. | MUSS |

#### 11.4.3 Wizard-Basisdaten

| Feld | Typ | Pflicht | Beschreibung |
|---|---|---:|---|
| Name | Text | Ja | z. B. „Diabetes“, „Zungenbrennen“, „Gefäßthema“. |
| Alternativbegriffe | Liste | Nein | Synonyme, medizinische Begriffe, Schreibvarianten. |
| Status | Auswahl | Ja | Beobachtung, Verdacht, ärztlich diagnostiziert, verworfen, chronisch, abgeklärt. |
| Startdatum | Datum/unsicher | Nein | Exaktes oder ungefähres Datum. |
| Kategorie | Mehrfachauswahl | Nein | Stoffwechsel, Herz/Kreislauf, Neurologie, Mund/Zähne, Orthopädie usw. |
| Kurzbeschreibung | Markdown/Text | Nein | Freie Beschreibung. |
| Sichtbarkeit/Sensibilität | Auswahl | Nein | normal, sensibel, sehr sensibel. |

#### 11.4.4 Wizard-Symptome

Der Nutzer soll direkt typische Symptome anlegen können:

- Symptomname,
- Beschreibung,
- Schweregrad-Skala,
- Häufigkeit,
- Dauer,
- typische Trigger,
- lindernde Faktoren,
- Begleitsymptome,
- erste Beobachtung,
- Notiz.

Der Wizard darf keine medizinische Interpretation anbieten. Er darf lediglich strukturieren.

#### 11.4.5 Wizard-Dokumente

Der Nutzer soll direkt Dateien hinzufügen können:

- Drag & Drop,
- Dateiauswahl,
- Dokumenttyp,
- Dokumentdatum,
- Korrespondent,
- kurze Inhaltsnotiz,
- Tags,
- Sensibilitätsstufe,
- Zuordnung zu Condition,
- Prüfsumme/Hash im Hintergrund.

#### 11.4.6 Wizard-Quellen

Der Nutzer soll direkt Informationen erfassen können:

- URL,
- Titel,
- Quelle/Autor/Organisation,
- Abrufdatum,
- Quellentyp,
- eigene Zusammenfassung,
- markierte Aussagen,
- Zuverlässigkeitseinschätzung,
- verknüpfte Fragen.

#### 11.4.7 Wizard-Fragen

Der Wizard muss eine Liste offener Fragen erzeugen können:

- Frage,
- Kategorie,
- Priorität,
- für wen gedacht,
- Bezug zu Symptom/Dokument/Quelle,
- gewünschter Termin,
- Status offen/geklärt/verschoben.

#### 11.4.8 Wizard-Ergebnis

Nach Abschluss erzeugt der Wizard ein **Condition-Paket**:

- Condition,
- erste Symptome,
- erste Beobachtungen,
- verknüpfte Dokumente,
- verknüpfte Quellen,
- offene Fragen,
- optionale Messwerte,
- optionale Medikamenten-/Maßnahmen-Notizen,
- Timeline-Ereignis „Condition angelegt“,
- Audit-Eintrag,
- optionaler Erstbericht als Markdown.

### 11.5 Einträge / Health Entries

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-ENT-001 | Das System muss einen allgemeinen Eintragstyp `HealthEntry` besitzen. | MUSS | Einträge können angelegt, gesucht, bearbeitet, archiviert werden. |
| PF-ENT-002 | Einträge müssen Typen unterstützen. | MUSS | Typen: Notiz, Beobachtung, Hypothese, Entscheidung, Arztgespräch, Befundnotiz, Quelle, Frage. |
| PF-ENT-003 | Einträge müssen Datum/Zeit, Titel, Text, Status, Tags und Zuordnungen besitzen. | MUSS | Felder sind sichtbar und filterbar. |
| PF-ENT-004 | Einträge sollen Markdown-Formatierung erlauben. | SOLL | Listen, Überschriften und Links funktionieren. |
| PF-ENT-005 | Eigene Hypothesen müssen als Hypothese markierbar sein. | MUSS | UI zeigt deutlich „Eigene Annahme“. |
| PF-ENT-006 | Entscheidungen müssen Begründung und Quellen enthalten können. | SOLL | Entscheidungseintrag zeigt „Warum“ und „auf Basis von“. |
| PF-ENT-007 | Änderungen an wichtigen Einträgen müssen auditierbar sein. | MUSS | Auditlog enthält vorher/nachher oder Änderungsnotiz. |

### 11.6 Dokumentenverwaltung

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-DOC-001 | Das System muss Dokumente importieren können. | MUSS | PDF/Bild/sonstige Datei kann gespeichert und verknüpft werden. |
| PF-DOC-002 | Originaldateien müssen unverändert bleiben. | MUSS | Metadaten und Notizen werden separat gespeichert. |
| PF-DOC-003 | Für importierte Dokumente muss eine Prüfsumme erzeugt werden. | MUSS | Hash ist in Metadaten sichtbar oder intern prüfbar. |
| PF-DOC-004 | Dokumente müssen Typ, Datum, Korrespondent, Tags und Zuordnungen besitzen. | MUSS | Dokumentliste ist danach filterbar. |
| PF-DOC-005 | Dokumente müssen mehrfach verknüpft werden können. | MUSS | Ein Laborbericht kann mehreren Conditions zugeordnet sein. |
| PF-DOC-006 | Dokumente dürfen nicht still hart gelöscht werden. | MUSS | Archiv/Papierkorb vorhanden. |
| PF-DOC-007 | Das System soll Dubletten anhand Hash erkennen. | SOLL | Import gleicher Datei zeigt Warnung. |
| PF-DOC-008 | Das System soll Dokumentvorschau unterstützen. | SOLL | PDF/Bild wird im Detailbereich oder extern geöffnet. |
| PF-DOC-009 | OCR soll später Scans durchsuchbar machen. | SPÄTER | OCR-Text wird getrennt vom Original gespeichert. |
| PF-DOC-010 | Annotationslayer soll später Markierungen speichern. | SPÄTER | Original bleibt unverändert. |

### 11.7 Quellen- und Evidenzverwaltung

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-SRC-001 | Das System muss Quellen verwalten können. | MUSS | Quelle kann mit URL, Titel, Typ, Datum und Notiz gespeichert werden. |
| PF-SRC-002 | Quellen müssen Aussagen/Notizen enthalten können. | MUSS | Nutzer kann wichtige Aussage paraphrasieren und verknüpfen. |
| PF-SRC-003 | Quellen müssen mit Conditions, Fragen und Entscheidungen verknüpft werden können. | MUSS | Quelle erscheint in den verknüpften Detailansichten. |
| PF-SRC-004 | Quellen sollen nach Typ klassifiziert werden. | SOLL | Studie, Leitlinie, Arztgespräch, Webseite, Buch, Herstellerinfo usw. |
| PF-SRC-005 | Quellen sollen eine einfache Vertrauenseinschätzung erhalten können. | SOLL | Nutzer markiert z. B. niedrig/mittel/hoch/eigene Einschätzung. |
| PF-SRC-006 | Das System darf Quellen nicht automatisch als medizinisch korrekt bewerten. | MUSS | Keine automatische Wahrheitseinstufung in V1. |
| PF-SRC-007 | Zotero-/DOI-/PubMed-Integration ist vorzubereiten, aber nicht V1-Pflicht. | SPÄTER | Datenmodell blockiert DOI/PMID nicht. |

### 11.8 Symptome, Verlauf und Alltag

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-SYM-001 | Das System muss Symptome definieren können. | MUSS | Symptomkatalog pro Profil/Condition vorhanden. |
| PF-SYM-002 | Das System muss konkrete Symptom-Beobachtungen erfassen können. | MUSS | Symptom, Datum/Zeit, Stärke und Notiz speicherbar. |
| PF-SYM-003 | Symptome müssen mit Conditions verknüpfbar sein. | MUSS | Condition zeigt Symptomverlauf. |
| PF-SYM-004 | Schweregrad-Skalen müssen konfigurierbar sein. | SOLL | 0–10, leicht/mittel/stark oder frei. |
| PF-SYM-005 | Trigger und lindernde Faktoren sollen dokumentierbar sein. | SOLL | Auswahl/Freitext verfügbar. |
| PF-SYM-006 | Tagesjournal soll mehrere Symptome und Kontext bündeln. | SOLL | Ein Tag kann Befinden, Schlaf, Ernährung, Besonderheiten enthalten. |
| PF-SYM-007 | Das System darf aus Symptomen keine Diagnose ableiten. | MUSS | Keine Diagnosevorschläge. |
| PF-SYM-008 | Körperkarte ist spätere Erweiterung. | SPÄTER | Datenmodell kann Lokalisation als Freitext speichern. |

### 11.9 Messwerte, Laborwerte und Vitalwerte

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-MEA-001 | Das System muss manuelle Messwerte erfassen können. | SOLL | Wert, Einheit, Datum, Quelle, Notiz speicherbar. |
| PF-MEA-002 | Laborwerte müssen Referenzbereich und Labor/Dokument referenzieren können. | SOLL | Referenzbereich wird als Quelle erfasst, nicht automatisch bewertet. |
| PF-MEA-003 | Vitalwerte wie Blutdruck, Puls, Gewicht, Blutzucker sollen unterstützt werden. | SOLL | Standardtypen vorhanden oder frei definierbar. |
| PF-MEA-004 | Messwerte müssen mit Conditions und Dokumenten verknüpfbar sein. | SOLL | Laborwert zeigt Ursprungsdokument. |
| PF-MEA-005 | Zeitreihendiagramme sollen später verfügbar sein. | SPÄTER/SOLL | Werte können als Tabelle exportiert werden; Diagramm später. |
| PF-MEA-006 | Automatische Grenzwertinterpretation ist in V1 ausgeschlossen. | AUSGESCHLOSSEN V1 | Keine Ampel „gefährlich/normal“ ohne ärztliche Konfiguration. |
| PF-MEA-007 | Geräteimport ist später möglich, aber nicht V1. | SPÄTER | Import-Schnittstellen vorbereitet. |

### 11.10 Medikamente und Maßnahmen

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-MED-001 | Das System muss Medikamenten-Notizen erfassen können. | SOLL | Name, Wirkstoff, Dosierungsnotiz, Zeitraum, Quelle, Notiz. |
| PF-MED-002 | Medikamente müssen mit Conditions, Symptomen und Arztterminen verknüpfbar sein. | SOLL | Detailansicht zeigt Zusammenhänge. |
| PF-MED-003 | Änderungen müssen als Verlauf dokumentiert werden können. | SOLL | Änderung erzeugt Timeline-Ereignis. |
| PF-MED-004 | Maßnahmen wie Ernährung, Bewegung, Schlaf, Stressreduktion sollen dokumentierbar sein. | SOLL | Maßnahmenlog verfügbar. |
| PF-MED-005 | Nebenwirkungsnotizen sollen möglich sein. | SOLL | Symptom kann mit Medikament/Maßnahme verknüpft werden. |
| PF-MED-006 | Das System darf Medikamente nicht empfehlen, absetzen oder ändern. | MUSS | UI enthält keine Therapieanweisung. |
| PF-MED-007 | Wechselwirkungsprüfung ist in V1 ausgeschlossen. | AUSGESCHLOSSEN V1 | Keine automatischen Interaktionswarnungen. |
| PF-MED-008 | Einnahme-Reminder sind spätere Erweiterung. | SPÄTER | Kein Alarm-/Medikationsmanagement in V1. |

### 11.11 Arzttermine, Fragen und Kommunikation

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-APT-001 | Das System muss Arzttermine oder medizinische Kontakte erfassen können. | MUSS | Termin mit Datum, Arzt/Organisation, Thema, Notizen speicherbar. |
| PF-APT-002 | Fragen müssen unabhängig gesammelt werden können. | MUSS | Frageeingang vorhanden. |
| PF-APT-003 | Fragen müssen Terminen und Conditions zugeordnet werden können. | MUSS | Terminansicht zeigt offene Fragen. |
| PF-APT-004 | Nach dem Termin müssen Gesprächsnotizen erfassbar sein. | MUSS | Termin kann Ergebnis, Antworten, neue Aufgaben enthalten. |
| PF-APT-005 | Aus einem Termin muss eine Arztmappe exportierbar sein. | SOLL | Export enthält Zusammenfassung, Fragen, ausgewählte Dokumente. |
| PF-APT-006 | Das System soll Aufgaben/Follow-ups nach Arzttermin erzeugen können. | SOLL | Aufgabe/Frage mit Fälligkeitsdatum. |
| PF-APT-007 | Nachrichten an Ärzte werden in V1 nicht direkt versendet. | AUSGESCHLOSSEN V1 | Exportdatei statt direkter Kommunikation. |

### 11.12 Suche, Filter, Tags und Synonyme

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-SCH-001 | Das System muss eine globale Suche besitzen. | MUSS | Suche findet Conditions, Einträge, Dokumente, Quellen, Fragen. |
| PF-SCH-002 | Suche muss Tags und Synonyme berücksichtigen. | MUSS | Synonym-Suche findet alternative Begriffe. |
| PF-SCH-003 | Suche muss nach Typ, Datum, Status und Priorität filterbar sein. | MUSS | Filterpanel vorhanden. |
| PF-SCH-004 | Volltextsuche über Notizen muss verfügbar sein. | MUSS | Text in Einträgen ist durchsuchbar. |
| PF-SCH-005 | Volltextsuche über PDF/OCR-Text ist später. | SPÄTER | OCR-Text-Index vorbereitbar. |
| PF-SCH-006 | Suchergebnisse müssen nachvollziehbar anzeigen, warum sie gefunden wurden. | SOLL | Trefferstelle oder Feld wird angezeigt. |

### 11.13 Timeline und Verlauf

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-TIM-001 | Das System muss eine Timeline anzeigen. | MUSS | Ereignisse chronologisch sortierbar. |
| PF-TIM-002 | Timeline muss nach Condition, Typ und Zeitraum filterbar sein. | MUSS | Nutzer sieht Verlauf eines Gesundheitsthemas. |
| PF-TIM-003 | Timeline muss manuelle und automatisch erzeugte Ereignisse enthalten können. | MUSS | Import, Termin, Symptom, Dokument, Entscheidung erscheinen. |
| PF-TIM-004 | Ungenaue Daten müssen möglich sein. | SOLL | „ca. März 2026“ oder nur Jahr/Monat später möglich. |
| PF-TIM-005 | Timeline darf keine Kausalität behaupten. | MUSS | Zusammenhang wird nur als Verknüpfung/Notiz dargestellt. |

### 11.14 Berichte, Export und Ausdruck

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-EXP-001 | Das System muss Markdown-Export unterstützen. | MUSS | Condition-Bericht als `.md` exportierbar. |
| PF-EXP-002 | Das System soll PDF-Export unterstützen. | SOLL | PDF enthält Inhaltsverzeichnis und Metadaten. |
| PF-EXP-003 | Das System muss Arztfragenliste exportieren können. | MUSS | Offene Fragen nach Termin/Thema exportierbar. |
| PF-EXP-004 | Das System soll Arztmappe exportieren können. | SOLL | Zusammenfassung + ausgewählte Dokumentliste + Fragen. |
| PF-EXP-005 | Export muss bewusst ausgewählte Inhalte verwenden. | MUSS | Nutzer wählt, was enthalten ist. |
| PF-EXP-006 | Export muss sensible Inhalte kennzeichnen oder ausschließen können. | MUSS | Filter „sensible Dokumente ausschließen“ möglich. |
| PF-EXP-007 | Exportpaket soll Manifest und Prüfsummen enthalten. | SOLL | ZIP mit Manifest später möglich. |
| PF-EXP-008 | Rohdatenexport muss in offenem Format möglich sein. | MUSS | JSON/CSV/Markdown-Export für Datenportabilität. |

### 11.15 Audit-Trail, Versionierung und Löschkonzept

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-AUD-001 | Das System muss wichtige Änderungen protokollieren. | MUSS | Auditlog zeigt Objekt, Aktion, Zeit, Benutzer/System. |
| PF-AUD-002 | Dokumentoriginale dürfen nicht verändert werden. | MUSS | Hash bleibt stabil. |
| PF-AUD-003 | Löschungen müssen zunächst Soft Delete/Archiv sein. | MUSS | Wiederherstellung möglich. |
| PF-AUD-004 | Wichtige Einträge sollen versioniert werden. | SOLL | Vorherige Fassung wieder einsehbar. |
| PF-AUD-005 | Import- und Exportvorgänge müssen protokolliert werden. | MUSS | Exporthistorie zeigt Datum und Inhaltstyp. |
| PF-AUD-006 | Endgültiges Löschen muss gesondert bestätigt werden. | SOLL | Warnung und Backup-Hinweis. |

### 11.16 Backup, Restore und Datenportabilität

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-BAC-001 | Das System muss lokale Backups erstellen können. | MUSS | Backup enthält Datenbank, Dokumente, Manifest. |
| PF-BAC-002 | Restore muss getestet und dokumentiert sein. | MUSS | Testfall stellt Backup in frischem Profil wieder her. |
| PF-BAC-003 | Backups sollen verschlüsselbar sein. | SOLL | Passwortgeschütztes Backup später/früh. |
| PF-BAC-004 | Backup darf Originaldokumente nicht verlieren. | MUSS | Dokumenthashes nach Restore identisch. |
| PF-BAC-005 | Export in offene Formate muss möglich sein. | MUSS | Nutzer kann Daten ohne App weiterverwenden. |
| PF-BAC-006 | Datenbankmigrationen müssen versioniert sein. | MUSS | SchemaVersion vorhanden. |

### 11.17 Sicherheit und Datenschutz

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-SEC-001 | Die Anwendung muss lokal ohne Konto funktionieren. | MUSS | Kein Login zu externem Dienst notwendig. |
| PF-SEC-002 | Es darf keine Telemetrie ohne ausdrückliche Zustimmung geben. | MUSS | Standard: keine Telemetrie. |
| PF-SEC-003 | Externe Datenübertragung muss standardmäßig deaktiviert sein. | MUSS | Keine KI/Cloud per Default. |
| PF-SEC-004 | Sensible Inhalte müssen markierbar sein. | MUSS | Objekt hat Sensibilitätsstufe. |
| PF-SEC-005 | Verschlüsselung muss architektonisch vorbereitet werden. | MUSS | Storage-Schicht kapselt Datei-/DB-Zugriff. |
| PF-SEC-006 | Verschlüsselung soll früh implementiert werden. | SOLL | Datenbank/Dokumente/Backup geschützt. |
| PF-SEC-007 | Das System muss klare Hinweise zur Verantwortung und Datensicherung enthalten. | MUSS | Hilfeseite/Startdialog vorhanden. |
| PF-SEC-008 | Logdateien dürfen keine sensiblen Inhalte enthalten. | MUSS | Logs enthalten IDs/Fehler, nicht medizinische Inhalte. |
| PF-SEC-009 | Kein Recovery-Backdoor. | MUSS | Vergessenes Passwort kann nicht heimlich umgangen werden. |

### 11.18 Einstellungen und Administration

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-SET-001 | Speicherorte müssen einsehbar sein. | MUSS | Einstellungen zeigen DB-, Dokument- und Backup-Pfad. |
| PF-SET-002 | Nutzer muss Export-/Backup-Pfade konfigurieren können. | SOLL | Pfadauswahl verfügbar. |
| PF-SET-003 | Nutzer muss Kategorien, Tags und Synonyme verwalten können. | MUSS | Verwaltungsdialog vorhanden. |
| PF-SET-004 | Nutzer soll Vorlagen verwalten können. | SOLL | Condition- und Reportvorlagen bearbeitbar. |
| PF-SET-005 | System muss Diagnose-/Debug-Informationen exportieren können, ohne Gesundheitsdaten preiszugeben. | SOLL | Supportpaket enthält technische Infos, keine sensiblen Inhalte. |

### 11.19 KI-Funktionen – spätere Erweiterung

KI-Funktionen sind ausdrücklich nicht Teil von V1. Sie dürfen später nur optional, transparent, deaktivierbar und mit klarer Quellen-/Unsicherheitskennzeichnung eingeführt werden.

| ID | Anforderung | Priorität | Akzeptanzkriterium |
|---|---|---:|---|
| PF-AI-001 | KI muss standardmäßig deaktiviert sein. | SPÄTER | Kein KI-Aufruf ohne Opt-in. |
| PF-AI-002 | KI darf keine Diagnose oder Therapieentscheidung als Fakt darstellen. | SPÄTER | Ausgabe enthält Unsicherheits- und Quellenhinweis. |
| PF-AI-003 | KI-Zusammenfassungen müssen Quellen referenzieren. | SPÄTER | Zusammenfassung zeigt Dokumente/Quellenbasis. |
| PF-AI-004 | Lokale KI soll bevorzugt werden, wenn sensible Daten verarbeitet werden. | SPÄTER | Lokaler Provider möglich. |
| PF-AI-005 | KI-Ausgaben müssen als Entwurf gespeichert werden. | SPÄTER | Nutzer bestätigt Übernahme. |

---

## 12. Datenmodell – Entwurf

### 12.1 Gemeinsame Felder

Fast alle Hauptobjekte sollen diese Felder besitzen:

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | GUID/ULID | stabile Objekt-ID |
| ProfileId | GUID/ULID | vorbereitet für mehrere Profile |
| Title/Name | Text | Anzeigename |
| Description/Notes | Markdown/Text | Beschreibung |
| CreatedAt | DateTime | Erstellzeitpunkt |
| UpdatedAt | DateTime | letzter Änderungszeitpunkt |
| ArchivedAt | DateTime? | Archivierungszeitpunkt |
| IsArchived | Bool | Soft Delete/Archiv |
| Sensitivity | Enum | normal, sensibel, sehr sensibel |
| SourceReliability | optional Enum | eigene Einschätzung bei Quellen |
| Tags | Relation | viele Tags |

### 12.2 Tabelle `Conditions`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| ProfileId | Guid | Profilbezug |
| Name | Text | Name des Gesundheitsthemas |
| ShortName | Text? | Kurzname |
| Description | Text | Beschreibung |
| Status | Enum | Beobachtung, Verdacht, diagnostiziert, abgeklärt, verworfen, chronisch |
| StartDate | Date? | Beginn, falls bekannt |
| StartDatePrecision | Enum | Tag, Monat, Jahr, ungefähr, unbekannt |
| Category | Text/Enum | Hauptkategorie |
| Sensitivity | Enum | Sensibilitätsstufe |
| CreatedByWizard | Bool | wurde per Wizard angelegt |
| CreatedAt/UpdatedAt/ArchivedAt | DateTime | Auditbasis |

### 12.3 Tabelle `ConditionSynonyms`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| ConditionId | Guid | Bezug zur Condition |
| Term | Text | Synonym oder medizinischer Begriff |
| Language | Text? | Sprache |
| Note | Text? | Hinweis |

### 12.4 Tabelle `HealthEntries`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| ProfileId | Guid | Profil |
| EntryType | Enum | Notiz, Beobachtung, Hypothese, Entscheidung, Gespräch, Frage usw. |
| Title | Text | Titel |
| BodyMarkdown | Text | Inhalt |
| EventDate | DateTime? | medizinisch/fachlich relevantes Datum |
| EventDatePrecision | Enum | Genauigkeit |
| Status | Enum | offen, in Prüfung, bestätigt, verworfen, erledigt |
| Priority | Enum | niedrig, normal, wichtig, kritisch |
| Sensitivity | Enum | Sensibilität |

### 12.5 Tabelle `Documents`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| OriginalFileName | Text | Ursprünglicher Dateiname |
| StoredFileName | Text | interner Dateiname |
| RelativePath | Text | Speicherort relativ zum Dokumentenroot |
| MimeType | Text | Dateityp |
| FileSize | Long | Größe |
| Sha256 | Text | Prüfsumme |
| DocumentType | Enum | Arztbrief, Laborbericht, Studie, Rezept, Foto usw. |
| DocumentDate | Date? | Datum des Dokuments |
| ImportDate | DateTime | Importzeitpunkt |
| CorrespondentId | Guid? | Arzt/Klinik/Labor |
| OcrText | Text? | späterer OCR-Text |
| Sensitivity | Enum | Sensibilität |
| Status | Enum | neu, geprüft, zugeordnet, archiviert |

### 12.6 Tabelle `Sources`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| SourceType | Enum | Webseite, Studie, Buch, Arztgespräch, Leitlinie, Dokument, eigenes Erlebnis |
| Title | Text | Titel |
| Url | Text? | URL |
| AuthorOrOrganization | Text? | Autor/Organisation |
| AccessedAt | Date? | Abrufdatum |
| PublishedAt | Date? | Publikationsdatum |
| Citation | Text? | Zitierhinweis |
| Summary | Text | eigene Zusammenfassung |
| Reliability | Enum? | eigene Einschätzung |
| Notes | Text | Notizen |

### 12.7 Tabelle `Symptoms` und `SymptomObservations`

`Symptoms` definiert wiederkehrende Symptome. `SymptomObservations` erfasst konkrete Vorkommen.

| Feld | Objekt | Beschreibung |
|---|---|---|
| Name | Symptom | Symptomname |
| Description | Symptom | Beschreibung |
| DefaultScale | Symptom | 0–10, Textskala, frei |
| ObservedAt | Observation | Zeitpunkt |
| Severity | Observation | Stärke |
| Duration | Observation | Dauer |
| Location | Observation | Körperstelle/Freitext |
| Triggers | Observation | mögliche Auslöser als Freitext/Tags |
| RelievingFactors | Observation | lindernde Faktoren |
| Note | Observation | Notiz |

### 12.8 Tabelle `Measurements`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| MeasurementType | Enum/Text | Blutdruck, Glukose, Laborwert, Gewicht usw. |
| Name | Text | z. B. HbA1c, LDL, Blutdruck systolisch |
| Value | Decimal/Text | Wert, auch Text möglich |
| Unit | Text | Einheit |
| ReferenceLow | Decimal? | Unterer Referenzwert |
| ReferenceHigh | Decimal? | Oberer Referenzwert |
| MeasuredAt | DateTime | Messdatum |
| SourceDocumentId | Guid? | Ursprungsdokument |
| LabOrDevice | Text? | Labor/Gerät |
| Note | Text | Notiz |

### 12.9 Tabelle `Questions`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| QuestionText | Text | Frage |
| Context | Text | Hintergrund |
| Priority | Enum | niedrig, normal, wichtig, kritisch |
| Status | Enum | offen, geplant, gefragt, beantwortet, verworfen |
| TargetPersonId | Guid? | Arzt/Organisation |
| AppointmentId | Guid? | geplanter Termin |
| Answer | Text? | Antwort nach Termin |
| AnswerSource | Text? | Quelle der Antwort |

### 12.10 Tabelle `Appointments`

| Feld | Typ | Beschreibung |
|---|---|---|
| Id | Guid | Primärschlüssel |
| StartsAt | DateTime | Terminbeginn |
| EndsAt | DateTime? | Terminende |
| PersonOrOrganizationId | Guid? | Arzt/Klinik/Labor |
| Reason | Text | Anlass |
| PreparationNotes | Text | Vorbereitung |
| ResultNotes | Text | Ergebnis |
| FollowUpNotes | Text | Nacharbeit |

### 12.11 Beziehungstabellen

Da fast alles verknüpfbar sein soll, werden Beziehungstabellen benötigt:

- ConditionDocuments
- ConditionSources
- ConditionSymptoms
- ConditionMeasurements
- ConditionMedications
- ConditionQuestions
- ConditionAppointments
- EntryDocuments
- EntrySources
- EntryQuestions
- ObjectTags
- ObjectRelationships

Die generische Tabelle `ObjectRelationships` kann später komplexe Beziehungen abbilden:

| Feld | Beschreibung |
|---|---|
| SourceObjectType | Typ des Ausgangsobjekts |
| SourceObjectId | ID Ausgangsobjekt |
| TargetObjectType | Typ des Zielobjekts |
| TargetObjectId | ID Zielobjekt |
| RelationshipType | erklärt, gehört zu, widerspricht, bestätigt, ausgelöst durch, dokumentiert |
| Note | optionale Beziehungsnotiz |

---

## 13. Benutzeroberfläche

### 13.1 Grundlayout

Die Anwendung soll ein ruhiges, technisches, gut lesbares Desktop-Layout verwenden.

Empfohlene Hauptbereiche:

1. **Linke Navigation**  
   Dashboard, Erkrankungen, Dokumente, Symptome, Messwerte, Quellen, Fragen, Termine, Timeline, Suche, Exporte, Einstellungen.

2. **Mittlere Listen-/Suchansicht**  
   Suchfeld, Filter, Ergebnisliste, Sortierung, Status.

3. **Rechte Detailansicht**  
   Detaildaten, Verknüpfungen, Notizen, Aktionen, Verlauf.

4. **Untere Statusleiste**  
   Speicherstatus, Backup-Hinweis, aktuelle Datenbank, letzte Änderung.

### 13.2 Dashboard

Das Dashboard soll zeigen:

- offene Fragen,
- nächste Termine,
- zuletzt importierte Dokumente,
- zuletzt bearbeitete Conditions,
- neue unzugeordnete Dokumente,
- ausstehende Follow-ups,
- kurze Sicherheits-/Backup-Hinweise,
- optional Tagesjournal-Schnellerfassung.

### 13.3 Condition-Detailansicht

Eine Condition-Detailansicht soll Register oder Abschnitte enthalten:

- Übersicht,
- Timeline,
- Symptome,
- Dokumente,
- Quellen,
- Messwerte,
- Medikamente/Maßnahmen,
- Fragen,
- Termine,
- Entscheidungen/Notizen,
- Export.

### 13.4 Kontextmenüs und Doppelklick

Die Anwendung soll erwartbare Desktop-Interaktionen unterstützen:

- Doppelklick auf Condition öffnet Detailansicht.
- Doppelklick auf Dokument öffnet Vorschau/externes Programm.
- Rechtsklick auf Eintrag zeigt Aktionen: bearbeiten, verknüpfen, exportieren, archivieren.
- Drag & Drop von Dokumenten in Condition ist möglich oder vorbereitet.

### 13.5 Wizard-UX

Der Wizard soll besonders sorgfältig gestaltet sein:

- linke Schrittübersicht,
- klare Fortschrittsanzeige,
- ruhige Sprache,
- kein medizinischer Alarmismus,
- Zwischenspeichern,
- Entwurf fortsetzen,
- zusammenfassender Abschluss,
- erzeugte Objekte nachvollziehbar anzeigen.

---

## 14. Workflows

### 14.1 Neue Erkrankung / neues Gesundheitsthema anlegen

1. Nutzer klickt „Neue Erkrankung / neues Gesundheitsthema“.
2. Wizard öffnet Startseite mit Hinweis zur Nicht-Diagnose.
3. Nutzer erfasst Basisdaten.
4. Nutzer ergänzt Symptome.
5. Nutzer importiert vorhandene Dokumente.
6. Nutzer ergänzt Quellen/Informationen.
7. Nutzer erfasst offene Fragen.
8. Nutzer prüft Zusammenfassung.
9. System erzeugt Condition-Paket.
10. Detailansicht öffnet sich.
11. Auditlog dokumentiert Anlage.

### 14.2 Arzttermin vorbereiten

1. Nutzer legt Termin an oder öffnet bestehenden Termin.
2. Nutzer ordnet Conditions zu.
3. System zeigt offene Fragen zu diesen Conditions.
4. Nutzer wählt relevante Dokumente und Messwerte aus.
5. Nutzer erzeugt Arztfragenliste oder Arztmappe.
6. Nach dem Termin ergänzt Nutzer Antworten und Folgeaufgaben.

### 14.3 Dokument importieren

1. Nutzer zieht Datei in Anwendung oder wählt Datei aus.
2. System berechnet Hash.
3. System prüft Dublette, falls Funktion verfügbar.
4. Nutzer setzt Typ, Datum, Korrespondent, Tags.
5. Nutzer ordnet Dokument Conditions/Terminen/Einträgen zu.
6. System speichert Original unverändert.
7. System erzeugt Audit-Eintrag.

### 14.4 Symptom erfassen

1. Nutzer wählt Symptom oder legt neues an.
2. Nutzer erfasst Zeitpunkt, Stärke, Dauer, Kontext.
3. Nutzer verknüpft Condition und ggf. Trigger/Maßnahme.
4. System speichert Beobachtung.
5. Timeline aktualisiert sich.

### 14.5 Quelle erfassen

1. Nutzer erfasst URL, Titel, Typ, Abrufdatum.
2. Nutzer notiert Kernaussage in eigenen Worten.
3. Nutzer bewertet Quelle optional nach eigener Einschätzung.
4. Nutzer verknüpft Quelle mit Condition, Frage oder Entscheidung.
5. Quelle wird suchbar.

### 14.6 Export erzeugen

1. Nutzer wählt Exporttyp: Condition-Bericht, Arztfragenliste, Arztmappe, Rohdaten.
2. Nutzer wählt Zeitraum und Inhalte.
3. Nutzer prüft sensible Inhalte.
4. System erzeugt Markdown/PDF/ZIP.
5. Export wird protokolliert.

---

## 15. Nichtfunktionale Anforderungen

### 15.1 Sicherheit

| ID | Anforderung | Priorität |
|---|---|---:|
| NF-SEC-001 | Standardmäßig lokale Datenhaltung. | MUSS |
| NF-SEC-002 | Keine externe Übertragung ohne aktive Zustimmung. | MUSS |
| NF-SEC-003 | Logs dürfen keine Gesundheitsdetails enthalten. | MUSS |
| NF-SEC-004 | Verschlüsselung muss konzeptionell vorbereitet sein. | MUSS |
| NF-SEC-005 | Verschlüsselung für produktive Nutzung ist anzustreben. | SOLL |
| NF-SEC-006 | Backup-Verschlüsselung soll früh verfügbar sein. | SOLL |
| NF-SEC-007 | Keine Backdoor für Masterpasswort. | MUSS |

### 15.2 Datenschutz

| ID | Anforderung | Priorität |
|---|---|---:|
| NF-DP-001 | Datensparsamkeit: möglichst wenig Pflichtfelder. | MUSS |
| NF-DP-002 | Sensibilitätsstufen für Objekte. | MUSS |
| NF-DP-003 | Exporte bewusst auswählbar. | MUSS |
| NF-DP-004 | Recht auf lokalen Datenexport. | MUSS |
| NF-DP-005 | Keine Telemetrie. | MUSS |
| NF-DP-006 | Support-/Diagnosedaten ohne Gesundheitsinhalte. | SOLL |

### 15.3 Robustheit

| ID | Anforderung | Priorität |
|---|---|---:|
| NF-ROB-001 | Datenbankzugriffe müssen transaktional erfolgen. | MUSS |
| NF-ROB-002 | Import muss fehlertolerant sein. | MUSS |
| NF-ROB-003 | Abbruch im Wizard darf keine inkonsistenten Daten hinterlassen. | MUSS |
| NF-ROB-004 | Backup/Restore muss automatisiert testbar sein. | MUSS |
| NF-ROB-005 | Migrationen müssen rückverfolgbar sein. | MUSS |

### 15.4 Performance

| ID | Anforderung | Zielwert |
|---|---|---|
| NF-PER-001 | Startzeit | unter 5 Sekunden bei normalem Datenbestand |
| NF-PER-002 | Suche Metadaten/Notizen | unter 1 Sekunde bei 10.000 Einträgen |
| NF-PER-003 | Dokumentimport | große Dateien ohne UI-Freeze |
| NF-PER-004 | Timeline | flüssig filterbar bei mehreren tausend Ereignissen |

### 15.5 Bedienbarkeit

| ID | Anforderung | Priorität |
|---|---|---:|
| NF-UX-001 | Häufige Aktionen müssen mit wenigen Klicks erreichbar sein. | MUSS |
| NF-UX-002 | Wizard muss verständlich und nicht überfordernd sein. | MUSS |
| NF-UX-003 | Komplexe Funktionen müssen erklärende Hilfetexte haben. | SOLL |
| NF-UX-004 | Tastaturbedienung soll unterstützt werden. | SOLL |
| NF-UX-005 | Fehlermeldungen müssen verständlich und handlungsorientiert sein. | MUSS |

### 15.6 Wartbarkeit

| ID | Anforderung | Priorität |
|---|---|---:|
| NF-MNT-001 | Keine Geschäftslogik in der UI. | MUSS |
| NF-MNT-002 | Schichtenarchitektur mit Domain/Application/Infrastructure/UI. | MUSS |
| NF-MNT-003 | Unit-Tests für Domain- und Application-Logik. | MUSS |
| NF-MNT-004 | XML-Kommentare für wichtige Klassen. | SOLL |
| NF-MNT-005 | Dokumentierte Architekturentscheidungen. | SOLL |

---

## 16. Technische Architektur

### 16.1 Empfohlener Stack

| Bereich | Empfehlung |
|---|---|
| Plattform | Windows Desktop zuerst |
| Sprache | C# |
| Runtime | .NET 8+ |
| UI | WPF mit MVVM oder WinForms für sehr schnellen Prototyp |
| Datenbank | SQLite |
| Datenzugriff | Repository Pattern, optional Dapper oder EF Core |
| Volltextsuche | SQLite FTS5 für Notizen/Metadaten, später OCR-Index |
| Dokumentablage | Dateisystem + Metadaten in DB |
| Export | Markdown zuerst, PDF später |
| Tests | xUnit |
| Logging | Serilog oder Microsoft.Extensions.Logging |
| Build | dotnet CLI + Visual Studio 2022 |

### 16.2 Schichten

```text
Sasd.HealthNotebook.App              # UI / Desktop-Anwendung
Sasd.HealthNotebook.Application      # Use Cases, Services, Workflows
Sasd.HealthNotebook.Domain           # Entitäten, Value Objects, Regeln
Sasd.HealthNotebook.Infrastructure   # SQLite, Dateiablage, Export, Logging
Sasd.HealthNotebook.ImportExport     # Import/Export/Backup später optional separat
Sasd.HealthNotebook.Tests            # Unit- und Integrationstests
```

### 16.3 Prinzipien

- Domain kennt keine UI.
- Application-Schicht steuert Workflows.
- Infrastructure kapselt Datenbank, Dateien, Hashing, Export.
- UI ruft Application Services auf.
- Dokumentoriginale werden nicht verändert.
- Alle Änderungen laufen über Services, nicht direkt durch UI.
- Import/Export wird protokolliert.

### 16.4 Beispiel-Services

| Service | Aufgabe |
|---|---|
| ConditionService | Conditions anlegen, bearbeiten, archivieren. |
| ConditionWizardService | Wizard-Entwurf, Validierung, Paket-Erstellung. |
| DocumentService | Dokumentimport, Hashing, Metadaten, Zuordnung. |
| SourceService | Quellen verwalten und verknüpfen. |
| SymptomService | Symptome und Beobachtungen erfassen. |
| MeasurementService | Messwerte erfassen. |
| AppointmentService | Termine und Gesprächsnotizen. |
| QuestionService | Fragen sammeln und zuordnen. |
| SearchService | Suche, Filter, Synonyme. |
| TimelineService | Timeline-Ereignisse aufbauen. |
| ExportService | Markdown/PDF/ZIP-Export. |
| BackupService | Backup und Restore. |
| AuditService | Änderungsprotokoll. |

---

## 17. Wizard-Architektur

Der Wizard soll nicht nur UI sein, sondern als eigener Workflow modelliert werden.

### 17.1 Wizard-Draft

Während der Nutzer den Wizard ausfüllt, entsteht ein `ConditionWizardDraft`.

Felder:

- DraftId,
- CreatedAt,
- UpdatedAt,
- CurrentStep,
- Basisdaten,
- SymptomDrafts,
- DocumentDrafts,
- SourceDrafts,
- MeasurementDrafts,
- MedicationDrafts,
- AppointmentDrafts,
- QuestionDrafts,
- ValidationMessages,
- IsCompleted.

### 17.2 Abschlusslogik

Beim Abschluss:

1. Draft validieren.
2. Transaktion starten.
3. Condition anlegen.
4. verknüpfte Objekte anlegen/importieren.
5. Beziehungen schreiben.
6. Timeline-Ereignisse erzeugen.
7. Audit-Ereignis erzeugen.
8. Transaktion committen.
9. Draft als abgeschlossen markieren.
10. Detailansicht öffnen.

Bei Fehler:

- Transaktion rollback,
- Draft bleibt erhalten,
- Fehlermeldung verständlich anzeigen,
- keine halbfertige Condition erzeugen.

---

## 18. Exportkonzepte

### 18.1 Condition-Bericht

Ein Condition-Bericht soll enthalten können:

- Titel und Kurzbeschreibung,
- Status,
- Zeitraum,
- wichtigste Symptome,
- relevante Dokumente,
- relevante Labor-/Messwerte,
- Medikamente/Maßnahmen als Notizen,
- offene Fragen,
- Quellen,
- Timeline-Auszug,
- eigene Entscheidungen/Hypothesen klar markiert,
- Exportdatum,
- Hinweis: keine ärztliche Diagnose durch die Software.

### 18.2 Arztfragenliste

Die Arztfragenliste soll möglichst kurz und praktisch sein:

- Termin,
- Arzt/Organisation,
- Hauptthemen,
- priorisierte Fragen,
- kurze Kontextnotizen,
- relevante Dokumentreferenzen,
- Platz für Antworten.

### 18.3 Arztmappe

Eine Arztmappe soll ein Exportpaket sein:

- Zusammenfassung als Markdown/PDF,
- Fragenliste,
- ausgewählte Dokumente,
- Manifest,
- optional Prüfsummen,
- Datenschutz-/Auswahlliste.

### 18.4 Rohdatenexport

Rohdatenexporte sollen für Zukunftssicherheit verfügbar sein:

- JSON für strukturierte Objekte,
- CSV für Tabellen/Messwerte,
- Markdown für lesbare Berichte,
- Dokumente im Originalformat,
- Manifest mit Version und Exportdatum.

---

## 19. Testkonzept

### 19.1 Unit-Tests

Muss getestet werden:

- Condition-Erstellung,
- Wizard-Draft-Validierung,
- Wizard-Abschluss als Transaktion,
- Tag-/Synonym-Suche,
- Dokumenthashing,
- Archivierung,
- Auditlog-Erstellung,
- Exportstruktur,
- Backup-Manifest.

### 19.2 Integrationstests

Muss getestet werden:

- SQLite-Persistenz,
- Datenbankmigration,
- Dokumentimport in Testablage,
- Backup und Restore,
- Volltextsuche,
- Export mit Dokumentreferenzen.

### 19.3 UI-/Smoke-Tests

Manuell mindestens je Release:

- App startet,
- neue Condition per Wizard anlegen,
- Dokument importieren,
- Quelle erfassen,
- Frage erfassen,
- Timeline prüfen,
- Export erzeugen,
- Archivieren/Wiederherstellen,
- Backup/Restore testen.

### 19.4 Sicherheitstests

- Logdateien auf Gesundheitsdaten prüfen.
- Backup-Inhalt prüfen.
- Export sensibler Inhalte prüfen.
- Dateipfade und Fehlerfälle prüfen.
- Restore in leerer Umgebung testen.

---

## 20. Roadmap-Vorschlag

### Phase 0 – Projektgrundlage

- Repositorystruktur,
- README,
- Lastenheft/Pflichtenheft,
- Architekturentscheidungen,
- Grunddatenmodell,
- Sicherheitsleitplanken.

### Phase 1 – Anwendungsschale

- .NET Solution,
- UI-Shell,
- Navigation,
- Logging ohne Gesundheitsdaten,
- Konfiguration,
- erste Tests.

### Phase 2 – Domain- und Storage-Grundlage

- Domain-Entitäten,
- SQLite-Persistenz,
- Repository Pattern,
- SchemaVersion,
- Audit-Basis,
- Soft Delete.

### Phase 3 – Condition-Verwaltung

- Conditions anlegen/bearbeiten/archivieren,
- Tags,
- Synonyme,
- Detailansicht,
- erste Suche.

### Phase 4 – Krankheits-Wizard V1

- Wizard-Draft,
- Basisdaten,
- Symptome,
- Dokumente,
- Quellen,
- Fragen,
- Zusammenfassung,
- transaktionale Anlage.

### Phase 5 – Dokumente und Quellen

- Dokumentenimport,
- Hashing,
- Metadaten,
- Quellenverwaltung,
- Verknüpfungen,
- einfache Vorschau.

### Phase 6 – Symptome, Fragen, Termine

- Symptomkatalog,
- Symptom-Beobachtungen,
- Fragenverwaltung,
- Arzttermine,
- Gesprächsnotizen.

### Phase 7 – Suche, Timeline, Export

- globale Suche,
- Filter,
- Timeline,
- Condition-Bericht Markdown,
- Arztfragenliste.

### Phase 8 – Backup, Restore, Sicherheitsbasis

- Backup/Restore,
- Manifest,
- Exportpakete,
- sensible Inhalte,
- optional Verschlüsselungsgrundlage.

### Phase 9 – Messwerte und Laborwerte

- Messwertmodell,
- manuelle Laborwerte,
- Vitalwerte,
- Tabellenexport,
- einfache Verlaufsansicht.

### Phase 10 – Professionalisierung

- PDF-Export,
- Dokumentenvorschau,
- Dublettenprüfung,
- Vorlagenverwaltung,
- bessere UI.

### Phase 11+ – Spätere Erweiterungen

- OCR,
- Diagramme,
- FHIR,
- Health Connect/Apple Health,
- Zotero/PubMed,
- KI-Zusammenfassung,
- mobile Begleit-App.

---

## 21. Akzeptanzkriterien für V1

V1 gilt als fachlich erfolgreich, wenn:

1. eine neue Erkrankung / ein neues Gesundheitsthema per Wizard angelegt werden kann,
2. im Wizard Symptome, Dokumente, Quellen und Fragen zugeordnet werden können,
3. die erzeugte Condition alle verknüpften Informationen in einer Detailansicht zeigt,
4. Dokumente unverändert gespeichert und per Hash identifizierbar sind,
5. Quellen und eigene Notizen klar unterscheidbar sind,
6. offene Arztfragen gesammelt und exportiert werden können,
7. eine Timeline den Verlauf eines Themas zeigt,
8. globale Suche über zentrale Inhalte funktioniert,
9. Archivierung statt stillem Löschen verwendet wird,
10. ein Backup erstellt und wiederhergestellt werden kann,
11. ein Markdown-Bericht exportiert werden kann,
12. die Anwendung ohne Cloud, Konto und externe Übertragung funktioniert,
13. keine Diagnose- oder Therapieempfehlung generiert wird,
14. Build und Tests reproduzierbar laufen,
15. wichtige Architekturentscheidungen dokumentiert sind.

---

## 22. Risiken und Gegenmaßnahmen

| Risiko | Auswirkung | Gegenmaßnahme |
|---|---|---|
| Zu großer Funktionsumfang | Projekt wird nicht fertig. | V1 streng begrenzen, Backlog dokumentieren. |
| Medizinprodukt-Grenze überschritten | Regulatorischer Aufwand. | Keine Diagnose/Therapie/automatische Interpretation in V1. |
| Datenverlust | Vertrauensverlust, Schaden. | Backup/Restore früh, Tests, Hashes, Transaktionen. |
| Datenschutzfehler | Sehr kritisch. | Lokal-first, keine Telemetrie, sensible Logs vermeiden. |
| Schlechte Bedienbarkeit | Nutzer pflegt Daten nicht. | Wizard, schnelle Notiz, gute Suche. |
| Zu viele Freitextdaten | Spätere Auswertung schwer. | Freitext + strukturierte Metadaten kombinieren. |
| Zu starres Datenmodell | Erweiterung teuer. | generische Beziehungen, Tags, Synonyme, flexible Entry-Typen. |
| KI-Halluzinationen | Falsche Sicherheit. | KI später, optional, nur mit Quellen/Unsicherheit. |
| Verschlüsselung ohne Recovery | Daten bei Passwortverlust weg. | klare Warnung, verschlüsselte Backups, kein Backdoor. |

---

## 23. Bewusst geparkte Funktionen

Diese Funktionen bleiben dokumentiert, sollen aber nicht V1 belasten:

- automatische Diagnosevorschläge,
- Therapieempfehlungen,
- Wechselwirkungsprüfung,
- CGM-Echtzeitdaten,
- Notfalltriage,
- Community-Funktionen,
- direkte Arztkommunikation,
- Cloud-Synchronisation,
- mobile App,
- KI-Medizinberater,
- automatischer Import aus Patientenportalen,
- Versicherungs-/Abrechnungsfunktionen,
- vollwertige elektronische Patientenakte,
- Multi-User-Freigabesystem,
- rechtssichere elektronische Signaturen.

---

## 24. Offene Architekturentscheidungen

| Entscheidung | Optionen | Empfehlung aktuell |
|---|---|---|
| UI-Technologie | WPF, WinForms, Avalonia, MAUI | WPF für gutes Desktop-UI; WinForms nur für sehr schnellen MVP. |
| Datenzugriff | EF Core, Dapper, eigenes Repository | Repository Pattern, darunter Dapper oder EF Core. |
| Verschlüsselung | SQLCipher, Dateicontainer, App-Level Crypto | früh evaluieren; Storage-Schicht abstrahieren. |
| PDF-Export | QuestPDF, Playwright/HTML, LaTeX | Markdown zuerst, PDF später. |
| OCR | Tesseract, Windows OCR, externe Engine | später modular. |
| KI | lokal, OpenAI, andere Cloud, keine KI | V1 keine KI; später lokal bevorzugt. |
| FHIR | R4, R5, kein FHIR | später prüfen; R4 ist häufig praktisch relevant, R5 aktueller Standard. |

---

## 25. Dokumentationsanforderungen

Das Repository soll mindestens enthalten:

```text
docs/
  000_Project_Overview.md
  010_Lastenheft.md
  020_Pflichtenheft.md
  030_Architecture.md
  040_Data_Model.md
  050_UI_Concept.md
  060_Security_Privacy.md
  070_Roadmap.md
  080_Test_Strategy.md
  090_Decision_Log.md
  phases/
  adr/
```

Für jede Entwicklungsphase soll dokumentiert werden:

- Ziel,
- Umfang,
- betroffene Dateien,
- Datenmodelländerungen,
- Tests,
- manuelle Prüfung,
- bekannte Einschränkungen,
- Commit-Vorschlag.

---

## 26. Quellen und fachliche Orientierung

Diese Quellen sind keine medizinische Beratung, sondern technische und regulatorische Orientierung:

- Findings Lab Notebook: https://findingsapp.com/
- Findings App Store Beschreibung: https://apps.apple.com/de/app/findings-lab-notebook/id922844272
- DSGVO Art. 9 – besondere Kategorien personenbezogener Daten: https://gdpr-info.eu/art-9-gdpr/
- EU Medical Device Guidance / MDCG-Dokumente: https://health.ec.europa.eu/medical-devices-sector/new-regulations/guidance-mdcg-endorsed-documents-and-other-guidance_en
- MDCG 2019-11 Software Qualification and Classification: https://health.ec.europa.eu/system/files/2020-09/md_mdcg_2019_11_guidance_en_0.pdf
- HL7 FHIR Overview: https://www.hl7.org/fhir/overview.html
- ELN-Grundlagen Forschungsdaten Thüringen: https://forschungsdaten-thueringen.de/eln.html

---

## 27. Vorläufiges Fazit

Das Projekt sollte nicht als medizinisches Expertensystem starten, sondern als **lokal-first Health Research Notebook**. Der wichtigste V1-Erfolg ist nicht KI, nicht FHIR und nicht Geräteintegration, sondern eine zuverlässige Grundstruktur:

- Erkrankungen/Themen sauber anlegen,
- Dokumente und Quellen sicher zuordnen,
- Symptome und Fragen erfassen,
- Timeline und Suche nutzen,
- Arztgespräche vorbereiten,
- Berichte exportieren,
- Daten lokal und kontrollierbar behalten.

Der vorgeschlagene Krankheits-Wizard ist dafür ein sehr guter Kern, weil er den Nutzer zwingt, wichtige Informationen früh geordnet zu erfassen, ohne ihn mit einem leeren Datenmodell allein zu lassen.

---

# Anhang A – Übernommener Feature-Backlog aus der Voranalyse

Der folgende Anhang übernimmt die vereinheitlichte Feature-Sammlung aus der vorherigen Recherche. Nicht alle Punkte sind Bestandteil von V1. Der Zweck dieses Anhangs ist, dass potenziell wichtige Ideen nicht verloren gehen und später bewusst gestrichen, verschoben oder umgesetzt werden können.

Legende für spätere Bewertung:

- **V1**: für die erste produktive Grundlage nötig,
- **V2/V3**: spätere funktionale Erweiterung,
- **Parken**: bewusst beobachten,
- **Streichen**: wahrscheinlich nicht passend,
- **Regulatorisch prüfen**: nur nach gesonderter Bewertung.

Die folgende Sammlung ist bewusst breit. Sie ist noch keine Priorisierung für V1.

### 4.1 Grundstruktur und Informationsmodell

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-001 | Erkrankungen / Themen | Eigene Bereiche für Diabetes, Gefäße, Zunge, Medikamente, Ernährung, Blutdruck, Befunde usw. | Guava, PatientsLikeMe, MyGNUHealth |
| F-002 | Mehrfach-Zuordnung | Ein Eintrag kann mehreren Themen zugeordnet sein, z. B. Diabetes + Ernährung + Laborwert. | Obsidian, Zotero, ELN |
| F-003 | Eintragstypen | Notiz, Befund, Symptom, Messwert, Medikament, Quelle, Arztfrage, Arzttermin, Entscheidung, Hypothese, Beobachtung. | Findings, eLabFTW, Bearable |
| F-004 | Flexible Metadaten | Je nach Eintragstyp unterschiedliche Felder, aber mit gemeinsamem Kern: Datum, Titel, Quelle, Tags, Status. | OpenMRS Concept Dictionary, ELN |
| F-005 | Statusmodell | Offen, in Prüfung, mit Arzt besprochen, bestätigt, verworfen, erledigt, archiviert. | ELN, Aufgabenverwaltung |
| F-006 | Priorität / Dringlichkeit | Niedrig, normal, wichtig, kritisch; besonders für Arztfragen und auffällige Befunde. | Task-Manager, PHR |
| F-007 | Tags | Freie Schlagworte wie „Arteriosklerose“, „B12“, „Brennen“, „Labor“, „Hausarzt“. | Paperless-ngx, Zotero, Obsidian |
| F-008 | Kategorien / Ordner | Grobe Struktur neben Tags: Befunde, Symptome, Medikamente, Quellen, Arzttermine. | eLabFTW, Joplin, DEVONthink |
| F-009 | Synonyme | Suche nach „Gefäßverkalkung“ findet auch „Arteriosklerose/Atherosklerose“. | Medizinische Terminologie, PKM |
| F-010 | Medizinische Begriffe | Optionales Glossar für Fachbegriffe mit einfacher Erklärung und Quelle. | Apple Health, Guava, Ada |
| F-011 | Beziehungen zwischen Einträgen | „Dieser Laborwert gehört zu diesem Arztbrief“, „Diese Quelle erklärt diese Frage“. | Obsidian, TheBrain, FHIR |
| F-012 | Hypothesen / Annahmen | Eigene Vermutungen klar als Vermutung markieren, nicht als Fakt. | Scientific Notebook, ELN |
| F-013 | Entscheidungsnotizen | Warum wurde etwas getan, nicht getan oder verworfen? | Lab Notebook, Audit Trail |
| F-014 | Verlaufsepisoden | Zusammengehörige Ereignisse als Episode gruppieren, z. B. „Zungenbrennen Frühjahr 2026“. | PHR, Case Management |
| F-015 | Personen und Rollen | Hausarzt, Facharzt, Labor, Krankenhaus, Apotheke, Angehörige, Pflegeperson. | MyChart, OpenEMR |
| F-016 | Organisationen | Praxis, Klinik, Labor, Krankenkasse, Apotheke. | Paperless-ngx Korrespondenten |
| F-017 | Kontaktinformationen | Adresse, Telefon, Portal-Link, Sprechzeiten, Notizen. | Patientenportal/Praxisverwaltung |
| F-018 | Terminbezug | Einträge können mit Arztterminen verknüpft werden. | MyChart, CareClinic |
| F-019 | Dokumentbezug | Jeder Befund kann mit PDF/Bild/Datei verknüpft werden. | Paperless-ngx, ELN |
| F-020 | Quellenbezug | Jede Aussage kann auf Webseite, Studie, Arztbrief, Gespräch oder eigene Beobachtung verweisen. | Zotero, Lab Notebook |

### 4.2 Erfassen, Sammeln und Importieren

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-021 | Schnelle Notiz | Minimalmaske für schnelle Gedanken, später sortieren. | Joplin, Obsidian, Evernote |
| F-022 | Strukturierter Eintrag | Formular je Eintragstyp mit sinnvollen Feldern. | ELN, Bearable |
| F-023 | Tagesjournal | Tägliche kurze Erfassung von Symptomen, Stimmung, Schlaf, Medikamenten, Besonderheiten. | Bearable, CareClinic |
| F-024 | Symptom-Schnellerfassung | Symptom + Stärke + Zeitpunkt + Kontext in wenigen Sekunden. | Bearable, CareClinic |
| F-025 | Freitext mit Metadaten | Kombination aus freiem Text und strukturierten Feldern. | Findings, Joplin |
| F-026 | Web Clipper | Webseiten, Artikel, PDFs, Ausschnitte und Kommentare speichern. | Zotero, Evernote, OneNote, Joplin |
| F-027 | PDF-Import | Arztbriefe, Befunde, Studien, Laborberichte importieren. | Paperless-ngx, Zotero |
| F-028 | Bild-Import | Fotos von Verpackungen, Befunden, Hautstellen, Messgeräten, Screenshots. | PHR, ELN |
| F-029 | Scan-Import | Eingescannte Arztbriefe, Laborzettel, Rezepte, Überweisungen. | Paperless-ngx |
| F-030 | Drag & Drop | Dateien per Drag & Drop an Einträge hängen. | Moderne DMS/ELN |
| F-031 | Clipboard-Import | Text aus Webseiten/Briefen übernehmen und Quelle festhalten. | Notiz-Apps |
| F-032 | E-Mail-Import | Arzt-/Labor-Mails oder Newsletter als Quelle ablegen. | DMS |
| F-033 | Ordner-Watch | Importordner überwachen, neue Dokumente automatisch anbieten. | Paperless-ngx |
| F-034 | Portal-Export-Ablage | Von Patientenportalen exportierte PDFs strukturiert ablegen. | MyChart, Apple Health |
| F-035 | Manuelle Laborwert-Erfassung | Laborwert, Einheit, Referenzbereich, Datum, Labor, Quelle. | Apple Health, Guava, MyTherapy |
| F-036 | Manuelle Vitalwert-Erfassung | Blutdruck, Puls, Gewicht, Blutzucker, Temperatur, Sauerstoffsättigung. | MyTherapy, Apple Health |
| F-037 | Medikamenten-Erfassung | Name, Wirkstoff, Dosierung als Notiz, Einnahmehinweise, Quelle. | MyTherapy, Medisafe |
| F-038 | Behandlungs-/Maßnahmen-Log | Ernährung, Bewegung, Schlaf, Stress, Medikamente, ärztliche Maßnahmen. | CareClinic, PatientsLikeMe |
| F-039 | Nebenwirkungsnotiz | Symptom/Veränderung mit Medikament oder Maßnahme verknüpfen. | MyTherapy, Medisafe |
| F-040 | Arztgesprächsnotiz | Gesprächsprotokoll mit Datum, Arzt, Themen, Ergebnissen, offenen Punkten. | OneNote, MyChart |
| F-041 | Frage-Eingang | Fragen jederzeit sammeln und später Arztterminen zuordnen. | PHR, Task-Manager |
| F-042 | Studien-/Artikel-Import | PubMed/DOI/URL speichern, Abstract, PDF und Notizen verknüpfen. | Zotero |
| F-043 | Audio-Notiz | Nach Arzttermin kurze Sprachnotiz erfassen. | Evernote/OneNote |
| F-044 | Handschrift / Skizze | Notizen per Stift, z. B. für Symptome auf Körperkarte. | OneNote |
| F-045 | Foto mit Kontext | Foto immer mit Datum, Körperstelle, Beschreibung, Thema speichern. | PHR |
| F-046 | Barcode/Packungsfoto | Medikamentenpackung fotografieren oder scannen. | Medisafe/MyTherapy-Idee |
| F-047 | CSV-Import | Laborwerte oder Messwerte aus Tabellen importieren. | Tidepool, DMS |
| F-048 | Geräteimport später | Blutzucker, Blutdruck, Smartwatch, CGM, Waage, Schlafdaten. | Apple Health, Health Connect, Tidepool |
| F-049 | FHIR-Import später | Strukturierte Gesundheitsdaten aus kompatiblen Systemen importieren. | FHIR, MyGNUHealth |
| F-050 | Import-Protokoll | Jeder Import erzeugt nachvollziehbare Log-/Historieninformation. | ELN, DMS |

### 4.3 Dokumentenverwaltung

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-051 | Dokumentenbibliothek | Zentrale Ablage für PDFs, Bilder, Scans, Office-Dokumente. | Paperless-ngx, DEVONthink |
| F-052 | OCR | Scans und Bilder durchsuchbar machen. | Paperless-ngx, DEVONthink |
| F-053 | Volltextindex | Text aus Notizen, PDFs, OCR und Metadaten durchsuchen. | Paperless-ngx, Zotero |
| F-054 | Dokumenttyp | Arztbrief, Laborbericht, Befund, Rezept, Überweisung, Rechnung, Studie, Screenshot. | Paperless-ngx |
| F-055 | Korrespondent | Arzt, Klinik, Labor, Krankenkasse, Apotheke. | Paperless-ngx |
| F-056 | Dokumentdatum | Unterscheidung zwischen Importdatum und medizinischem Datum. | DMS |
| F-057 | Dokumentstatus | Neu, geprüft, zugeordnet, offen, archiviert. | DMS/ELN |
| F-058 | Automatische Vorschläge | Tag-/Typ-/Korrespondentenvorschläge anhand bisheriger Zuordnung. | Paperless-ngx |
| F-059 | Mehrfachverknüpfung | Ein Dokument kann mit mehreren Themen, Symptomen und Terminen verknüpft werden. | DMS/PKM |
| F-060 | Versionen von Dokumenten | Neue Versionen statt Überschreiben, z. B. aktualisierte Befunde. | RSpace, DMS |
| F-061 | Original unverändert speichern | Importdatei bleibt unverändert; Notizen/Metadaten separat. | Revisionssicherheit |
| F-062 | Prüfsumme / Hash | Nachweis, dass Originaldokument nicht verändert wurde. | GNU Health Crypto, DMS |
| F-063 | Annotationslayer | Markierungen/Kommentare separat speichern, Original bleibt sauber. | Zotero, PDF-Tools |
| F-064 | Seitenmarken | Wichtige Seiten in langen PDFs markieren. | PDF-Reader |
| F-065 | Dokumentvorschau | PDF/Bild im Programm anzeigen. | DEVONthink, Paperless-ngx |
| F-066 | Schnelle Zuordnung | Dokument per Kontextmenü Thema/Arzt/Labor zuordnen. | DMS |
| F-067 | Dokumentenpakete | Mehrere Dokumente für Arzttermin bündeln. | PHR |
| F-068 | Exportmappe | Ausgewählte Dokumente + Zusammenfassung als Paket exportieren. | Patientenportal/PHR |
| F-069 | Löschschutz | Dokumente nicht versehentlich hart löschen; Papierkorb/Archiv. | DMS |
| F-070 | Dublettenprüfung | Gleiche PDFs/Scans erkennen. | DMS |
| F-071 | Sensible Dokumente markieren | Besonders vertraulich: psychiatrisch, genetisch, sexuell, rechtlich usw. | Datenschutz |
| F-072 | Dokumentenablage auf Dateisystem | Dateien strukturiert im Dateisystem, Metadaten in SQLite. | Paperless-ngx-Idee |
| F-073 | Datenbankinterne Ablage optional | Kleine Anhänge optional direkt in DB oder verschlüsseltem Container. | Vault-Ansatz |
| F-074 | Backup-freundliche Struktur | Klare Ordner, Manifest, Prüfsummen. | SASD-Stil |
| F-075 | Importfehlerliste | Nicht verarbeitbare Dokumente sauber protokollieren. | DMS |

### 4.4 Symptome, Verlauf und Alltag

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-076 | Symptomkatalog | Eigene Symptome definieren: Brennen, Schmerzen, Müdigkeit, Schwindel usw. | Bearable |
| F-077 | Schweregrad-Skala | 0–10 oder leicht/mittel/stark. | Bearable, CareClinic |
| F-078 | Häufigkeit | einmalig, täglich, nachts, nach Essen, nach Belastung. | Symptomtracker |
| F-079 | Dauer | Minuten, Stunden, Tage, dauerhaft. | Symptomtracker |
| F-080 | Lokalisation | Körperstelle oder Freitext. | CareClinic Body Map |
| F-081 | Körperkarte später | Körperregion visuell markieren. | CareClinic-Idee |
| F-082 | Trigger | Essen, Stress, Schlaf, Bewegung, Medikamente, Wetter, Alkohol, Fett usw. | Bearable/CareClinic |
| F-083 | Lindernde Faktoren | Ruhe, Essen, Trinken, Medikament, Wärme, Bewegung, Schlaf. | Symptomtracker |
| F-084 | Begleitsymptome | Ein Symptomeintrag kann mehrere Symptome gruppieren. | PHR |
| F-085 | Tagesform | Allgemeines Befinden, Energie, Stimmung. | Bearable |
| F-086 | Schlaf | Dauer, Qualität, Unterbrechungen, Tagesmüdigkeit. | Apple Health, Bearable |
| F-087 | Ernährungskontext | Mahlzeiten, besondere Lebensmittel, Fett, Zucker, Alkohol, Unverträglichkeiten. | Bearable/CareClinic |
| F-088 | Bewegung | Schritte, Aktivität, Belastung, Spaziergang. | Health Connect |
| F-089 | Stresslevel | subjektive Belastung und Ereignisse. | Bearable |
| F-090 | Stuhlgang / Verdauung | optionales Modul für Darm/Verdauung. | Bearable |
| F-091 | Schmerzjournal | Ort, Stärke, Art, Dauer, Auslöser. | CareClinic |
| F-092 | Stimmung / mentale Belastung | Stimmungsskala, Angst/Stress, kurze Notizen. | Bearable |
| F-093 | Energie-/Pacing-Modul | Belastungsgrenze, Crash/Überlastung, Erholungszeit. | Visible |
| F-094 | Flare-up-Erfassung | akute Verschlechterung mit Start/Ende, Auslösern und Verlauf. | CareClinic |
| F-095 | Monats-/Wochenübersicht | Muster in einfacher Kalenderansicht sehen. | Bearable/MyTherapy |
| F-096 | Verlaufsgrafiken | Symptomstärke über Zeit. | Bearable |
| F-097 | Korrelationen | Symptom vs. Schlaf, Essen, Medikament, Aktivität, Laborwert. | Bearable, Guava |
| F-098 | Notiz an Messwert | Ereignis mit Blutdruck/Glukose/Laborwert verknüpfen. | xDrip/Tidepool |
| F-099 | Grenzwert-Markierung | Werte außerhalb Ziel-/Referenzbereich markieren, ohne medizinische Bewertung. | Apple Health, Guava |
| F-100 | Beobachtungsprojekte | Zeitlich begrenzte Beobachtung: „2 Wochen Ernährung und Zungenbrennen“. | ELN/Scientific Notebook |

### 4.5 Laborwerte und Vitalwerte

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-101 | Laborwertdefinition | Name, Kategorie, Einheit, alternative Namen. | Apple Health, FHIR Observation |
| F-102 | Referenzbereich | Untere/obere Grenze, Quelle, Laborabhängigkeit. | Guava/Apple Health |
| F-103 | Zielbereich | Persönlicher Zielbereich getrennt vom Referenzbereich. | Diabetes-Tools |
| F-104 | Einheitenumrechnung | z. B. mg/dL vs. mmol/L für bestimmte Werte. | Gesundheitsplattformen |
| F-105 | Laborpanel | Mehrere Werte aus einem Laborbericht als Gruppe. | PHR |
| F-106 | Verlaufsgrafik pro Wert | Laborwert über Zeit. | Apple Health/Guava |
| F-107 | Vergleich mit Referenzbereich | Grafische Markierung, aber keine Diagnose. | Apple Health |
| F-108 | Laborquelle | Labor, Arzt, PDF, Datum, Importmethode. | DMS/PHR |
| F-109 | Auffälligkeit manuell markieren | Nutzer markiert: „mit Arzt besprechen“. | PHR |
| F-110 | Interpretation als Notiz | Freitext, getrennt von Rohwert. | Lab Notebook |
| F-111 | Wert aus PDF extrahieren später | OCR/AI-Vorschlag, manuell bestätigen. | Guava/Paperless/AI |
| F-112 | Vitalwerte | Blutdruck, Puls, Gewicht, Blutzucker, Temperatur, SpO₂. | MyTherapy/Health Connect |
| F-113 | Messbedingungen | nüchtern, nach Essen, nach Bewegung, morgens, abends. | Diabetes/BP Tracking |
| F-114 | Messgerät | Gerätetyp, Modell, Quelle, Genauigkeitsnotiz. | Health Connect/Tidepool |
| F-115 | Ziel- und Alarmbereiche später | Nur wenn regulatorisch sauber abgegrenzt. | xDrip/Nightscout |
| F-116 | Rohdatenexport | CSV/JSON für eigene Auswertungen. | Tidepool |
| F-117 | Statistik-Basics | Minimum, Maximum, Durchschnitt, Trend, Anzahl Messungen. | Health Tracker |
| F-118 | Zeitfenstervergleich | Woche/Monat/Quartal/Jahr vergleichen. | Health Tracker |
| F-119 | Kommentar pro Wert | „nach Spaziergang“, „Messung wiederholt“, „Labor nüchtern“. | Tracking-Apps |
| F-120 | Datenqualität | Wert bestätigt, aus OCR vorgeschlagen, manuell erfasst, importiert. | Data Governance |

### 4.6 Medikamente, Maßnahmen und Behandlungsnotizen

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-121 | Medikamentenliste | Name, Wirkstoff, Stärke, Form, Start/Ende, Arzt, Grund. | MyTherapy, Medisafe |
| F-122 | Einnahmeplan | Tageszeit, Dosis als Freitext/Struktur, Hinweis. | Medisafe |
| F-123 | Einnahmehistorie | genommen, ausgelassen, verschoben, Grund. | Medisafe/MyTherapy |
| F-124 | Vorrat / Refill | Vorrat, Rezept läuft aus, Nachbestellung. | MyTherapy |
| F-125 | Nebenwirkungsbeobachtung | Symptom mit Medikamentenstart/-änderung verknüpfen. | MyTherapy |
| F-126 | Medikamentenänderung | Änderung mit Datum, Grund und Quelle dokumentieren. | PHR |
| F-127 | Therapie-/Maßnahmenliste | Ernährung, Bewegung, Schlaf, Physiotherapie, Supplemente, ärztliche Maßnahmen. | CareClinic |
| F-128 | Wirksamkeit subjektiv | Nutzer bewertet: besser/schlechter/unklar. | PatientsLikeMe |
| F-129 | Keine Therapieempfehlung | System dokumentiert nur, gibt keine Einnahmeempfehlungen. | Sicherheitsabgrenzung |
| F-130 | Wechselwirkungswarnung später | Nur mit validierter Datenquelle und regulatorischer Prüfung. | Medisafe |
| F-131 | Beipackzettel-Ablage | PDF/Foto/Link zum Medikament. | DMS |
| F-132 | Packungsfoto | Foto der Packung als Identifikationshilfe. | Health Apps |
| F-133 | Arzt-/Apothekenquelle | Wer hat Medikament empfohlen/verordnet? | PHR |
| F-134 | Verträglichkeitstagebuch | Wirkung/Nebenwirkung über Zeit. | MyTherapy/Bearable |
| F-135 | Maßnahmenvergleich | z. B. Ernährung A vs. B, aber nur deskriptiv. | Guava/CareClinic |
| F-136 | Reminder optional | Lokale Erinnerungen ohne Cloud. | Medisafe/MyTherapy |
| F-137 | Reminder-Historie | Erinnerung ausgelöst, bestätigt, ignoriert. | Medication Apps |
| F-138 | Medikamentenexport | Liste für Arzttermin/Notfallmappe. | Apple Health/MyChart |
| F-139 | Notfallinformationen | Medikamente, Allergien, Diagnosen, Kontakte als Export. | Apple Health |
| F-140 | Allergien/Unverträglichkeiten | Stoff, Reaktion, Quelle, Schwere, Datum. | Apple Health/MyChart |

### 4.7 Arzttermine, Fragen und Kommunikation

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-141 | Arztterminverwaltung | Termin, Arzt, Anlass, Themen, Dokumente, Fragen. | MyChart |
| F-142 | Fragenliste | Fragen sammeln, priorisieren, abhaken. | PHR |
| F-143 | Terminmappe | Automatisch relevante Befunde/Notizen/Dokumente zusammenstellen. | PHR |
| F-144 | Gesprächsprotokoll | Ergebnis, Empfehlungen, offene Punkte, nächste Schritte. | OneNote |
| F-145 | To-dos aus Termin | Bluttest machen, Befund abholen, Rezept klären. | Task-Manager |
| F-146 | Wiedervorlage | Frage/Befund nach X Wochen erneut prüfen. | Task-Manager |
| F-147 | Arztbrief-Erwartung | Notiz: Befund/Brief steht noch aus. | PHR |
| F-148 | Dokumente an Termin knüpfen | Befund, Labor, Fragen, Verlaufsauszug. | DMS |
| F-149 | Export für Arzt | Kurzer Bericht als PDF/Markdown: Anlass, Verlauf, Fragen, aktuelle Medikamente. | CareClinic/MyTherapy |
| F-150 | Ausdruckfreundliche Ansicht | Für Praxisbesuch optimiertes Layout. | PHR |
| F-151 | „Was wurde besprochen?“ | Nach Termin strukturierte Nachdokumentation. | Lab Notebook |
| F-152 | Entscheidung aus Termin | Festhalten, was entschieden wurde und warum. | ELN/Audit |
| F-153 | Arztantwort als Quelle | Ärztliche Aussage als Quelle mit Datum dokumentieren. | Source Management |
| F-154 | Portalnachrichten archivieren | Nachrichten aus MyChart/anderen Portalen als Dokument/Notiz speichern. | Patientenportal |
| F-155 | Keine direkte Arztkommunikation in V1 | V1 sendet keine Nachrichten an Ärzte. | Risikoreduktion |

### 4.8 Quellen, Studien und Evidenz

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-156 | Quellenverwaltung | Webseite, Studie, Buch, Video, Arztgespräch, Leitlinie, Forum, eigene Beobachtung. | Zotero |
| F-157 | Quellentyp | Studie, Leitlinie, Herstellerinfo, Patientenforum, Arztbrief, Laborbericht. | Zotero |
| F-158 | Quellenqualität | hoch/mittel/niedrig/unbekannt; Begründung. | Evidence-Based Medicine |
| F-159 | Vertrauensstufe | offiziell, wissenschaftlich, kommerziell, Erfahrungsbericht, ungeprüft. | Rechercheprozess |
| F-160 | Zitat/Exzerpt | Kurze Textstelle mit Link/Seite. | Zotero |
| F-161 | Annotationen | Markierungen und eigene Kommentare zu PDFs/Artikeln. | Zotero |
| F-162 | DOI/PubMed-ID | Wissenschaftliche Quelle eindeutig referenzieren. | Zotero |
| F-163 | URL + Abrufdatum | Webquelle mit Datum dokumentieren. | Wissenschaftliches Arbeiten |
| F-164 | Quellen-Snapshot später | Webseite als PDF/HTML archivieren. | Evernote/DEVONthink |
| F-165 | Aussage aus Quelle extrahieren | Einzelne Aussage als Knowledge Item erfassen. | PKM |
| F-166 | Widerspruch markieren | Quelle A widerspricht Quelle B. | Evidence Notebook |
| F-167 | Bestätigt durch | Aussage wurde durch Arzt/Labor/weitere Quelle bestätigt. | Rechercheprozess |
| F-168 | Verworfen / überholt | Quelle oder Annahme ist veraltet oder nicht mehr relevant. | ELN |
| F-169 | Evidenzkarte | Thema → Quellen → Aussagen → eigene Fragen. | Obsidian/TheBrain |
| F-170 | Literatur-Sammlung | Studien zu Erkrankungen/Medikamenten/Ernährung sammeln. | Zotero |
| F-171 | Import aus Zotero später | Quellen aus Zotero verknüpfen statt doppelt pflegen. | Zotero API |
| F-172 | Quellenexport | Bibliografie für interne Dokumentation. | Zotero |
| F-173 | Leitlinienbereich | Offizielle Leitlinien getrennt von allgemeinen Artikeln. | Medical Research |
| F-174 | Patientenberichte getrennt | Erfahrungsberichte klar als solche markieren. | PatientsLikeMe |
| F-175 | Quellenreview | Regelmäßig prüfen, ob Quelle noch aktuell ist. | Research Workflow |

### 4.9 Suche, Navigation und Wissensgraph

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-176 | Globale Volltextsuche | Notizen, Dokumente, OCR, Quellen, Tags. | Paperless-ngx, DEVONthink |
| F-177 | Facettensuche | Typ, Thema, Arzt, Zeitraum, Dokumenttyp, Status. | DMS |
| F-178 | Gespeicherte Suchen | z. B. „offene Arztfragen“, „Laborwerte 2026“, „ungeprüfte Quellen“. | DEVONthink Smart Groups |
| F-179 | Smart Collections | Automatische Sammlungen anhand Regeln. | DEVONthink, Paperless |
| F-180 | Backlinks | Zeigen, welche Einträge auf diesen Eintrag verweisen. | Obsidian/Logseq |
| F-181 | Wissensgraph | Themen, Symptome, Medikamente, Quellen und Befunde visuell verknüpfen. | Obsidian/TheBrain |
| F-182 | Timeline | Chronologische Gesundheitsgeschichte. | PHR |
| F-183 | Kalenderansicht | Symptome, Termine, Medikamente, Messwerte nach Datum. | Health Tracker |
| F-184 | Dashboard | Aktuelle offenen Fragen, neue Dokumente, letzte Symptome, nächste Termine. | PHR/Notion |
| F-185 | Favoriten | Wichtige Dokumente/Quellen/Fragen schnell erreichbar. | PKM |
| F-186 | Zuletzt geändert | Arbeitsverlauf nachvollziehen. | Notiz-Apps |
| F-187 | Unsortierte Inbox | Neue Dokumente/Notizen zuerst in Inbox, später zuordnen. | DEVONthink/Joplin |
| F-188 | Related Items | Ähnliche Dokumente/Einträge vorschlagen. | DEVONthink |
| F-189 | Synonym-Suche | Medizinische Begriffe und Alltagssprache verbinden. | SASD-spezifisch |
| F-190 | Filter für Arzttermin | Nur relevante, ausgewählte Inhalte anzeigen. | PHR |
| F-191 | Filter für Export | Sensible Inhalte gezielt ein-/ausschließen. | Datenschutz |
| F-192 | Suchprotokoll optional | Was wurde gesucht und gefunden, für eigene Nachvollziehbarkeit. | Research Workflow |
| F-193 | Lesezeichen im Dokument | Wichtige Stellen in PDFs verknüpfen. | Zotero/DEVONthink |
| F-194 | Kontextmenüs | Schnelle Aktionen auf Einträgen, Dokumenten, Themen. | Desktop-UX |
| F-195 | Doppelklick-Aktionen | Dokument öffnen, Eintrag bearbeiten, Quelle anzeigen. | SASD-UX-Stil |

### 4.10 Reports, Export und Ausdruck

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-196 | Markdown-Export | Einträge, Themen, Arztfragen als Markdown exportieren. | Obsidian/Joplin |
| F-197 | PDF-Export | Arztbericht, Verlauf, Medikamentenliste, Dokumentenmappe. | Findings, CareClinic |
| F-198 | Arzttermin-Bericht | Kurzfassung: Anlass, Verlauf, Fragen, relevante Befunde. | PHR |
| F-199 | Jahresbericht | Überblick über wichtige Ereignisse, Diagnosen, Laborwerte, Medikamente. | PHR |
| F-200 | Medikamentenliste | Aktuelle Medikamente und Änderungen. | Apple Health/MyTherapy |
| F-201 | Laborwertbericht | Ausgewählte Werte mit Verlauf und Quellen. | Apple Health/Guava |
| F-202 | Symptomverlauf | Symptomstärke und wichtige Ereignisse. | Bearable |
| F-203 | Dokumentenliste | Alle vorhandenen Befunde mit Datum, Arzt, Typ. | DMS |
| F-204 | Quellenliste | Literatur/Links zu einem Thema. | Zotero |
| F-205 | Fragenliste | Ausdruckbare Liste für Arzttermin. | PHR |
| F-206 | Notfallkarte | Diagnosen, Medikamente, Allergien, Notfallkontakte. | Apple Health |
| F-207 | Exportprofil | „Für Hausarzt“, „Für Diabetologen“, „Für eigene Akte“. | Datenschutz |
| F-208 | Redaktionsmodus | Sensible Inhalte vor Export manuell prüfen. | Datenschutz |
| F-209 | Anonymisierter Export | Für Forschung/Forum: personenbezogene Daten entfernen. | Privacy |
| F-210 | ZIP-Export | Markdown + Dokumente + Manifest. | SASD-Dokumentationsstil |
| F-211 | CSV-Export | Messwerte/Laborwerte für Tabellenanalyse. | Health Tracker |
| F-212 | JSON-Export | Vollständiger maschinenlesbarer Export. | Interoperabilität |
| F-213 | FHIR-Export später | Ausgewählte Daten als FHIR-Ressourcen. | HL7 FHIR |
| F-214 | Druckvorschau | Vor dem Ausdruck prüfen. | Desktop Apps |
| F-215 | Export-Log | Wann wurde was exportiert? | Audit/Datenschutz |

### 4.11 Sicherheit, Datenschutz und Vertrauenswürdigkeit

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-216 | Lokal-first | Daten liegen primär lokal, keine Cloud-Pflicht. | Obsidian, Anytype, Joplin |
| F-217 | Verschlüsselung at rest | Datenbank und Dokumente verschlüsseln. | Joplin, Anytype, OWASP |
| F-218 | Master-Passwort | Zugriffsschutz für Gesundheitsdaten. | Secret Manager |
| F-219 | Schlüsselableitung | Sichere Passwortableitung, z. B. Argon2id. | OWASP |
| F-220 | Keine Telemetrie | Keine Nutzungsdaten ohne bewusste Freigabe. | Privacy-first |
| F-221 | Sperrbildschirm | App nach Inaktivität sperren. | Passwortmanager |
| F-222 | Rollen später | Nutzer, Angehöriger, Arzt, Read-only-Export. | ELN/EHR |
| F-223 | Selektive Freigabe | Nur ausgewählte Inhalte exportieren/teilen. | MyChart |
| F-224 | Export-Warnung | Vor Export sensibler Daten warnen. | Datenschutz |
| F-225 | Audit-Trail | Änderungen an wichtigen Daten nachvollziehen. | SciNote, ELN, Paperless |
| F-226 | Änderungsverlauf | Alte Versionen von Notizen behalten. | RSpace, Obsidian-Git |
| F-227 | Soft Delete | Archiv/Papierkorb statt hartem Löschen. | DMS |
| F-228 | Backup | Manuelles und automatisches verschlüsseltes Backup. | DMS/SASD |
| F-229 | Restore-Test | Backup regelmäßig testbar machen. | IT-Betrieb |
| F-230 | Prüfsummenmanifest | Exporte und Backups mit Hashes prüfen. | GNU Health Crypto |
| F-231 | Datenminimierung | Nur erfassen, was wirklich gebraucht wird. | DSGVO |
| F-232 | Sensibilitätsstufen | Normal, vertraulich, sehr vertraulich. | Datenschutz |
| F-233 | Lokale Protokolle | Fehler- und Auditlogs ohne Gesundheitsinhalte oder mit Redaction. | Security |
| F-234 | Secret Scanner | Warnen, wenn API-Keys/Passwörter in Notizen geraten. | SASD Prompt Manager-Idee |
| F-235 | Datenschutzmodus | Bildschirmansicht blendet sensible Werte aus. | PHR/Privacy |
| F-236 | Panic Lock | Schnell sperren/minimieren. | Privacy |
| F-237 | Datenportabilität | Nutzer kann alle Daten exportieren. | DSGVO/PHR |
| F-238 | Offlinefähigkeit | Voll nutzbar ohne Internet. | Local-first |
| F-239 | Update-Sicherheit | Signierte Updates später. | Desktop Security |
| F-240 | Kein Vendor Lock-in | Markdown/JSON/SQLite/Dokumente offen dokumentieren. | SASD-Stil |

### 4.12 KI-gestützte Funktionen – nur optional und später

| ID | Feature | Beschreibung | Vorbilder |
|---|---|---|---|
| F-241 | Zusammenfassung von Arztbriefen | Arztbrief in einfache Stichpunkte zusammenfassen; immer mit Quellenstellen. | Guava AI-Idee |
| F-242 | Dokumentklassifikation | OCR-Text → Vorschlag: Laborbericht, Arztbrief, Rechnung. | Paperless + AI |
| F-243 | Tag-Vorschläge | Automatische Tags anhand Inhalt. | Paperless-ngx |
| F-244 | Quellenzusammenfassung | Studie/Webseite kurz zusammenfassen. | Zotero/AI |
| F-245 | Widerspruchserkennung | Quelle A sagt X, Quelle B sagt Y; Nutzer entscheidet. | Evidence Notebook |
| F-246 | Arztfragen vorschlagen | Aus offenen Notizen mögliche Fragen ableiten. | PHR |
| F-247 | Timeline-Zusammenfassung | Verlauf in verständlicher Prosa zusammenfassen. | AI |
| F-248 | Laborwert-Extraktion | Werte aus PDF/OCR vorschlagen, aber manuell bestätigen. | Guava/Paperless |
| F-249 | Begriffserklärung | Medizinische Begriffe in einfacher Sprache erklären. | Apple Health/Guava |
| F-250 | Lokales LLM bevorzugt | Bei Gesundheitsdaten möglichst lokale Modelle oder explizite Zustimmung. | Datenschutz |
| F-251 | Quellenpflicht | KI-Antworten immer mit Quelle/Belegstelle. | Trustworthy AI |
| F-252 | Kein Diagnosemodus | KI darf nicht als Diagnoseassistent auftreten. | MDR-Risiko |
| F-253 | Halluzinationswarnung | KI-Ergebnisse als Vorschläge kennzeichnen. | AI Safety |
| F-254 | Redaction vor Cloud-KI | Personenbezogene Daten entfernen, wenn externer Dienst genutzt wird. | Datenschutz |
| F-255 | KI-Protokoll | Welche Daten wurden an welches Modell gegeben? | Audit |

### 4.13 Technische Architektur und Betrieb

| ID | Feature | Beschreibung |
|---|---|---|
| F-256 | Desktop-App zuerst | Windows Desktop als erste Plattform, passend zu SASD-Desktopprojekten. |
| F-257 | C#/.NET | Gute Passung zu bestehenden SASD-Projekten. |
| F-258 | WPF oder WinForms | WPF für langfristig bessere UI; WinForms für schnellen Start. |
| F-259 | SQLite | Lokale relationale Datenbank, gut für V1. |
| F-260 | SQLite FTS5 | Volltextsuche für Notizen und Dokumenttext. |
| F-261 | Dateisystem-Dokumentstore | Dokumente als Dateien, Metadaten in SQLite. |
| F-262 | Verschlüsselter Vault später | Datenbank + Dokumente in sicherem Container. |
| F-263 | Repository Pattern | Saubere Trennung von UI, Application, Domain, Infrastructure. |
| F-264 | Domain-Modell | Conditions, Entries, Documents, Sources, Measurements, Medications, Appointments. |
| F-265 | Migrationen | Datenbankschema versionieren. |
| F-266 | Import-Pipeline | Dokumentimport mit Schritten: Datei aufnehmen, OCR, Klassifikation, Zuordnung. |
| F-267 | Export-Pipeline | Markdown/PDF/ZIP mit Manifest. |
| F-268 | Plugin-Schnittstelle später | Für Importer, Exporter, KI, Geräte. |
| F-269 | Testdatenmodus | Demo-Daten ohne echte Gesundheitsdaten. |
| F-270 | Unit Tests | Domain-Logik und Services testen. |
| F-271 | Integrationstests | Datenbank, Import, Export, Suche testen. |
| F-272 | UI Smoke Tests manuell | Start, Eintrag anlegen, Dokument importieren, Export prüfen. |
| F-273 | Logging ohne Gesundheitsdaten | Logs dürfen keine Befundtexte oder Diagnosen enthalten. |
| F-274 | Fehlerdialoge | Technisch hilfreich, aber datenschutzarm. |
| F-275 | Updatefähiges Schema | Frühe Datenmodelle dürfen später erweiterbar sein. |
| F-276 | Portable Mode | App und Daten auf verschlüsseltem Laufwerk nutzbar. |
| F-277 | Mehrere Vaults | Privat, Demo, Test, ggf. Angehörige getrennt. |
| F-278 | Backup-Assistent | Nutzer versteht, wo Daten liegen und wie gesichert wird. |
| F-279 | Datenexport-Assistent | Kein Lock-in. |
| F-280 | Entwicklerdokumentation | Datenmodell, Sicherheitsmodell, Exportformat dokumentieren. |

---
---

# Anhang B – Erste Priorisierung nach MoSCoW

## B.1 Must-have für V1

- lokale Desktop-Anwendung ohne Cloud-Zwang,
- Condition-/Gesundheitsthema-Verwaltung,
- Wizard zum Anlegen neuer Conditions,
- Symptome und Beobachtungen,
- Dokumentenimport mit Hash und Metadaten,
- Quellenverwaltung,
- Arztfragen,
- Termine/Gesprächsnotizen in einfacher Form,
- Tags und Synonyme,
- Suche,
- Timeline,
- Markdown-Export,
- Backup/Restore,
- Soft Delete/Archiv,
- Auditlog für wichtige Aktionen,
- klare medizinische Abgrenzung.

## B.2 Should-have für frühe Folgeversion

- PDF-Export,
- Dokumentvorschau,
- Dublettenprüfung,
- einfache Labor-/Vitalwerttabellen,
- Reportvorlagen,
- Vorlagen für Condition-Wizard,
- verschlüsselte Backups,
- UI-Verbesserungen,
- einfache Diagramme,
- Importordner.

## B.3 Could-have

- OCR,
- PDF-Annotationen,
- Web Clipper,
- Zotero-/PubMed-/DOI-Integration,
- Kalenderintegration,
- Körperkarte,
- Geräteimport,
- FHIR-Export,
- mobile Begleit-App,
- lokale KI-Zusammenfassungen.

## B.4 Won't-have in V1

- Diagnoseautomat,
- Therapieempfehlungen,
- Medikamentenwechselwirkungsprüfung,
- Notfalltriage,
- Cloud-Synchronisation,
- Community-Funktionen,
- direkte Arztkommunikation,
- automatisches Patientenportal-Scraping,
- externe KI ohne explizites Opt-in.

---

# Anhang C – Vorschlag für erste Repository-Beschreibung

**Kurzbeschreibung für GitHub:**

> Local-first desktop application for organizing personal health research, findings, documents, symptoms, questions, sources and timelines. Built as a privacy-focused SASD notebook, not as a diagnostic or therapy recommendation tool.

---

# Anhang D – Vorschlag für erste Commit-Reihenfolge

1. `docs: add initial health notebook product specification`
2. `chore: create solution and project structure`
3. `feat: add application shell and navigation`
4. `feat: add domain model foundation`
5. `feat: add sqlite persistence baseline`
6. `feat: add condition management`
7. `feat: add condition wizard draft workflow`
8. `feat: add document import and hashing`
9. `feat: add sources and questions`
10. `feat: add timeline and markdown export`
11. `feat: add backup and restore baseline`

---

# Anhang E – Glossar

| Begriff | Bedeutung |
|---|---|
| Condition | Erkrankung, Verdacht, Problemkomplex oder Gesundheitsthema. |
| HealthEntry | Allgemeiner dokumentierter Eintrag. |
| SymptomObservation | Konkrete Beobachtung eines Symptoms zu einem Zeitpunkt. |
| Source | Quelle einer Information, z. B. Studie, Webseite, Arztgespräch. |
| Audit-Trail | Nachvollziehbares Änderungsprotokoll. |
| Soft Delete | Archivieren statt endgültig löschen. |
| FHIR | Standard für den elektronischen Austausch von Gesundheitsinformationen. |
| ELN | Electronic Lab Notebook. |
| PHR | Personal Health Record. |
| OCR | Texterkennung aus Bildern/Scans. |
| Medizinprodukt-Grenze | Grenze, ab der Software wegen medizinischer Zweckbestimmung regulatorisch relevant werden kann. |
