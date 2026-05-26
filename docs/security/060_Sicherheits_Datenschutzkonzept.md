# 060 - Sicherheits- und Datenschutzkonzept

Projekt: SASD Health Research Notebook  
Stand: 2026-05-25  
Dokumenttyp: Sicherheits- und Datenschutzkonzept  
Status: Entwurf  

## 1. Ziel

Dieses Dokument beschreibt, wie das SASD Health Research Notebook mit sensiblen Gesundheitsinformationen umgehen soll. Es ist kein juristisches Gutachten, sondern eine technische und organisatorische Planungsgrundlage.

Gesundheitsdaten sind besonders schützenswert. Deshalb müssen Datenschutz, Verschlüsselung, Logging, Backup, Exportkontrolle und spätere KI-Funktionen bereits in der Architektur berücksichtigt werden.

## 2. Schutzbedarf

| Datenart | Schutzbedarf | Beispiele |
|---|---|---|
| Gesundheitsthemen | sehr hoch | Diagnosen, Verdachtsfälle |
| Symptome | sehr hoch | Schweregrad, Verlauf, persönliche Beobachtungen |
| Dokumente | sehr hoch | Arztbriefe, Laborberichte, Scans |
| Medikamente | sehr hoch | Medikamente, Dosierung, Nebenwirkungen |
| Laborwerte | sehr hoch | Blutwerte, Referenzbereiche |
| Arztfragen | hoch | geplante Fragen, Antworten |
| Quellen | mittel bis hoch | Webseiten, Fachartikel, persönliche Notizen |
| technische Logs | niedrig bis hoch | nur niedrig, wenn frei von Gesundheitsdaten |
| Backups | sehr hoch | vollständige Kopie des Datenbestands |
| Exporte | sehr hoch | Arztmappe, PDF, Markdown, ZIP |

## 3. Rechtliche Orientierung

- Die DSGVO behandelt Gesundheitsdaten als besondere Kategorie personenbezogener Daten.
- Software mit medizinischer Zweckbestimmung kann in Europa regulatorisch relevant werden.
- FHIR ist ein wichtiger Interoperabilitätsstandard im Gesundheitsbereich, aber für V1 nur ein Ausblick.

Referenzen:

- DSGVO Art. 9: https://gdpr-info.eu/art-9-gdpr/
- Europäische Kommission MDR/IVDR Guidance: https://health.ec.europa.eu/medical-devices-sector/new-regulations/guidance-mdcg-endorsed-documents-and-other-guidance_en
- MDCG 2019-11 Rev.1 Update: https://health.ec.europa.eu/latest-updates/update-mdcg-2019-11-rev1-qualification-and-classification-software-regulation-eu-2017745-and-2025-06-17_en
- HL7 FHIR Overview: https://www.hl7.org/fhir/overview.html

## 4. Zweckbegrenzung

Die Anwendung soll ausschließlich folgende Zwecke unterstützen:

- persönliche Gesundheitsdokumentation
- strukturierte Ablage von Informationen
- Quellen- und Dokumentenverwaltung
- Vorbereitung von Arztgesprächen
- Verlaufserfassung
- Export eigener Zusammenfassungen
- Rechercheorganisation

Nicht unterstützt:

- automatische Diagnose
- automatische Therapieempfehlung
- Risikovorhersage mit Handlungsempfehlung
- Medikationsänderungsempfehlung
- Notfallbeurteilung
- Ersatz für ärztliche Beratung

## 5. Bedrohungsmodell

