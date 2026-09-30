# 000 - Dokumentationsübersicht

Projekt: SASD Health Research Notebook  
Stand: 2026-09-30  
Dokumenttyp: Projekt- und Dokumentationslandkarte  
Status: Baseline 2.1

## 1. Zweck

Diese Seite ist der Einstiegspunkt für die verbindliche Projektdokumentation.

Das Projekt verarbeitet potenziell hochsensible Gesundheitsinformationen. Dokumentation, Sicherheitsgrenzen, Fachmodell und technische Umsetzung müssen deshalb gemeinsam weiterentwickelt werden.

## 2. Aktueller Projektstand

Das Repository befindet sich nicht mehr nur in der Konzeptphase. Es existieren zwei startbare Desktop-Frontends über denselben Domain/Application/Infrastructure-Schichten und derselben lokalen JSON-Persistenz:

- WinForms als aktuelles primäres Entwicklungsfrontend mit Dashboard, Navigation, HealthTopic-Grid, Wizard und Deutsch/Englisch-Lokalisierung;
- WPF als buildbare Referenzoberfläche und Kompatibilitätscheck.

Kurzfristiger technischer Fokus:

1. WinForms visuell weiter an die Konzept-Screenshots annähern;
2. bestehende HealthTopic-Funktion und gemeinsame Persistenz stabil halten;
3. WPF buildbar halten, aber neue Features nicht doppelt entwickeln;
4. WinForms-Navigation und Views als Basis für spätere Fachmodule stabilisieren;
5. anschließend vertikale Fach-Slices entwickeln.

## 3. Medizinische Produktgrenze

Das Health Research Notebook dokumentiert und organisiert. Es diagnostiziert nicht und gibt keine Therapie-, Dosis- oder Medikamentenentscheidungen aus.

Vom Nutzer eingetragene Ziele/Routinen sowie dokumentierte professionelle Anweisungen müssen in Datenmodell und UI von automatisch erzeugten Empfehlungen klar unterscheidbar bleiben. Automatische medizinische Empfehlungen sind nicht Teil der Baseline.

## 4. Dokumentenlandkarte

| Dokument | Zweck |
|---|---|
| `README.md` | öffentliche Projektübersicht und Build-Einstieg |
| `AGENTS.md` | verbindliche Repository-Anweisungen für Codex/Agenten |
| `docs/SASD_Health_Research_Notebook_Lastenheft.md` | fachliche Ausgangsanforderungen |
| `docs/SASD_Health_Research_Notebook_Pflichtenheft.md` | technische Ausgangsspezifikation |
| `docs/changes/160_Dokumentationsrevision_2_1.md` | konsolidierte Änderung Baseline 2.1 |
| `docs/requirements/150_Feature_Backlog_und_Anforderungskatalog.md` | breiter Feature-Backlog |
| `docs/requirements/155_Fachmodule_Baseline_2_1.md` | verbindliche IDs für neue Fachmodule |
| `docs/architecture/030_Architekturkonzept.md` | Schichten und Systemarchitektur |
| `docs/database/040_Datenmodell_Datenbankdesign.md` | Datenmodell und Persistenzplanung |
| `docs/ui-ux/050_UI_UX_Konzept.md` | übergreifendes UI-/UX-Konzept |
| `docs/ui-ux/055_UI_Zielbild_und_Screenshot_Plan.md` | konkreter Weg zum Screenshot-Ziel |
| `docs/security/060_Sicherheits_Datenschutzkonzept.md` | Datenschutz- und Sicherheitsmodell |
| `docs/knowledge/070_Dokumenten_Quellenkonzept.md` | Dateien, Quellen, Fundstellen |
| `docs/knowledge/080_Such_Wissenskonzept.md` | Suche, Beziehungen, Timeline |
| `docs/export/090_Import_Exportkonzept.md` | Import, Export und Arztmappe |
| `docs/testing/100_Testkonzept.md` | Teststrategie |
| `docs/roadmap/110_Roadmap.md` | Phasenübersicht |
| `docs/roadmap/115_Milestone_und_Release_Plan.md` | detaillierte Meilensteine |
| `docs/adr/130_Architekturentscheidungen_ADR.md` | Architekturentscheidungen |
| `docs/development/140_Repository_Struktur_Entwicklungsleitlinien.md` | allgemeine Entwicklungsregeln |
| `docs/development/145_Codex_Arbeitsauftrag.md` | konkrete Codex-Arbeitsweise |

## 5. Fachliches Zielbild Baseline 2.1

Das Zielbild umfasst langfristig:

- HealthTopic;
- Observation / SymptomObservation;
- Measurement;
- NutritionEntry;
- ContextSnapshot / WeatherSnapshot;
- Session für Arzt, Coaching, Therapie/Beratung;
- Question;
- HealthAction;
- Routine / RoutineProgress;
- Reminder;
- Source / SourceLocation / EvidenceNote;
- Document / MediaResource;
- ContactReference.

Diese Liste ist ein Zielmodell. Nicht alle Objekte werden sofort implementiert.

## 6. Dokumentationsprinzipien

1. Repository-Stand vor Chat-Erinnerung.
2. Kleine, nachvollziehbare Änderungen.
3. Sicherheitsentscheidungen schriftlich festhalten.
4. Lasten-/Pflichtenheft nicht bei jeder Idee vollständig umschreiben; Revisionen versionieren.
5. Quellen, Aussagen, eigene Interpretation und professionelle Dokumente unterscheiden.
6. Lokal-first und keine Gesundheitsdaten in Logs.
7. Archivieren statt still löschen.
8. Neue Fachmodule als vertikale Slices.
9. UI-Zielbilder dürfen keine statischen Mock-ups anstelle funktionierender Software erzeugen.
10. Codex/Agenten müssen `AGENTS.md` beachten.

## 7. Nächste Arbeit

1. WinForms-Primärstrategie als Dokumentationsnachtrag übernehmen.
2. Nächsten Codex-UI-Sprint nach `145_Codex_Arbeitsauftrag.md` auf WinForms ausführen.
3. WinForms-Dashboard visuell weiter an das Konzeptbild angleichen.
4. WinForms-Wizard visuell vereinheitlichen.
5. Navigation Host stabilisieren.
6. WPF als Referenz buildbar halten.
7. Danach erster Fach-Slice: HealthEntry/Timeline oder Sources/SourceLocation.
