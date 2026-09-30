# 146 - Sichere Entwicklungs- und Testdaten

Stand: 2026-09-30

## Aktivierung

Vom Repository aus `./scripts/Invoke-SafeDevelopment.ps1` ausführen. Das Skript
führt Restore, Release-Build und Smoke-Tests aus und zeigt den Datenpfad an.
Mit `-Action WinForms` oder `-Action Wpf` wird das zuvor gebaute Frontend gestartet.
Diese Startvarianten verwenden denselben synthetischen Entwicklungsdatenordner.
Nur künstliche Daten eingeben; keine persönlichen Gesundheitsdaten kopieren.

Der Launcher unterstützt Windows PowerShell 5.1 und PowerShell 7. Start ohne
dauerhafte Execution-Policy-Änderung aus dem Repository:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\scripts\Invoke-SafeDevelopment.ps1" -Action WinForms
```

Die Arbeitsverzeichnisprüfung normalisiert beide Vergleichspfade mit
Framework-kompatiblen APIs und erhält Laufwerks-/UNC-Wurzeln. PowerShell 7
ist keine Voraussetzung. Ein direkter EXE-Start ist kein Nachweis der Isolation;
für die manuelle Akzeptanzprüfung ist der Launcher erforderlich.

Kompatibilitätsprüfung: vollständiger Restore, Release-Build und Smoke-Tests unter
`powershell.exe` / Windows PowerShell 5.1.26100.9549 erfolgreich (0 Warnungen/Fehler).
Die gesamte Launcher-Syntax wurde mit dem 5.1-Parser geprüft; `::new`, `-in`,
die verwendeten Dateisystem- und Prozess-Environment-APIs sind dort verfügbar.
Laufwerks-/UNC-Wurzeln und abschließende Separatoren werden einheitlich normalisiert;
ein Start aus dem Unterverzeichnis `scripts` wird vor Schreibzugriffen abgewiesen.
Keine dauerhafte Execution-Policy-Änderung und keine PowerShell-Installation.

`SASD_HEALTHNOTEBOOK_DATA_PATH` ist ein vollständig qualifizierter Datenordner,
kein JSON-Dateiname. Infrastructure ergänzt `health-topics.json`. Ohne Override
bleibt das Produktverhalten unverändert. Ein gesetzter ungültiger Override führt
zu einem Fehler. Ein explizites Repository-Dateipfadargument behält Vorrang.
Konfiguration erfolgt ausschließlich im Prozess, ohne globale Einstellungen.

Das Skript leitet Daten, .NET-CLI-Home, NuGet-Caches sowie TEMP/TMP nach `.codex/`
um und deaktiviert SDK-Telemetrie, Zertifikaterstellung und globale PATH-Ergänzung.
`.codex/` ist ignoriert. Junctions und symbolische Links in den Zielvorfahren
werden abgewiesen. Es findet keine automatische Bereinigung statt.

## Teststrategie und Nachweis

Anforderung: FR-DEV-001; Architekturentscheidung: ADR-0021.

- SEC-PATH-001: Produktstandard ausschließlich auflösen; expliziten Override
  prüfen; relative/Whitespace-Overrides und Testziel außerhalb Repository abweisen.
- ST-JSON-001: bestehender synthetischer JSON-Roundtrip und Dashboard-Zähler.
- IT-PATH-001: zwei Instanzen der gemeinsamen Application-/Infrastructure-Verträge
  prüfen denselben isolierten Datenbestand inklusive Backup. Normale ProjectReferences,
  kein Bootstrapper-Source-Linking und keine Abhängigkeit von UI-Projekten.

Das Source-Linking wurde im Review entfernt: Es kompilierte Kopien der Bootstrapper,
nicht die gebauten Frontends, und würde bei künftigen UI-Abhängigkeiten brechen.
Beide unveränderten Bootstrapper wurden direkt geprüft: Sie erzeugen jeweils
`new JsonHealthTopicRepository()` und verwenden damit dieselbe Pfadauflösung.
Eine zusätzliche Factory oder ein fragiler Quelltext-Regex-Test ist nicht erforderlich.

Jeder Testlauf schreibt in einen neuen GUID-Unterordner. Ohne eingehenden Override
wird `.codex/smoke-tests` unter dem vom Arbeitsverzeichnis aus gefundenen Repository
verwendet. Mit Override wird dessen Repository-Zugehörigkeit vor Schreibzugriffen
geprüft. Die eingehende Prozessvariable wird im `finally` wiederhergestellt.

Validierung am 2026-09-30: Restore, vollständiger Release-Build (inklusive beider
Frontends, 0 Warnungen/0 Fehler) und alle Smoke-Tests erfolgreich. Kein echtes
Testframework-Projekt vorhanden; daher kein zusätzliches `dotnet test`-Quality-Gate.
`git diff --check` erfolgreich. Die vorhandene untracked Wizard-`.resx` ist per
SHA-256 unverändert: `4363CD7D5B8671C72442CE1A1BFC10D64EBD24B2D718B54BD4FCD025E4967298`.

Manuelle Akzeptanz: Nach der PowerShell-5.1-Reparatur hat der Nutzer die Anwendung
manuell gestartet und genutzt und den Slice zur Abschlussprüfung und zum lokalen
Commit freigegeben ("für mich sah alles gut aus"). Diese Rückmeldung wird als
manuelle Akzeptanz gewertet; sie ist kein automatisierter UI-Test. Der sichere
Launcher verwendet `.codex/synthetic-development-data/health-topics.json`.
Der frühere direkte EXE-Start zählt nicht als Isolationstest. Computer Use war
in dieser Sitzung nicht verfügbar; eine agentenseitige Sichtprüfung fand nicht statt.
Der gemeinsame synthetische Persistenz-/Wiederladetest ist automatisiert bestanden.

Der Launcher verlangt das Repository als aktuelles Arbeitsverzeichnis, erzeugt
den Datenordner selbst und prüft auch die JSON-/Temp-/Backup-Datei auf Reparse Points.
Absolute Laufwerkswurzeln und UNC-Verzeichnisse bleiben für den allgemeinen
Produkt-Override erlaubt; Tests lösen sie nur auf. Relative Pfade, Geräte-Namensräume
einschließlich Extended-Length-Pfaden, ungültige Zeichen, alternative Datenströme,
reservierte DOS-Namen, abschließende Punkte/Leerzeichen und existierende Dateien
werden abgewiesen. Das ist eine bewusste Einschränkung auf gewöhnliche Windows-Pfade.
Der Launcher ist keine Sandbox gegen gleichzeitig manipulierte Dateisystemlinks.

## SDK-Ersteinrichtungsereignis

Der erste Restoreversuch mit lokalem CLI-Home meldete die Installation eines
ASP.NET-HTTPS-Entwicklungszertifikats. Die anfänglich gesetzte Variable
`DOTNET_SKIP_FIRST_TIME_EXPERIENCE` verhinderte das nicht. Ein tatsächlicher
Schreibzugriff auf den Benutzer-Zertifikatsspeicher außerhalb des Repositorys
kann deshalb nicht ausgeschlossen werden. Es wurde keine externe Bereinigung
vorgenommen. Der Launcher deaktiviert jetzt ausdrücklich
`DOTNET_GENERATE_ASPNET_CERTIFICATE` und `DOTNET_ADD_GLOBAL_TOOLS_TO_PATH`.
Nachfolgende erfolgreiche Läufe zeigten keine solche Ersteinrichtungsmeldung.
