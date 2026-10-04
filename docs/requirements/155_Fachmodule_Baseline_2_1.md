# 155 - Fachmodule und Anforderungen Baseline 2.1

Projekt: SASD Health Research Notebook  
Stand: 2026-09-30  
Dokumenttyp: ergänzender Anforderungskatalog  
Status: verbindliche Ergänzung zu Lasten- und Pflichtenheft

## 1. Zweck

Dieses Dokument gibt den seit der ersten Baseline konkretisierten Fachmodulen stabile Anforderungs-IDs. Es soll verhindern, dass wichtige Entscheidungen nur in Chats existieren.

## 2. Übergreifende Regeln

| ID | Anforderung |
|---|---|
| FR-GEN-001 | Jede fachliche Information kann optional einem oder mehreren Gesundheitsthemen zugeordnet werden. |
| FR-GEN-002 | Eigene Beobachtung, externe Aussage, dokumentierte professionelle Aussage und eigene Schlussfolgerung müssen unterscheidbar bleiben. |
| FR-GEN-003 | Die Anwendung darf aus Gesundheitsdaten keine Diagnose oder Therapieanweisung generieren. |
| FR-GEN-004 | Vom Nutzer definierte Ziele und Aufgaben müssen als solche gekennzeichnet sein. |
| FR-GEN-005 | Quellen- und Herkunftsangaben dürfen nicht stillschweigend verloren gehen. |
| FR-GEN-006 | Alle Kernfunktionen müssen lokal ohne Cloud nutzbar bleiben. |
| FR-GEN-007 | Sensible Freitexte und Gesundheitswerte dürfen nicht in technische Logs geschrieben werden. |
| FR-DEV-001 | Entwicklungs- und Testläufe müssen einen expliziten isolierten Datenordner verwenden können. Beide Frontends respektieren denselben Override; ohne Override bleibt der Produktpfad unverändert. Ungültige gesetzte Overrides dürfen nicht auf persönliche Daten zurückfallen. Smoke-Tests schreiben ausschließlich synthetische Daten in neue Lauf-Unterordner innerhalb des Repositorys. |
| FR-UI-001 | Die bestehende WinForms-Schale, Dashboardkarten, Themenliste und Grunddaten-Wizard müssen bei dokumentierter Mindestgröße in Deutsch/Englisch ohne abgeschnittene Hauptaktionen bedienbar bleiben. Navigation besitzt unterscheidbare Auswahl-, Hover- und Fokuszustände; Refresh erhält die Themenauswahl und leere Listen zeigen einen lesbaren Hinweis. Der reine UI-Baseline-2-Auftrag umfasste keine zusätzlichen Fachmodule; spätere fachliche Slices übernehmen dieselben UI-Qualitätsregeln. |

## 3. Beobachtungen und Kontext

| ID | Anforderung |
|---|---|
| FR-OBS-001 | Nutzer können eine Beobachtung mit Datum/Uhrzeit, Titel, Text und optionalem Gesundheitsthema erfassen. |
| FR-OBS-002 | Beobachtungen können Kontextfaktoren wie Ernährung, Alkohol, Schlaf, Stress, Bewegung, Wetter und Medikamente referenzieren. |
| FR-OBS-003 | Ein Zusammenhang wird als Beobachtung oder mögliche Beziehung formuliert, nicht als bewiesene Ursache. |
| FR-OBS-004 | Wiederholte ähnliche Beobachtungen dürfen deskriptiv zusammengefasst werden. |
| FR-OBS-005 | Beobachtungen können für ein späteres Arzt-/Coach-Gespräch markiert werden. |

Akzeptanz für FR-OBS-001, HealthEntry/Timeline Slice 1: Ein Nutzer erstellt eine
Notiz, Beobachtung oder Recherchenotiz mit fachlichem Datum/Uhrzeit, Pflicht-Titel
(maximal 160 Zeichen), optionalem Text (maximal 4000 Zeichen) und optional einem
vorhandenen Gesundheitsthema. Nach Speichern und Neustart erscheint der Eintrag
im WinForms-Verlauf mit Zeitpunkt, Typ, Titel und Themenname, neueste fachliche
Zeitpunkte zuerst. Kein medizinischer Inhalt wird ausgewertet. FR-OBS-002 bis
FR-OBS-005 bleiben außerhalb dieses Slices.

