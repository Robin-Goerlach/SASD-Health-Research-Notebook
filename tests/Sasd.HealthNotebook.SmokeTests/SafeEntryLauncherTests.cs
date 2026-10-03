using System.Diagnostics;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>SEC-ENTRY-002 / SEC-SRC-002 / SEC-MEA-002: actual PowerShell 5.1 file guards in synthetic repositories.</summary>
internal static class SafeEntryLauncherTests
{
    internal static async Task RunAsync()
    {
        string sourceLauncher = Path.Combine(Program.FindRepositoryRoot(), "scripts", "Invoke-SafeDevelopment.ps1");
        foreach (string fileName in new[] { "health-entries.json", "health-entries.json.tmp",
            "health-entries.backup.json", "health-entries.json.lock",
            "sources.json", "sources.json.tmp", "sources.backup.json", "sources.json.lock",
            "measurements.json", "measurements.json.tmp", "measurements.backup.json", "measurements.json.lock",
            "sessions.json", "sessions.json.tmp", "sessions.backup.json", "sessions.json.lock",
        "health-actions.json", "health-actions.json.tmp", "health-actions.backup.json", "health-actions.json.lock" })
        {
            // Keep fixtures near .codex: deeply nested GUID roots can exceed the
            // ordinary path limit of Windows PowerShell 5.1 / .NET Framework.
            string root = Path.Combine(Program.FindRepositoryRoot(), ".codex", "launcher-guards", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "scripts"));
            File.Copy(sourceLauncher, Path.Combine(root, "scripts", "Invoke-SafeDevelopment.ps1"));
            await File.WriteAllTextAsync(Path.Combine(root, "Sasd.HealthNotebook.sln"), "Synthetic launcher guard fixture.");
            string probePath = Path.Combine(root, "Probe.ps1");
            // Junctions need no symlink privilege. Both link and target remain under
            // this fresh synthetic root; nothing is cleaned up or redirected outside it.
            await File.WriteAllTextAsync(probePath, """
                param([string] $Root, [string] $FileName)
                $ErrorActionPreference = 'Stop'
                $dataPath = Join-Path $Root '.codex/synthetic-development-data'
                $targetPath = Join-Path $Root 'synthetic-junction-target'
                [IO.Directory]::CreateDirectory($dataPath) | Out-Null
                [IO.Directory]::CreateDirectory($targetPath) | Out-Null
                New-Item -ItemType Junction -Path (Join-Path $dataPath $FileName) -Target $targetPath | Out-Null
                Set-Location -LiteralPath $Root
                & (Join-Path $Root 'scripts/Invoke-SafeDevelopment.ps1')
                """);
            var start = new ProcessStartInfo("powershell.exe") { WorkingDirectory = root,
                UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (string argument in new[] { "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", probePath, "-Root", root, "-FileName", fileName })
                start.ArgumentList.Add(argument);
            using var process = Process.Start(start) ?? throw new InvalidOperationException("Cannot run launcher guard probe.");
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> error = process.StandardError.ReadToEndAsync();
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try { await process.WaitForExitAsync(timeout.Token); }
            catch (OperationCanceledException)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
                throw new InvalidOperationException("Launcher guard probe timed out.");
            }
            string diagnostic = await output + await error;
            if (process.ExitCode == 0 || !diagnostic.Contains("A development persistence file is a junction or symbolic link.", StringComparison.Ordinal))
                throw new InvalidOperationException("Safe launcher did not reject a persistence link.");
        }
    }
}
