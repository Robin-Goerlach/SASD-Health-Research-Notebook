using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>Measurement list with explicit lifecycle commands; numeric values are formatted only for display.</summary>
public sealed class MeasurementsView : UserControl
{
    private readonly Button _editButton = new() { AutoSize = true, MinimumSize = new(110, UiMetrics.ActionHeight) };
    private readonly Button _deleteButton = new() { AutoSize = true, MinimumSize = new(110, UiMetrics.ActionHeight) };
    private readonly Label _filterLabel = new() { AutoSize = true, Margin = new(3, 12, 6, 3), TabStop = false };
    private readonly ComboBox _typeFilter = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180, Margin = new(3, 8, 12, 3) };
    private MeasurementType? _filterType;
    private bool _updatingFilter;
    private readonly DataGridView _grid;
    private readonly Label _emptyStateLabel;
    private IReadOnlyList<MeasurementSummary> _measurements = Array.Empty<MeasurementSummary>();
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
        var list = new Panel { Dock = DockStyle.Fill, TabIndex = 1 }; list.Controls.Add(_grid); list.Controls.Add(_emptyStateLabel);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, WrapContents = false, TabIndex = 0 };
        _typeFilter.TabIndex = 0; _editButton.TabIndex = 1; _deleteButton.TabIndex = 2;
        buttons.Controls.Add(_filterLabel); buttons.Controls.Add(_typeFilter); buttons.Controls.Add(_editButton); buttons.Controls.Add(_deleteButton);
        Controls.Add(list); Controls.Add(buttons);
        _editButton.Click += (_, _) => EditRequested?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (_, _) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        _grid.CurrentCellChanged += (_, _) => _editButton.Enabled = _deleteButton.Enabled = _grid.CurrentRow is not null;
        _typeFilter.SelectedIndexChanged += (_, _) =>
        {
            if (_updatingFilter || _typeFilter.SelectedItem is not FilterChoice choice) return;
            _filterType = choice.Type;
            BindVisibleMeasurements();
        };
        ApplyTexts();
        SetMeasurements(Array.Empty<MeasurementSummary>());
    }
    /// <summary>Updates labels after changing the UI language.</summary>
    public void ApplyTexts()
    {
        _editButton.Text = AppStrings.Edit; _deleteButton.Text = AppStrings.Delete;
        _filterLabel.Text = _typeFilter.AccessibleName = AppStrings.MeasurementFilterLabel;
        // Keep the enum selection, not translated display text or a ComboBox index.
        // Rebinding labels during a language change must not briefly apply the All filter.
        var choices = new[] { new FilterChoice(null, AppStrings.MeasurementsFilterAll) }
            .Concat(Enum.GetValues<MeasurementType>().Select(type => new FilterChoice(type, AppStrings.MeasurementFilterTypeText(type)))).ToList();
        _updatingFilter = true;
        try
        {
            _typeFilter.DisplayMember = nameof(FilterChoice.Label);
            _typeFilter.Items.Clear();
            _typeFilter.Items.AddRange(choices.Cast<object>().ToArray());
            _typeFilter.SelectedIndex = choices.FindIndex(choice => choice.Type == _filterType);
        }
        finally { _updatingFilter = false; }
        string[] headers = { AppStrings.EntryTime, AppStrings.MeasurementKind, AppStrings.MeasurementValues,
            AppStrings.MeasurementUnitLabel, AppStrings.HealthTopics, AppStrings.MeasurementNote };
        for (int index = 0; index < headers.Length; index++) _grid.Columns[index].HeaderText = headers[index];
        BindVisibleMeasurements();
    }
    /// <summary>Displays chronologically ordered application summaries; retains selection by ID.</summary>
    public void SetMeasurements(IReadOnlyList<MeasurementSummary> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        // The presenter supplies the complete list. Retain it even when no filtered rows
        // are visible: switching back to All must never require writes or lose records.
        _measurements = entries;
        BindVisibleMeasurements();
    }
    private void BindVisibleMeasurements()
    {
        Guid? selected = (_grid.CurrentRow?.DataBoundItem as MeasurementRow)?.Id;
        // Filtering is a display projection only. Neither Domain entities nor JSON data
        // are modified, and no filtered subset is passed back as persisted truth.
        var entries = _measurements.Where(entry => !_filterType.HasValue || entry.Measurement.MeasurementType == _filterType.Value).ToList();
        _grid.DataSource = entries.Select(entry => new MeasurementRow(entry.Measurement.Id,
            AppStrings.FormatDateTime(entry.Measurement.OccurredAt), AppStrings.MeasurementTypeText(entry.Measurement.MeasurementType),
            AppStrings.FormatMeasurementValues(entry.Measurement), AppStrings.MeasurementUnitText(entry.Measurement.Unit),
            entry.HealthTopicTitle ?? (entry.Measurement.HealthTopicId.HasValue ? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic),
            Preview(entry.Measurement.Note))).ToList();
        _editButton.Enabled = _deleteButton.Enabled = entries.Count > 0;
        _grid.Visible = entries.Count > 0;
        _grid.TabStop = _grid.Visible;
        _emptyStateLabel.Visible = !_grid.Visible;
        _emptyStateLabel.Text = _filterType.HasValue ? AppStrings.MeasurementsFilteredEmpty : AppStrings.MeasurementsEmpty;
        // Retain the selected ID only if it remains visible. Otherwise use the first
        // visible row, or clear selection and disable lifecycle commands for zero matches.
        _grid.CurrentCell = _grid.Rows.Count > 0 ? _grid.Rows[0].Cells[0] : null;
        foreach (DataGridViewRow row in _grid.Rows)
            if (row.DataBoundItem is MeasurementRow item && item.Id == selected) _grid.CurrentCell = row.Cells[0];
    }
    private static string Preview(string? text) => text is null ? "" : text.Length <= 160 ? text : text[..160] + "…";
    private sealed record MeasurementRow(Guid Id, string Time, string Type, string Values, string Unit, string Topic, string Preview);
    private sealed record FilterChoice(MeasurementType? Type, string Label);
    /// <summary>Transient display filter; null means all types. Never persisted.</summary>
    public MeasurementType? SelectedMeasurementType => _filterType;
    /// <summary>Explicit selected-record edit command.</summary>
    public event EventHandler? EditRequested;
    /// <summary>Explicit selected-record delete command; shell confirms before the use case.</summary>
    public event EventHandler? DeleteRequested;
    /// <summary>Selected exact measurement snapshot.</summary>
    public Measurement? SelectedMeasurement => _measurements.SingleOrDefault(item => item.Measurement.Id == (_grid.CurrentRow?.DataBoundItem as MeasurementRow)?.Id)?.Measurement;

}