## 4. Ernährungstagebuch

| ID | Anforderung |
|---|---|
| FR-NUT-001 | Nutzer können Mahlzeiten und Getränke mit Zeitpunkt erfassen. |
| FR-NUT-002 | Ein Eintrag kann grobe Menge, Bestandteile, besondere Lebensmittel und freie Notizen enthalten. |
| FR-NUT-003 | Alkohol kann als eigener Getränketyp dokumentiert werden. |
| FR-NUT-004 | Essenspausen/Fasten können als vom Nutzer dokumentierter Kontext erfasst werden. |
| FR-NUT-005 | Ernährungseinträge können mit Messwerten, Symptomen, Gewicht und Gesundheitsthemen verknüpft werden. |
| FR-NUT-006 | V1 benötigt keine verpflichtende Kalorien- oder Nährwertdatenbank. |
| FR-NUT-007 | Die Anwendung gibt keine automatische Diät- oder Ernährungstherapieempfehlung. |

## 5. Messwerte und Vitalwerte

| ID | Anforderung |
|---|---|
| FR-MEA-001 | Blutdruck mit systolischem und diastolischem Wert muss strukturiert erfassbar sein. |
| FR-MEA-002 | Puls, Gewicht, Blutzucker und Temperatur müssen als Messwerttypen vorgesehen sein. |
| FR-MEA-003 | Benutzerdefinierte Messwerttypen sollen später möglich sein. |
| FR-MEA-004 | Messzeitpunkt, Einheit, Erfassungsart und optionale Messsituation werden gespeichert. |
| FR-MEA-005 | Rohwert und persönliche Notiz/Interpretation werden getrennt gespeichert. |
| FR-MEA-006 | Messwerte können mit Dokument, Gesundheitsthema, Beobachtung und Session verknüpft werden. |
| FR-MEA-007 | Die Anwendung darf Messwerte nicht automatisch diagnostisch bewerten. |
| FR-MEA-008 | Die Messwertliste kann rein darstellend nach bestehender Messart gefiltert werden; vollständige Daten bleiben erhalten. |

Akzeptanz für FR-MEA-001/002/004/005/007, Measurement/Vitalwerte Slice 1:
Der Nutzer dokumentiert Blutdruck mit getrennten systolischen/diastolischen Zahlen
und optionalem Puls oder einen Einzelwert für Puls, Körpertemperatur, Blutzucker
oder Körpergewicht. Zeitpunkt mit Offset, feste Einheit (mmHg, /min, °C, mg/dL, kg),
manuelle Erfassungsart, optionale Messsituation und eigene Notiz bleiben beim
Wiederladen erhalten. Neueste fachliche Zeitpunkte stehen zuerst. Zahlen werden
numerisch gespeichert; nur Pflichtwerte, Struktur, endliche und nichtnegative Zahlen
werden geprüft, ohne medizinische Grenzwerte oder Bewertung. Optionaler Themenbezug
(Teilumfang FR-MEA-006) löst den aktuellen Titel auf; fehlende/archivierte Themen
verlieren keine Messung. Dokument-/Beobachtungs-/Session-Verknüpfungen, allgemeine
Timeline-Integration und benutzerdefinierte Typen bleiben spätere Slices.

## 6. Wetter- und Kontext-Snapshots

| ID | Anforderung |
|---|---|
| FR-CTX-001 | Wetteranreicherung ist optional und vom Nutzer aktivierbar/deaktivierbar. |
| FR-CTX-002 | Ein Wetter-Snapshot wird zum fachlichen Zeitpunkt des Eintrags gespeichert und danach nicht automatisch überschrieben. |
| FR-CTX-003 | Temperatur, Luftdruck, Luftfeuchtigkeit, Niederschlag, Wetterlage und Wind können gespeichert werden. |
| FR-CTX-004 | Quelle, Abrufzeitpunkt und grobe Ortsreferenz werden gespeichert. |
| FR-CTX-005 | Ausfall eines Wetterdienstes darf die lokale Erfassung eines Messwerts nicht blockieren. |
| FR-CTX-006 | Standortdaten werden datensparsam behandelt; exakte GPS-Koordinaten sind für den Kernfall nicht erforderlich. |

