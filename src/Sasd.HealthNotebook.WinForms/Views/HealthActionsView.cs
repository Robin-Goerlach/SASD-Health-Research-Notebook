using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>Proportional action/routine/history workspace with explicit personal documentation.</summary>
public sealed class HealthActionsView : UserControl
{
    private readonly DataGridView _actionsGrid = Grid("Title", "Category", "Status");
    private readonly DataGridView _routinesGrid = Grid("Title", "Status");
    private readonly DataGridView _progressGrid = Grid("Time", "Status", "Count");
    private readonly Label _emptyStateLabel = Empty();
    private readonly Label _routinesEmpty = Empty();
    private readonly Label _progressEmpty = Empty();
    private readonly Label _actionsHeading = Heading();
    private readonly Label _routinesHeading = Heading();
    private readonly Label _progressHeading = Heading();
    private readonly RichTextBox _actionDetails = Details();
    private readonly RichTextBox _routineDetails = Details();
    private readonly RichTextBox _progressDetails = Details();
    private readonly Button _newRoutineButton = ActionButton();
    private readonly Button _routineStatusButton = ActionButton();
    private readonly Button _newProgressButton = ActionButton();
    private readonly HealthActionOverviewControl _overview = new();
    private readonly TableLayoutPanel _workspace;
    private IReadOnlyList<HealthActionSummary> _actions = Array.Empty<HealthActionSummary>();
    private IReadOnlyList<Routine> _routines = Array.Empty<Routine>();
    private IReadOnlyList<ProgressEntry> _entries = Array.Empty<ProgressEntry>();
    private readonly CheckBox _showArchivedCheckBox = new() { AutoSize = true, Dock = DockStyle.Top, Height = 28 };
    private readonly Button _editButton = new() { AutoSize = true, MinimumSize = new(90, UiMetrics.ActionHeight) };
    private readonly Button _archiveButton = new() { AutoSize = true, MinimumSize = new(76, UiMetrics.ActionHeight) };
    private readonly Button _historyButton = ActionButton();
    private readonly Button _editRoutineButton = ActionButton();
    private readonly Button _editProgressButton = ActionButton();
    private readonly Button _deleteProgressButton = ActionButton();
    private readonly ThreeStateGridSort<Row> _actionSort;
    private readonly ThreeStateGridSort<Row> _routineSort;
    private readonly ThreeStateGridSort<Row> _progressSort;
    private bool _binding;
    /// <summary>Creates aligned surfaces with standard spacing and no draggable splitters.</summary>
    public HealthActionsView()
    {
        _actionSort = new ThreeStateGridSort<Row>(_actionsGrid, row => row.Id)
            .Text(0, row => row.Title).Column(1, row => row.SortCategory).Column(2, row => (row.SortArchived, row.SortStatus));
        _routineSort = new ThreeStateGridSort<Row>(_routinesGrid, row => row.Id)
            .Text(0, row => row.Title).Column(1, row => row.SortStatus);
        _progressSort = new ThreeStateGridSort<Row>(_progressGrid, row => row.Id)
            .Column(0, row => row.SortTime).Column(1, row => row.SortStatus).Column(2, row => row.SortCount);
        _actionSort.Rebound += UpdateDetails;
        _routineSort.Rebound += UpdateDetails;
        _progressSort.Rebound += UpdateDetails;
        Dock = DockStyle.Fill; Margin = Padding.Empty; BackColor = UiColors.WindowBackground; Font = UiFonts.Body;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        root.RowStyles.Add(new(SizeType.Absolute, UiMetrics.CompactOverviewHeight)); root.RowStyles.Add(new(SizeType.Percent, 100));
        _overview.Margin = new(0, 0, 0, UiMetrics.StandardSpacing); root.Controls.Add(_overview, 0, 0);
        _workspace = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty, TabIndex = 1 };
        _workspace.ColumnStyles.Add(new(SizeType.Percent, 46)); _workspace.ColumnStyles.Add(new(SizeType.Percent, 54));
        _workspace.RowStyles.Add(new(SizeType.Percent, 50)); _workspace.RowStyles.Add(new(SizeType.Percent, 50));
        var actionPane = Pane(_actionsHeading, _actionsGrid, _emptyStateLabel, _actionDetails, 36, _editButton, _archiveButton, _historyButton);
        actionPane.Margin = new(0, 0, UiMetrics.StandardSpacing, 0);
        _workspace.Controls.Add(actionPane, 0, 0); _workspace.SetRowSpan(actionPane, 2);
        var routinePane = Pane(_routinesHeading, _routinesGrid, _routinesEmpty, _routineDetails, 24, _newRoutineButton, _routineStatusButton, _editRoutineButton);
        routinePane.TabIndex = 1; routinePane.Margin = new(0, 0, 0, UiMetrics.StandardSpacing);
        var progressPane = Pane(_progressHeading, _progressGrid, _progressEmpty, _progressDetails, 28, _newProgressButton, _editProgressButton, _deleteProgressButton);
        progressPane.TabIndex = 2; progressPane.Margin = Padding.Empty;
        _workspace.Controls.Add(routinePane, 1, 0); _workspace.Controls.Add(progressPane, 1, 1);
        root.Controls.Add(_workspace, 0, 1); Controls.Add(root);
        // CurrentCellChanged reports the newly selected row; SelectionChanged can report the old one.
        _actionsGrid.CurrentCellChanged += (_, _) => { if (!_binding && !_actionSort.IsRebinding) { UpdateDetails(); ActionSelected?.Invoke(this, EventArgs.Empty); } };
        _routinesGrid.CurrentCellChanged += (_, _) => { if (!_binding && !_routineSort.IsRebinding) { BindProgress(); UpdateDetails(); } };
        _progressGrid.CurrentCellChanged += (_, _) => { if (!_binding && !_progressSort.IsRebinding) UpdateDetails(); };
        _historyButton.Click += (_, _) => HistoryRequested?.Invoke(this, EventArgs.Empty);
        _editRoutineButton.Click += (_, _) => EditRoutineRequested?.Invoke(this, EventArgs.Empty);
        _editProgressButton.Click += (_, _) => EditProgressRequested?.Invoke(this, EventArgs.Empty);
        _deleteProgressButton.Click += (_, _) => DeleteProgressRequested?.Invoke(this, EventArgs.Empty);
        _newRoutineButton.Click += (_, _) => NewRoutineRequested?.Invoke(this, EventArgs.Empty);
        _routineStatusButton.Click += (_, _) => RoutineStatusRequested?.Invoke(this, EventArgs.Empty);
        _newProgressButton.Click += (_, _) => NewProgressRequested?.Invoke(this, EventArgs.Empty);
        Controls.Add(_showArchivedCheckBox);
        _showArchivedCheckBox.CheckedChanged += (_, _) => ArchiveFilterChanged?.Invoke(this, EventArgs.Empty);
        _editButton.Click += (_, _) => EditRequested?.Invoke(this, EventArgs.Empty);
        _archiveButton.Click += (_, _) => ArchiveRequested?.Invoke(this, EventArgs.Empty);
        ApplyTexts();
    }
    /// <summary>Raised after the current action has changed.</summary>
    public event EventHandler? ActionSelected;
    /// <summary>Explicit add-routine command.</summary>
    public event EventHandler? NewRoutineRequested;
    /// <summary>Explicit pause/reactivate command.</summary>
    public event EventHandler? RoutineStatusRequested;
    /// <summary>Explicit add-history command.</summary>
    public event EventHandler? NewProgressRequested;
    /// <summary>Selected action or null.</summary>
    public HealthAction? SelectedAction => _actions.SingleOrDefault(item => item.Action.Id == Id(_actionsGrid))?.Action;
    /// <summary>Selected routine or null.</summary>
    public Routine? SelectedRoutine => _routines.SingleOrDefault(item => item.Id == Id(_routinesGrid));
    /// <summary>Selected execution record or null.</summary>
    public ProgressEntry? SelectedProgress => _entries.SingleOrDefault(item => item.Id == Id(_progressGrid));
    /// <summary>Binds actions while preserving ID selection and suppressing intermediate binding events.</summary>
    public void SetActions(IReadOnlyList<HealthActionSummary> actions, Guid? preferredId = null)
    {
        Guid? selected = preferredId ?? SelectedAction?.Id; _binding = true;
        try
        {
            _actions = actions;
            _actionSort.SetRows(actions.Select(item => new Row(item.Action.Id, item.Action.Title, AppStrings.ActionTypeText(item.Action.ActionType),
                item.Action.IsArchived ? AppStrings.Archived : AppStrings.ActionStatusText(item.Action.Status),
                SortCategory: (int)item.Action.ActionType, SortStatus: (int)item.Action.Status, SortArchived: item.Action.IsArchived)).ToList());
            Select(_actionsGrid, selected); ShowEmpty(_actionsGrid, _emptyStateLabel, actions.Count == 0);
        }
        finally { _binding = false; }
        UpdateDetails();
    }
    /// <summary>Binds one coherent child snapshot; history follows the selected routine.</summary>
    public void SetDetails(IReadOnlyList<Routine> routines, IReadOnlyList<ProgressEntry> entries, Guid? routineId = null, Guid? progressId = null)
    {
        _binding = true;
        try
        {
            _routines = routines; _entries = entries;
            _routineSort.SetRows(routines.Select(item => new Row(item.Id, item.Title, Status: AppStrings.RoutineStatusText(item.Status), SortStatus: (int)item.Status)).ToList());
            Select(_routinesGrid, routineId); ShowEmpty(_routinesGrid, _routinesEmpty, routines.Count == 0);
            BindProgress(progressId);
        }
        finally { _binding = false; }
        UpdateDetails();
    }
    /// <summary>Updates compact counts.</summary>
    public void SetOverview(HealthActionOverview overview) => _overview.SetOverview(overview);
    /// <summary>Localizes headings; presenter refreshes row projections.</summary>
    public void ApplyTexts()
    {
        _showArchivedCheckBox.Text = AppStrings.ShowArchived; _editButton.Text = AppStrings.Edit;
        _historyButton.Text = AppStrings.RevisionCommand;
        _editRoutineButton.Text = _editProgressButton.Text = AppStrings.Edit; _deleteProgressButton.Text = AppStrings.Delete;
        _actionsHeading.Text = AppStrings.ActionList; _routinesHeading.Text = AppStrings.RoutineList; _progressHeading.Text = AppStrings.ProgressHistory;
        _emptyStateLabel.Text = AppStrings.ActionsEmpty; _newRoutineButton.Text = AppStrings.NewRoutine; _newProgressButton.Text = AppStrings.NewProgress;
        _actionsGrid.Columns[0].HeaderText = AppStrings.ActionTitle; _actionsGrid.Columns[1].HeaderText = AppStrings.ActionKind; _actionsGrid.Columns[2].HeaderText = AppStrings.ActionState;
        _routinesGrid.Columns[0].HeaderText = AppStrings.RoutineTitle; _routinesGrid.Columns[1].HeaderText = AppStrings.ActionState;
        _progressGrid.Columns[0].HeaderText = AppStrings.EntryDate; _progressGrid.Columns[1].HeaderText = AppStrings.ProgressState; _progressGrid.Columns[2].HeaderText = AppStrings.ProgressCountColumn;
        _actionDetails.AccessibleName = AppStrings.ActionDescription; _routineDetails.AccessibleName = AppStrings.RoutineSchedule; _progressDetails.AccessibleName = AppStrings.ProgressNote;
        _actionsGrid.AccessibleName = AppStrings.ActionList; _routinesGrid.AccessibleName = AppStrings.RoutineList; _progressGrid.AccessibleName = AppStrings.ProgressHistory;
        _overview.ApplyTexts(); SetActions(_actions); SetDetails(_routines, _entries, SelectedRoutine?.Id, SelectedProgress?.Id);
    }
    private void BindProgress(Guid? preferredId = null)
    {
        bool priorBinding = _binding; _binding = true;
        try
        {
            Guid? selected = preferredId ?? SelectedProgress?.Id;
            var entries = _entries.Where(item => item.RoutineId == SelectedRoutine?.Id).ToList();
            _progressSort.SetRows(entries.Select(item => new Row(item.Id, Status: AppStrings.CompletionText(item.Completion), Time: Time(item.OccurredAt), Count: item.Count?.ToString() ?? string.Empty, SortTime: item.OccurredAt, SortStatus: (int)item.Completion, SortCount: item.Count)).ToList());
            Select(_progressGrid, selected); ShowEmpty(_progressGrid, _progressEmpty, entries.Count == 0);
        }
        finally { _binding = priorBinding; }
    }
    private void UpdateDetails()
    {
        var summary = _actions.SingleOrDefault(item => item.Action.Id == Id(_actionsGrid)); var action = summary?.Action;
        _actionDetails.Text = action is null ? string.Empty : string.Join(Environment.NewLine,
            action.Title, AppStrings.ActionKind + ": " + AppStrings.ActionTypeText(action.ActionType),
            AppStrings.ActionState + ": " + AppStrings.ActionStatusText(action.Status),
            AppStrings.EntryTopic + ": " + (summary!.HealthTopicTitle ?? (action.HealthTopicId.HasValue ? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic)),
            AppStrings.ActionOrigin + ": " + AppStrings.ActionOriginText(action.Origin),
            AppStrings.ActionSourceLink + ": " + (summary.SourceTitle ?? (action.SourceId.HasValue ? AppStrings.MissingActionLink : AppStrings.NoActionLink)),
            AppStrings.ActionSessionLink + ": " + (summary.SessionTitle ?? (action.SessionId.HasValue ? AppStrings.MissingActionLink : AppStrings.NoActionLink)), action.OriginNote, action.Description);
        _editButton.Enabled = _archiveButton.Enabled = _historyButton.Enabled = action is not null;
        _archiveButton.Text = action?.IsArchived == true ? AppStrings.Reactivate : AppStrings.Archive;
        _editRoutineButton.Enabled = SelectedRoutine is not null;
        _editProgressButton.Enabled = _deleteProgressButton.Enabled = SelectedProgress is not null;
        var routine = SelectedRoutine;
        _routineDetails.Text = routine is null ? string.Empty : string.Join(Environment.NewLine,
            AppStrings.RoutineSchedule + ": " + routine.ScheduleText, routine.Description);
        _progressDetails.Text = SelectedProgress?.Note ?? string.Empty;
        SetDetailVisibility(_actionDetails, action is not null);
        SetDetailVisibility(_routineDetails, routine is not null);
        SetDetailVisibility(_progressDetails, SelectedProgress is not null);
        _newRoutineButton.Enabled = action is not null; _routineStatusButton.Enabled = _newProgressButton.Enabled = routine is not null;
        _routineStatusButton.Text = routine?.Status == RoutineStatus.Paused ? AppStrings.ActivateRoutine : AppStrings.PauseRoutine;
        _routinesEmpty.Text = action is null ? AppStrings.SelectAction : AppStrings.RoutinesEmpty;
        _progressEmpty.Text = routine is null ? AppStrings.SelectRoutine : AppStrings.ProgressEmpty;
    }
    private static void SetDetailVisibility(Control details, bool show)
    {
        details.Visible = show;
        var pane = (TableLayoutPanel)details.Parent!;
        // In empty states let the list/guidance occupy the whole content area.
        pane.SetRowSpan(pane.GetControlFromPosition(0, 2)!, show ? 1 : 2);
    }
    private static string Time(DateTimeOffset time) => time.ToLocalTime().ToString(AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy HH:mm" : "MM/dd/yyyy HH:mm");
    private static Guid? Id(DataGridView grid) => (grid.CurrentRow?.DataBoundItem as Row)?.Id;
    private static void Select(DataGridView grid, Guid? id) { foreach (DataGridViewRow row in grid.Rows) if ((row.DataBoundItem as Row)?.Id == id) { grid.CurrentCell = row.Cells[0]; break; } }
    private static void ShowEmpty(DataGridView grid, Label label, bool empty) { grid.Visible = grid.TabStop = !empty; label.Visible = empty; }
    private static Label Empty() => new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Padding = new(UiMetrics.StandardSpacing), ForeColor = UiColors.SecondaryText };
    private static Label Heading() => new() { Dock = DockStyle.Fill, Font = UiFonts.CardTitle, ForeColor = UiColors.PrimaryAccent, UseMnemonic = false, TextAlign = ContentAlignment.MiddleLeft };
    private static RichTextBox Details() => new() { Dock = DockStyle.Fill, ReadOnly = true, DetectUrls = false, ScrollBars = RichTextBoxScrollBars.Vertical, BorderStyle = BorderStyle.None, BackColor = UiColors.CardBackground, Font = UiFonts.Body, TabIndex = 3 };
    private static Button ActionButton()
    {
        var button = new Button { AutoSize = true, MinimumSize = new(76, UiMetrics.ActionHeight), FlatStyle = FlatStyle.Flat, BackColor = UiColors.CardBackground, Margin = new(0, 0, UiMetrics.StandardSpacing, 0) };
        button.FlatAppearance.BorderColor = UiColors.BorderColor; return button;
    }
    private static Control Pane(Label heading, DataGridView grid, Label empty, RichTextBox details, int detailPercent, params Button[] actions)
    {
        var pane = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new(UiMetrics.StandardSpacing), BackColor = UiColors.CardBackground, Margin = Padding.Empty };
        pane.RowStyles.Add(new(SizeType.Absolute, 30)); pane.RowStyles.Add(new(SizeType.Absolute, actions.Length == 0 ? 0 : 44));
        pane.RowStyles.Add(new(SizeType.Percent, 100 - detailPercent)); pane.RowStyles.Add(new(SizeType.Percent, detailPercent));
        pane.Controls.Add(heading, 0, 0);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = Padding.Empty, TabIndex = 0 };
        for (int i = 0; i < actions.Length; i++) { actions[i].TabIndex = i; buttons.Controls.Add(actions[i]); }
        pane.Controls.Add(buttons, 0, 1);
        var list = new Panel { Dock = DockStyle.Fill, Margin = new(0, 4, 0, 4), TabIndex = 1 }; list.Controls.Add(grid); list.Controls.Add(empty);
        pane.Controls.Add(list, 0, 2); pane.Controls.Add(details, 0, 3);
        var surface = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, Margin = Padding.Empty, BackColor = UiColors.CardBackground };
        surface.Controls.Add(pane); return surface;
    }
    private static DataGridView Grid(params string[] properties)
    {
        var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, BackgroundColor = UiColors.CardBackground, BorderStyle = BorderStyle.None, Font = UiFonts.Body,
            EnableHeadersVisualStyles = false, GridColor = UiColors.BorderColor, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal };
        grid.DefaultCellStyle.Padding = new(6); grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.DefaultCellStyle.SelectionBackColor = UiColors.ListSelectionBackground; grid.DefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        grid.AlternatingRowsDefaultCellStyle.BackColor = UiColors.AlternateRowBackground;
        grid.ColumnHeadersDefaultCellStyle.BackColor = grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiColors.WindowBackground;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        grid.ColumnHeadersDefaultCellStyle.Padding = new(6); grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        foreach (string property in properties) grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = property, MinimumWidth = 60, FillWeight = property switch { "Title" => 180, "Time" => 160, "Status" => 130, "Count" => 65, _ => 100 }, SortMode = DataGridViewColumnSortMode.NotSortable });
        return grid;
    }
    // Presentation keys preserve numbers/instants and enum order across localization;
    // they never replace the domain entities used by explicit lifecycle commands.
    private sealed record Row(Guid Id, string Title = "", string Category = "", string Status = "", string Time = "", string Count = "",
        DateTimeOffset? SortTime = null, int SortStatus = 0, int SortCategory = 0, int? SortCount = null, bool SortArchived = false);
    /// <summary>Includes archived parents in the normal workspace.</summary>
    public bool IncludeArchived => _showArchivedCheckBox.Checked;
    /// <summary>Explicit filter refresh command.</summary>
    public event EventHandler? ArchiveFilterChanged;
    /// <summary>Selected-parent edit command.</summary>
    public event EventHandler? EditRequested;
    /// <summary>Selected-parent archive/reactivate command.</summary>
    public event EventHandler? ArchiveRequested;
    /// <summary>Selected instruction revision history.</summary>
    public event EventHandler? HistoryRequested;
    /// <summary>Selected routine edit command.</summary>
    public event EventHandler? EditRoutineRequested;
    /// <summary>Selected execution edit command.</summary>
    public event EventHandler? EditProgressRequested;
    /// <summary>Selected execution delete command.</summary>
    public event EventHandler? DeleteProgressRequested;

}
