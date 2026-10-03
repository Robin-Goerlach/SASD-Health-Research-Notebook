namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>A stale editor cannot overwrite or delete a record changed since it was opened.</summary>
public sealed class LifecycleConflictException : InvalidOperationException
{
    /// <summary>Content-free diagnostic; UI supplies localized guidance.</summary>
    public LifecycleConflictException() : base("The record changed. Reload before trying again.") { }
}
