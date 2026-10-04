using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>Session workspace with questions and user-documented next steps.</summary>
public sealed class SessionsView : UserControl
{
    private readonly DataGridView _sessionsGrid = Grid("Time", "Title", "Status");
    private readonly DataGridView _questionsGrid = Grid("Text", "Status");
    private readonly DataGridView _followUpsGrid = Grid("Text", "Status", "DueDate");
    private readonly Label _emptyStateLabel = Empty();
    private readonly Label _questionsEmpty = Empty();
    private readonly Label _followUpsEmpty = Empty();
    private readonly TextBox _sessionDetails = Details();
    private readonly TextBox _questionDetails = Details();
    private readonly TextBox _followUpDetails = Details();
    private readonly Button _newQuestionButton = Action();
    private readonly Button _answerButton = Action();
    private readonly Button _newFollowUpButton = Action();
    private readonly Button _statusButton = Action();
    private readonly TabPage _questionsTab = new();
    private readonly TabPage _followUpsTab = new();
    private IReadOnlyList<SessionSummary> _sessions = Array.Empty<SessionSummary>();
    private IReadOnlyList<SessionQuestion> _questions = Array.Empty<SessionQuestion>();
    private IReadOnlyList<SessionFollowUp> _followUps = Array.Empty<SessionFollowUp>();
    private readonly CheckBox _showArchivedCheckBox = new() { AutoSize = true, Dock = DockStyle.Top, Height = 28 };
    private readonly Button _editButton = new() { AutoSize = true, MinimumSize = new(90, UiMetrics.ActionHeight) };
    private readonly Button _archiveButton = new() { AutoSize = true, MinimumSize = new(100, UiMetrics.ActionHeight) };
    private bool _binding;
    /// <summary>Creates proportional lists/details, keeping both lower detail fields aligned.</summary>
    public SessionsView()
    {
        Dock = DockStyle.Fill; BackColor = UiColors.WindowBackground; Margin = Padding.Empty;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
        layout.ColumnStyles.Add(new(SizeType.Percent, 45)); layout.ColumnStyles.Add(new(SizeType.Percent, 55));
        layout.RowStyles.Add(new(SizeType.Percent, 60)); layout.RowStyles.Add(new(SizeType.Percent, 40));
        var left = Pane(_sessionsGrid, _emptyStateLabel, _editButton, _archiveButton); left.Margin = new Padding(0, 0, UiMetrics.StandardSpacing, 0);
        layout.Controls.Add(left, 0, 0);
        var tabs = new TabControl { Dock = DockStyle.Fill, Margin = Padding.Empty, TabIndex = 1 };
        _questionsTab.Controls.Add(Pane(_questionsGrid, _questionsEmpty, _newQuestionButton, _answerButton));
        _followUpsTab.Controls.Add(Pane(_followUpsGrid, _followUpsEmpty, _newFollowUpButton, _statusButton));
        tabs.TabPages.Add(_questionsTab); tabs.TabPages.Add(_followUpsTab); layout.Controls.Add(tabs, 1, 0);
        _sessionDetails.Margin = new Padding(0, 3, UiMetrics.StandardSpacing, 3); layout.Controls.Add(_sessionDetails, 0, 1);
        var right = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 3, 0, 3), TabIndex = 2 };
        right.Controls.Add(_followUpDetails); right.Controls.Add(_questionDetails); _followUpDetails.Visible = false;
        tabs.SelectedIndexChanged += (_, _) => { _questionDetails.Visible = tabs.SelectedTab == _questionsTab; _followUpDetails.Visible = tabs.SelectedTab == _followUpsTab; };
        layout.Controls.Add(right, 1, 1); Controls.Add(layout);
        // CurrentCellChanged observes the new row, preserving the Sources regression fix.
        _sessionsGrid.CurrentCellChanged += (_, _) => { if (!_binding) { UpdateDetails(); SessionSelected?.Invoke(this, EventArgs.Empty); } };
        _questionsGrid.CurrentCellChanged += (_, _) => UpdateDetails(); _followUpsGrid.CurrentCellChanged += (_, _) => UpdateDetails();
        _newQuestionButton.Click += (_, _) => NewQuestionRequested?.Invoke(this, EventArgs.Empty);
        _answerButton.Click += (_, _) => AnswerRequested?.Invoke(this, EventArgs.Empty);
        _newFollowUpButton.Click += (_, _) => NewFollowUpRequested?.Invoke(this, EventArgs.Empty);
        _statusButton.Click += (_, _) => FollowUpStatusRequested?.Invoke(this, EventArgs.Empty);
        Controls.Add(_showArchivedCheckBox);
        _showArchivedCheckBox.CheckedChanged += (_, _) => ArchiveFilterChanged?.Invoke(this, EventArgs.Empty);
        _editButton.Click += (_, _) => EditRequested?.Invoke(this, EventArgs.Empty);
        _archiveButton.Click += (_, _) => ArchiveRequested?.Invoke(this, EventArgs.Empty);
        ApplyTexts(); SetSessions(Array.Empty<SessionSummary>()); SetDependents(Array.Empty<SessionQuestion>(), Array.Empty<SessionFollowUp>());
    }
    /// <summary>Selection now refers to the new current session.</summary>
    public event EventHandler? SessionSelected;
    /// <summary>Requests a user question editor.</summary>
    public event EventHandler? NewQuestionRequested;
    /// <summary>Requests an answer editor for the selected question.</summary>
    public event EventHandler? AnswerRequested;
    /// <summary>Requests a user-entered next step.</summary>
    public event EventHandler? NewFollowUpRequested;
    /// <summary>Requests an explicit next-step status change.</summary>
    public event EventHandler? FollowUpStatusRequested;
    /// <summary>Selected shared Domain session.</summary>
    public Session? SelectedSession => _sessions.SingleOrDefault(item => item.Session.Id == Id(_sessionsGrid))?.Session;
    /// <summary>Selected shared question.</summary>
    public SessionQuestion? SelectedQuestion => _questions.SingleOrDefault(item => item.Id == Id(_questionsGrid));
    /// <summary>Selected shared next step.</summary>
    public SessionFollowUp? SelectedFollowUp => _followUps.SingleOrDefault(item => item.Id == Id(_followUpsGrid));
    /// <summary>Refreshes sessions with selection retention.</summary>
    public void SetSessions(IReadOnlyList<SessionSummary> sessions, Guid? preferredId = null)
    {
        var selected = preferredId ?? Id(_sessionsGrid); _binding = true;
        try
        {
            _sessions = sessions;
            _sessionsGrid.DataSource = sessions.Select(item => new Row(item.Session.Id, Time(item.Session.ScheduledAt), item.Session.Title, (item.Session.IsArchived ? AppStrings.Archived : AppStrings.SessionStatusText(item.Session.Status)))).ToList();
            Select(_sessionsGrid, selected); ShowEmpty(_sessionsGrid, _emptyStateLabel, sessions.Count == 0);
        }
        finally { _binding = false; }
        UpdateDetails();
    }
    /// <summary>Displays only the currently selected session's children.</summary>
    public void SetDependents(IReadOnlyList<SessionQuestion> questions, IReadOnlyList<SessionFollowUp> followUps,
        Guid? preferredQuestionId = null, Guid? preferredFollowUpId = null)
    {
        Guid? questionId = preferredQuestionId ?? Id(_questionsGrid), followUpId = preferredFollowUpId ?? Id(_followUpsGrid);
        _questions = questions; _followUps = followUps;
        _questionsGrid.DataSource = questions.Select(item => new Row(item.Id, Text: Preview(item.Text), Status: item.IsAnswered ? AppStrings.SessionAnswered : AppStrings.SessionOpen)).ToList();
        _followUpsGrid.DataSource = followUps.Select(item => new Row(item.Id, Text: Preview(item.Text), Status: AppStrings.FollowUpStatusText(item.Status), DueDate: Date(item.DueDate))).ToList();
        Select(_questionsGrid, questionId); Select(_followUpsGrid, followUpId);
        ShowEmpty(_questionsGrid, _questionsEmpty, questions.Count == 0); ShowEmpty(_followUpsGrid, _followUpsEmpty, followUps.Count == 0); UpdateDetails();
    }
    /// <summary>Updates headings; presenter refreshes localized row projections.</summary>
    public void ApplyTexts()
    {
        _showArchivedCheckBox.Text = AppStrings.ShowArchived; _editButton.Text = AppStrings.Edit;
        _emptyStateLabel.Text = AppStrings.SessionsEmpty; _questionsTab.Text = AppStrings.SessionQuestions; _followUpsTab.Text = AppStrings.SessionFollowUps;
        _newQuestionButton.Text = AppStrings.NewSessionQuestion; _answerButton.Text = AppStrings.EditSessionAnswer;
        _newFollowUpButton.Text = AppStrings.NewSessionFollowUp; _statusButton.Text = AppStrings.ToggleSessionFollowUp;
        _sessionsGrid.Columns[0].HeaderText = AppStrings.EntryDate; _sessionsGrid.Columns[1].HeaderText = AppStrings.SessionTitle; _sessionsGrid.Columns[2].HeaderText = AppStrings.SessionState;
        _questionsGrid.Columns[0].HeaderText = AppStrings.SessionQuestionText; _questionsGrid.Columns[1].HeaderText = AppStrings.SessionState;
        _followUpsGrid.Columns[0].HeaderText = AppStrings.SessionNextStep; _followUpsGrid.Columns[1].HeaderText = AppStrings.SessionState; _followUpsGrid.Columns[2].HeaderText = AppStrings.SessionDueDate;
        _sessionDetails.AccessibleName = AppStrings.Sessions; _questionDetails.AccessibleName = AppStrings.SessionAnswerNote; _followUpDetails.AccessibleName = AppStrings.SessionFollowUps;
        UpdateDetails();
    }
    private void UpdateDetails()
    {
        var summary = _sessions.SingleOrDefault(item => item.Session.Id == Id(_sessionsGrid));
        var session = summary?.Session;
        _editButton.Enabled = _archiveButton.Enabled = session is not null;
        _archiveButton.Text = session?.IsArchived == true ? AppStrings.Reactivate : AppStrings.Archive;
        _sessionDetails.Text = session is null ? string.Empty : string.Join(Environment.NewLine,
            AppStrings.SessionTitle + ": " + session.Title, AppStrings.EntryDate + ": " + Time(session.ScheduledAt),
            AppStrings.SessionKind + ": " + AppStrings.SessionTypeText(session.SessionType), AppStrings.SessionState + ": " + AppStrings.SessionStatusText(session.Status),
            AppStrings.EntryTopic + ": " + (summary!.HealthTopicTitle ?? (session.HealthTopicId.HasValue ? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic)),
            AppStrings.SessionContact + ": " + session.ContactText, AppStrings.SessionNotes + ": " + session.Notes);
        var question = SelectedQuestion; var followUp = SelectedFollowUp;
        _questionDetails.Text = question is null ? string.Empty : AppStrings.SessionQuestionText + ":\r\n" + question.Text + "\r\n\r\n" + AppStrings.SessionAnswerNote + ":\r\n" + question.AnswerNote;
        _followUpDetails.Text = followUp is null ? string.Empty : AppStrings.SessionNextStep + ":\r\n" + followUp.Text + "\r\n" + AppStrings.SessionDueDate + ": " + Date(followUp.DueDate);
        _newQuestionButton.Enabled = _newFollowUpButton.Enabled = session is not null;
        _answerButton.Enabled = question is not null; _statusButton.Enabled = followUp is not null;
        _questionsEmpty.Text = session is null ? AppStrings.SessionSelectionEmpty : AppStrings.SessionQuestionsEmpty;
        _followUpsEmpty.Text = session is null ? AppStrings.SessionSelectionEmpty : AppStrings.SessionFollowUpsEmpty;
    }
    private static string Time(DateTimeOffset time) => time.ToLocalTime().ToString(AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy HH:mm" : "MM/dd/yyyy HH:mm");
    private static string Date(DateOnly? date) => date?.ToString(AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy") ?? string.Empty;
    private static string Preview(string text) => text.Length > 120 ? text[..120] + "…" : text;
    private static Guid? Id(DataGridView grid) => (grid.CurrentRow?.DataBoundItem as Row)?.Id;
    private static void Select(DataGridView grid, Guid? id) { foreach (DataGridViewRow row in grid.Rows) if ((row.DataBoundItem as Row)?.Id == id) { grid.CurrentCell = row.Cells[0]; break; } }
    private static void ShowEmpty(DataGridView grid, Label empty, bool show) { grid.Visible = grid.TabStop = !show; empty.Visible = show; }
    private static Label Empty() => new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Padding = new(UiMetrics.StandardSpacing), ForeColor = UiColors.SecondaryText };
    private static Button Action() => new() { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, MinimumSize = new(100, UiMetrics.ActionHeight), FlatStyle = FlatStyle.Flat, BackColor = UiColors.CardBackground };
    private static TextBox Details() => new() { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, BackColor = UiColors.CardBackground, Font = UiFonts.Body, TabIndex = 2 };
    private static Control Pane(DataGridView grid, Label empty, params Button[] actions)
    {
        var pane = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Margin = Padding.Empty };
        pane.RowStyles.Add(new(SizeType.AutoSize)); pane.RowStyles.Add(new(SizeType.Percent, 100));
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        for (int index = 0; index < actions.Length; index++) { actions[index].TabIndex = index; buttons.Controls.Add(actions[index]); }
        pane.Controls.Add(buttons, 0, 0);
        var list = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty, TabIndex = 1 }; list.Controls.Add(grid); list.Controls.Add(empty); pane.Controls.Add(list, 0, 1); return pane;
    }
    private static DataGridView Grid(params string[] properties)
    {
        var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, BackgroundColor = UiColors.CardBackground, BorderStyle = BorderStyle.None, Font = UiFonts.Body, EnableHeadersVisualStyles = false };
        grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True; grid.DefaultCellStyle.Padding = new(6);
        grid.DefaultCellStyle.SelectionBackColor = UiColors.ListSelectionBackground; grid.DefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        grid.ColumnHeadersDefaultCellStyle.BackColor = UiColors.WindowBackground; grid.AlternatingRowsDefaultCellStyle.BackColor = UiColors.AlternateRowBackground;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.ColumnHeadersDefaultCellStyle.Padding = new(6);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiColors.WindowBackground;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        grid.GridColor = UiColors.BorderColor;
        foreach (string property in properties) grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = property, MinimumWidth = 70, FillWeight = property is "Text" or "Title" ? 160 : 100, SortMode = DataGridViewColumnSortMode.NotSortable });
        return grid;
    }
    private sealed record Row(Guid Id, string Time = "", string Title = "", string Status = "", string Text = "", string DueDate = "");
    /// <summary>Includes archived parents in the normal workspace.</summary>
    public bool IncludeArchived => _showArchivedCheckBox.Checked;
    /// <summary>Explicit filter refresh command.</summary>
    public event EventHandler? ArchiveFilterChanged;
    /// <summary>Selected-parent edit command.</summary>
    public event EventHandler? EditRequested;
    /// <summary>Selected-parent archive/reactivate command.</summary>
    public event EventHandler? ArchiveRequested;

}
