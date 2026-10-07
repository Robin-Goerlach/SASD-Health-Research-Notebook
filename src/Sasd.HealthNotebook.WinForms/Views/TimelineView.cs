using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>Timeline using Application projections with explicit selected-record lifecycle commands.</summary>
public sealed class TimelineView : UserControl
{
    private readonly Button _editButton = new() { AutoSize = true, MinimumSize = new(110, UiMetrics.ActionHeight) };
    private readonly Button _deleteButton = new() { AutoSize = true, MinimumSize = new(110, UiMetrics.ActionHeight) };
    private readonly DataGridView _grid;
    private readonly ThreeStateGridSort<TimelineRow> _sort;
    private readonly Label _emptyStateLabel;
    private IReadOnlyList<TimelineItem> _entries = Array.Empty<TimelineItem>();
    /// <summary>Creates a responsive timeline with a readable empty state.</summary>
    public TimelineView()
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
        string[] properties = { "Time", "Type", "Title", "Topic", "Preview" };
        int[] widths = { 135, 100, 160, 130, 150 };
        for (int index = 0; index < properties.Length; index++)
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = properties[index],
                MinimumWidth = widths[index], FillWeight = index == 2 || index == 4 ? 25 : 16, SortMode = DataGridViewColumnSortMode.NotSortable });
        _sort = new ThreeStateGridSort<TimelineRow>(_grid, row => row.SelectionId)
            .Column(0, row => row.Data.OccurredAt).Text(1, row => row.Type)
            .Text(2, row => row.Title).Text(3, row => row.Topic).Text(4, row => row.FullContent);
        _sort.Rebound += UpdateCommands;
        var list = new Panel { Dock = DockStyle.Fill, TabIndex = 1 }; list.Controls.Add(_grid); list.Controls.Add(_emptyStateLabel);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, WrapContents = false, TabIndex = 0 };
        _editButton.TabIndex = 0; _deleteButton.TabIndex = 1; buttons.Controls.Add(_editButton); buttons.Controls.Add(_deleteButton);
        Controls.Add(list); Controls.Add(buttons);
        _editButton.Click += (_, _) => EditRequested?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (_, _) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        // Keep a focused lifecycle command enabled during transient empty binding rows.
        // Rebound publishes command state after the shared ID-restoration transaction.
        _grid.CurrentCellChanged += (_, _) => { if (!_sort.IsRebinding) UpdateCommands(); };
        ApplyTexts();
        SetEntries(Array.Empty<HealthEntrySummary>());
    }
    /// <summary>Updates labels after changing the UI language.</summary>
    public void ApplyTexts()
    {
        _editButton.Text = AppStrings.Edit; _deleteButton.Text = AppStrings.Delete;
        _emptyStateLabel.Text = AppStrings.TimelineEmpty;
        string[] headers = { AppStrings.EntryTime, AppStrings.EntryType, AppStrings.ColumnTitle,
            AppStrings.HealthTopics, AppStrings.EntryContent };
        for (int index = 0; index < headers.Length; index++) _grid.Columns[index].HeaderText = headers[index];
        SetItems(_entries);
    }
    /// <summary>Displays chronologically ordered application summaries; retains selection by ID.</summary>
    public void SetEntries(IReadOnlyList<HealthEntrySummary> entries)
    {
        SetItems(entries.Select(entry => new TimelineItem(entry, null)).ToList());
    }
    private readonly Dictionary<(bool Measurement, Guid Id), Guid> _selectionIds = new();
    /// <summary>Displays both source kinds; retains selection even when source IDs collide.</summary>
    public void SetItems(IReadOnlyList<TimelineItem> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        _entries = entries;
        _sort.SetRows(entries.Select(item =>
        {
            var key = (item.Entry is null, item.Id);
            if (!_selectionIds.TryGetValue(key, out var selectionId))
                _selectionIds[key] = selectionId = Guid.NewGuid();
            var measurement = item.Measurement?.Measurement;
            string title = item.Entry?.Title ?? AppStrings.MeasurementTypeText(measurement!.MeasurementType);
            string preview = item.Entry?.Content ?? MeasurementPreview(measurement!)
                + (string.IsNullOrEmpty(measurement?.Note) ? "" : " · " + measurement?.Note);
            return new TimelineRow(item.Id, AppStrings.FormatDateTime(item.OccurredAt),
                item.Entry is not null ? AppStrings.HealthEntryTypeText(item.Entry.EntryType) : AppStrings.Measurements,
                title, item.HealthTopicTitle ?? (item.HealthTopicId.HasValue ? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic),
                preview.Length <= 160 ? preview : preview[..160] + "…", item, selectionId, preview);
        }).ToList());
        _grid.Visible = entries.Count > 0;
        _grid.TabStop = _grid.Visible;
        _emptyStateLabel.Visible = !_grid.Visible;
    }
    private static string MeasurementPreview(Sasd.HealthNotebook.Domain.Measurement measurement)
    {
        string values = AppStrings.FormatMeasurementValues(measurement);
        string unit = " " + AppStrings.MeasurementUnitText(measurement.Unit);
        int lineBreak = values.IndexOf(Environment.NewLine, StringComparison.Ordinal);
        return lineBreak < 0 ? values + unit : values.Insert(lineBreak, unit);
    }
    private void UpdateCommands() => _editButton.Enabled = _deleteButton.Enabled = _grid.CurrentRow is not null;
    private sealed record TimelineRow(Guid Id, string Time, string Type, string Title, string Topic, string Preview, TimelineItem Data, Guid SelectionId, string FullContent);
    /// <summary>Explicit selected-record edit command.</summary>
    public event EventHandler? EditRequested;
    /// <summary>Explicit selected-record delete command; shell confirms before the use case.</summary>
    public event EventHandler? DeleteRequested;
    /// <summary>Exact source snapshot selected in the mixed timeline.</summary>
    public TimelineItem? SelectedItem => (_grid.CurrentRow?.DataBoundItem as TimelineRow)?.Data;
    /// <summary>Selected timeline identity.</summary>
    public Guid? SelectedEntryId => SelectedItem?.Entry?.Id;
    /// <summary>Exact displayed record and conflict token for deletion.</summary>
    public HealthEntrySummary? SelectedEntry => SelectedItem?.Entry;

}
