using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>Read-only timeline using Application projections.</summary>
public sealed class TimelineView : UserControl
{
    private readonly DataGridView _grid;
    private readonly Label _emptyStateLabel;
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
        Controls.Add(_grid);
        Controls.Add(_emptyStateLabel);
        ApplyTexts();
        SetEntries(Array.Empty<HealthEntrySummary>());
    }
    /// <summary>Updates labels after changing the UI language.</summary>
    public void ApplyTexts()
    {
        _emptyStateLabel.Text = AppStrings.TimelineEmpty;
        string[] headers = { AppStrings.EntryTime, AppStrings.EntryType, AppStrings.ColumnTitle,
            AppStrings.HealthTopics, AppStrings.EntryContent };
        for (int index = 0; index < headers.Length; index++) _grid.Columns[index].HeaderText = headers[index];
    }
    /// <summary>Displays chronologically ordered application summaries; retains selection by ID.</summary>
    public void SetEntries(IReadOnlyList<HealthEntrySummary> entries)
    {
        Guid? selected = (_grid.CurrentRow?.DataBoundItem as TimelineRow)?.Id;
        _grid.DataSource = entries.Select(entry => new TimelineRow(entry.Id, AppStrings.FormatDateTime(entry.OccurredAt),
            AppStrings.HealthEntryTypeText(entry.EntryType), entry.Title,
            entry.HealthTopicTitle ?? (entry.HealthTopicId.HasValue ? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic),
            entry.Content.Length <= 160 ? entry.Content : entry.Content[..160] + "…")).ToList();
        _grid.Visible = entries.Count > 0;
        _grid.TabStop = _grid.Visible;
        _emptyStateLabel.Visible = !_grid.Visible;
        foreach (DataGridViewRow row in _grid.Rows)
            if (row.DataBoundItem is TimelineRow item && item.Id == selected) _grid.CurrentCell = row.Cells[0];
    }
    private sealed record TimelineRow(Guid Id, string Time, string Type, string Title, string Topic, string Preview);
}