## 7. Handlungsanweisungen und Action Plan

| ID | Anforderung |
|---|---|
| FR-ACT-001 | Nutzer können eine konkrete HealthAction für ein Gesundheitsthema erfassen. |
| FR-ACT-002 | Die Herkunft wird gespeichert: selbst definiert, Arzt, Therapeut, Coach, Quelle, sonstig. |
| FR-ACT-003 | Eine HealthAction kann auf eine Session und/oder Source verweisen. |
| FR-ACT-004 | Zeitraum, Status, Notiz und Erledigungszustand können gespeichert werden. |
| FR-ACT-005 | Die Anwendung erzeugt keine HealthAction automatisch aufgrund einer Krankheit oder eines Messwerts. |
| FR-ACT-006 | Änderungen an dokumentierten professionellen Anweisungen müssen nachvollziehbar bleiben. |

## 8. Routinen und Fortschritt

| ID | Anforderung |
|---|---|
| FR-ROU-001 | Nutzer können aus eigener Entscheidung oder einer HealthAction eine wiederkehrende Routine anlegen. |
| FR-ROU-002 | Routine und ursprüngliche HealthAction bleiben getrennte Objekte/Verantwortlichkeiten. |
| FR-ROU-003 | Routinen können täglich, wöchentlich oder an ausgewählten Tagen geplant werden. |
| FR-ROU-004 | Fortschritt unterstützt Boolean, Zähler, Menge, Dauer, Messaufgabe und Dokumentationsaufgabe. |
| FR-ROU-005 | Beispiel Zähler: 2 von 4 erledigt, 2 offen. |
| FR-ROU-006 | Tagesfortschritt wird historisch gespeichert und nicht nur als aktueller Zustand überschrieben. |
| FR-ROU-007 | Routine kann pausiert oder für einen Tag übersprungen werden, ohne die Historie zu löschen. |

### Akzeptanz: HealthAction + Routine + Progress Slice 1

FR-ACT-001/002/003/005, Teilumfang FR-ACT-004 und FR-ROU-001/002/003/004/006/007:
Eine selbst erfasste Maßnahme mit Titel, persönlicher Kategorie, Status, Beschreibung
und berichteter Herkunft kann mit/ohne ein vorhandenes Gesundheitsthema gespeichert
werden. Optionale Source-/Session-IDs verweisen auf vorhandene Herkunftsdatensätze;
aktuelle Titel werden nur angezeigt, ohne Inhalte zu kopieren. Herkunft und Maßnahmeninhalt sind im Slice create-only (keine Änderung
professioneller Anweisungen implementiert). Dies ist eine aktuelle Funktionsgrenze,
kein fachliches Bearbeitungsverbot. Fehlende/archivierte Themen verlieren keine Maßnahme.
Eine getrennte Routine gehört genau einer vorhandenen Maßnahme; persönlicher Rhythmus
ist optionaler Freitext (auch täglich/wöchentlich/ausgewählte Tage), ohne Scheduler.
Pausieren/Reaktivieren erhält alle historischen Einträge. Fortschritt gehört genau
einer Routine und dokumentiert Zeitpunkt mit Offset, durchgeführt/nicht durchgeführt/
übersprungen, optionaler nichtnegativer eigener Anzahl und eigene Notiz; neueste tatsächliche Zeitpunkte zuerst. Deutsch/English,
Neustart und Referenzintegrität sind abgesichert. Aktive Maßnahmen/Routinen und
heutige Einträge sind reine Dokumentationszahlen, keine medizinische Bewertung.
Zeiträume, Maßnahmenbearbeitung/Audit (FR-ACT-006),
strukturierte Tagesplanung, Zielzähler/Menge/Dauer/Mess-/Dokumentationsaufgaben und
Zielquoten (Rest FR-ROU-003/004/005), eigenständige Routinen ohne Action, Reminder,
allgemeine Timeline und weitere Modulbeziehungen bleiben spätere Slices.
Verbindliche Lebenszyklusregeln: Bearbeiten ist zulässig, setzt ModifiedAt und erhält
Identität und Referenzen, ohne medizinische Neubewertung. FR-ACT-006 bleibt gültig.
Archivieren ist für langlebige Objekte die bevorzugte Standardaktion; die Datensätze
bleiben technisch vorhanden und können in normalen Listen optional ausgeblendet
werden. Löschen erfordert fachliche Sicherheit und explizite Bestätigung, ohne stille
Cascades. Abhängige Datensätze blockieren das Löschen oder benötigen einen eigenen
späteren Lösch-Workflow. Bearbeiten, Archivieren und Löschen sind im aktuellen Slice
noch nicht implementiert; Pausieren/Reaktivieren ist keine Archivierung.
Details und manuelles Gate: Dokument 152.

