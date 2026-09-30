using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>
/// Displays health-topic summaries in a WinForms DataGridView.
/// </summary>
public sealed class HealthTopicsView : UserControl
{
    private readonly DataGridView _grid;
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
            ForeColor = UiColors.PrimaryText,
            Text = "Gesundheitsthemen"
        };

        _emptyStateLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            Text = "Noch keine Gesundheitsthemen vorhanden. Lege über „New Health Topic“ das erste Thema an.",
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
            EnableHeadersVisualStyles = false
        };

        _grid.ColumnHeadersDefaultCellStyle.BackColor = UiColors.CardBackground;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = UiColors.PrimaryText;
        _grid.ColumnHeadersDefaultCellStyle.Font = UiFonts.CardTitle;
        _grid.RowTemplate.Height = 34;

        AddColumn("Title", "Title", 190);
        AddColumn("Status", "Status", 130);
        AddColumn("Priority", "Priority", 160);
        AddColumn("CreatedAt", "Created", 140);
        AddColumn("ShortDescription", "Short description", 360, DataGridViewAutoSizeColumnMode.Fill);

        Controls.Add(_grid);
        Controls.Add(_emptyStateLabel);
        Controls.Add(_titleLabel);
    }

    /// <summary>
    /// Sets the title shown above the grid.
    /// </summary>
    public void SetTitle(string title)
    {
        _titleLabel.Text = title;
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

    private void AddColumn(
        string dataPropertyName,
        string headerText,
        int width,
        DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.None)
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = dataPropertyName,
            HeaderText = headerText,
            Width = width,
            AutoSizeMode = autoSizeMode
        });
    }
}
