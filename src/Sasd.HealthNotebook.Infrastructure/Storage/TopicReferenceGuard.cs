namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>Checks new topic links while the referencing store's writer lock is held.</summary>
internal static class TopicReferenceGuard
{
    internal static async Task ValidateAsync(string storePath, Dictionary<Guid, Guid?> before, IEnumerable<(Guid Id, Guid? Topic)> after, CancellationToken cancellationToken)
    {
        var newLinks = after.Where(item => item.Topic.HasValue && (!before.TryGetValue(item.Id, out var old) || old != item.Topic)).Select(item => item.Topic!.Value).Distinct().ToList();
        if (newLinks.Count == 0) return; // Preserve unchanged missing legacy references.
        var topics = await new JsonHealthTopicRepository(Path.Combine(Path.GetDirectoryName(storePath)!, "health-topics.json")).GetAllAsync(cancellationToken).ConfigureAwait(false);
        if (newLinks.Any(id => !topics.Any(topic => topic.Id == id))) throw new ArgumentException("The selected topic no longer exists.");
    }
    internal static void RejectLink(string path)
    { for (string? current = Path.GetFullPath(path); current is not null; current = Path.GetDirectoryName(current)) if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new IOException("Storage must not traverse filesystem links."); }
}
