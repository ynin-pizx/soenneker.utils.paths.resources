using Soenneker.Extensions.String;
using Soenneker.Utils.Runtime;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace Soenneker.Utils.Paths.Resources;

/// <summary>
/// A utility library for retrieving the Resources path across environments
/// </summary>
public static class ResourcesPathUtil
{
    /// <summary>
    /// Absolute path to the "Resources" folder across environments.
    /// Order:
    /// - Azure Functions/App Service (any): HOME/site/wwwroot/Resources
    /// - GitHub Actions: GITHUB_WORKSPACE/Resources
    /// - If HOME is set (containers/custom): HOME/site/wwwroot/Resources
    /// - Fallback: AppContext.BaseDirectory/Resources
    /// </summary>
    [Pure]
    public static string Get()
    {
        // Azure Functions/App Service → canonical mount
        if (RuntimeUtil.IsAzureFunction || RuntimeUtil.IsAzureAppService)
        {
            string? home = Environment.GetEnvironmentVariable("HOME");

            if (home.HasContent())
                return Path.Combine(home, "site", "wwwroot", "Resources");
        }

        if (RuntimeUtil.IsGitHubAction)
        {
            string? gha = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");

            if (gha.HasContent())
                return Path.Combine(gha, "Resources");
        }

        // Containers / custom setups where HOME exists
        string? homeEnv = Environment.GetEnvironmentVariable("HOME");

        if (homeEnv.HasContent())
            return Path.Combine(homeEnv, "site", "wwwroot", "Resources");

        // Local dev / generic fallback
        return Path.Combine(AppContext.BaseDirectory, "Resources");
    }

    /// <summary>
    /// Absolute path to a file under /Resources.
    /// </summary>
    [Pure]
    public static string GetResourceFilePath(string fileName) =>
        Path.Combine(Get(), fileName);
}