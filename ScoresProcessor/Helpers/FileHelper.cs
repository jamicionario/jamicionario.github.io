
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;
public static class FileHelper
{
    private static readonly Dictionary<string, string> NecessaryReplacements = new() {
        // "#" in filenames creates problems when serving files in web.
        // URL "/files/image#41.png" will request "/files/image" only! :/
        {"#", "No."} ,
    };
    public static string CleanNameForWeb(string scoreName, ILogger logger)
    {
        string clean = scoreName;
        foreach ((string find, string replace) in NecessaryReplacements)
        {
            clean = clean.Replace(find, replace);
        }

        if (scoreName != clean)
        {
            logger.LogWarning(
                "Score '{scoreName}' contains problematic characters, cleaning to '{clean}'.",
                scoreName,
                clean
                );
        }
        return clean;
    }
}