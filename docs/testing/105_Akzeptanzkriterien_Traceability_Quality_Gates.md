# 105 - Akzeptanzkriterien, Traceability und Quality Gates

Projekt: SASD Health Research Notebook  
Stand: 2026-10-05
Dokumenttyp: Requirements-Traceability / Akzeptanz- und Release-Qualität  
Status: aktiv

## 1. Zweck

Dieses Dokument verbindet Anforderungen, Akzeptanzkriterien, Implementierung und Tests.

Das Ziel ist nicht nur, dass unterschiedliche Entwickler oder Codex-Läufe "ähnlichen" Code erzeugen. Das Ziel ist, dass unabhängig vom verwendeten Entwicklungsmodus dieselbe **beobachtbare fachliche Funktionalität** nachgewiesen werden kann.

Dafür gilt:

> Eine Anforderung gilt nicht als umgesetzt, nur weil Code existiert oder die Solution baut. Sie gilt als umgesetzt, wenn ihr Akzeptanzkriterium erfüllt und angemessen geprüft ist.

## 2. Quellen der Anforderungen

Die wesentlichen Anforderungsquellen sind:

1. `docs/SASD_Health_Research_Notebook_Pflichtenheft.md` mit `PF-*`-IDs und Akzeptanzkriterien;
2. `docs/requirements/155_Fachmodule_Baseline_2_1.md` mit neueren `FR-*`-IDs;
3. `docs/changes/160_Dokumentationsrevision_2_1.md` für spätere Präzisierungen und Vorrangregeln;
4. akzeptierte ADRs für technische Entscheidungen.

Bei Widersprüchen gilt: aktuelle Branch-Realität und explizit neuere Baseline-/ADR-Entscheidungen gehen älteren Entwurfsformulierungen vor. Ein fachlicher Widerspruch darf nicht still durch Implementierungsannahmen "gelöst" werden.

## 3. Qualität eines Akzeptanzkriteriums

Ein Akzeptanzkriterium soll so konkret sein, dass zwei unabhängige Implementierungen gegen dasselbe Ergebnis geprüft werden können.

Bevorzugtes Muster:

- **Given**: definierter Ausgangszustand;
- **When**: konkrete Benutzeraktion oder Systemaktion;
- **Then**: beobachtbares Ergebnis;
- **And**: relevante Datenintegritäts-, Datenschutz- oder Fehlerfallbedingungen.

Beispiel:

**PF-CON-003 – Synonyme**

Given eine Condition "Arterielle Hypertonie" mit Synonym "Bluthochdruck",  
When nach "Bluthochdruck" gesucht wird,  
Then muss die Condition "Arterielle Hypertonie" im Suchergebnis erscheinen.

Ungeeignete Kriterien sind Formulierungen wie "funktioniert gut", "ist sinnvoll" oder "ist benutzerfreundlich", sofern sie nicht durch konkrete Prüfpunkte ergänzt werden.

## 4. Test-ID-Konvention

Neue Tests sollen eine nachvollziehbare ID bzw. eindeutige Zuordnung zur Anforderung erhalten.

| Präfix | Testart |
|---|---|
| `UT-` | Unit Test |
| `IT-` | Integration Test |
| `ST-` | technischer Smoke Test |
| `E2E-` | End-to-End Workflow |
| `SEC-` | Security-/Privacy-Test |
| `MIG-` | Migration-/Kompatibilitätstest |
| `UI-` | UI-Automation oder dokumentierter manueller UI-Test |

Der Testname im Code darf weiterhin lesbar nach Verhalten benannt sein. Die Requirement-ID soll im Testnamen, Trait/Category, Kommentar oder in der Traceability-Tabelle auffindbar sein.

## 5. Traceability-Matrix

Für jede release-relevante MUSS-Anforderung soll langfristig folgende Beziehung nachvollziehbar sein:

```text
Requirement ID
    -> Akzeptanzkriterium
        -> Implementierung
            -> automatisierter Test / dokumentierter manueller Test
                -> CI-/Release-Status
```

Empfohlenes Tabellenformat:

