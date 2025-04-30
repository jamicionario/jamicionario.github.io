using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ScoresProcessor.Helpers;
public class MetadataBuilder(ScoresConfig config, ILogger<MetadataBuilder> logger)
{
    /// <summary>
    /// Generates and exports the metadata corresponding to the data in <paramref name="results"/>.
    /// </summary>
    public void ExportMetadataFor(IEnumerable<Result> results)
    {
        string metadata = GenerateMetadataFor(results);
        File.WriteAllText(config.MetadataFileName, metadata);
    }

    private string GenerateMetadataFor(IEnumerable<Result> results)
    {
        string[] SelectCategoriesFor(Target item)
        {
            string relativeMsczPath = Path.GetRelativePath(config.MasterDataFolder, item.Mscz);
            string dirName = Path.GetDirectoryName(relativeMsczPath)
                ?? throw new FolderException("Could not parse folder path into separate folder names.");
            string[] folders = dirName
                .Split(
                    [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                    StringSplitOptions.RemoveEmptyEntries
                );
            return folders;
        }
        object ProcessInfo(Result item, int index)
        {
            string[] categories = SelectCategoriesFor(item);
            bool isPortuguese = categories[0].Contains("Danças portuguesas", StringComparison.InvariantCultureIgnoreCase);
            string category = categories[1];
            string[] subcategories = categories.Skip(2).ToArray();
            string searchableName = NormalizeStringForSearch(item.ScoreName);
            var pages = item.ScoreImages.Select(score => Path.GetRelativePath(config.JamicionarioPublicFolder, score));
            return new
            {
                // We want indexed to 1, not to 0, as it will be user-facing: in the URL.
                number = index + 1,
                name = item.ScoreName,
                searchableName,
                pages,
                isPortuguese,
                category,
                subcategories,
            };
        }
        var information = results
            // Order alphabetically.
            .OrderBy(result => result.Mscz)
            .Select(ProcessInfo);
        return JsonConvert.SerializeObject(information, Formatting.Indented);
    }

    private static string NormalizeStringForSearch(string scoreName)
    {
        return scoreName.ToLowerInvariant().Trim();
    }

    /// <summary>
    /// Exports the label information for the scores.
    /// </summary>
    public void ExportLabels(LabeledTarget[] labelInfo)
    {
        var information = labelInfo
            // Order alphabetically.
            .OrderBy(info => info.ScoreName)
            .Select(info => new {
                scoreName = info.ScoreName,
                labels = info.Labels,
            });
        string json = JsonConvert.SerializeObject(information, Formatting.Indented);
        File.WriteAllText(config.LabelsFileName, json);
    }

    public static Dictionary<string, string> ProcessLabels(IEnumerable<(string name, string value)> matches)
    {
        return matches
            .Where(SeemsToHaveValue)
            .ToDictionary(match => match.name, match => match.value);
    }

    // These labels/metaTags are of no interest to Jamicionário.
    private static readonly HashSet<string> LabelNamesToSkip = [
        "mscVersion",
        "platform",
    ];
    // These are found values that are in effect empty.
    private static readonly HashSet<string> PseudoEmptyValues = [
        ".....",
        "Template",
    ];
    // These are particular "placeholder" values that, while not being "foo: Foo", are equivalent to it.
    private static readonly Dictionary<string, string> KnownEmptyValues = new() {
        {"workTitle", "Title"},
    };
    private static bool SeemsToHaveValue((string name, string value) match)
    {
        // Ignore some labels such as "platform: Linux".
        if (LabelNamesToSkip.Contains(match.name))
        {
            return false;
        }
        // Filter empty.
        if (string.IsNullOrWhiteSpace(match.value))
        {
            return false;
        }
        if (PseudoEmptyValues.Contains(match.value))
        {
            return false;
        }
        // Filter pseudo-empty such as "composer: Composer".
        if (string.Equals(match.name, match.value, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }
        // Filter known pseudo-empty such as "workTitle: Title".
        if (KnownEmptyValues.TryGetValue(match.name, out string? toSkip)
            && toSkip.Equals(match.value, StringComparison.InvariantCultureIgnoreCase)
            )
        {
            return false;
        }
        return true;
    }
}
