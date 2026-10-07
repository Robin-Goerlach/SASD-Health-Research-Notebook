using System.Reflection;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    // UI-NAV-001 / UI-GRID-001: exercise real controls and header events. Reflection
    // is confined to the test harness; production sorting uses explicit typed keys.
    private static void CheckGridUx(string testPath, UiLanguage language)
    {
        var stores = Directory.GetFiles(testPath, "*", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(path => path, File.ReadAllBytes);
        using var host = new Form { Size = new(1120, 740) };
        ShowOffScreen(host);
        using var nav = new NavigationControl();
        var hints = Field<ToolTip>(nav, "_navigationToolTip");
        var buttons = Descendants(nav).OfType<NavigationButton>().OrderBy(button => button.TabIndex).ToArray();
        var pages = Enum.GetValues<NavigationPage>();
        Assert(buttons.Length == pages.Length, "Navigation tooltip coverage incomplete.");
        foreach (UiLanguage current in new[] { language, OtherLanguage(language), language })
        {
            AppLanguage.SetLanguage(current); nav.ApplyTexts();
            for (int i = 0; i < pages.Length; i++)
            {
                string hint = hints.GetToolTip(buttons[i]) ?? string.Empty;
                Assert(hint == AppStrings.NavigationHint(pages[i]) && hint.Length is > 20 and < 110
                    && !hint.Contains('\n'), "Tooltip missing, stale or too long.");
                var measured = TextRenderer.MeasureText(hint, SystemFonts.DefaultFont);
                Assert(measured.Width < 800, "Navigation tooltip exceeds available screen width.");
                NavigationPage? requested = null;
                EventHandler<NavigationPageChangedEventArgs> handler = (_, e) => requested = e.Page;
                nav.PageRequested += handler; buttons[i].PerformClick(); nav.PageRequested -= handler;
                Assert(requested == pages[i] && buttons[i].IsSelected, "Tooltip changed navigation behavior.");
            }
        }

        DateTimeOffset time = new(2026, 10, 5, 12, 0, 0, TimeSpan.FromHours(2));
        var topics = new[]
        {
            new HealthTopicSummary { Id = Guid.NewGuid(), Title = "CODEX TEST – Beta", CreatedAt = time.AddDays(1), Status = HealthTopicStatus.Historical, ShortDescription = "" },
            new HealthTopicSummary { Id = Guid.NewGuid(), Title = "CODEX TEST – Alpha", CreatedAt = time, Status = HealthTopicStatus.Observation, ShortDescription = "Z" },
            new HealthTopicSummary { Id = Guid.NewGuid(), Title = "CODEX TEST – alpha", CreatedAt = time.ToOffset(TimeSpan.FromHours(-5)), Status = HealthTopicStatus.Suspected, ShortDescription = "A" }
        };
        using (var view = new HealthTopicsView())
        {
            Attach(view); view.SetTopics(topics);
            var grid = Field<DataGridView>(view, "_grid");
            var ids = topics.Select(item => item.Id).ToArray();
            Cycle(grid, 0, new[] { ids[1], ids[2], ids[0] }, new[] { ids[0], ids[1], ids[2] });
            Cycle(grid, 3, new[] { ids[1], ids[2], ids[0] }, new[] { ids[0], ids[1], ids[2] }); // Equal instants, different offsets.
            Cycle(grid, 1, new[] { ids[1], ids[2], ids[0] }, new[] { ids[0], ids[2], ids[1] });
            CheckRefreshAndLanguage(grid, () => view.SetTopics(new[] { topics[2], topics[0], topics[1] }), view.ApplyTexts,
                new[] { ids[2], ids[1], ids[0] }, new[] { ids[0], ids[2], ids[1] }, new[] { ids[2], ids[0], ids[1] });
            EveryColumn(grid);
            Header(grid, 0); Header(grid, 3);
            Assert(grid.Columns[0].HeaderCell.SortGlyphDirection == SortOrder.None
                && grid.Columns[3].HeaderCell.SortGlyphDirection == SortOrder.Ascending, "Changing column retained the old arrow.");
            Header(grid, 3); Header(grid, 3);
            Header(grid, 0);
            Capture(host, Path.Combine(testPath, $"grid-topics-{language}.png"));
        }
        using (var view = new TimelineView())
        {
            Attach(view);
            var entries = new[] { time.AddDays(1), time, time.AddHours(1) }
                .Select((at, i) => new HealthEntrySummary(Guid.NewGuid(), null, null, HealthEntryType.Observation,
                    at, "CODEX TEST – Entry " + i, i == 0 ? "Z" : "A")).ToArray();
            view.SetEntries(entries); var grid = Field<DataGridView>(view, "_grid");
            Cycle(grid, 0, new[] { entries[1].Id, entries[2].Id, entries[0].Id }, new[] { entries[0].Id, entries[2].Id, entries[1].Id });
            EveryColumn(grid);
            var cultureEntries = entries.Select((entry, index) => entry with { Title = new[] { "Z", "Ä", "a" }[index] }).ToArray();
            view.SetEntries(cultureEntries);
            Cycle(grid, 2, new[] { entries[2].Id, entries[1].Id, entries[0].Id }, new[] { entries[0].Id, entries[1].Id, entries[2].Id });
        }
        using (var view = new TimelineView())
        {
            Attach(view);
            var entry = new HealthEntrySummary(Guid.NewGuid(), null, null, HealthEntryType.Note,
                time, "CODEX TEST – Mixed", "Synthetic") { ModifiedAt = time };
            var measurement = Measurement.Create(MeasurementType.BloodPressure, time.AddHours(1),
                systolic: 100, diastolic: 60, pulse: 42) with { Id = entry.Id };
            var mixed = new[] { new TimelineItem(null, new MeasurementSummary(measurement, null)), new TimelineItem(entry, null) };
            view.SetItems(mixed);
            var grid = Field<DataGridView>(view, "_grid");
            Assert(grid.Rows.Count == 2 && view.SelectedItem!.Measurement is not null, "Mixed timeline source lost.");
            Header(grid, 0);
            Assert(view.SelectedItem!.Measurement is not null && view.SelectedEntry is null, "Colliding source IDs changed selection.");
            view.ApplyTexts();
            Assert(view.SelectedItem!.Measurement!.Measurement.ModifiedAt == measurement.ModifiedAt, "Mixed refresh lost source token.");
            Assert(grid.Rows.Cast<DataGridViewRow>().Any(row => row.Cells[4].Value.ToString()!.Contains("mmHg")), "Pressure unit missing.");
            Capture(host, Path.Combine(testPath, $"timeline-mixed-{language}.png"));
        }
        using (var view = new MeasurementsView())
        {
            Attach(view);
            var entries = new[] { 100d, 2d, 10d }.Select((number, i) => new MeasurementSummary(
                Measurement.Create(MeasurementType.Pulse, time.AddDays(i), value: number, note: i == 0 ? null : i == 1 ? "" : "A"), null)).ToArray();
            view.SetMeasurements(entries); var grid = Field<DataGridView>(view, "_grid");
            var ids = entries.Select(item => item.Measurement.Id).ToArray();
            Cycle(grid, 2, new[] { ids[1], ids[2], ids[0] }, new[] { ids[0], ids[2], ids[1] });
            Cycle(grid, 5, ids, new[] { ids[2], ids[0], ids[1] }); // null/empty equal and stable.
            Header(grid, 2); AppLanguage.SetLanguage(OtherLanguage(language)); view.ApplyTexts();
            Assert(GridIds(grid).SequenceEqual(new[] { ids[1], ids[2], ids[0] }), "Localized numeric sort changed.");
            AppLanguage.SetLanguage(language); view.ApplyTexts(); Header(grid, 2); Header(grid, 2);
            EveryColumn(grid);
            // Structured pressure sorts by components, never the formatted combined value.
            var pressure = new[] { (100d, 90d), (20d, 12d), (100d, 80d) }.Select(value => new MeasurementSummary(
                Measurement.Create(MeasurementType.BloodPressure, time, systolic: value.Item1, diastolic: value.Item2), null)).ToArray();
            view.SetMeasurements(pressure);
            Cycle(grid, 2, new[] { pressure[1].Measurement.Id, pressure[2].Measurement.Id, pressure[0].Measurement.Id },
                new[] { pressure[0].Measurement.Id, pressure[2].Measurement.Id, pressure[1].Measurement.Id });
            Header(grid, 2);
            Capture(host, Path.Combine(testPath, $"grid-measurements-{language}.png"));
        }
        using (var view = new SessionsView())
        {
            Attach(view);
            var sessions = new[] { SessionStatus.Completed, SessionStatus.Planned, SessionStatus.Cancelled }
                .Select((status, i) => new SessionSummary(Session.Create(time.AddDays(2 - i), "CODEX TEST – Session " + i,
                    SessionType.OtherConsultation, status), null)).ToArray();
            view.SetSessions(sessions); var grid = Field<DataGridView>(view, "_sessionsGrid");
            var ids = sessions.Select(item => item.Session.Id).ToArray();
            int selectionLoads = 0; view.SessionSelected += (_, _) => selectionLoads++;
            Cycle(grid, 2, new[] { ids[1], ids[0], ids[2] }, new[] { ids[2], ids[0], ids[1] });
            selectionLoads = 0; Header(grid, 0);
            Assert(selectionLoads == 0, "Visual sort triggered a transient parent load."); Header(grid, 0); Header(grid, 0);
            EveryColumn(grid);
            var questions = new[] { true, false, false }.Select((answered, i) => SessionQuestion.Create(ids[0], "CODEX TEST – Question " + i, isAnswered: answered)).ToArray();
            var followUps = new DateOnly?[] { new(2026, 10, 6), null, new(2026, 10, 5) }
                .Select((due, i) => SessionFollowUp.Create(ids[0], "CODEX TEST – Follow-up " + i, dueDate: due)).ToArray();
            view.SetSessions(sessions, ids[0]);
            view.SetDependents(questions, followUps);
            var questionGrid = Field<DataGridView>(view, "_questionsGrid");
            Cycle(questionGrid, 1, new[] { questions[1].Id, questions[2].Id, questions[0].Id }, new[] { questions[0].Id, questions[1].Id, questions[2].Id });
            EveryColumn(questionGrid);
            var childGrid = Field<DataGridView>(view, "_followUpsGrid");
            view.OpenFollowUps();
            Cycle(childGrid, 2, new[] { followUps[1].Id, followUps[2].Id, followUps[0].Id }, new[] { followUps[0].Id, followUps[2].Id, followUps[1].Id });
            EveryColumn(childGrid);
            AppLanguage.SetLanguage(OtherLanguage(language)); view.ApplyTexts();
            Assert(view.SelectedFollowUp is not null && childGrid.Columns[2].HeaderText == AppStrings.SessionDueDate, "Session language refresh lost child selection.");
            AppLanguage.SetLanguage(language); view.ApplyTexts();
            Header(grid, 2); Header(childGrid, 2);
            Capture(host, Path.Combine(testPath, $"grid-sessions-{language}.png"));
        }
        using (var view = new SourcesView())
        {
            Attach(view);
            var sources = new[] { "Z", "A", "A" }.Select(title => new SourceSummary(Source.Create(SourceType.Book, "CODEX TEST – " + title), null)).ToArray();
            view.SetSources(sources); EveryColumn(Field<DataGridView>(view, "_sourcesGrid"));
            var locations = new[] { "Z", "A", "A" }.Select(locator => SourceLocation.Create(sources[0].Source.Id, SourceLocationType.Section, locator)).ToArray();
            var notes = locations.Select(location => EvidenceNote.Create(sources[0].Source.Id, location.Id, "CODEX TEST – " + location.Locator, null, "Synthetic note")).ToArray();
            view.SetDependents(locations, notes);
            EveryColumn(Field<DataGridView>(view, "_locationsGrid")); EveryColumn(Field<DataGridView>(view, "_notesGrid"));
        }
        using (var view = new HealthActionsView())
        {
            Attach(view);
            var actions = new[] { "Z", "A", "A" }.Select(title => new HealthActionSummary(HealthAction.Create("CODEX TEST – " + title), null)).ToArray();
            view.SetActions(actions); EveryColumn(Field<DataGridView>(view, "_actionsGrid"));
            var routines = new[] { "Z", "A", "A" }.Select(title => Routine.Create(view.SelectedAction!.Id, "CODEX TEST – " + title)).ToArray();
            var entries = new int?[] { 100, null, 2 }.Select(count => ProgressEntry.Create(routines[0].Id, time, ProgressCompletion.Performed, count: count)).ToArray();
            view.SetDetails(routines, entries, routines[0].Id);
            EveryColumn(Field<DataGridView>(view, "_routinesGrid"));
            view.SetDetails(routines, entries, routines[0].Id);
            var grid = Field<DataGridView>(view, "_progressGrid");
            Cycle(grid, 2, new[] { entries[1].Id, entries[2].Id, entries[0].Id }, new[] { entries[0].Id, entries[2].Id, entries[1].Id });
            EveryColumn(grid);
        }
        var agenda = new DashboardAgenda(
            Enumerable.Range(0, 7).Select(i => new AgendaSession(Guid.NewGuid(), time.AddDays(6 - i), "CODEX TEST – " + i, null)).ToArray(),
            Enumerable.Range(0, 8).Select(i => new AgendaFollowUp(Guid.NewGuid(), Guid.NewGuid(), "CODEX TEST – " + i,
                i == 0 ? null : new DateOnly(2026, 10, 5).AddDays(i), i == 0 ? FollowUpDueGroup.NoDate : FollowUpDueGroup.Later)).ToArray());
        using (var view = new DashboardAgendaView())
        {
            Attach(view); view.SetAgenda(agenda);
            var grid = Field<DataGridView>(view, "_sessionsGrid");
            var ids = agenda.Sessions.Take(5).Select(item => item.SessionId).ToArray();
            Cycle(grid, 0, Enumerable.Reverse(ids).ToArray(), ids); EveryColumn(grid); EveryColumn(Field<DataGridView>(view, "_followUpsGrid"));
            Header(grid, 0); view.SetAgenda(agenda); AppLanguage.SetLanguage(OtherLanguage(language)); view.ApplyTexts();
            Assert(GridIds(grid).SequenceEqual(Enumerable.Reverse(ids)), "Agenda refresh/language lost sort.");
            AppLanguage.SetLanguage(language); view.ApplyTexts();
            Capture(host, Path.Combine(testPath, $"grid-agenda-{language}.png"));
        }
        foreach (bool sessions in new[] { true, false })
        {
            using var dialog = new AgendaListForm(agenda, sessions); ShowOffScreen(dialog);
            var view = Descendants(dialog).OfType<DashboardAgendaView>().Single();
            var grid = Field<DataGridView>(view, sessions ? "_sessionsGrid" : "_followUpsGrid");
            Assert(grid.Rows.Count == (sessions ? 7 : 8), "Full agenda list truncated."); EveryColumn(grid);
        }
        var after = Directory.GetFiles(testPath, "*", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToArray();
        Assert(after.Order().SequenceEqual(stores.Keys.Order()), "Grid UX created a store.");
        foreach (var file in stores) Assert(File.ReadAllBytes(file.Key).SequenceEqual(file.Value), "Grid UX wrote a store.");
        Console.WriteLine($"Navigation tooltips and typed three-state grid sorting passed: {language}");

        void Attach(Control view) { host.Controls.Clear(); host.Controls.Add(view); System.Windows.Forms.Application.DoEvents(); }
    }

    private static UiLanguage OtherLanguage(UiLanguage language) => language == UiLanguage.German ? UiLanguage.English : UiLanguage.German;
    private static Guid RowId(object row)
    {
        var type = row.GetType();
        return (Guid?)(type.GetProperty("Id")?.GetValue(row) ?? type.GetProperty("FollowUpId")?.GetValue(row)
            ?? type.GetProperty("SessionId")?.GetValue(row)) ?? throw new InvalidOperationException("Missing stable row ID.");
    }
    private static Guid[] GridIds(DataGridView grid) => grid.Rows.Cast<DataGridViewRow>().Select(row => RowId(row.DataBoundItem)).ToArray();
    private static void Header(DataGridView grid, int column)
    {
        typeof(DataGridView).GetMethod("OnColumnHeaderMouseClick", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(grid,
            new object[] { new DataGridViewCellMouseEventArgs(column, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0)) });
        System.Windows.Forms.Application.DoEvents();
    }
    private static void Cycle(DataGridView grid, int column, Guid[]? ascending = null, Guid[]? descending = null)
    {
        var original = GridIds(grid);
        Assert(original.Length > 0 && grid.Columns[column].SortMode == DataGridViewColumnSortMode.Programmatic,
            $"Sortable data column not registered: {grid.Columns[column].DataPropertyName}, rows={original.Length}, mode={grid.Columns[column].SortMode}.");
        grid.CurrentCell = grid.Rows[^1].Cells[0]; var selected = RowId(grid.CurrentRow!.DataBoundItem);
        for (int click = 1; click <= 6; click++)
        {
            Header(grid, column);
            var expectedGlyph = click % 3 == 1 ? SortOrder.Ascending : click % 3 == 2 ? SortOrder.Descending : SortOrder.None;
            Assert(grid.Columns[column].HeaderCell.SortGlyphDirection == expectedGlyph, "Wrong sort glyph/cycle.");
            Assert(grid.Columns.Cast<DataGridViewColumn>().Count(item => item.HeaderCell.SortGlyphDirection != SortOrder.None) == (expectedGlyph == SortOrder.None ? 0 : 1), "Multiple active glyphs.");
            Assert(RowId(grid.CurrentRow!.DataBoundItem) == selected, "Sorting lost stable ID selection.");
            Guid[]? expected = click % 3 == 0 ? original : click % 3 == 1 ? ascending : descending;
            if (expected is not null) Assert(GridIds(grid).SequenceEqual(expected), "Typed sort or exact Original order incorrect.");
        }
    }
    private static void EveryColumn(DataGridView grid)
    { for (int column = 0; column < grid.Columns.Count; column++) Cycle(grid, column); }
    private static void CheckRefreshAndLanguage(DataGridView grid, Action refresh, Action localize, Guid[] asc, Guid[] desc, Guid[] original)
    {
        Header(grid, 0); refresh();
        Assert(GridIds(grid).SequenceEqual(asc), "Refresh did not reapply active semantic sorting.");
        UiLanguage before = AppLanguage.Current; AppLanguage.SetLanguage(OtherLanguage(before)); localize();
        Assert(GridIds(grid).SequenceEqual(asc), "Language switch lost sorting.");
        AppLanguage.SetLanguage(before); localize(); Header(grid, 0);
        Assert(GridIds(grid).SequenceEqual(desc), "Language switch lost cycle state."); Header(grid, 0);
        Assert(GridIds(grid).SequenceEqual(original), "Original did not adopt new presenter sequence.");
    }
}