| Requirement | Akzeptanz | Test-ID(s) | Automatisiert | Status | Bemerkung |
|---|---|---|---:|---|---|
| PF-... | kurz zusammengefasst | UT-/IT-/E2E-... | ja/nein | offen/teilweise/erfüllt | Hinweise |
| FR-DEV-001 | Ohne Override unveränderter Produktpfad (nur auflösen); mit absolutem Override isoliertes JSON inklusive Backup; ungültige Overrides abweisen. Smoke-Test-Ziel außerhalb Repository abweisen. | SEC-PATH-001, IT-PATH-001, ST-JSON-001 | ja | erfüllt | Tests der gemeinsamen Application-/Infrastructure-Verträge mit normalen ProjectReferences. Beide Bootstrapper im Review auf parameterlosen gemeinsamen Repository-Konstruktor geprüft; manuelle Nutzerakzeptanz nach PowerShell-5.1-Reparatur in Dokument 146 festgehalten. |

| FR-UI-001 | Deutsche/englische Aktionen und Grunddaten bei Mindestgröße ohne Clipping; Navigation mit Fokus; leere Liste und stabile Refresh-Auswahl; synthetisches Anlegen/Wiederladen. | UI-LAYOUT-001, UI-WIZARD-001, UI-SRC-LAYOUT-001 | teilweise | erfüllt | Automatisierte Controls und Rendervergleich; manuelle Baseline-Akzeptanz bestätigt, PR #11 gemergt, siehe Dokument 147. Sources-Detailfelder zusätzlich bei Normal-/Mindestgröße in beiden Tabs gleich hoch/ausgerichtet geprüft; Dokument 149. |
| FR-OBS-001 | Note/Observation/Research mit fachlichem Zeitpunkt, Titel, Text und optionalem vorhandenem Thema erstellen; chronologisch wiederladen; Themen-JSON unverändert. | UT-ENTRY-001, IT-ENTRY-001, SEC-ENTRY-001, SEC-ENTRY-002, UI-ENTRY-001 | teilweise | erfüllt (Slice 1) | Domain/Service/JSON, Launcher-Dateiguards und echter WinForms-Dialog in Deutsch/English automatisiert; manuelle Nutzerakzeptanz am 2026-10-01 bestätigt. Siehe Dokument 148. |
| FR-SRC-001, FR-SRC-002, FR-SRC-003 | Sieben Quellenkategorien, Metadaten und optionale externe ID erstellen/wiederladen; Mindestvalidierung. | UT-SRC-001, IT-SRC-001, UI-SRC-001 | teilweise | erfüllt (Slice 1; manuell akzeptiert) | Gemeinsame Domain/Application und WinForms-Dialog DE/EN; siehe Dokument 149. |
| FR-SRC-004 | Mehrere konkrete Fundstellen gehören einer vorhandenen Quelle; nach Neustart und erneuter Quellenauswahl ohne Refresh sichtbar. | UT-SRC-001, IT-SRC-001, UI-SRC-001, IT-SRC-RELOAD-001, UI-SRC-RELOAD-001 | teilweise | Slice 1 manuell akzeptiert; Auswahlregression automatisiert abgesichert | Frische Instanzen, originale Location-IDs/Metadaten, zwei Quellen und Wechsel ohne verdeckenden Refresh. Ursache/Reparatur in Dokument 149. |
| FR-SRC-005, FR-SRC-006 | Aussage, Originalexzerpt und eigene Einordnung getrennt; nur Fundstellen derselben Quelle zulässig. | UT-SRC-001, IT-SRC-001, SEC-SRC-001, UI-SRC-001 | teilweise | erfüllt (Slice 1; manuell akzeptiert) | Application prüft Referenzen, Store prüft unter Writer-Lock erneut; keine Bewertung. |
| FR-DEV-001 / Sources | Gemeinsamer Datenpfad und vier Sources-Dateiguards; bestehende Topic-/Entry-Dateien unverändert. | IT-SRC-001, SEC-SRC-001, SEC-SRC-002 | ja | automatisiert geprüft | Version/Kennung, fremde/beschädigte Stores, Backup, Temp/Lock und tatsächlicher PowerShell-5.1-Launcher. |
| FR-MEA-001, FR-MEA-002 | Blutdruckkomponenten und optionaler Puls getrennt; alle fünf Typen numerisch erstellen/wiederladen, Einheiten sichtbar. | UT-MEA-001, IT-MEA-001, UI-MEA-001/002 | teilweise | implementiert; Blutdruck-Feldkorrektur manuell akzeptiert | Domain/Application, separater Store und echte WinForms-Dialoge DE/EN; Nutzerakzeptanz am 2026-10-01 in Dokument 150. PR #18 korrigiert Create-Initialisierung und trennt Systolic von Value; UI-MEA-002 prüft Create/Edit, Typwechsel, Tab und versteckte Werte. Nutzer bestätigt die korrigierte Blutdruckerfassung; Measurement-Type-Filter ebenfalls manuell akzeptiert; Abschlussbestätigung 2026-10-05 in Dokument 153. |
| FR-MEA-004, FR-MEA-005 | Zeitpunkt/Offset, typgebundene Einheit, Erfassungsart Manual, optionale Messsituation und persönliche Notiz getrennt erhalten. | UT-MEA-001, IT-MEA-001, UI-MEA-001 | teilweise | implementiert und manuell akzeptiert (Slice 1) | Keine Umrechnung, Zahlen sind keine formatierten Strings. |
| FR-MEA-006 | Optionales vorhandenes Thema zuordnen; aktuellen/archivierten Titel auflösen; fehlendes Thema verliert keine Messung. | IT-MEA-001, UI-MEA-001 | ja | teilweise (Themenbezug) | Dokument-/Beobachtungs-/Session-Verknüpfungen noch offen. Keine redundanten Titel oder Cascading Deletes. |
| FR-MEA-008, FR-UI-001 | Reiner Messart-Anzeigefilter; Alle/fünf Typen, eigener Empty State, sichere sichtbare Auswahl, Filter bleibt bei Edit/Delete/Refresh/Sprachwechsel. | UI-MEA-003 | teilweise | implementiert und manuell akzeptiert (Bestätigung 2026-10-05) | DE/EN, bytegenaue Stores bei Filteraktionen, Neustart mit Alle, Mindestgröße/Tab/Clipping; Dokument 153. |
| FR-LIF-004/005 | Topic-Lebenszyklus mit aktuellem/historischem Referenzschutz; Frage Edit/unbeantwortet Delete; Follow-up Edit/bestätigt Delete. | IT-LIF-002, UI-LIF-002 | teilweise | implementiert und manuell akzeptiert (2026-10-05) | Multi-Store-Locks, late-writer-Abweisung, Stale-Tokens, Antworten-/Kindererhalt, DE/EN und Restart; Dokument 153. |
| FR-MEA-007 | Nur Struktur, Pflichtwerte, Endlichkeit und nichtnegative Zahlen prüfen; keine medizinischen Schwellen oder Bewertung. | UT-MEA-001, UI-MEA-001, Code-Review | teilweise | implementiert und manuell akzeptiert (Slice 1) | Technisch darstellbare Extremwerte bleiben zulässig; keine Ampeln/Alarme. |
| FR-DEV-001 / Measurements | Gemeinsamer Pfad und vier Messwert-Dateiguards; Topic-/Entry-/Source-Dateien bytegenau erhalten. | IT-MEA-001, SEC-MEA-001, SEC-MEA-002 | ja | automatisiert geprüft | Kennung/Version, beschädigte/fremde/zukünftige Stores, Backup, bestehende Temp-/Lock-Dateien, Windows PowerShell 5.1. |
| FR-SES-001, FR-SES-002 | Sechs Gesprächstypen, Zeitpunkt, Titel/Anlass, Status, optionales Thema/Kontaktfreitext erstellen und chronologisch laden. | UT-SES-001, IT-SES-001, UI-SES-001 | teilweise | implementiert und manuell akzeptiert (Slice 1) | Gemeinsame Schichten, WinForms DE/EN; Nutzerakzeptanz am 2026-10-01, Dokument 151. Kontaktreferenzmodul bleibt offen. |
| FR-SES-003, FR-SES-004, FR-SES-008 | Nutzerfragen, explizite Reihenfolge, getrennte Antwortnotiz und eigene Gesprächsnotizen speichern; beantwortet/offen ändern. | UT-SES-001, IT-SES-001, UI-SES-001 | teilweise | Slice-1-Teilumfang manuell akzeptiert | Keine medizinische Interpretation; Dokument-/Messwert-/Quellenverknüpfungen und späteres Bearbeiten der Session-Notiz bleiben offen. |
| FR-SES-007, PF-APT-006 | Eigene nächste Schritte mit optionalem Kalenderdatum erfassen, erledigen/wieder öffnen und nach Neustart erhalten. | UT-SES-001, IT-SES-001, UI-SES-001 | teilweise | implementiert und manuell akzeptiert (Slice 1) | Keine Reminder oder automatisch erzeugten Aufgaben. |
| FR-DEV-001 / Sessions | Versionierter atomarer Session-/Question-/FollowUp-Store, Elternreferenzen, vier Dateiguards, vier bestehende Stores unverändert. | IT-SES-001, SEC-SES-001, SEC-SES-002 | ja | automatisiert geprüft | Isolierte synthetische Daten und echter PowerShell-5.1-Launcher. |

