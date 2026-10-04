#Requires -Version 5.1
<#
.SYNOPSIS
Builds/tests or runs a desktop frontend with synthetic repository-local data.
.DESCRIPTION
Sets process-only environment variables and restores them afterwards. Does not
delete data, migrate personal data or change user/system configuration.
#>
param(
    [ValidateSet('Validate', 'WinForms', 'Wpf')]
    [string] $Action = 'Validate'
)

$ErrorActionPreference = 'Stop'
# .NET Framework (Windows PowerShell 5.1) has no TrimEndingDirectorySeparator.
# Normalize both sides alike and retain the separator of drive/UNC roots.
function Get-ComparableDirectoryPath([string] $Path) {
    $fullPath = [IO.Path]::GetFullPath($Path)
    $pathRoot = [IO.Path]::GetPathRoot($fullPath)
    $trimmedPath = $fullPath.TrimEnd([char[]]@('\', '/'))
    if ($trimmedPath.Length -le $pathRoot.Length) {
        return $pathRoot.TrimEnd([char[]]@('\', '/')) + [IO.Path]::DirectorySeparatorChar
    }
    return $trimmedPath
}
$repositoryRoot = Get-ComparableDirectoryPath (Join-Path $PSScriptRoot '..')
$localRoot = Join-Path $repositoryRoot '.codex'
if (-not [StringComparer]::OrdinalIgnoreCase.Equals((Get-ComparableDirectoryPath (Get-Location).Path), $repositoryRoot)) {
    throw 'Run the safe launcher from the repository working directory.'
}
if (-not (Test-Path -LiteralPath (Join-Path $repositoryRoot 'Sasd.HealthNotebook.sln'))) {
    throw 'Cannot identify the repository root.'
}
# A junction could bypass the workspace boundary even when the text of a path is local.
$probe = [IO.DirectoryInfo]::new($localRoot)
while ($null -ne $probe) {
    if ($probe.Exists -and ($probe.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        throw 'Safe development paths must not traverse junctions or symbolic links.'
    }
    $probe = $probe.Parent
}
$settings = @{
    SASD_HEALTHNOTEBOOK_DATA_PATH = (Join-Path $localRoot 'synthetic-development-data')
    DOTNET_CLI_HOME = (Join-Path $localRoot 'dotnet-home')
    NUGET_PACKAGES = (Join-Path $localRoot 'nuget-packages')
    NUGET_HTTP_CACHE_PATH = (Join-Path $localRoot 'nuget-http-cache')
    NUGET_PLUGINS_CACHE_PATH = (Join-Path $localRoot 'nuget-plugins-cache')
    TEMP = (Join-Path $localRoot 'temp')
    TMP = (Join-Path $localRoot 'temp')
    DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
    DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
    DOTNET_ADD_GLOBAL_TOOLS_TO_PATH = 'false'
    DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE = 'true'
    DOTNET_CLI_USE_MSBUILD_SERVER = '0'
    MSBUILDDISABLENODEREUSE = '1'
}
$previous = @{}
try {
    foreach ($name in $settings.Keys) {
        $previous[$name] = [Environment]::GetEnvironmentVariable($name, 'Process')
        if ($name -in @('TEMP', 'TMP', 'DOTNET_CLI_HOME', 'NUGET_PACKAGES', 'NUGET_HTTP_CACHE_PATH', 'NUGET_PLUGINS_CACHE_PATH')) {
            $target = $settings[$name]
            $directory = [IO.DirectoryInfo]::new($target)
            if ($directory.Exists -and ($directory.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
                throw 'A tooling directory is a junction or symbolic link.'
            }
            [IO.Directory]::CreateDirectory($target) | Out-Null
        }
        [Environment]::SetEnvironmentVariable($name, $settings[$name], 'Process')
    }
    $dataDirectory = [IO.DirectoryInfo]::new($settings.SASD_HEALTHNOTEBOOK_DATA_PATH)
    if ($dataDirectory.Exists -and ($dataDirectory.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        throw 'The development data directory is a junction or symbolic link.'
    }
    [IO.Directory]::CreateDirectory($dataDirectory.FullName) | Out-Null
    foreach ($fileName in @('health-topics.json', 'health-topics.json.tmp', 'health-topics.backup.json', 'health-topics.json.lock',
        'health-entries.json', 'health-entries.json.tmp', 'health-entries.backup.json', 'health-entries.json.lock',
        'sources.json', 'sources.json.tmp', 'sources.backup.json', 'sources.json.lock',
        'measurements.json', 'measurements.json.tmp', 'measurements.backup.json', 'measurements.json.lock',
        'sessions.json', 'sessions.json.tmp', 'sessions.backup.json', 'sessions.json.lock',
        'health-actions.json', 'health-actions.json.tmp', 'health-actions.backup.json', 'health-actions.json.lock')) {
        $file = Get-Item -LiteralPath (Join-Path $dataDirectory.FullName $fileName) -Force -ErrorAction SilentlyContinue
        if ($null -ne $file -and ($file.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
            throw 'A development persistence file is a junction or symbolic link.'
        }
    }
    Write-Host "Synthetic data root: $($settings.SASD_HEALTHNOTEBOOK_DATA_PATH)"
    Push-Location -LiteralPath $repositoryRoot
    try {
        if ($Action -eq 'Validate') {
            & dotnet restore Sasd.HealthNotebook.sln
            if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
            & dotnet build Sasd.HealthNotebook.sln --configuration Release --no-restore
            if ($LASTEXITCODE -ne 0) { throw 'Release build failed.' }
            & dotnet run --project tests/Sasd.HealthNotebook.SmokeTests --configuration Release --no-build
            if ($LASTEXITCODE -ne 0) { throw 'Smoke tests failed.' }
            & dotnet run --project tests/Sasd.HealthNotebook.WinForms.SmokeTests --configuration Release --no-build
            if ($LASTEXITCODE -ne 0) { throw 'WinForms smoke tests failed.' }
        } else {
            & dotnet run --project "src/Sasd.HealthNotebook.$Action" --configuration Release --no-build
            if ($LASTEXITCODE -ne 0) { throw 'Desktop application failed.' }
        }
    } finally {
        Pop-Location
    }
} finally {
    foreach ($name in $previous.Keys) {
        [Environment]::SetEnvironmentVariable($name, $previous[$name], 'Process')
    }
}
