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

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthTopicsView" /> class.
    /// </summary>
    public HealthTopicsView()
    {
        BackColor = UiColors.WindowBackground;
        Dock = DockStyle.Fill;

        _titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 32,
            Font = UiFonts.CardTitle,
            ForeColor = UiColors.PrimaryText
        };

        _emptyStateLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            TextAlign = ContentAlignment.MiddleLeft,
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
            BorderStyle = BorderStyle.FixedSingle,
            RowHeadersVisible = false,
            GridColor = UiColors.BorderColor,
            Font = UiFonts.Body,
            EnableHeadersVisualStyles = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        };

        _grid.ColumnHeadersDefaultCellStyle.BackColor = UiColors.CardBackground;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = UiColors.PrimaryText;
        _grid.ColumnHeadersDefaultCellStyle.Font = UiFonts.CardTitle;
        _grid.RowTemplate.Height = 34;

        _titleColumn = CreateColumn("Title", 190);
        _statusColumn = CreateColumn("Status", 140);
        _priorityColumn = CreateColumn("Priority", 190);
        _createdColumn = CreateColumn("CreatedAt", 150);
        _shortDescriptionColumn = CreateColumn("ShortDescription", 360, DataGridViewAutoSizeColumnMode.Fill);

        _grid.Columns.AddRange(
            _titleColumn,
            _statusColumn,
            _priorityColumn,
            _createdColumn,
            _shortDescriptionColumn);

        Controls.Add(_grid);
        Controls.Add(_emptyStateLabel);
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
        _emptyStateLabel.Text = AppStrings.EmptyHealthTopics;
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

        var rows = summaries
            .Select(HealthTopicGridRow.FromSummary)
            .ToList();

        _grid.DataSource = new BindingList<HealthTopicGridRow>(rows);
        _emptyStateLabel.Visible = rows.Count == 0;
    }

    private static DataGridViewTextBoxColumn CreateColumn(
        string dataPropertyName,
        int width,
        DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.None)
    {
        return new DataGridViewTextBoxColumn
        {
            DataPropertyName = dataPropertyName,
            Width = width,
            AutoSizeMode = autoSizeMode
        };
    }
}