Die Matrix muss nicht für alle zukünftigen KANN-/SPÄTER-Anforderungen vorab ausgefüllt werden. Sie wächst mit den implementierten vertikalen Slices.

## 6. Aktuelle Test-Baseline

Der aktuelle Stand besitzt:

- Release-Build in GitHub Actions;
- den dependency-light Smoke-Test `tests/Sasd.HealthNotebook.SmokeTests`;
- automatisierte Ausführung des Smoke-Tests in `.github/workflows/dotnet.yml`.

Der vorhandene Smoke-Test prüft derzeit insbesondere:

- Erzeugen eines synthetischen HealthTopic über den Application Service;
- Schreiben über das JSON Repository;
- erneutes Laden aus einer neuen Repository-/Service-Instanz;
- Erhalt des Titels;
- grundlegende Dashboard-Zähler.

Arbeits-ID für diese Prüfung:

- `ST-JSON-001` – Shared JSON persistence round trip.

Diese Prüfung ist wertvoll, aber **keine ausreichende fachliche Testabdeckung** des Pflichtenhefts. Sie deckt nur einen kleinen Ausschnitt der HealthTopic-/Persistenz-Baseline ab.

Insbesondere fehlen derzeit noch echte automatisierte Unit-/Integrationstests für große Teile der `PF-*`- und `FR-*`-Anforderungen.

