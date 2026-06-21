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

    public ScoresConfig ReadConfig()
    {
        IConfigurationRoot built = new ConfigurationBuilder()
            .AddJsonFile(ConfigFile, optional: true)
            // Settings can be configured with env vars.
            // Example: set "JAMICTIONARY__MuseScoreExecutablePath" to "<...>/MuseScore4".
            // It should be case insensitive, but that has not been tested.
            .AddEnvironmentVariables(prefix: "jamictionary")
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
            config = ScoresConfig.Default;
        }

        var converted = config with
        {
            MuseScoreExecutablePath = UnixSafeOptional(config.MuseScoreExecutablePath),
            JamictionaryWebsiteFolder = UnixSafe(config.JamictionaryWebsiteFolder),
            JamictionaryDataFolder = UnixSafe(config.JamictionaryDataFolder),
        };
        logger.LogTrace("Configuration value read: {config}", converted);
        Validate(converted);
        return converted;
    }

    /// <summary>
    /// Validate that the configuration meets some requirements.
    /// </summary>
    /// <exception cref="ConfigurationException">Thrown if the a setting is not well configured.</exception>
    private void Validate(ScoresConfig config)
    {
        // The way that we are binding/getting the options does not allow us to automatically validate them.
        // Otherwise we would use the regular automatic validation provided by the framework:
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0#options-validation

        if (!Directory.Exists(config.JamictionaryPublicFolder))
        {
            logger.LogError("The configured Jamictionary public folder does not seem to exist.");
            throw new ConfigurationException("The configured JamictionaryPublicFolder does not exist.");
        }

        if (!Directory.Exists(config.JamictionaryDataFolder))
        {
            logger.LogError("The configured master data folder does not seem to exist.");
            throw new ConfigurationException("The configured JamictionaryDataFolder does not exist.");
        }


        if (!config.JamictionaryPublicFolder.Contains("/public/")
            && !config.JamictionaryPublicFolder.EndsWith("/public"))
        {
            logger.LogWarning("The configured Jamictionary public folder does not seem to be a path like .../public . This may cause errors.");
        }
    }

    /// <summary>
    /// Checks if a path is a "~/..." unix-path and converts it to an absolute path if it is.
    /// </summary>
    private string? UnixSafeOptional(string? path)
    {
        if (path == null)
        {
            return path;
        }
        return UnixSafe(path);
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
                logger.LogError("Configuration path starts with ~, but is not '~' or '~/(...)'. Cannot understand it, breaking.");
                throw new ConfigurationException("Configuration path is not understood.");
        }
    }
}
