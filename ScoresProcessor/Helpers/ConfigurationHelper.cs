using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;
public class ConfigurationHelper
{
    const string ConfigFile = "config.json";
    const string SampleFile = "config.sample.json";
    private static ILogger Logger { get; } = ScoresProcessor.LogFactory.CreateLogger<ScoresProcessor>();

    // TODO: validate.
    // See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0#options-validation
    public static ScoresConfig ReadConfig()
    {
        IConfigurationRoot built = new ConfigurationBuilder()
            .AddJsonFile(ConfigFile, optional: true)
            // Settings can be configured with env vars.
            // Example: set "JAMICIONARIO__MuseScoreExecutablePath" to "<...>/MuseScore4".
            // It should be case insensitive, but I haven't tested it.
            .AddEnvironmentVariables(prefix: "jamicionario")
            .Build();
        ScoresConfig? config = built.Get<ScoresConfig>();
        if (config != null)
        {
            Logger.LogDebug("Found configuration, overriding values.");
        }
        else
        {
            Logger.LogInformation("No configuration found, using defaults."
                + $" To configure the application, copy the file {SampleFile} to {ConfigFile}, and edit it."
                );
            config = new()
            {
                JamicionarioPublicFolder = "~/code/jamicionario/public",
                MasterDataFolder = "~/code/jamicionario/public/data",
            };
        }

        return config with
        {
            JamicionarioPublicFolder = UnixSafe(config.JamicionarioPublicFolder),
            MasterDataFolder = UnixSafe(config.MasterDataFolder),
        };
    }

    /// <summary>
    /// Checks if a path is a "~/..." unix-path and converts it to an absolute path if it is.
    /// </summary>
    private static string UnixSafe(string path)
    {
        if (!path.StartsWith('~'))
        {
            return path;
        }
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            // Substring from the second character to the end.
            path[1..]
            );
    }
}