## 7. Mindestregel für neue Implementierung

Ab sofort gilt für neue oder wesentlich geänderte MUSS-Funktionalität:

1. Requirement-ID bestimmen.
2. Akzeptanzkriterium prüfen und bei Bedarf konkretisieren.
3. Implementierung in kleinem vertikalem Slice durchführen.
4. Mindestens einen Erfolgsfall automatisiert prüfen, sofern technisch sinnvoll.
5. Relevante Fehler-/Grenzfälle automatisieren, insbesondere bei Datenintegrität, Import/Export, Backup/Restore, Migration und Datenschutz.
6. Nicht sinnvoll automatisierbare visuelle Kriterien als `UI-*`-Prüfung dokumentieren.
7. CI muss grün sein.
8. Traceability-Zuordnung aktualisieren.

Eine Funktion darf nicht deshalb ohne Test bleiben, weil der verwendete Codex-Modus schnell arbeiten soll.

## 8. Release Quality Gates

Ein Release Candidate darf erst als fachlich prüfbar gelten, wenn mindestens:

- Release-Build erfolgreich ist;
- alle automatisierten Tests erfolgreich sind;
- keine bekannten stillen Datenverlustpfade offen sind;
- Migration/Backward Compatibility für persistierte Daten geprüft ist;
- Security-/Privacy-Kernregeln geprüft sind;
- alle für den Release vorgesehenen MUSS-Anforderungen einen nachvollziehbaren Status besitzen;
- offene Abweichungen ausdrücklich als bekannte Lücke dokumentiert sind;
- die wichtigsten End-to-End-Workflows manuell oder automatisiert geprüft wurden.