| Bedrohung | Auswirkung | Gegenmaßnahme |
|---|---|---|
| Verlust des Laptops | Offenlegung aller Gesundheitsdaten | Verschlüsselung, Master-Passwort, Backups geschützt |
| Malware auf dem System | Datenabfluss | lokale App kann das nicht vollständig verhindern; Warnung und sichere Speicherung |
| versehentlicher Export | Daten an falsche Person | Exportvorschau, Warnung, Auswahlkontrolle |
| Cloud-Sync durch Betriebssystem | Daten landen in Cloud | Speicherortwarnung, kein Standard in OneDrive/Dropbox ohne Hinweis |
| sensible Logs | Gesundheitsdaten in Logdateien | strikte Logging-Regeln |
| beschädigte Datenbank | Datenverlust | regelmäßige Backups, Restore-Test |
| fehlerhafte Migration | Datenverlust | Backup vor Migration, Migrationstests |
| KI-Upload sensibler Daten | Datenschutzverletzung | keine externe KI in V1, später explizite Zustimmung |
| falsche Interpretation | gesundheitlicher Schaden | klare Nicht-Diagnose-Grenze, Quellenbewertung, Arztfragen |

## 6. Verschlüsselungskonzept

### 6.1 Minimaler Zielzustand

Für einen privaten Prototyp kann zunächst ohne Verschlüsselung gestartet werden, wenn dies klar dokumentiert ist. Für produktive Nutzung mit echten Gesundheitsdaten sollte Verschlüsselung umgesetzt werden.

### 6.2 Empfohlener Zielzustand

- Datenbank verschlüsseln
- Dokumentenablage verschlüsseln oder in verschlüsseltem Container speichern
- Backups immer verschlüsseln
- Master-Passwort oder Windows Credential Manager verwenden
- automatische Sperre nach Inaktivität

### 6.3 Optionen

| Option | Vorteile | Nachteile |
|---|---|---|
| SQLCipher | bewährte SQLite-Verschlüsselung | zusätzliche Abhängigkeit, Lizenz-/Paketprüfung nötig |
| verschlüsselter Container | schützt Datenbank und Dateien gemeinsam | komplexere Integration |
| appseitige Dateiverschlüsselung | kontrollierbar | mehr Fehlerquellen |
| Windows BitLocker als Mindestschutz | einfach, systemweit | nicht app-spezifisch, nicht ausreichend bei entsperrtem System |

## 7. Master-Passwort und Recovery

Ein Master-Passwort schützt Daten, erzeugt aber Recovery-Risiken.

Grundsatz:

- Keine Backdoor.
- Vergessenes Master-Passwort darf nicht heimlich umgangen werden können.
- Recovery muss ausdrücklich geplant werden.

Mögliche spätere Recovery-Optionen:

- Recovery-Key, den der Nutzer ausdruckt
- exportierter Wiederherstellungsschlüssel
- separates Notfall-Backup
- Hinweis: ohne Recovery-Key keine Wiederherstellung möglich

## 8. Backup-Sicherheit

Backups enthalten den vollständigen Gesundheitsdatenbestand.

Anforderungen:

- Backup bewusst starten oder geplant ausführen
- Backup verschlüsseln
- Backup-Prüfsumme erstellen
- Restore regelmäßig testen
- Backup-Status im Dashboard anzeigen
- keine Backups ungeschützt in Cloud-Ordner schreiben
- Backup-Metadaten speichern, aber keine sensiblen Inhalte im Protokoll

## 9. Export-Sicherheit

Exporte sind besonders riskant, weil sie die geschützte Umgebung verlassen.

Anforderungen:

- Exportvorschau
- explizite Auswahl der Inhalte
- Warnung vor sensiblen Daten
- optional Passwortschutz für ZIP/PDF
- Exportprotokoll ohne Detailinhalte
- temporäre Exportdateien löschen
- Hinweis, wenn Export in Cloud-Ordner gespeichert wird

## 10. Logging-Regeln

Nicht loggen:

- Diagnosen
- Symptome
- Laborwerte
- Medikamentennamen
- Arztbriefinhalte
- Dateinamen mit medizinischem Inhalt
- Suchbegriffe
- Notiztexte

Erlaubt:

- technische Fehlercodes
- Anzahl verarbeiteter Objekte
- Dauer von Operationen
- Pfade nur gekürzt oder anonymisiert
- App-Version

Beispiel guter Logeintrag:

```text
2026-05-25 18:12:03 INFO DocumentImport Completed Count=3 DurationMs=842
```

