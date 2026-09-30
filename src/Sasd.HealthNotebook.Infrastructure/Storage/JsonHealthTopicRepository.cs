using System.Text.Json;
using System.Text.Json.Serialization;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>
/// JSON-based repository for health topics.
///
/// This repository is intentionally simple and transparent for Milestone 1.
/// It writes all topics to one local JSON file and creates a backup copy before
/// overwriting an existing file. This reduces the risk of silent data loss while
/// keeping the implementation easy to inspect.
///
/// Important privacy rule: this class must not log health data. Exceptions should
/// remain technical and should not include user-entered medical content.
/// </summary>
public sealed class JsonHealthTopicRepository : IHealthTopicRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonHealthTopicRepository" /> class.
    /// </summary>
    /// <param name="filePath">
    /// Optional JSON file path. When omitted, the shared configured data path is used.
    /// Tests can pass an isolated file path to avoid touching real user data.
    /// </param>
    public JsonHealthTopicRepository(string? filePath = null)
    {
        _filePath = string.IsNullOrWhiteSpace(filePath)
            ? LocalHealthNotebookPaths.HealthTopicsFilePath
            : filePath;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            return await LoadInternalAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(topic);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            List<HealthTopic> topics = await LoadInternalAsync(cancellationToken).ConfigureAwait(false);

            if (topics.Any(existing => existing.Id == topic.Id))
            {
                throw new InvalidOperationException("A health topic with the same identifier already exists.");
            }

            topics.Add(topic);
            await SaveInternalAsync(topics, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(topics);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await SaveInternalAsync(topics, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<HealthTopic>> LoadInternalAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return new List<HealthTopic>();
        }

        await using FileStream stream = File.OpenRead(_filePath);

        List<HealthTopic>? topics = await JsonSerializer
            .DeserializeAsync<List<HealthTopic>>(stream, SerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        return topics ?? new List<HealthTopic>();
    }

    private async Task SaveInternalAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken)
    {
        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string tempFilePath = _filePath + ".tmp";
        string backupFilePath = Path.Combine(
            Path.GetDirectoryName(_filePath) ?? string.Empty,
            Path.GetFileNameWithoutExtension(_filePath) + ".backup" + Path.GetExtension(_filePath));

        await using (FileStream stream = File.Create(tempFilePath))
        {
            await JsonSerializer.SerializeAsync(stream, topics, SerializerOptions, cancellationToken).ConfigureAwait(false);
        }

        // Keep a small last-good backup before replacing the active JSON file.
        // This is not a complete backup strategy, but it is a useful safety net for Milestone 1.
        if (File.Exists(_filePath))
        {
            File.Copy(_filePath, backupFilePath, overwrite: true);
        }

        File.Move(tempFilePath, _filePath, overwrite: true);
    }
}
