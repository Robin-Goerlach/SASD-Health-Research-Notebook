using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>
/// Displays health-topic summaries in a WinForms DataGridView.
/// </summary>
public sealed class HealthTopicsView : UserControl
{
    private readonly DataGridView _grid;
    private readonly DataGridViewTextBoxColumn _titleColumn;
    private readonly DataGridViewTextBoxColumn _statusColumn;
    private readonly DataGridViewTextBoxColumn _priorityColumn;
    private readonly DataGridViewTextBoxColumn _createdColumn;
    private readonly DataGridViewTextBoxColumn _shortDescriptionColumn;
    private readonly Label _emptyStateLabel;
    private readonly Label _titleLabel;
    private readonly Button _editButton = new() { AutoSize = true, MinimumSize = new(100, UiMetrics.ActionHeight) };
    private readonly Button _archiveButton = new() { AutoSize = true, MinimumSize = new(100, UiMetrics.ActionHeight) };
    private readonly Button _deleteButton = new() { AutoSize = true, MinimumSize = new(100, UiMetrics.ActionHeight) };
    private readonly CheckBox _showArchived = new() { AutoSize = true, Margin = new(8, 10, 3, 3) };
    private IReadOnlyList<HealthTopicSummary> _topics = Array.Empty<HealthTopicSummary>();

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthTopicsView" /> class.
    /// </summary>
    public HealthTopicsView()
    {
        BackColor = UiColors.WindowBackground;
        Dock = DockStyle.Fill;
        Margin = Padding.Empty;

        _titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 32,
            Font = UiFonts.CardTitle,
            ForeColor = UiColors.PrimaryText
        };

