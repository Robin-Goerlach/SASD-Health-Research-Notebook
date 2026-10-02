using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>Personal action, routine and execution documentation; no medical decisions.</summary>
public sealed class HealthActionService
{
    private readonly IHealthActionRepository _repository;
    private readonly IHealthTopicRepository _topics;
    /// <summary>Uses shared UI-independent persistence contracts.</summary>
    public HealthActionService(IHealthActionRepository repository, IHealthTopicRepository topics)
    { _repository = repository ?? throw new ArgumentNullException(nameof(repository)); _topics = topics ?? throw new ArgumentNullException(nameof(topics)); }
    /// <summary>Creates an action with optional existing topic, including archived topics.</summary>
    public async Task<HealthAction> CreateActionAsync(CreateHealthActionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var action = HealthAction.Create(request.Title, request.ActionType, request.Status, request.HealthTopicId,
            request.Description, request.Origin, request.OriginNote);
        if (action.HealthTopicId.HasValue && !(await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false)).Any(topic => topic.Id == action.HealthTopicId))
            throw new ArgumentException("The selected topic does not exist.");
        await _repository.AddActionAsync(action, cancellationToken).ConfigureAwait(false); return action;
    }
    /// <summary>Lists newest created actions first with stable ID tie-breaking and current topic titles.</summary>
    public async Task<IReadOnlyList<HealthActionSummary>> GetActionsAsync(CancellationToken cancellationToken = default)
    {
        var store = await _repository.LoadAsync(cancellationToken).ConfigureAwait(false);
        var topics = (await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false)).ToDictionary(topic => topic.Id);
        return store.Actions.OrderByDescending(action => action.CreatedAt).ThenBy(action => action.Id)
            .Select(action => new HealthActionSummary(action, action.HealthTopicId.HasValue && topics.TryGetValue(action.HealthTopicId.Value, out var topic) ? topic.Title : null)).ToList();
    }
    /// <summary>Loads children of one action; history uses newest actual instants first.</summary>
    public async Task<HealthActionNotebook> GetActionDetailsAsync(Guid actionId, CancellationToken cancellationToken = default)
    {
        var store = await _repository.LoadAsync(cancellationToken).ConfigureAwait(false);
        var routines = store.Routines.Where(routine => routine.HealthActionId == actionId).OrderBy(routine => routine.CreatedAt).ThenBy(routine => routine.Id).ToList();
        var ids = routines.Select(routine => routine.Id).ToHashSet();
        return new(store.Actions.Where(action => action.Id == actionId).ToList(), routines,
            store.ProgressEntries.Where(entry => ids.Contains(entry.RoutineId)).OrderByDescending(entry => entry.OccurredAt).ThenByDescending(entry => entry.CreatedAt).ThenBy(entry => entry.Id).ToList());
    }
    /// <summary>Adds a separately configured routine to an existing action.</summary>
    public async Task<Routine> CreateRoutineAsync(CreateRoutineRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var routine = Routine.Create(request.HealthActionId, request.Title, request.Description, request.ScheduleText, request.Status);
        if (!(await _repository.LoadAsync(cancellationToken).ConfigureAwait(false)).Actions.Any(action => action.Id == routine.HealthActionId))
            throw new ArgumentException("The selected action does not exist.");
        await _repository.AddRoutineAsync(routine, cancellationToken).ConfigureAwait(false); return routine;
    }
    /// <summary>Records performed/not performed/skipped explicitly, including history for paused routines.</summary>
    public async Task<ProgressEntry> CreateProgressEntryAsync(CreateProgressEntryRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entry = ProgressEntry.Create(request.RoutineId, request.OccurredAt, request.Completion, request.Note);
        if (!(await _repository.LoadAsync(cancellationToken).ConfigureAwait(false)).Routines.Any(routine => routine.Id == entry.RoutineId))
            throw new ArgumentException("The selected routine does not exist.");
        await _repository.AddProgressEntryAsync(entry, cancellationToken).ConfigureAwait(false); return entry;
    }
    /// <summary>Pauses/reactivates only the selected routine; history remains intact.</summary>
    public Task SetRoutineStatusAsync(Guid id, RoutineStatus status, CancellationToken cancellationToken = default) =>
        _repository.SetRoutineStatusAsync(id, status, cancellationToken);
    /// <summary>Counts documentation only, without goals, targets or effectiveness scores.</summary>
    public async Task<HealthActionOverview> GetOverviewAsync(DateOnly localDate, CancellationToken cancellationToken = default)
    {
        var store = await _repository.LoadAsync(cancellationToken).ConfigureAwait(false);
        return new(store.Actions.Count(action => action.Status == HealthActionStatus.Active),
            store.Routines.Count(routine => routine.Status == RoutineStatus.Active),
            store.ProgressEntries.Count(entry => DateOnly.FromDateTime(entry.OccurredAt.LocalDateTime) == localDate));
    }
}