## 9. Notification Service

| ID | Anforderung |
|---|---|
| FR-NOT-001 | Benachrichtigungen beziehen sich nur auf explizit konfigurierte Routinen, Termine oder Aufgaben. |
| FR-NOT-002 | Benachrichtigungszeiten und Zeitfenster sind konfigurierbar. |
| FR-NOT-003 | Snooze, erledigt und heute überspringen sollen unterstützt werden. |
| FR-NOT-004 | Ruhezeiten sind konfigurierbar. |
| FR-NOT-005 | Ein diskreter Modus zeigt auf Wunsch keine konkreten Gesundheitsdetails im Toast. |
| FR-NOT-006 | Notification-Historie darf keine unnötigen Gesundheitsfreitexte duplizieren. |

## 10. Quellen und Belegstellen

| ID | Anforderung |
|---|---|
| FR-SRC-001 | Quellen unterstützen Webseite, PDF/Dokument, Buch, Studie, Gespräch, professionelle Aussage und eigene Beobachtung. |
| FR-SRC-002 | URL, Autor/Institution, Titel, Publikationsdatum und Abrufdatum können gespeichert werden. |
| FR-SRC-003 | DOI, ISBN oder andere externe IDs sind optional speicherbar. |
| FR-SRC-004 | Eine SourceLocation kann Seite, Absatz, Abschnitt, Kapitel oder andere Fundstelle speichern. |
| FR-SRC-005 | Ein kurzes Zitat/Exzerpt und eine eigene Paraphrase werden unterscheidbar gespeichert. |
| FR-SRC-006 | Eine EvidenceNote verbindet Aussage, Quelle, Fundstelle und eigene Einordnung. |
| FR-SRC-007 | Vertrauensbewertung ist subjektive Recherchemetadaten und keine automatische medizinische Wahrheitsbewertung. |
| FR-SRC-008 | Eine Vertrauensbewertung soll zusätzlich eine Begründung oder Kategorie unterstützen. |
| FR-SRC-009 | Quellen können als ungeprüft, geprüft, mit Arzt/Apotheke besprochen, widersprüchlich oder verworfen markiert werden. |

Akzeptanz für FR-SRC-001/002/004/005/006, Sources Slice 1: Eine Quelle mit einem
der sieben Typen lässt sich lokal anlegen; mindestens Titel, absolute HTTP(S)-URL
oder Autor/Institution ist vorhanden. Publikations-/Abrufdatum und externe ID
(FR-SRC-003) sind optional. Eine konkrete Fundstelle gehört genau einer vorhandenen
Quelle. Eine Quellen-Notiz enthält Aussage und eigene Zusammenfassung, optional
Originalexzerpt und eine Fundstelle derselben Quelle. Fremde Fundstellen werden
abgewiesen. Nach Neustart bleiben Metadaten, getrennte Texte und Beziehungen erhalten;
WinForms bietet Deutsch/English. Eine Quelle kann optional einem vorhandenen Thema
zugeordnet werden (kleine Slice-1-Teilmenge von FR-GEN-001). Kein Wahrheitsurteil,
keine medizinische Bewertung; FR-SRC-007 bis FR-SRC-009 bleiben außerhalb dieses Slices.