Für das Health Research Notebook gehören zu den besonders kritischen Release-Bereichen:

- Speichern und Wiederladen;
- Archivieren/Wiederherstellen;
- Migrationen;
- Import/Export;
- Backup/Restore;
- keine Gesundheitsinhalte in Logs;
- keine stillen Überschreibungen;
- keine medizinisch übergriffige automatische Bewertung.

## 9. Umgang mit Codex-Iterationsschleifen

Mehrere Korrekturschleifen sind nicht automatisch ein Qualitätsproblem. Sie werden problematisch, wenn die Schleifen aus unklaren Anforderungen oder fehlenden Tests entstehen.

Zur Begrenzung von Wiederholungen:

- kleine, abgeschlossene Slices bevorzugen;
- Requirement-ID und Akzeptanzkriterium im Auftrag nennen;
- vorhandene Tests vor Änderungen lesen, wenn sie den betroffenen Bereich abdecken;
- Tests nach jeder kohärenten Änderung ausführen;
- bei wiederholtem Fehlschlag die Ursache analysieren statt nur Symptome zu patchen;
- bei Architektur-, Persistenz-, Sicherheits- oder Datenverlustthemen eine High-effort-Prüfung einsetzen.

## 10. Professionalisierungsstufe

Schnell erzeugter Code kann später auf ein professionelles Niveau gebracht werden, **wenn** die fachliche Semantik erhalten bleibt und der Code nicht bereits schwer rückbaubare technische Schulden in Persistenzformaten, Datenmigrationen, Sicherheitsgrenzen oder öffentlichen Schnittstellen verankert hat.

Deshalb soll die Professionalisierung nicht als ein einziger "Optimierungslauf am Ende" verstanden werden, sondern als gestufter Prozess:

1. funktionale Baseline;
2. automatisierte fachliche Absicherung;
3. Architektur-/Refactoring-Pass;
4. Security-/Privacy-/Data-Integrity-Pass;
5. Performance nur mit Messdaten optimieren;
6. Release-Review gegen Requirements und Traceability.

Ein High-effort-Review kann einfachen oder mittelreifen Code deutlich verbessern. Er ersetzt aber keine fehlenden Akzeptanzkriterien und keine Tests, die das gewünschte Verhalten objektiv festhalten.

## 11. Traceability – HealthAction/Routine/Progress Slice 1

| Anforderungen | Akzeptanz und Nachweis | Stand / Grenzen |
|---|---|---|
| FR-ACT-001/002/003/005, FR-GEN-001/002/003/005/006/007 | Manuelle Action mit optionalem Thema und berichteter Herkunft; UT/IT/SEC-ACT-001, UI-ACT-001 | Implementiert und manuell akzeptiert (Slice 1); keine automatisch abgeleitete Maßnahme oder medizinische Bewertung. |
| FR-ACT-004 (Teil) | Titel, Status, Beschreibung, eigene Historie; UT/IT-ACT-001 | Zeitraum/Action-Bearbeitung offen; Herkunft und Inhalt create-only. |
| FR-ROU-001/002 | Getrennte Routine zu bestehender Action, geprüfte Elternbeziehungen; UT/IT/SEC-ACT-001, UI-ACT-001 | Eigenständige Routine ohne Action offen. |
| FR-ROU-003/004 (Teil) | Persönlicher Rhythmus als Text, explizite Durchführung/optionale Anzahl; UT/IT-ACT-001, UI-ACT-001 | Strukturierte Planung/Menge/Dauer/Zielquoten offen. |
| FR-ROU-006/007 | Historie, Pausieren/Reaktivieren und Skipped-Eintrag ohne Löschung; UT/IT-ACT-001, UI-ACT-001 | Keine automatische Tagesaggregation oder Reminder. |
| FR-DEV-001 | Isolierte synthetische Daten, vier Dateiguards; SEC-ACT-001/002 | Windows PowerShell 5.1; vorhandene Stores bytegenau erhalten. |
| FR-UI-001 (erweiterter Teilumfang) | Neue Seite DE/EN, gleiche Oberkanten, proportionale Panels, kompakte Karten, sichtbare Buttons bei Mindestgröße; UI-ACT-002 | Renderprüfung plus bestehende Regressionen; manuelle Nutzerakzeptanz am 2026-10-03 bestätigt (Dokument 152). |
| Dokumentative Dashboardzählung | Aktive Actions/Routinen, alle heutigen Einträge; UT-ACT-001, DASH-ACT-001 | Keine Gesundheits-/Wirksamkeitsbewertung. |

