using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>One source workspace with dependent lists and readable, separate original/personal text.</summary>
public sealed class SourcesView : UserControl
{
    private readonly DataGridView _sourcesGrid;
    private readonly DataGridView _locationsGrid;
    private readonly DataGridView _notesGrid;
    private readonly Label _emptyStateLabel;
    private readonly Label _locationsEmptyLabel;
    private readonly Label _notesEmptyLabel;
    private readonly TextBox _sourceDetails = ReadOnlyText();
    private readonly TextBox _locationDetails = ReadOnlyText();
    private readonly TextBox _noteDetails = ReadOnlyText();
    private readonly Button _newLocationButton = ActionButton();
    private readonly Button _newNoteButton = ActionButton();
    private readonly TabPage _locationsTab = new();
    private readonly TabPage _notesTab = new();
    private IReadOnlyList<SourceSummary> _sources = Array.Empty<SourceSummary>();
    private IReadOnlyList<SourceLocation> _locations = Array.Empty<SourceLocation>();
    private IReadOnlyList<EvidenceNote> _notes = Array.Empty<EvidenceNote>();
    private bool _binding;
    /// <summary>Creates a compact two-pane workspace with keyboard-accessible tabs.</summary>
    public SourcesView()
    {
        Dock = DockStyle.Fill; Margin = Padding.Empty; BackColor = UiColors.WindowBackground;
        _sourcesGrid = Grid(new[] { "Name", "Type" }, new[] { 160, 110 });
        _locationsGrid = Grid(new[] { "Type", "Locator" }, new[] { 90, 140 });
        _notesGrid = Grid(new[] { "Statement", "Location" }, new[] { 170, 100 });
        _emptyStateLabel = EmptyLabel(); _locationsEmptyLabel = EmptyLabel(); _notesEmptyLabel = EmptyLabel();
        // A shared proportional detail row keeps both sides aligned despite the
        // right-hand tab header and action buttons consuming different list space.
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        var sourcePane = Pane(_sourcesGrid, _emptyStateLabel, null);
        sourcePane.Margin = new Padding(0, 0, UiMetrics.StandardSpacing, 0);
        layout.Controls.Add(sourcePane, 0, 0);
        var tabs = new TabControl { Dock = DockStyle.Fill, TabIndex = 1, Margin = Padding.Empty };
        _locationsTab.Controls.Add(Pane(_locationsGrid, _locationsEmptyLabel, _newLocationButton));
        _notesTab.Controls.Add(Pane(_notesGrid, _notesEmptyLabel, _newNoteButton));
        tabs.TabPages.Add(_locationsTab); tabs.TabPages.Add(_notesTab); layout.Controls.Add(tabs, 1, 0);
        _sourceDetails.Margin = new Padding(0, 3, UiMetrics.StandardSpacing, 3);
        layout.Controls.Add(_sourceDetails, 0, 1);
        var dependentDetails = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 3, 0, 3), TabIndex = 2 };
        dependentDetails.Controls.Add(_noteDetails); dependentDetails.Controls.Add(_locationDetails);
        _noteDetails.Visible = false;
        tabs.SelectedIndexChanged += (_, _) =>
        {
            _locationDetails.Visible = tabs.SelectedTab == _locationsTab;
            _noteDetails.Visible = tabs.SelectedTab == _notesTab;
        };
        layout.Controls.Add(dependentDetails, 1, 1);
        Controls.Add(layout);
        // SelectionChanged runs before CurrentRow changes. Dependents must be loaded
        // from the new current source; otherwise the presenter discards the old result.
        _sourcesGrid.CurrentCellChanged += (_, _) => { if (!_binding) { UpdateSourceDetails(); SourceSelected?.Invoke(this, EventArgs.Empty); } };
        _locationsGrid.SelectionChanged += (_, _) => UpdateLocationDetails();
        _notesGrid.SelectionChanged += (_, _) => UpdateNoteDetails();
        _newLocationButton.Click += (_, _) => NewLocationRequested?.Invoke(this, EventArgs.Empty);
        _newNoteButton.Click += (_, _) => NewNoteRequested?.Invoke(this, EventArgs.Empty);
        ApplyTexts(); SetSources(Array.Empty<SourceSummary>()); SetDependents(Array.Empty<SourceLocation>(), Array.Empty<EvidenceNote>());
    }
    /// <summary>Occurs when the selected source changes.</summary>
    public event EventHandler? SourceSelected;
    /// <summary>Requests a new location dialog.</summary>
    public event EventHandler? NewLocationRequested;
    /// <summary>Requests a new source note dialog.</summary>
    public event EventHandler? NewNoteRequested;
    /// <summary>The selected shared Domain entity, not a second UI source model.</summary>
    public Source? SelectedSource => _sources.SingleOrDefault(item => item.Source.Id == SelectedId(_sourcesGrid))?.Source;
    /// <summary>Updates the whole source list and preserves selection or selects a newly created ID.</summary>
    public void SetSources(IReadOnlyList<SourceSummary> sources, Guid? preferredId = null)
    {
        Guid? selected = preferredId ?? SelectedId(_sourcesGrid);
        _binding = true;
        try
        {
            _sources = sources;
            _sourcesGrid.DataSource = sources.Select(item => new SourceRow(item.Source.Id, AppStrings.SourceDisplayName(item.Source),
                AppStrings.SourceTypeText(item.Source.SourceType))).ToList();
            SelectId(_sourcesGrid, selected);
            SetEmpty(_sourcesGrid, _emptyStateLabel, sources.Count == 0);
        }
        finally { _binding = false; }
        UpdateSourceDetails();
    }
    /// <summary>Displays only the selected source's dependents; old details are cleared on selection change.</summary>
    public void SetDependents(IReadOnlyList<SourceLocation> locations, IReadOnlyList<EvidenceNote> notes)
    {
        Guid? selectedLocation = SelectedId(_locationsGrid), selectedNote = SelectedId(_notesGrid);
        _locations = locations; _notes = notes;
        _locationsGrid.DataSource = locations.Select(location => new LocationRow(location.Id,
            AppStrings.SourceLocationTypeText(location.LocationType), location.Locator)).ToList();
        _notesGrid.DataSource = notes.Select(note => new NoteRow(note.Id, Preview(note.Statement),
            note.SourceLocationId.HasValue ? locations.Single(location => location.Id == note.SourceLocationId).Locator : AppStrings.NoSourceLocation)).ToList();
        SelectId(_locationsGrid, selectedLocation); SelectId(_notesGrid, selectedNote);
        SetEmpty(_locationsGrid, _locationsEmptyLabel, locations.Count == 0);
        SetEmpty(_notesGrid, _notesEmptyLabel, notes.Count == 0);
        UpdateDependentEmptyTexts();
        _newLocationButton.Enabled = _newNoteButton.Enabled = SelectedSource is not null;
        UpdateLocationDetails(); UpdateNoteDetails();
    }
    /// <summary>Updates all localized headings and current details.</summary>
    public void ApplyTexts()
    {
        _emptyStateLabel.Text = AppStrings.SourcesEmpty;
        UpdateDependentEmptyTexts();
        _locationsTab.Text = AppStrings.SourceLocations; _notesTab.Text = AppStrings.SourceNotes;
        _newLocationButton.Text = AppStrings.NewSourceLocation; _newNoteButton.Text = AppStrings.NewSourceNote;
        _sourcesGrid.Columns[0].HeaderText = AppStrings.SourceTitle; _sourcesGrid.Columns[1].HeaderText = AppStrings.EntryType;
        _locationsGrid.Columns[0].HeaderText = AppStrings.LocationType; _locationsGrid.Columns[1].HeaderText = AppStrings.SourceLocator;
        _notesGrid.Columns[0].HeaderText = AppStrings.SourceStatement; _notesGrid.Columns[1].HeaderText = AppStrings.NoteLocation;
        _sourceDetails.AccessibleName = AppStrings.Sources; _locationDetails.AccessibleName = AppStrings.LocationNote;
        _noteDetails.AccessibleName = AppStrings.SourceNotes;
        UpdateSourceDetails(); UpdateLocationDetails(); UpdateNoteDetails();
    }
    private void UpdateSourceDetails()
    {
        var item = _sources.SingleOrDefault(source => source.Source.Id == SelectedId(_sourcesGrid));
        if (item is null) { _sourceDetails.Text = string.Empty; return; }
        var source = item.Source;
        _sourceDetails.Text = string.Join(Environment.NewLine, new[] {
            AppStrings.SourceTitle + ": " + source.Title, AppStrings.EntryType + ": " + AppStrings.SourceTypeText(source.SourceType),
            AppStrings.SourceUrl + ": " + source.Url, AppStrings.SourceAuthor + ": " + source.AuthorOrInstitution,
            AppStrings.SourcePublicationDate + ": " + FormatDate(source.PublicationDate), AppStrings.SourceAccessedAt + ": " + FormatDate(source.AccessedAt),
            AppStrings.SourceIdentifier + ": " + source.ExternalIdentifier,
            AppStrings.EntryTopic + ": " + (item.HealthTopicTitle ?? (source.HealthTopicId.HasValue ? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic)) });
    }
    private void UpdateLocationDetails() => _locationDetails.Text = _locations.SingleOrDefault(location => location.Id == SelectedId(_locationsGrid))?.Note ?? string.Empty;
    private void UpdateDependentEmptyTexts()
    {
        bool selected = SelectedSource is not null;
        _locationsEmptyLabel.Text = selected ? AppStrings.LocationsEmpty : AppStrings.SourceSelectionEmpty;
        _notesEmptyLabel.Text = selected ? AppStrings.NotesEmpty : AppStrings.SourceSelectionEmpty;
    }
    private void UpdateNoteDetails()
    {
        var note = _notes.SingleOrDefault(item => item.Id == SelectedId(_notesGrid));
        _noteDetails.Text = note is null ? string.Empty : string.Join(Environment.NewLine + Environment.NewLine,
            AppStrings.SourceStatement + ":" + Environment.NewLine + note.Statement,
            AppStrings.SourceExcerpt + ":" + Environment.NewLine + note.Excerpt,
            AppStrings.SourceAssessment + ":" + Environment.NewLine + note.OwnParaphraseOrAssessment);
    }
    private static string FormatDate(DateOnly? date) => date?.ToString(AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy") ?? string.Empty;
    private static string Preview(string text) => text.Length > 160 ? text[..160] + "…" : text;
    private static Guid? SelectedId(DataGridView grid) => grid.CurrentRow?.DataBoundItem switch
    { SourceRow row => row.Id, LocationRow row => row.Id, NoteRow row => row.Id, _ => null };
    private static void SelectId(DataGridView grid, Guid? id)
    {
        foreach (DataGridViewRow row in grid.Rows)
        {
            Guid current = row.DataBoundItem switch { SourceRow item => item.Id, LocationRow item => item.Id, NoteRow item => item.Id, _ => Guid.Empty };
            if (current == id) { grid.CurrentCell = row.Cells[0]; break; }
        }
    }
    private static void SetEmpty(DataGridView grid, Label label, bool empty) { grid.Visible = !empty; grid.TabStop = !empty; label.Visible = empty; }
    private static Button ActionButton() => new() { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
        MinimumSize = new Size(140, UiMetrics.ActionHeight), FlatStyle = FlatStyle.Flat, BackColor = UiColors.CardBackground };
    private static TextBox ReadOnlyText() => new() { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,
        ScrollBars = ScrollBars.Vertical, BackColor = UiColors.CardBackground, ForeColor = UiColors.PrimaryText,
        BorderStyle = BorderStyle.FixedSingle, Font = UiFonts.Body, TabIndex = 2 };
    private static Label EmptyLabel() => new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
        Padding = new Padding(UiMetrics.StandardSpacing), ForeColor = UiColors.SecondaryText, Font = UiFonts.Body };
    private static Control Pane(DataGridView grid, Label empty, Button? action)
    {
        var pane = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        pane.RowStyles.Add(new RowStyle(SizeType.Absolute, action is null ? 0 : 48));
        pane.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        if (action is not null) { action.Anchor = AnchorStyles.Left; pane.Controls.Add(action, 0, 0); }
        var list = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty, TabIndex = 1 };
        list.Controls.Add(grid); list.Controls.Add(empty); pane.Controls.Add(list, 0, 1);
        return pane;
    }
    private static DataGridView Grid(string[] properties, int[] widths)
    {
        var grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false, ReadOnly = true,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false, MultiSelect = false, RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, BackgroundColor = UiColors.CardBackground,
            BorderStyle = BorderStyle.None, GridColor = UiColors.BorderColor, Font = UiFonts.Body, EnableHeadersVisualStyles = false };
        grid.DefaultCellStyle.Padding = new Padding(6); grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.DefaultCellStyle.SelectionBackColor = UiColors.ListSelectionBackground; grid.DefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        grid.ColumnHeadersDefaultCellStyle.BackColor = UiColors.WindowBackground; grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = UiColors.PrimaryText;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiColors.WindowBackground;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.AlternatingRowsDefaultCellStyle.BackColor = UiColors.AlternateRowBackground;
        for (int i = 0; i < properties.Length; i++) grid.Columns.Add(new DataGridViewTextBoxColumn {
            DataPropertyName = properties[i], MinimumWidth = widths[i], FillWeight = i == 0 ? 60 : 40, SortMode = DataGridViewColumnSortMode.NotSortable });
        return grid;
    }
    private sealed record SourceRow(Guid Id, string Name, string Type);
    private sealed record LocationRow(Guid Id, string Type, string Locator);
    private sealed record NoteRow(Guid Id, string Statement, string Location);
}
