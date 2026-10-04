using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Presentation helpers preserve precise stored instants and absent optional text in unchanged editors.</summary>
internal static class EditSupport
{
    public static DateTimeOffset Instant(DateTime date, DateTime clock, DateTimeOffset? original)
    {
        var local = DateTime.SpecifyKind(date.Date.AddHours(clock.Hour).AddMinutes(clock.Minute), DateTimeKind.Unspecified);
        if (original.HasValue)
        {
            var shown = original.Value.LocalDateTime;
            if (shown.Date == local.Date && shown.Hour == local.Hour && shown.Minute == local.Minute) return original.Value;
        }
        if (TimeZoneInfo.Local.IsInvalidTime(local) || TimeZoneInfo.Local.IsAmbiguousTime(local))
            throw new ArgumentException("Ambiguous or invalid local time.");
        return new DateTimeOffset(local, TimeZoneInfo.Local.GetUtcOffset(local));
    }
    public static string? Optional(string text, string? original) => text.Length == 0 && original is null ? null : text;
}
