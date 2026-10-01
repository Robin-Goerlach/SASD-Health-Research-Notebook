using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>Read-only measurement list; numeric domain values are formatted only for display.</summary>
public sealed class MeasurementsView : UserControl
{
    private readonly DataGridView _grid;
    private readonly Label _emptyStateLabel;
    /// <summary>Creates a responsive measurement list with a readable empty state.</summary>
    public MeasurementsView()
    {
        Dock = DockStyle.Fill;
        Margin = Padding.Empty;
        BackColor = UiColors.WindowBackground;
        _emptyStateLabel = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(UiMetrics.LargeSpacing), Font = UiFonts.Body, ForeColor = UiColors.SecondaryText };
        _grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, MultiSelect = false,
            RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, BorderStyle = BorderStyle.None,
            BackgroundColor = UiColors.CardBackground, GridColor = UiColors.BorderColor, Font = UiFonts.Body,
            EnableHeadersVisualStyles = false, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal };
        _grid.DefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
        _grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        _grid.DefaultCellStyle.SelectionBackColor = UiColors.ListSelectionBackground;
        _grid.DefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = UiColors.WindowBackground;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = UiColors.PrimaryText;
        _grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiColors.WindowBackground;
        _grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        _grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8);
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _grid.AlternatingRowsDefaultCellStyle.BackColor = UiColors.AlternateRowBackground;
        string[] properties = { "Time", "Type", "Values", "Unit", "Topic", "Preview" };
        int[] widths = { 125, 100, 120, 60, 120, 130 };
        for (int index = 0; index < properties.Length; index++)
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = properties[index],
                MinimumWidth = widths[index], FillWeight = index == 2 || index == 4 ? 25 : 16, SortMode = DataGridViewColumnSortMode.NotSortable });
        Controls.Add(_grid);
        Controls.Add(_emptyStateLabel);
        ApplyTexts();
        SetMeasurements(Array.Empty<MeasurementSummary>());
    }
    /// <summary>Updates labels after changing the UI language.</summary>
    public void ApplyTexts()
    {
        _emptyStateLabel.Text = AppStrings.MeasurementsEmpty;
        string[] headers = { AppStrings.EntryTime, AppStrings.MeasurementKind, AppStrings.MeasurementValues,
            AppStrings.MeasurementUnitLabel, AppStrings.HealthTopics, AppStrings.MeasurementNote };
        for (int index = 0; index < headers.Length; index++) _grid.Columns[index].HeaderText = headers[index];
    }
    /// <summary>Displays chronologically ordered application summaries; retains selection by ID.</summary>
    public void SetMeasurements(IReadOnlyList<MeasurementSummary> entries)
    {
        Guid? selected = (_grid.CurrentRow?.DataBoundItem as MeasurementRow)?.Id;
        _grid.DataSource = entries.Select(entry => new MeasurementRow(entry.Measurement.Id,
            AppStrings.FormatDateTime(entry.Measurement.OccurredAt), AppStrings.MeasurementTypeText(entry.Measurement.MeasurementType),
            AppStrings.FormatMeasurementValues(entry.Measurement), AppStrings.MeasurementUnitText(entry.Measurement.Unit),
            entry.HealthTopicTitle ?? (entry.Measurement.HealthTopicId.HasValue ? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic),
            Preview(entry.Measurement.Note))).ToList();
        _grid.Visible = entries.Count > 0;
        _grid.TabStop = _grid.Visible;
        _emptyStateLabel.Visible = !_grid.Visible;
        foreach (DataGridViewRow row in _grid.Rows)
            if (row.DataBoundItem is MeasurementRow item && item.Id == selected) _grid.CurrentCell = row.Cells[0];
    }
    private static string Preview(string? text) => text is null ? "" : text.Length <= 160 ? text : text[..160] + "…";
    private sealed record MeasurementRow(Guid Id, string Time, string Type, string Values, string Unit, string Topic, string Preview);
}
