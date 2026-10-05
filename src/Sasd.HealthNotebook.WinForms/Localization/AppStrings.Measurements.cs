using System.Globalization;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.WinForms.Localization;

/// <summary>Localized measurement presentation with explicit units and no clinical judgments.</summary>
public static partial class AppStrings
{
    /// <summary>Measurement navigation title.</summary>
    public static string Measurements => Text("Measurements", "Vitalwerte");
    /// <summary>Primary action.</summary>
    public static string NewMeasurement => Text("New measurement", "Neuen Messwert erfassen");
    /// <summary>Page guidance.</summary>
    public static string MeasurementsDescription => Text("Manually documented measurements, newest first. No medical assessment.", "Manuell dokumentierte Messwerte, neueste zuerst. Keine medizinische Bewertung.");
    /// <summary>Empty list guidance.</summary>
    public static string MeasurementsEmpty => Text("No measurements yet. Choose New measurement to document one.", "Noch keine Messwerte. Mit Neuen Messwert erfassen eine Messung dokumentieren.");
    /// <summary>Compact display filter label.</summary>
    public static string MeasurementFilterLabel => Text("Measurement type", "Messart");
    /// <summary>Unrestricted display filter.</summary>
    public static string MeasurementsFilterAll => Text("All", "Alle");
    /// <summary>Empty filtered projection; does not imply an empty notebook.</summary>
    public static string MeasurementsFilteredEmpty => Text("No measurements of this type.", "Keine Messwerte dieser Art vorhanden.");
    /// <summary>Compact names for the transient type filter.</summary>
    public static string MeasurementFilterTypeText(MeasurementType type) => type switch
    {
        MeasurementType.Temperature => Text("Temperature", "Temperatur"),
        MeasurementType.Weight => Text("Weight", "Gewicht"),
        _ => MeasurementTypeText(type)
    };
    /// <summary>Category label.</summary>
    public static string MeasurementKind => Text("Measurement type", "Messwertart");
    /// <summary>Numeric values heading.</summary>
    public static string MeasurementValues => Text("Measurement(s)", "Messwert(e)");
    /// <summary>Unit column.</summary>
    public static string MeasurementUnitLabel => Text("Unit", "Einheit");
    /// <summary>Second pressure component label.</summary>
    public static string MeasurementDiastolic => Text("Diastolic blood pressure (mmHg)", "Diastolischer Blutdruck (mmHg)");
    /// <summary>Optional pressure pulse label.</summary>
    public static string MeasurementPulseOptional => Text("Pulse (/min, optional)", "Puls (/min, optional)");
    /// <summary>Optional situation editor.</summary>
    public static string MeasurementContext => Text("Measurement situation (optional)", "Messsituation (optional)");
    /// <summary>Optional note editor.</summary>
    public static string MeasurementNote => Text("My note (optional)", "Meine Notiz (optional)");
    /// <summary>Personal note guidance.</summary>
    public static string MeasurementNoteHelp => Text("Your own note, up to 4000 characters.\r\nStored separately from the numbers, without assessment.", "Eigene Notiz, höchstens 4000 Zeichen.\r\nGetrennt von den Zahlen gespeichert, ohne Bewertung.");
    /// <summary>Situation guidance.</summary>
    public static string MeasurementContextHelp => Text("Optional measurement situation, up to 500 characters.\r\nOnly what you wish to document; no inferred context.", "Optionale Messsituation, höchstens 500 Zeichen.\r\nNur selbst dokumentierte Angaben; kein abgeleiteter Kontext.");
    /// <summary>Numeric syntax guidance.</summary>
    public static string MeasurementNumberHelp => Text("Enter a nonnegative number in the shown unit.\r\nUse a decimal point, for example 36.5. No automatic conversion.", "Nichtnegative Zahl in der angezeigten Einheit eingeben.\r\nDezimalkomma verwenden, zum Beispiel 36,5. Keine automatische Umrechnung.");
    /// <summary>Validation feedback without user-entered values.</summary>
    public static string MeasurementValidationFailed => Text("Check date/time, required numbers and topic. Use a decimal point; no negative values, NaN or infinity.", "Datum/Uhrzeit, Pflichtzahlen und Thema prüfen. Dezimalkomma verwenden; keine negativen Zahlen, NaN oder Unendlich.");
    /// <summary>Privacy-safe save operation.</summary>
    public static string OperationSaveMeasurement => Text("save measurement", "Messwert speichern");
    /// <summary>Save action.</summary>
    public static string SaveMeasurement => Text("Save measurement", "Messwert speichern");
    /// <summary>Loaded count.</summary>
    public static string FormatLoadedMeasurements(int count) => Text($"Loaded {count} measurements.", $"{count} Messwerte geladen.");
    /// <summary>Supported category names.</summary>
    public static string MeasurementTypeText(MeasurementType type) => type switch
    {
        MeasurementType.BloodPressure => Text("Blood pressure", "Blutdruck"),
        MeasurementType.Pulse => Text("Pulse", "Puls"),
        MeasurementType.Temperature => Text("Body temperature", "Körpertemperatur"),
        MeasurementType.BloodGlucose => Text("Blood glucose", "Blutzucker"),
        MeasurementType.Weight => Text("Body weight", "Körpergewicht"),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
    /// <summary>Explicit unit symbols; no unit conversion.</summary>
    public static string MeasurementUnitText(MeasurementUnit unit) => unit switch
    {
        MeasurementUnit.MmHg => "mmHg", MeasurementUnit.PerMinute => "/min", MeasurementUnit.Celsius => "°C",
        MeasurementUnit.MgPerDeciliter => "mg/dL", MeasurementUnit.Kilogram => "kg",
        _ => throw new ArgumentOutOfRangeException(nameof(unit))
    };
    /// <summary>Specific primary field label including the fixed unit.</summary>
    public static string MeasurementPrimaryLabel(MeasurementType type) =>
        (type == MeasurementType.BloodPressure ? Text("Systolic blood pressure", "Systolischer Blutdruck") : MeasurementTypeText(type))
        + " (" + MeasurementUnitText(Measurement.UnitFor(type)) + ")";
    /// <summary>Number culture follows the selected UI language, not the operating-system culture.</summary>
    public static CultureInfo MeasurementCulture => CultureInfo.GetCultureInfo(AppLanguage.Current == UiLanguage.German ? "de-DE" : "en-US");
    /// <summary>Formats numbers for display only; persisted truth remains numeric.</summary>
    public static string FormatMeasurementValues(Measurement measurement)
    {
        if (measurement.MeasurementType != MeasurementType.BloodPressure) return measurement.Value!.Value.ToString("G", MeasurementCulture);
        string text = measurement.Systolic!.Value.ToString("G", MeasurementCulture) + " / " + measurement.Diastolic!.Value.ToString("G", MeasurementCulture);
        if (measurement.Pulse.HasValue) text += Environment.NewLine + MeasurementTypeText(MeasurementType.Pulse) + ": "
            + measurement.Pulse.Value.ToString("G", MeasurementCulture) + " /min";
        return text;
    }
}
