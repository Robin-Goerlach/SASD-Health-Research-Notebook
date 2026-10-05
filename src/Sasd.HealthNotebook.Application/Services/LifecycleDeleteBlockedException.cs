namespace Sasd.HealthNotebook.Application.Services;

/// <summary>A reference or recorded answer prevents hard deletion. Contains no user data.</summary>
public sealed class LifecycleDeleteBlockedException : InvalidOperationException
{
    /// <summary>Creates content-free lifecycle feedback.</summary>
    public LifecycleDeleteBlockedException() : base("Deletion is blocked by retained documentation.") { }
}
