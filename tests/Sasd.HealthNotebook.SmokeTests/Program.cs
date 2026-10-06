using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>
/// Very small smoke-test runner without external NuGet dependencies.
///
/// This is not meant to replace xUnit/NUnit later. It only proves that the core
/// JSON workflow works in a clean environment while keeping the early milestone
/// dependency-free.
/// </summary>
internal static class Program
{
    private static async Task<int> Main()
    {
        string? originalOverride = Environment.GetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable);
        try
        {
            string repositoryRoot = FindRepositoryRoot();
            string testRoot = originalOverride is null
                ? Path.Combine(repositoryRoot, ".codex", "smoke-tests")
                : originalOverride;
            ValidateTestRoot(repositoryRoot, testRoot);
            RunPathResolutionTests(testRoot);
            string testDirectory = Path.Combine(testRoot, Guid.NewGuid().ToString("N"));
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testDirectory);
            Console.WriteLine($"Synthetic test data directory: {LocalHealthNotebookPaths.DataDirectory}");
            await RunSharedJsonPersistenceSmokeTestAsync();
            await RunSharedRepositoryContractTestAsync();
            await HealthEntryTests.RunAsync();
            await SourceTests.RunAsync();
            await SourceLocationReloadTests.RunAsync();
            await MeasurementTests.RunAsync();
            await SessionTests.RunAsync();
            await HealthActionTests.RunAsync();
            await LifecycleTests.RunAsync();
            await ParentLifecycleTests.RunAsync();
            await DashboardAgendaTests.RunAsync();
            await TimelineTests.RunAsync();
            await SafeEntryLauncherTests.RunAsync();
            Console.WriteLine("Smoke tests passed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Smoke tests failed.");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        finally
        {
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, originalOverride);
        }
    }

    // FR-DEV-001 / SEC-PATH-001: resolving the product default must never read or write it.
    private static void RunPathResolutionTests(string testRoot)
    {
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, null);
        string expectedDefault = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SASD-GmbH", "HealthResearchNotebook", "data");
        Assert(LocalHealthNotebookPaths.DataDirectory == expectedDefault, "Product default changed.");
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testRoot);
        Assert(LocalHealthNotebookPaths.DataDirectory == Path.GetFullPath(testRoot), "Override was not used.");
        Assert(LocalHealthNotebookPaths.HealthTopicsFilePath == Path.Combine(Path.GetFullPath(testRoot), "health-topics.json"),
            "JSON path does not use the override.");
        string normalizedPath = Path.Combine(testRoot, "child", "..", "normalized");
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, normalizedPath);
        Assert(LocalHealthNotebookPaths.DataDirectory == Path.GetFullPath(normalizedPath), "Path was not normalized.");
        foreach (string absolutePath in new[] { Path.GetPathRoot(testRoot)!, @"\\synthetic-server\synthetic-share\data" })
        {
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, absolutePath);
            Assert(LocalHealthNotebookPaths.DataDirectory == Path.GetFullPath(absolutePath), "Absolute product path rejected.");
        }
        foreach (string invalidPath in new[] { " ", "relative-data", "C:relative-data", @"\root-relative",
            Path.Combine(testRoot, "invalid*directory"), Path.Combine(testRoot, "NUL"),
            Path.Combine(testRoot, "trailing."), Path.Combine(testRoot, "file:stream"), @"\\.\C:\data",
            @"\\?\C:\data", "//?/C:/data", @"\\synthetic-server" })
        {
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, invalidPath);
            bool rejected = false;
            try { _ = LocalHealthNotebookPaths.DataDirectory; }
            catch (InvalidOperationException) { rejected = true; }
            Assert(rejected, $"Invalid synthetic override was not rejected: {invalidPath}");
        }
        bool outsideRejected = false;
        try { ValidateTestRoot(FindRepositoryRoot(), Path.GetPathRoot(testRoot)!); }
        catch (InvalidOperationException) { outsideRejected = true; }
        Assert(outsideRejected, "Test runner must reject data outside the repository.");
    }

    internal static string FindRepositoryRoot()
    {
        for (DirectoryInfo? directory = new(Environment.CurrentDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Sasd.HealthNotebook.sln")))
            {
                return directory.FullName;
            }
        }
        throw new InvalidOperationException("Run smoke tests from within the repository.");
    }

    private static void ValidateTestRoot(string repositoryRoot, string testRoot)
    {
        Assert(Path.IsPathFullyQualified(testRoot), "Test data root must be fully qualified.");
        string fullPath = Path.GetFullPath(testRoot);
        Assert(fullPath.StartsWith(repositoryRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase),
            "Test data root must be inside the repository.");
        // Reject junctions/symlinks before creating anything; lexical containment alone is insufficient.
        for (DirectoryInfo? directory = new(fullPath); directory is not null; directory = directory.Parent)
        {
            Assert(!directory.Exists || (directory.Attributes & FileAttributes.ReparsePoint) == 0,
                "Test data path must not traverse a junction or symbolic link.");
        }
    }

    // FR-DEV-001 / IT-PATH-001: shared contracts, independent of desktop assemblies.
    private static async Task RunSharedRepositoryContractTestAsync()
    {
        var firstService = new HealthTopicService(new JsonHealthTopicRepository());
        var secondService = new HealthTopicService(new JsonHealthTopicRepository());
        Assert((await firstService.GetTopicSummariesAsync()).Count == 1, "Service did not use isolated JSON.");
        await secondService.CreateTopicAsync(new CreateHealthTopicRequest
        {
            Title = "Synthetic composition-test topic",
            Status = HealthTopicStatus.Observation,
            Priority = HealthTopicPriority.PrepareForDoctor
        });
        Assert((await firstService.GetTopicSummariesAsync()).Count == 2, "Services do not share isolated JSON.");
        Assert(File.Exists(LocalHealthNotebookPaths.HealthTopicsFilePath), "Expected isolated JSON file.");
        Assert(File.Exists(Path.Combine(LocalHealthNotebookPaths.DataDirectory, "health-topics.backup.json")),
            "Expected backup alongside isolated JSON.");
        string isolatedDirectory = LocalHealthNotebookPaths.DataDirectory;
        try
        {
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable,
                LocalHealthNotebookPaths.HealthTopicsFilePath);
            bool rejected = false;
            try { _ = LocalHealthNotebookPaths.DataDirectory; }
            catch (InvalidOperationException) { rejected = true; }
            Assert(rejected, "An existing file must not be accepted as a data directory.");
        }
        finally
        {
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, isolatedDirectory);
        }
    }

    private static async Task RunSharedJsonPersistenceSmokeTestAsync()
    {
        // The WinForms and WPF frontends both use the same HealthTopicService and
        // JsonHealthTopicRepository composition. This smoke test verifies the
        // shared persistence contract below the UI layer without opening windows.
        var firstRepository = new JsonHealthTopicRepository();
        var firstFrontendService = new HealthTopicService(firstRepository);

        await firstFrontendService.CreateTopicAsync(new CreateHealthTopicRequest
        {
            Title = "Synthetic smoke-test topic",
            Status = HealthTopicStatus.Observation,
            Priority = HealthTopicPriority.PrepareForDoctor,
            ShortDescription = "Only a technical smoke-test entry.",
            Notes = "This is synthetic data and not a real health record."
        });

        var secondRepository = new JsonHealthTopicRepository();
        var secondFrontendService = new HealthTopicService(secondRepository);

        var topics = await secondFrontendService.GetTopicSummariesAsync();
        var overview = await secondFrontendService.GetDashboardOverviewAsync();

        Assert(topics.Count == 1, "Expected exactly one topic after reload.");
        Assert(topics[0].Title == "Synthetic smoke-test topic", "Expected the topic title to survive reload.");
        Assert(overview.TotalTopics == 1, "Expected dashboard total to be 1.");
        Assert(overview.PrepareForDoctorCount == 1, "Expected prepare-for-doctor count to be 1.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
