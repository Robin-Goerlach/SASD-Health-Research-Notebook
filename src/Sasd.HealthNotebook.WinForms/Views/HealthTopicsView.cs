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
        Guid? selectedId = (_grid.CurrentRow?.DataBoundItem as HealthTopicGridRow)?.Id;

        var rows = summaries
            .Select(HealthTopicGridRow.FromSummary)
            .ToList();

        _grid.DataSource = new BindingList<HealthTopicGridRow>(rows);
        _emptyStateLabel.Visible = rows.Count == 0;
        _grid.Visible = rows.Count != 0;
        _grid.TabStop = rows.Count != 0;
        if (_emptyStateLabel.Visible) { _emptyStateLabel.BringToFront(); }
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
}
