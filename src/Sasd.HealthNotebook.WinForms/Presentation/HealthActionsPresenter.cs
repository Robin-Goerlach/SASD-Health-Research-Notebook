using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>Loads coherent action snapshots and rejects obsolete asynchronous selections.</summary>
public sealed class HealthActionsPresenter(HealthActionService service, HealthActionsView view)
{
    private int _generation;
    private int _loadGeneration;
    private Guid? _selectionActionId;
    private Guid? _routineId;
    private Guid? _progressId;
    /// <summary>Refreshes actions/counts and selected children, preserving identity.</summary>
    public async Task<int> LoadAsync(Guid? preferredId = null)
    {
        int loadGeneration = ++_loadGeneration;
        var actions = await service.GetActionsAsync();
        var overview = await service.GetOverviewAsync(DateOnly.FromDateTime(DateTime.Now));
        if (view.IsDisposed || loadGeneration != _loadGeneration) return actions.Count;
        view.SetActions(actions, preferredId); view.SetOverview(overview); await LoadSelectionAsync(); return actions.Count;
    }
    /// <summary>Clears children immediately; only applies the latest selected-action response.</summary>
    public async Task LoadSelectionAsync(Guid? preferredRoutine = null, Guid? preferredProgress = null)
    {
        int generation = ++_generation; var action = view.SelectedAction;
        // An overlapping refresh may observe the intentionally cleared child grids.
        // Retain the last IDs for the same action, but never carry them to another action.
        if (_selectionActionId != action?.Id) { _routineId = null; _progressId = null; }
        _selectionActionId = action?.Id;
        var selectedRoutine = view.SelectedRoutine;
        bool matchingRoutine = selectedRoutine is not null && selectedRoutine.HealthActionId == action?.Id;
        _routineId = preferredRoutine ?? (matchingRoutine ? (Guid?)selectedRoutine!.Id : null) ?? _routineId;
        _progressId = preferredProgress ?? (matchingRoutine && view.SelectedProgress?.RoutineId == _routineId ? (Guid?)view.SelectedProgress!.Id : null) ?? _progressId;
        view.SetDetails(Array.Empty<Routine>(), Array.Empty<ProgressEntry>());
        if (action is null) return;
        var details = await service.GetActionDetailsAsync(action.Id);
        if (!view.IsDisposed && generation == _generation && view.SelectedAction?.Id == action.Id)
        {
            view.SetDetails(details.Routines, details.ProgressEntries, _routineId, _progressId);
            _routineId = view.SelectedRoutine?.Id; _progressId = view.SelectedProgress?.Id;
        }
    }
}
