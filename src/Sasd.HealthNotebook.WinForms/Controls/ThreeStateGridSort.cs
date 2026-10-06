using System.Globalization;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Controls;

/// <summary>
/// Local, read-only row ordering. Views explicitly register typed keys; display strings
/// are never parsed into dates or numbers. No presenter, repository or reflection is used.
/// </summary>
internal sealed class ThreeStateGridSort<TRow>(DataGridView grid, Func<TRow, Guid> identity)
    where TRow : class
{
    private readonly Dictionary<int, Comparison<TRow>> _columns = new();
    private TRow[] _original = Array.Empty<TRow>();
    private int? _activeColumn;
    private SortOrder _direction;

    /// <summary>
    /// Covers the complete native DataSource/current-cell transition and ID restoration.
    /// Views must suppress command-state changes as well as selection callbacks here:
    /// disabling a focused button can transfer focus into DataGridView.OnEnter while
    /// SetCurrentCellAddressCore is still running. Rebound is the safe publication point.
    /// </summary>
    public bool IsRebinding { get; private set; }
    /// <summary>Refresh details once, after the stable ID has been restored.</summary>
    public event Action? Rebound;

    /// <summary>Registers a semantic key, with null before non-null in ascending order.</summary>
    public ThreeStateGridSort<TRow> Column<TKey>(int index, Func<TRow, TKey> key)
    {
        _columns.Add(index, (left, right) => Comparer<TKey>.Default.Compare(key(left), key(right)));
        Enable(index);
        return this;
    }

    /// <summary>Text uses the current UI culture, ignores case, and treats null as empty.</summary>
    public ThreeStateGridSort<TRow> Text(int index, Func<TRow, string?> key)
    {
        _columns.Add(index, (left, right) => StringComparer.Create(
            CultureInfo.GetCultureInfo(AppLanguage.Current == UiLanguage.German ? "de-DE" : "en-US"),
            ignoreCase: true).Compare(key(left) ?? string.Empty, key(right) ?? string.Empty));
        Enable(index);
        return this;
    }

    private void Enable(int index)
    {
        // Only registered, visible data columns are eligible. Native automatic sorting
        // cannot restore the presenter's sequence, so we own the cycle and glyphs.
        grid.Columns[index].SortMode = DataGridViewColumnSortMode.Programmatic;
        if (_columns.Count != 1) return;
        grid.ColumnHeaderMouseClick += HeaderClick;
        grid.Disposed += (_, _) => grid.ColumnHeaderMouseClick -= HeaderClick;
    }

    /// <summary>
    /// Replace the baseline on every view refresh, retaining active column/direction.
    /// Copy it separately: sorting an already sorted DataSource would lose Original.
    /// </summary>
    public void SetRows(IEnumerable<TRow> rows)
    {
        _original = rows.ToArray();
        Apply();
    }

    private void HeaderClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || !_columns.ContainsKey(e.ColumnIndex)
            || !grid.Columns[e.ColumnIndex].Visible) return;
        // A different column starts Asc. Repeated clicks cycle Asc -> Desc -> Original.
        _direction = _activeColumn != e.ColumnIndex ? SortOrder.Ascending
            : _direction == SortOrder.Ascending ? SortOrder.Descending : SortOrder.None;
        _activeColumn = _direction == SortOrder.None ? null : e.ColumnIndex;
        Apply();
    }

    private void Apply()
    {
        Guid? selected = grid.CurrentRow?.DataBoundItem is TRow row ? identity(row) : null;
        int column = grid.CurrentCell?.ColumnIndex ?? 0;
        var indexed = _original.Select((item, index) => (item, index)).ToArray();
        if (_activeColumn.HasValue)
        {
            Comparison<TRow> compare = _columns[_activeColumn.Value];
            Array.Sort(indexed, (left, right) =>
            {
                // Reverse the key comparison, never the tie-breaker. Equal values retain
                // baseline order in BOTH directions, including null/empty ties.
                int result = _direction == SortOrder.Ascending
                    ? compare(left.item, right.item) : compare(right.item, left.item);
                return result != 0 ? result : left.index.CompareTo(right.index);
            });
        }
        IsRebinding = true;
        try
        {
            grid.DataSource = indexed.Select(item => item.item).ToList();
            foreach (DataGridViewRow bound in grid.Rows)
                if (bound.DataBoundItem is TRow item && identity(item) == selected)
                { grid.CurrentCell = bound.Cells[column]; break; }
            foreach (DataGridViewColumn header in grid.Columns)
                header.HeaderCell.SortGlyphDirection = header.Index == _activeColumn ? _direction : SortOrder.None;
        }
        finally { IsRebinding = false; }
        Rebound?.Invoke();
    }
}
