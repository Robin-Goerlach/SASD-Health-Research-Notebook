using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>Stable parent/child identities, never a guessed row index or delayed timer.</summary>
public sealed class AgendaTargetEventArgs(Guid sessionId, Guid? followUpId = null) : EventArgs
{
    /// <summary>Session to open.</summary>
    public Guid SessionId { get; } = sessionId;
    /// <summary>Optional exact child to select.</summary>
    public Guid? FollowUpId { get; } = followUpId;
}

/// <summary>
/// Displays the Application projection. Only preview truncation and localized formatting
/// happen here; eligibility, dates and baseline ordering/grouping remain Application concerns.
/// A column click only reorders the already bounded preview locally.
/// </summary>
public sealed class DashboardAgendaView : UserControl
{
    /// <summary>Bounded preview; full lists remain available in the read-only agenda dialog.</summary>
    public const int SessionPreviewLimit = 5;
    /// <summary>One additional short next step fits the denser follow-up preview.</summary>
    public const int FollowUpPreviewLimit = 6;
    private readonly DataGridView _sessionsGrid = Grid();
    private readonly DataGridView _followUpsGrid = Grid();
    private readonly Label _sessionsTitle = Heading();
    private readonly Label _followUpsTitle = Heading();
    private readonly Label _sessionsEmpty = Empty();
    private readonly Label _followUpsEmpty = Empty();
    private readonly Button _showSessions = More();
    private readonly Button _showFollowUps = More();
    private readonly ThreeStateGridSort<Row> _sessionSort;
    private readonly ThreeStateGridSort<Row> _followUpSort;
    private readonly bool _preview;
    private readonly Control? _unusedSection;
    private DashboardAgenda _agenda = new(Array.Empty<AgendaSession>(), Array.Empty<AgendaFollowUp>());
    private LoadState _state = LoadState.Loading;

