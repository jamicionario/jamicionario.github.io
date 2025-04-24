using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;
public class ConfigurationReader(ILogger logger)
{
    const string ConfigFile = "config.json";
    const string SampleFile = "config.sample.json";

    public static ScoresConfig ReadConfig(ILogger logger)
    {
        ConfigurationReader helper = new(logger);
        return helper.ReadConfig();
    }

    // TODO: validate.
    // See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0#options-validation
    public ScoresConfig ReadConfig()
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
            logger.LogDebug("Found configuration, overriding values.");
        }
        else
        {
            logger.LogInformation("No configuration found, using defaults."
                + $" To configure the application, copy the file {SampleFile} to {ConfigFile}, and edit it."
                );
            config = new()
            {
                JamicionarioPublicFolder = "~/code/jamicionario/public",
                MasterDataFolder = "~/Dropbox/Jamicionario Tripeiro",
            };
        }

        var converted = config with
        {
            JamicionarioPublicFolder = UnixSafe(config.JamicionarioPublicFolder),
            MasterDataFolder = UnixSafe(config.MasterDataFolder),
        };
        logger.LogTrace("Configuration value read: {config}", converted);
        return converted;
    }

    /// <summary>
    /// Checks if a path is a "~/..." unix-path and converts it to an absolute path if it is.
    /// </summary>
    private string UnixSafe(string path)
    {
        if (!path.StartsWith('~'))
        {
            return path;
        }
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        switch (path)
        {
            case "~":
                return userFolder;
            case not null when path.StartsWith("~/"):
                return Path.Combine(
                    userFolder,
                    // Substring from the third character to the end: remove "~/" from the start.
                    path[2..]
                    );
            default:
                logger.LogError("Configuration path starts with ~, but is not '~' or '~/...'. Cannot understand it, breaking.");
                throw new ConfigurationException("Configuration path is not understood.");
        }
    }
}