## 11. Medien und Übungsbilder

| ID | Anforderung |
|---|---|
| FR-MED-001 | Bilder können lokal importiert und mehreren fachlichen Objekten zugeordnet werden. |
| FR-MED-002 | Titel, Beschreibung, Datum, Tags, Herkunft und Urheberhinweis können gespeichert werden. |
| FR-MED-003 | Bilder können Bestandteil einer Übungsanleitung mit mehreren Schritten sein. |
| FR-MED-004 | Originaldatei bleibt erhalten; Vorschaubilder dürfen separat generiert werden. |
| FR-MED-005 | Große Binärdateien sollen nicht zwingend als BLOB in SQLite gespeichert werden. |
| FR-MED-006 | Die Anwendung entscheidet nicht, ob eine Übung medizinisch geeignet ist. |

## 12. Sessions: Arzt, Coaching und Beratung

| ID | Anforderung |
|---|---|
| FR-SES-001 | Ein allgemeines Session-Modell unterstützt Arztbesuch, Kontrolle, Coaching, Physiotherapie, Ernährungsberatung und weitere Beratungstypen. |
| FR-SES-002 | Eine Session besitzt Datum/Uhrzeit, Typ, Anlass, Status und optionale Kontakt-/Organisationsreferenz. |
| FR-SES-003 | Vor einer Session können Fragen, Dokumente, Messwerte und zu besprechende Themen gesammelt werden. |
| FR-SES-004 | Während/nach einer Session können besprochene Themen und eigene Gesprächsnotizen erfasst werden. |
| FR-SES-005 | Die Herkunft einer Aussage ist markierbar: während Gespräch notiert, aus Erinnerung, schriftliches Dokument, Laborbericht oder andere Quelle. |
| FR-SES-006 | Entscheidungen/Vereinbarungen können dokumentiert und mit HealthActions verknüpft werden. |
| FR-SES-007 | Offene Punkte, erwartete Dokumente, Rückrufe und Folgetermine können nachverfolgt werden. |
| FR-SES-008 | Statt eines medizinisch verbindlichen Diagnosetexts wird dokumentiert, was der Nutzer verstanden oder schriftlich erhalten hat. |
| FR-SES-009 | Session-Inhalte können in eine Arzt-/Coach-Vorbereitungs- und Nachbereitungsansicht exportiert werden. |

### Implementierungsstand: Session Slice 1

FR-SES-001/002: sechs Gesprächstypen, Zeitpunkt/Titel/Status und optionaler Kontaktfreitext
sowie ein einzelnes optionales HealthTopic. FR-SES-003/004/008 teilweise: sessionbezogene
Fragen mit getrennter dokumentierter Antwort und eigene Gesprächsnotiz, keine Interpretation.
FR-SES-007 teilweise / PF-APT-006: nutzererfasste nächste Schritte, Open/Done und optionale
Kalenderfälligkeit ohne Reminder. Atomarer gemeinsamer sessions.json-Store; Dokument 151.
Dokument-/Messwert-/Quellenbeziehungen, Herkunftsmarkierungen, HealthActions, Export und
unabhängiger Frageeingang bleiben offen. Slice 1 ist implementiert und manuell akzeptiert (2026-10-01).

## 13. Kontakte

| ID | Anforderung |
|---|---|
| FR-CON-001 | Relevante Ansprechpartner können mit Name, Organisation, Fachrichtung/Rolle und Kontaktdaten referenziert werden. |
| FR-CON-002 | Das Health Notebook entwickelt kein vollständiges CRM. |
| FR-CON-003 | Eine ContactReference unterstützt eine optionale externe Kontakt-ID. |
| FR-CON-004 | Spätere Integration einer zentralen SASD-Kontakteverwaltung darf ohne Migration der fachlichen Session-/HealthTopic-Beziehungen möglich sein. |
| FR-CON-005 | Kontaktfreitexte werden datensparsam gehalten. |

