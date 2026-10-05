using System.Reflection;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    // UI-DASH-001: synthetic persisted fixtures, real shell/navigation, both languages.
    private static void CheckDashboardAgenda(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "agenda-" + language); Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            using var shell = NewSessionShell(); ShowOffScreen(shell); WaitForReload(shell);
            var view = Field<DashboardAgendaView>(shell, "_agendaView");
            Assert(Field<Label>(view, "_sessionsEmpty").Visible && Field<Label>(view, "_sessionsEmpty").Text == AppStrings.UpcomingSessionsEmpty, "Agenda empty session state missing.");
            Assert(Field<Label>(view, "_followUpsEmpty").Visible && Field<Label>(view, "_followUpsEmpty").Text == AppStrings.OpenFollowUpsEmpty, "Agenda empty follow-up state missing.");
            shell.Size = shell.MinimumSize; Capture(shell, Path.Combine(root, "agenda-empty-minimum.png"));
            var service = CreateSessionService();
            DateTimeOffset now = DateTimeOffset.Now; var today = DateOnly.FromDateTime(now.DateTime);
            var sessions = new List<Session>(); var followUps = new List<SessionFollowUp>();
            Task.Run(async () =>
            {
                await new HealthTopicService(new JsonHealthTopicRepository()).CreateTopicAsync(new() { Title = "CODEX TEST – Agenda topic" });
                for (int i = 0; i < 7; i++) sessions.Add(await service.CreateSessionAsync(new(now.AddDays(i + 1), "CODEX TEST – Upcoming session " + i, ContactText: "Synthetic institution")));
                var completed = await service.CreateSessionAsync(new(now.AddDays(-1), "CODEX TEST – Completed conversation", Status: SessionStatus.Completed));
                for (int i = 0; i < 9; i++)
                {
                    DateOnly? due = i == 0 ? today.AddDays(-1) : i < 3 ? today : i < 5 ? today.AddDays(1) : null;
                    followUps.Add(await service.CreateFollowUpAsync(new(completed.Id, "CODEX TEST – Open follow-up " + i, DueDate: due)));
                }
            }).GetAwaiter().GetResult();
            var files = Directory.GetFiles(root).Where(path => !path.EndsWith(".png")).ToDictionary(path => path, File.ReadAllBytes);
            WaitForReload(shell);
            var sessionGrid = Field<DataGridView>(view, "_sessionsGrid"); var followGrid = Field<DataGridView>(view, "_followUpsGrid");
            Assert(sessionGrid.Rows.Count == 5 && followGrid.Rows.Count == 6 && view.Agenda.Sessions.Count == 7 && view.Agenda.FollowUps.Count == 9, "Preview truncated the complete projection or exceeded limits.");
            Assert(Field<Button>(view, "_showSessions").Visible && Field<Button>(view, "_showFollowUps").Visible, "Show all missing.");
            Assert(!Field<Label>(view, "_sessionsEmpty").Visible && !Field<Label>(view, "_followUpsEmpty").Visible, "Empty state covers loaded data.");
            foreach (Size size in new[] { shell.MinimumSize, new Size(1280, 820) })
            {
                shell.Size = size; System.Windows.Forms.Application.DoEvents();
                foreach (var control in new Control[] { view, sessionGrid, followGrid, Field<Button>(view, "_showSessions"), Field<Button>(view, "_showFollowUps") }) AssertWithinParent(control);
                foreach (var title in new[] { Field<Label>(view, "_sessionsTitle"), Field<Label>(view, "_followUpsTitle") }) AssertTextFits(title);
                foreach (var grid in new[] { sessionGrid, followGrid })
                {
                    Assert(grid.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) <= grid.ClientSize.Width && grid.Height > grid.ColumnHeadersHeight + grid.RowTemplate.Height, "Agenda grid clipped or horizontal scrolling required.");
                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        var cell = row.Cells[0]; var style = cell.InheritedStyle;
                        var available = new Size(cell.Size.Width - style.Padding.Horizontal, cell.Size.Height - style.Padding.Vertical);
                        var measured = TextRenderer.MeasureText(cell.FormattedValue?.ToString() ?? string.Empty,
                            style.Font ?? grid.Font, new Size(available.Width, int.MaxValue), TextFormatFlags.WordBreak);
                        Assert(measured.Height <= available.Height, "Two-line agenda date/group text is clipped.");
                    }
                }
                var topicGrid = Field<DataGridView>(Field<HealthTopicsView>(shell, "_dashboardTopicsView"), "_grid");
                Assert(topicGrid.Height >= topicGrid.ColumnHeadersHeight + topicGrid.Rows[0].Height, "Dashboard topic row is clipped at minimum size.");
                Capture(shell, Path.Combine(root, $"agenda-filled-{size.Width}.png"));
            }
            sessionGrid.Focus(); Assert(sessionGrid.Focused, "Agenda keyboard focus inaccessible.");
            shell.SelectNextControl(sessionGrid, true, true, true, true);
            Assert(Field<Button>(view, "_showSessions").Focused, "Tab traversal cannot reach Show all.");
            ActivateAgendaRow(sessionGrid, 1, mouse: true);
            var sessionView = Field<SessionsView>(shell, "_sessionsView");
            PumpUntil(() => sessionView.SelectedSession?.Id == sessions[1].Id, "exact upcoming session navigation");
            ReturnToAgenda(shell); ActivateAgendaRow(followGrid, 2);
            PumpUntil(() => sessionView.SelectedFollowUp?.Id == followUps[2].Id, "exact follow-up navigation");
            var tab = Field<TabPage>(sessionView, "_followUpsTab");
            Assert(((TabControl)tab.Parent!).SelectedTab == tab && Field<DataGridView>(sessionView, "_followUpsGrid").Focused, "Follow-up tab/focus missing.");
            ReturnToAgenda(shell);
            OpenFullAgenda(Field<Button>(view, "_showSessions"), sessions.Count, sessions[6].Id, null, root);
            PumpUntil(() => sessionView.SelectedSession?.Id == sessions[6].Id, "session beyond preview navigation");
            ReturnToAgenda(shell);
            OpenFullAgenda(Field<Button>(view, "_showFollowUps"), followUps.Count, followUps[8].SessionId, followUps[8].Id, root);
            PumpUntil(() => sessionView.SelectedFollowUp?.Id == followUps[8].Id, "child beyond preview navigation");
            ReturnToAgenda(shell);
            var languageSelector = Field<ComboBox>(shell, "_languageComboBox");
            languageSelector.SelectedIndex = language == UiLanguage.German ? 1 : 0; WaitForReload(shell);
            Assert(Field<Label>(view, "_sessionsTitle").Text == AppStrings.UpcomingSessions && followGrid.Rows.Count == 6, "Language switch lost agenda.");
            languageSelector.SelectedIndex = language == UiLanguage.German ? 0 : 1; WaitForReload(shell);
            Assert(files.All(pair => pair.Value.SequenceEqual(File.ReadAllBytes(pair.Key)))
                && files.Keys.Order().SequenceEqual(Directory.GetFiles(root).Where(path => !path.EndsWith(".png")).Order()), "Read/navigation/localization rewrote a store or created a cache.");

            // Actual workspace command modifies one child; returning must use fresh data.
            ActivateAgendaRow(followGrid, 0); PumpUntil(() => sessionView.SelectedFollowUp?.Id == followUps[0].Id, "follow-up before completion");
            Field<Button>(sessionView, "_statusButton").PerformClick();
            PumpUntil(() => sessionView.SelectedFollowUp?.Status == SessionFollowUpStatus.Done, "follow-up workspace completion");
            ReturnToAgenda(shell);
            Assert(view.Agenda.FollowUps.Count == 8 && view.Agenda.FollowUps.All(item => item.FollowUpId != followUps[0].Id), "Return reused stale follow-ups.");
            WaitForReload(shell); Assert(view.Agenda.FollowUps.Count == 8, "Explicit refresh restored completed follow-up.");
            shell.Close();
            CheckAgendaRacesAndErrors(root);
            Console.WriteLine("Dashboard agenda preview/navigation/return/localization/no-write/race checks passed: " + language);
        }
        finally { AppLanguage.SetLanguage(language); Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }

    private static void ActivateAgendaRow(DataGridView grid, int index, bool mouse = false)
    {
        grid.CurrentCell = grid.Rows[index].Cells[1]; grid.Focus();
        if (mouse) typeof(DataGridView).GetMethod("OnCellClick", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(grid, new object[] { new DataGridViewCellEventArgs(1, index) });
        else typeof(Control).GetMethod("OnKeyDown", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(grid, new object[] { new KeyEventArgs(Keys.Enter) });
    }
    private static void ReturnToAgenda(MainForm shell)
    {
        Field<NavigationButton>(Field<NavigationControl>(shell, "_navigation"), "_dashboardButton").PerformClick();
        WaitForReload(shell);
    }
    private static void OpenFullAgenda(Button button, int expectedCount, Guid sessionId, Guid? followUpId, string root)
    {
        Exception? failure = null; bool handled = false;
        using var timer = new System.Windows.Forms.Timer { Interval = 30 };
        // Test harness only: detects the modal dialog. Production navigation has no timer.
        timer.Tick += (_, _) =>
        {
            var dialog = System.Windows.Forms.Application.OpenForms.OfType<AgendaListForm>().SingleOrDefault();
            if (dialog is null || handled) return;
            handled = true; timer.Stop();
            try
            {
                var grid = Descendants(dialog).OfType<DataGridView>().Single(item => item.Visible);
                Assert(grid.Rows.Count == expectedCount, "Full agenda still has preview limit.");
                Capture(dialog, Path.Combine(root, followUpId.HasValue ? "agenda-all-followups.png" : "agenda-all-sessions.png"));
                ActivateAgendaRow(grid, expectedCount - 1);
                Assert(dialog.Target?.SessionId == sessionId && dialog.Target?.FollowUpId == followUpId, "Full agenda returned wrong identity.");
            }
            catch (Exception ex) { failure = ex; dialog.Close(); }
        };
        timer.Start(); button.PerformClick();
        Assert(handled, "Full agenda did not open."); if (failure is not null) throw failure;
    }

    private static void CheckAgendaRacesAndErrors(string root)
    {
        var now = new DateTimeOffset(2026, 10, 5, 12, 0, 0, TimeSpan.FromHours(2));
        var session = Session.Create(now.AddDays(1), "CODEX TEST – stale", SessionType.DoctorVisit, SessionStatus.Planned);
        var oldSnapshot = new SessionNotebook(new[] { session }, Array.Empty<SessionQuestion>(), Array.Empty<SessionFollowUp>());
        var empty = new SessionNotebook(Array.Empty<Session>(), Array.Empty<SessionQuestion>(), Array.Empty<SessionFollowUp>());
        var gate = new TaskCompletionSource<SessionNotebook>(TaskCreationOptions.RunContinuationsAsynchronously);
        var repository = new AgendaSnapshots(gate.Task, Task.FromResult(empty));
        var service = new SessionService(repository, new JsonHealthTopicRepository());
        using var host = new Form(); using var view = new DashboardAgendaView(); host.Controls.Add(view); ShowOffScreen(host);
        var presenter = new DashboardAgendaPresenter(service, view);
        var older = presenter.LoadAsync(now); var newer = presenter.LoadAsync(now);
        PumpUntil(() => newer.IsCompleted, "new agenda response"); newer.GetAwaiter().GetResult();
        gate.SetResult(oldSnapshot); PumpUntil(() => older.IsCompleted, "old agenda response"); older.GetAwaiter().GetResult();
        Assert(view.Agenda.Sessions.Count == 0 && Field<Label>(view, "_sessionsEmpty").Text == AppStrings.UpcomingSessionsEmpty, "Stale agenda overwrote new empty snapshot.");

        var failedGate = new TaskCompletionSource<SessionNotebook>(TaskCreationOptions.RunContinuationsAsynchronously);
        repository.Responses.Enqueue(failedGate.Task); repository.Responses.Enqueue(Task.FromResult(empty));
        older = presenter.LoadAsync(now); newer = presenter.LoadAsync(now);
        PumpUntil(() => newer.IsCompleted, "newer success before stale error"); newer.GetAwaiter().GetResult();
        failedGate.SetException(new IOException("Synthetic failure")); PumpUntil(() => older.IsCompleted, "stale failure"); older.GetAwaiter().GetResult();
        Assert(Field<Label>(view, "_sessionsEmpty").Text == AppStrings.UpcomingSessionsEmpty, "Stale error replaced newer data.");

        repository.Responses.Enqueue(Task.FromException<SessionNotebook>(new IOException("Synthetic failure")));
        var failed = presenter.LoadAsync(now); PumpUntil(() => failed.IsCompleted, "current agenda error");
        Assert(failed.Exception?.InnerException is IOException && Field<Label>(view, "_sessionsEmpty").Text == AppStrings.AgendaFailed
            && !Field<DataGridView>(view, "_sessionsGrid").Visible, "Load failure became empty data.");
        Capture(host, Path.Combine(root, "agenda-error.png"));
        var leaveGate = new TaskCompletionSource<SessionNotebook>(TaskCreationOptions.RunContinuationsAsynchronously);
        repository.Responses.Enqueue(leaveGate.Task); older = presenter.LoadAsync(now); presenter.Invalidate();
        leaveGate.SetResult(oldSnapshot); PumpUntil(() => older.IsCompleted, "invalidated navigation load"); older.GetAwaiter().GetResult();
        Assert(view.Agenda.Sessions.Count == 0, "Navigation-away invalidation published stale data.");
        host.Close();

        // Two overlapping exact-child navigations: the first detail read finishes last.
        var a = SessionFollowUp.Create(session.Id, "CODEX TEST – Child A");
        var b = SessionFollowUp.Create(session.Id, "CODEX TEST – Child B");
        var full = new SessionNotebook(new[] { session }, Array.Empty<SessionQuestion>(), new[] { a, b });
        var detailGate = new TaskCompletionSource<SessionNotebook>(TaskCreationOptions.RunContinuationsAsynchronously);
        var childRepository = new AgendaSnapshots(Task.FromResult(full), detailGate.Task, Task.FromResult(full), Task.FromResult(full));
        using var childHost = new Form(); using var sessionView = new SessionsView(); childHost.Controls.Add(sessionView); ShowOffScreen(childHost);
        var sessionPresenter = new SessionsPresenter(new SessionService(childRepository, new JsonHealthTopicRepository()), sessionView);
        var first = sessionPresenter.NavigateAsync(session.Id, a.Id);
        PumpUntil(() => childRepository.Responses.Count == 2, "first child detail entered");
        var second = sessionPresenter.NavigateAsync(session.Id, b.Id);
        PumpUntil(() => second.IsCompleted, "latest exact child navigation"); second.GetAwaiter().GetResult();
        detailGate.SetResult(full); PumpUntil(() => first.IsCompleted, "older child detail"); first.GetAwaiter().GetResult();
        Assert(sessionView.SelectedFollowUp?.Id == b.Id, "Older child navigation replaced latest target.");
        childRepository.Responses.Enqueue(Task.FromResult(empty));
        var missing = sessionPresenter.NavigateAsync(session.Id); PumpUntil(() => missing.IsCompleted, "missing target");
        Assert(missing.Exception?.InnerException is LifecycleConflictException, "Missing target silently selected a different session.");
        childRepository.Responses.Enqueue(Task.FromResult(full)); childRepository.Responses.Enqueue(Task.FromResult(full));
        missing = sessionPresenter.NavigateAsync(session.Id, Guid.NewGuid()); PumpUntil(() => missing.IsCompleted, "missing exact child");
        Assert(missing.Exception?.InnerException is LifecycleConflictException && sessionView.SelectedFollowUp is null, "Missing child fell back to a different editable child.");
        childRepository.Responses.Enqueue(Task.FromResult(full));
        childRepository.Responses.Enqueue(Task.FromResult(full with { Sessions = new[] { session with { IsArchived = true } } }));
        missing = sessionPresenter.NavigateAsync(session.Id, a.Id); PumpUntil(() => missing.IsCompleted, "archived between list and details");
        Assert(missing.Exception?.InnerException is LifecycleConflictException && sessionView.SelectedFollowUp is null, "Concurrent parent archive became successful exact navigation.");
        childHost.Close();
    }

    /// <summary>Read-only controlled snapshots; every write fails to catch accidental mutations.</summary>
    private sealed class AgendaSnapshots(params Task<SessionNotebook>[] responses) : ISessionRepository
    {
        internal Queue<Task<SessionNotebook>> Responses { get; } = new(responses);
        public Task<SessionNotebook> LoadAsync(CancellationToken cancellationToken = default) => Responses.Dequeue();
        public Task AddSessionAsync(Session value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddQuestionAsync(SessionQuestion value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddFollowUpAsync(SessionFollowUp value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task SetQuestionAnswerAsync(Guid id, bool answered, string? note, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task SetFollowUpStatusAsync(Guid id, SessionFollowUpStatus status, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task UpdateSessionAsync(Session replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task SetSessionArchivedAsync(Guid id, bool archived, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task UpdateQuestionAsync(SessionQuestion replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteQuestionAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task UpdateFollowUpAsync(SessionFollowUp replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DeleteFollowUpAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