FR-ACT-006 und übrige FR-ROU-003/004/005 sind nicht als vollständig umgesetzt
markiert. Die Lebenszyklusregeln in Dokument 152 sind verbindliche Vorgaben;
Bearbeiten/Archivieren/Löschen sind noch nicht implementiert oder durch Tests als
erfüllt nachgewiesen. Pausieren/Reaktivieren ist keine Archivierung.
Slice 1 ist implementiert und manuell akzeptiert (2026-10-03); genaue manuelle Abdeckung in Dokument 152. Bearbeiten/Archivieren/Löschen sind ausdrücklich Folgearbeit und kein Blocker dieses Slices. Ready for review nach grüner Abschlussprüfung/CI und synchronem Branch; die allgemeinen Release-Quality-Gates bleiben verbindlich.


## Traceability – Edit / Archive / Delete Baseline Slice 1

| Requirement | Akzeptanz / Test | Stand |
|---|---|---|
| FR-LIF-001 | Sechs Objekte korrigierbar, Id/CreatedAt und Referenzen erhalten, ModifiedAt steigt; No-op, Concurrency und Offsetpräzision; IT-LIF-001, UI-LIF-001 | Implementiert, manuelle Abnahme offen |
| FR-LIF-002 | Separates Session-/Action-Archiv, Filter/Reaktivierung und Kindschutz; IT-LIF-001, UI-LIF-001 | Implementiert; manuelle Lifecycle-Abnahme offen, Routine-Archiv bleibt Folgearbeit |
| FR-LIF-003 | Drei sichere Einzellöschungen mit Default-Cancel/Bestätigung und Reload; keine Parent-Delete-API; IT/SEC-LIF-001, UI-LIF-001 | Implementiert, manuelle Abnahme offen |
| FR-ACT-006 | Vorherige professionelle/quellenbasierte Action-Inhalte/Herkunft atomar in Revision, read-only Historie inkl. Verweise; IT-LIF-001, UI-LIF-001 | Slice-1-Lösung implementiert, manuelle Abnahme offen |
| FR-GEN-003/005/007, FR-DEV-001 | Keine Bewertung/Logs, Referenzerhalt, isolierte Daten, v1 ohne Rewrite/v2-Write/Backup/Temp/Lock; SEC/MIG-LIF-001 | Automatisierte Prüfung, bekannte Link-Race unverändert |
| FR-LIF-004 | Topic Edit/Archive/Reactivate; Delete ohne aktuelle/historische Referenzen, sonst Sperre; IT-LIF-002, UI-LIF-002 | Implementiert und manuell akzeptiert am 2026-10-05 anhand der Prüfliste in Dokument 153; Einzelreferenz-/Concurrency-Matrix automatisiert |
| FR-LIF-005 | Question Edit, Delete nur unbeantwortet ohne Antwortnotiz; FollowUp Edit und Delete Cancel/Confirm; IT-LIF-002, UI-LIF-002 | Implementiert und manuell akzeptiert am 2026-10-05, einschließlich Neustart, DE/EN, Tastatur und Mindestgröße |

Diese neuere Ergänzung ersetzt die historischen create-only/Lebenszyklus-offen-Aussagen
in den früheren Slice-Zeilen. Keine allgemeine Audit- oder Parent-Löschplattform.


PR #18: Ready for Review nach autorisiertem Abschlusslauf/Push; kein Merge.
Dokument 153 grenzt die bestätigte manuelle Abnahme von den weiterhin offenen älteren
Lifecycle-Prüfungen und FR-ACT-006 ab. Frühere create-only-Abnahmen ersetzen diese nicht.
