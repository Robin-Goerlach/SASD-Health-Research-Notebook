using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>
/// Very small smoke-test runner without external NuGet dependencies.
///
/// This is not meant to replace xUnit/NUnit later. It only proves that the core
/// JSON workflow works in a clean environment while keeping Milestone 1 dependency-free.
/// </summary>
internal static class Program
{
    private static async Task<int> Main()
    {
        try
        {
            await RunJsonRoundTripSmokeTestAsync();
            Console.WriteLine("Smoke tests passed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Smoke tests failed.");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static async Task RunJsonRoundTripSmokeTestAsync()
    {
        string testDirectory = Path.Combine(Path.GetTempPath(), "SASD-HealthNotebook-SmokeTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testDirectory);

        string jsonFilePath = Path.Combine(testDirectory, "health-topics.json");

        var repository = new JsonHealthTopicRepository(jsonFilePath);
        var service = new HealthTopicService(repository);

        await service.CreateTopicAsync(new CreateHealthTopicRequest
        {
            Title = "Smoke test topic",
            Status = HealthTopicStatus.Observation,
            Priority = HealthTopicPriority.PrepareForDoctor,
            ShortDescription = "Only a technical smoke-test entry.",
            Notes = "This is not real health data."
        });

        var reloadedRepository = new JsonHealthTopicRepository(jsonFilePath);
        var reloadedService = new HealthTopicService(reloadedRepository);

        var topics = await reloadedService.GetTopicSummariesAsync();
        var overview = await reloadedService.GetDashboardOverviewAsync();

        Assert(topics.Count == 1, "Expected exactly one topic after reload.");
        Assert(topics[0].Title == "Smoke test topic", "Expected the topic title to survive reload.");
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
