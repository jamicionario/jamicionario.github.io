
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;

public static class FileHelper
{
    private static readonly OrderedDictionary<string, string> NecessaryReplacements = new() {
        // This is just organizational and internal to the "Data Team" 's work.
        { " (Jamicionario)", "" },
        { "(Jamicionario)", "" },
        // "#" in filenames creates problems when serving files in web.
        // URL "/files/image#41.png" will request "/files/image" only! :/
        { "#", "No." },
        // Parenthesis in filenames create problems when serving files in web.
        // They are a special character, like ? and # and / and etc.
        // See e.g. https://webmasters.stackexchange.com/a/78114
        { " (", " " },
        { "(", " " },
        { ")", "" },
    };
    public static string CleanNameForWeb(string scoreName, ILogger logger)
    {
        string clean = scoreName;
        foreach ((string find, string replace) in NecessaryReplacements)
        {
            clean = clean.Replace(find, replace);
        }
        clean = clean.Trim();

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