## 14. Coaching

| ID | Anforderung |
|---|---|
| FR-COA-001 | Coaching wird als Session-Typ und nicht als fest eingebautes TK-spezifisches Subsystem modelliert. |
| FR-COA-002 | Vor einer Coaching-Sitzung können Fortschritt, offene Ziele, relevante Beobachtungen und Fragen zusammengestellt werden. |
| FR-COA-003 | Nach einer Coaching-Sitzung können neue Ziele, Aufgaben, Übungen, Quellen und Folgetermine dokumentiert werden. |
| FR-COA-004 | Ein bestehendes oder späteres TK-Coach-Projekt kann als spezialisierter Workflow/Client auf diesen allgemeinen Konzepten aufsetzen. |

## 15. Wechselwirkungsnotizen

| ID | Anforderung |
|---|---|
| FR-INT-001 | Nutzer können mögliche Medikament-/Lebensmittel-/Supplement-Zusammenhänge dokumentieren. |
| FR-INT-002 | Quelle, eigene Beobachtung und offene Frage an Arzt/Apotheke können verknüpft werden. |
| FR-INT-003 | Status unterstützt mindestens ungeprüft, zu klären, besprochen und verworfen. |
| FR-INT-004 | Ohne validierte medizinische Datenbasis darf keine automatische Interaktionsentscheidung getroffen werden. |

## 16. Such-, Timeline- und Exportwirkung

Alle oben genannten Objekte sollen langfristig:

- über Gesundheitsthema und Datum auffindbar sein;
- in einer Timeline erscheinen können;
- in der globalen Suche indexierbar sein, soweit datenschutzrechtlich vertretbar;
- gezielt in Exporte ein- oder ausgeschlossen werden können.

## 17. Implementierungsprinzip

Diese Anforderungen sind ein Zielbild. Implementiert wird in kleinen vertikalen Slices. Ein Slice enthält nach Möglichkeit Domain, Application, Persistenz, WPF, Tests und Dokumentation für einen zusammenhängenden Anwendungsfall.

## 18. Edit / Archive / Delete Baseline Slice 1

| ID | Anforderung / konkrete Akzeptanz |
|---|---|
| FR-LIF-001 | Measurement, HealthEntry, Session, HealthAction, Routine und ProgressEntry sind korrigierbar. Id/CreatedAt und Kinder bleiben erhalten, ModifiedAt steigt bei Änderung. Unverändertes Speichern schreibt nicht; veraltete Editoren werden abgewiesen. |
| FR-LIF-002 | Sessions und HealthActions sind separat archivierbar/reaktivierbar; fachlicher Status und Kinder bleiben erhalten. Normale Arbeitslisten blenden Archive aus; „Archivierte anzeigen“ erlaubt Reaktivierung. Routine bleibt pausierbar; Pause ist keine Archivierung. |
| FR-LIF-003 | Measurement, HealthEntry und ProgressEntry dürfen einzeln nach konkreter Bestätigung gelöscht werden. Elternobjekte HealthTopic, Session, HealthAction, Routine und Sources haben in diesem Slice keine Hard-Delete-API oder -UI; keine Cascades. |
| FR-ACT-006 / Slice 1 | Änderungen professioneller Maßnahmen (Arzt/Therapeut/Coach oder quellenbasierte Herkunft, auch Wechsel/Entfernen der Herkunft) bewahren den vorherigen Inhalt samt Herkunft als atomare HealthActionRevision; Historie bleibt im UI lesbar. |

Neuere Slice-Spezifikation: Dokument 153. Die create-only-Aussagen oben beschreiben
die historischen Fachslices. Die ältere Einschränkung auf falsch importierte/leere
Testdaten in Datenmodell §7 wird für diese drei korrigierbaren, aktuell kinderlosen
Datensatztypen ausdrücklich durch FR-LIF-003 präzisiert. Sources bleiben Folgearbeit.