    /// <summary>Creates a two-section preview or a single complete agenda list.</summary>
    public DashboardAgendaView(bool preview = true, bool? sessionsOnly = null)
    {
        _sessionSort = new ThreeStateGridSort<Row>(_sessionsGrid, row => row.SessionId)
            .Column(0, row => row.SortTime).Text(1, row => row.Text);
        _followUpSort = new ThreeStateGridSort<Row>(_followUpsGrid, row => row.FollowUpId!.Value)
            .Column(0, row => (row.SortGroup, row.SortDueDate)).Text(1, row => row.Text);
        _sessionSort.Rebound += () => SetRowToolTips(_sessionsGrid);
        _followUpSort.Rebound += () => SetRowToolTips(_followUpsGrid);
        _preview = preview;
        Dock = DockStyle.Fill; Margin = Padding.Empty; BackColor = UiColors.WindowBackground;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = sessionsOnly.HasValue ? 1 : 2, Margin = Padding.Empty };
        var sessions = Section(_sessionsTitle, _sessionsGrid, _sessionsEmpty, _showSessions);
        var followUps = Section(_followUpsTitle, _followUpsGrid, _followUpsEmpty, _showFollowUps);
        layout.RowStyles.Add(new(SizeType.Percent, 100));
        for (int i = 0; i < layout.ColumnCount; i++) layout.ColumnStyles.Add(new(SizeType.Percent, 100F / layout.ColumnCount));
        if (sessionsOnly.HasValue)
        {
            layout.Controls.Add(sessionsOnly.Value ? sessions : followUps, 0, 0);
            // Keep detached controls alive for the shared relabeling path; dispose
            // the unused section with the view because the layout does not own it.
            _unusedSection = sessionsOnly.Value ? followUps : sessions;
        }
        else
        {
            sessions.Margin = new Padding(0, 0, UiMetrics.StandardSpacing, 0);
            layout.Controls.Add(sessions, 0, 0); layout.Controls.Add(followUps, 1, 0);
        }
        Controls.Add(layout);
        Connect(_sessionsGrid); Connect(_followUpsGrid);
        _showSessions.Click += (_, _) => ShowAllSessionsRequested?.Invoke(this, EventArgs.Empty);
        _showFollowUps.Click += (_, _) => ShowAllFollowUpsRequested?.Invoke(this, EventArgs.Empty);
        ApplyTexts();
    }

    /// <summary>Requests exact navigation after mouse or Enter activation.</summary>
    public event EventHandler<AgendaTargetEventArgs>? TargetRequested;
    /// <summary>Requests the complete upcoming list.</summary>
    public event EventHandler? ShowAllSessionsRequested;
    /// <summary>Requests the complete open next-step list.</summary>
    public event EventHandler? ShowAllFollowUpsRequested;
    /// <summary>Last successfully loaded ephemeral projection; never persisted.</summary>
    public DashboardAgenda Agenda => _agenda;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    { if (disposing) _unusedSection?.Dispose(); base.Dispose(disposing); }

    /// <summary>Loading is distinct from a successfully empty notebook.</summary>
    public void SetLoading() { _state = LoadState.Loading; ApplyState(); }
    /// <summary>Unreadable data must never claim that all tasks disappeared.</summary>
    public void SetFailed() { _state = LoadState.Failed; ApplyState(); }
    /// <summary>Publishes one accepted projection; retains IDs through localized rebinding.</summary>
    public void SetAgenda(DashboardAgenda agenda)
    { _agenda = agenda ?? throw new ArgumentNullException(nameof(agenda)); _state = LoadState.Ready; ApplyTexts(); }

    /// <summary>Relabels the existing projection and reapplies local ordering without reading or writing stores.</summary>
    public void ApplyTexts()
    {
        _sessionsTitle.Text = AppStrings.UpcomingSessions; _followUpsTitle.Text = AppStrings.OpenFollowUps;
        _showSessions.Text = _showFollowUps.Text = AppStrings.AgendaShowAll;
        _sessionsGrid.AccessibleName = AppStrings.UpcomingSessions; _followUpsGrid.AccessibleName = AppStrings.OpenFollowUps;
        _sessionsGrid.Columns[0].HeaderText = AppStrings.AgendaDate;
        _followUpsGrid.Columns[0].HeaderText = AppStrings.AgendaGrouping;
        _sessionsGrid.Columns[1].HeaderText = AppStrings.SessionTitle;
        _followUpsGrid.Columns[1].HeaderText = AppStrings.OpenFollowUps;
        _sessionSort.SetRows((_preview ? _agenda.Sessions.Take(SessionPreviewLimit) : _agenda.Sessions)
            .Select(item => new Row(item.SessionId, null, item.ScheduledAt.ToLocalTime().ToString(DateFormat + "\nHH:mm"), item.Title,
                string.Join(Environment.NewLine, item.Title, item.ContactText ?? string.Empty), SortTime: item.ScheduledAt)).ToArray());
        _followUpSort.SetRows((_preview ? _agenda.FollowUps.Take(FollowUpPreviewLimit) : _agenda.FollowUps)
            .Select(item => new Row(item.SessionId, item.FollowUpId, AppStrings.AgendaDueGroup(item.Group)
                + (item.DueDate.HasValue ? "\n" + item.DueDate.Value.ToString(DateFormat) : string.Empty), item.Text, item.Text, SortGroup: item.Group, SortDueDate: item.DueDate)).ToArray());
        ApplyState();
    }

    private static string DateFormat => AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy";
    private void ApplyState()
    {
        bool ready = _state == LoadState.Ready;
        _sessionsGrid.Visible = ready && _agenda.Sessions.Count > 0;
        _followUpsGrid.Visible = ready && _agenda.FollowUps.Count > 0;
        _sessionsEmpty.Visible = !_sessionsGrid.Visible; _followUpsEmpty.Visible = !_followUpsGrid.Visible;
        string? stateText = _state == LoadState.Loading ? AppStrings.AgendaLoading : _state == LoadState.Failed ? AppStrings.AgendaFailed : null;
        _sessionsEmpty.Text = stateText ?? AppStrings.UpcomingSessionsEmpty;
        _followUpsEmpty.Text = stateText ?? AppStrings.OpenFollowUpsEmpty;
        _showSessions.Visible = _preview && ready && _agenda.Sessions.Count > SessionPreviewLimit;
        _showFollowUps.Visible = _preview && ready && _agenda.FollowUps.Count > FollowUpPreviewLimit;
    }

    private void Connect(DataGridView grid)
    {
        grid.CellClick += (_, e) => { if (e.RowIndex >= 0) Activate(grid); };
        grid.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; e.SuppressKeyPress = true; Activate(grid); } };
    }
    private void Activate(DataGridView grid)
    {
        if (_state == LoadState.Ready && grid.CurrentRow?.DataBoundItem is Row row)
            TargetRequested?.Invoke(this, new(row.SessionId, row.FollowUpId));
    }
    private static void SetRowToolTips(DataGridView grid)
    {
        foreach (DataGridViewRow row in grid.Rows)
        {
            var item = (Row)row.DataBoundItem;
            row.Cells[1].ToolTipText = item.Detail;
        }
    }
    private static Control Section(Label title, DataGridView grid, Label empty, Button more)
    {
        var list = new Panel { Dock = DockStyle.Fill, TabIndex = 0 };
        list.Controls.Add(grid); list.Controls.Add(empty);
        var panel = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty, BackColor = UiColors.CardBackground };
        panel.Controls.Add(list); panel.Controls.Add(more); panel.Controls.Add(title);
        return panel;
    }
    private static Label Heading() => new() { Dock = DockStyle.Top, Height = 30, Font = UiFonts.CardTitle,
        ForeColor = UiColors.PrimaryText, Padding = new Padding(8, 5, 0, 0), UseMnemonic = false };
    private static Label Empty() => new() { Dock = DockStyle.Fill, Font = UiFonts.Body,
        ForeColor = UiColors.SecondaryText, Padding = new Padding(12), TextAlign = ContentAlignment.MiddleCenter };
    private static Button More()
    {
        var button = new Button { Dock = DockStyle.Bottom, Height = 30, FlatStyle = FlatStyle.Flat,
            ForeColor = UiColors.PrimaryAccent, BackColor = UiColors.CardBackground, TabIndex = 1 };
        button.FlatAppearance.BorderColor = UiColors.BorderColor;
        return button;
    }
    private static DataGridView Grid()
    {
        var grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false,
            AllowUserToResizeColumns = false, ReadOnly = true, MultiSelect = false, RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = UiColors.CardBackground, BorderStyle = BorderStyle.None, Font = UiFonts.Body,
            ScrollBars = ScrollBars.Vertical, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            GridColor = UiColors.BorderColor, EnableHeadersVisualStyles = false, TabIndex = 0 };
        grid.RowTemplate.Height = UiMetrics.AgendaRowHeight;
        grid.ColumnHeadersHeight = 26; grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.DefaultCellStyle.Padding = new Padding(5, 2, 5, 2);
        grid.DefaultCellStyle.ForeColor = UiColors.PrimaryText;
        grid.DefaultCellStyle.SelectionBackColor = UiColors.ListSelectionBackground;
        grid.DefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        grid.ColumnHeadersDefaultCellStyle.BackColor = UiColors.WindowBackground;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = UiColors.SecondaryText;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiColors.WindowBackground;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiColors.SecondaryText;
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.When), FillWeight = 32, MinimumWidth = 95, SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True, Font = UiFonts.Small } });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.Text), FillWeight = 68, MinimumWidth = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
        return grid;
    }
    private enum LoadState { Loading, Ready, Failed }
    private sealed record Row(Guid SessionId, Guid? FollowUpId, string When, string Text, string Detail, DateTimeOffset? SortTime = null, FollowUpDueGroup SortGroup = default, DateOnly? SortDueDate = null);
}