Beispiel schlechter Logeintrag:

```text
Importiert: Laborwerte_Diabetes_HbA1c_2026.pdf
```

## 11. Datenschutzfreundliche Defaults

| Bereich | Default |
|---|---|
| Cloud | aus |
| KI | aus |
| Telemetrie | aus |
| externe Links | nur öffnen nach Bestätigung |
| automatische Dokumentanalyse | aus oder lokal |
| Export | minimaler Inhalt, Nutzer wählt bewusst mehr aus |
| Logs | keine Gesundheitsdaten |
| Backups | verschlüsselt empfohlen |

## 12. Datenschutz in der UI

Die UI soll Sicherheit sichtbar machen:

- Backupstatus
- Verschlüsselungsstatus
- Warnung bei ungeschütztem Speicherort
- Hinweis bei Export sensibler Daten
- Hinweis vor externer KI-Nutzung
- Datenschutzseite in Einstellungen

## 13. Rollen und Mehrbenutzer

V1 ist Einzelnutzer. Keine Rollen/Rechte.

Später denkbar:

- Nur-Lesen-Modus
- getrennte Profile
- Familien-/Betreuer-Modell
- Freigabe einzelner Exporte

Mehrbenutzerbetrieb erhöht Datenschutz- und Sicherheitsaufwand erheblich und wird nicht für V1 empfohlen.

## 14. Medizinprodukt-Risikogrenze

Um nicht in Richtung Medizinprodukt zu rutschen, gelten in V1:

- keine automatische Diagnose
- keine Therapieempfehlung
- keine Medikamenteninteraktionsprüfung
- keine Notfalltriage
- keine algorithmische Bewertung wie „gefährlich“ ohne Arztkontext
- keine Handlungsempfehlung „nehmen/absetzen/sofort handeln“

Erlaubt sind:

- Dokumentation eigener Werte
- Hinweis, dass Referenzbereiche vom Nutzer/Arzt/Labor stammen
- Markierung „außerhalb eingegebenem Referenzbereich“ als rein dokumentarische Information
- Vorbereitung von Fragen an Ärzte

## 15. Sicherheitsanforderungen für Entwicklung

- keine echten Gesundheitsdaten in Testdaten
- keine sensiblen Daten im Git-Repository
- `.gitignore` für Datenbank, Backups, Exporte, Logs
- Beispiel-/Demodaten künstlich erzeugen
- Dependency-Updates dokumentieren
- Sicherheitschecks vor Release
- Test für Logs ohne Gesundheitsdaten

## 16. Sicherheits-Checkliste vor V1

- [ ] Datenbankpfad nicht versehentlich im Repository
- [ ] Dokumentenspeicher nicht im Repository
- [ ] Logs enthalten keine Gesundheitsdaten
- [ ] Backup kann erstellt werden
- [ ] Backup kann wiederhergestellt werden
- [ ] Export zeigt Warnung
- [ ] Exportauswahl funktioniert
- [ ] Entwürfe werden gespeichert
- [ ] Archivieren statt Löschen funktioniert
- [ ] Fehlerfälle beim Dokumentimport sind abgesichert
- [ ] Datenschutzhinweise in Hilfe/Einstellungen vorhanden
- [ ] Nicht-Diagnose-Hinweis sichtbar dokumentiert

## 17. Verweise auf technische Sicherheitsstandards

- OWASP ASVS als Orientierung für Sicherheitsanforderungen: https://owasp.org/www-project-application-security-verification-standard/
- SQLCipher als mögliche SQLite-Verschlüsselung: https://www.zetetic.net/sqlcipher/

## 18. Offene Entscheidungen

- Verschlüsselung direkt in V1 oder V1.1?
- Master-Passwort verpflichtend oder optional?
- SQLCipher verwenden oder zunächst OS-Verschlüsselung plus späterer Ausbau?
- Soll die Anwendung beim Start automatisch sperren?
- Wie lange bleiben Wizard-Entwürfe erhalten?
- Wie wird ein vollständiger sicherer Export wieder gelöscht?