        _emptyStateLabel = new Label
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(UiMetrics.LargeSpacing),
            BackColor = UiColors.CardBackground,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            TextAlign = ContentAlignment.MiddleCenter,
            Visible = false
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = UiColors.CardBackground,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            GridColor = UiColors.BorderColor,
            Font = UiFonts.Body,
            EnableHeadersVisualStyles = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        };

        _grid.ColumnHeadersDefaultCellStyle.BackColor = UiColors.WindowBackground;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = UiColors.PrimaryText;
        _grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiColors.WindowBackground;
        _grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        _grid.ColumnHeadersDefaultCellStyle.Font = UiFonts.CardTitle;
        _grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8);
        _grid.DefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
        _grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        _grid.DefaultCellStyle.ForeColor = UiColors.PrimaryText;
        _grid.DefaultCellStyle.SelectionBackColor = UiColors.ListSelectionBackground;
        _grid.DefaultCellStyle.SelectionForeColor = UiColors.PrimaryText;
        _grid.AlternatingRowsDefaultCellStyle.BackColor = UiColors.AlternateRowBackground;
        _grid.RowTemplate.MinimumHeight = 40;

        _titleColumn = CreateColumn("Title", 170, 25);
        _statusColumn = CreateColumn("Status", 110, 15);
        _priorityColumn = CreateColumn("Priority", 150, 21);
        _createdColumn = CreateColumn("CreatedAt", 125, 16);
        _shortDescriptionColumn = CreateColumn("ShortDescription", 160, 23);

        _grid.Columns.AddRange(
            _titleColumn,
            _statusColumn,
            _priorityColumn,
            _createdColumn,
            _shortDescriptionColumn);

        var list = new Panel { Dock = DockStyle.Fill, TabIndex = 1 }; list.Controls.Add(_grid); list.Controls.Add(_emptyStateLabel);
        var commands = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, WrapContents = false, TabIndex = 0 };
        _editButton.TabIndex = 0; _archiveButton.TabIndex = 1; _deleteButton.TabIndex = 2; _showArchived.TabIndex = 3;
        commands.Controls.AddRange(new Control[] { _editButton, _archiveButton, _deleteButton, _showArchived });
        Controls.Add(list); Controls.Add(commands);
        _editButton.Click += (_, _) => EditRequested?.Invoke(this, EventArgs.Empty);
        _archiveButton.Click += (_, _) => ArchiveRequested?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (_, _) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        _showArchived.CheckedChanged += (_, _) => BindTopics();
        _grid.CurrentCellChanged += (_, _) => UpdateCommands();
        Controls.Add(_titleLabel);

        ApplyTexts();
    }

    /// <summary>
    /// Sets the title shown above the grid.
    /// </summary>
    public void SetTitle(string title)
    {
        _titleLabel.Text = title;
    }

    /// <summary>
    /// Applies localized labels and grid headers.
    /// </summary>
    public void ApplyTexts()
    {
        _editButton.Text = AppStrings.Edit; _deleteButton.Text = AppStrings.Delete; _showArchived.Text = AppStrings.ShowArchived;
        _emptyStateLabel.Text = AppStrings.EmptyHealthTopics; UpdateCommands();
        _titleColumn.HeaderText = AppStrings.ColumnTitle;
        _statusColumn.HeaderText = AppStrings.ColumnStatus;
        _priorityColumn.HeaderText = AppStrings.ColumnPriority;
        _createdColumn.HeaderText = AppStrings.ColumnCreated;
        _shortDescriptionColumn.HeaderText = AppStrings.ColumnShortDescription;
    }

    /// <summary>
    /// Displays the supplied topic summaries.
    /// </summary>
    public void SetTopics(IReadOnlyList<HealthTopicSummary> summaries)
    {
        ArgumentNullException.ThrowIfNull(summaries);
        _topics = summaries; BindTopics();
    }
    private void BindTopics()
    {
        var summaries = _topics.Where(item => _showArchived.Checked || item.Status != Sasd.HealthNotebook.Domain.HealthTopicStatus.Archived);
        Guid? selectedId = (_grid.CurrentRow?.DataBoundItem as HealthTopicGridRow)?.Id;

        var rows = summaries
            .Select(HealthTopicGridRow.FromSummary)
            .ToList();

        _grid.DataSource = new BindingList<HealthTopicGridRow>(rows);
        _emptyStateLabel.Visible = rows.Count == 0;
        _grid.Visible = rows.Count != 0;
        _grid.TabStop = rows.Count != 0;
        if (_emptyStateLabel.Visible) { _emptyStateLabel.BringToFront(); }
        if (_grid.Rows.Count > 0) _grid.CurrentCell = _grid.Rows[0].Cells[0]; else _grid.CurrentCell = null;
        UpdateCommands();
        if (selectedId.HasValue)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.DataBoundItem is HealthTopicGridRow topic && topic.Id == selectedId.Value)
                {
                    _grid.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }
    }

    private static DataGridViewTextBoxColumn CreateColumn(
        string dataPropertyName,
        int minimumWidth,
        float fillWeight)
    {
        return new DataGridViewTextBoxColumn
        {
            DataPropertyName = dataPropertyName,
            MinimumWidth = minimumWidth,
            FillWeight = fillWeight,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };
    }
    private void UpdateCommands()
    {
        var selected = SelectedTopic; _editButton.Enabled = _archiveButton.Enabled = _deleteButton.Enabled = selected is not null;
        _archiveButton.Text = selected?.Status == Sasd.HealthNotebook.Domain.HealthTopicStatus.Archived ? AppStrings.Reactivate : AppStrings.Archive;
    }
    /// <summary>Selected displayed summary including its conflict token.</summary>
    public HealthTopicSummary? SelectedTopic => _topics.SingleOrDefault(item => item.Id == (_grid.CurrentRow?.DataBoundItem as HealthTopicGridRow)?.Id);
    /// <summary>Selected topic correction command.</summary>
    public event EventHandler? EditRequested;
    /// <summary>Selected topic archive/reactivate command.</summary>
    public event EventHandler? ArchiveRequested;
    /// <summary>Explicit selected topic delete command.</summary>
    public event EventHandler? DeleteRequested;
}
