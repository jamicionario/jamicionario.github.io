using Humanizer;

namespace ScoresProcessor.Helpers;
public class MetadataBuilder(ScoresConfig config)
{
    // TODO: export all this data-configuration to a prominent place.
    //       In its own file, not here in the middle of the code and down by SeemsToHaveValue() etc...
    private static class Categories
    {
        // Keep synchronized with CategoriesOfInterest in file categories.service.ts .
        private static readonly HashSet<string> All = [
            Region,
            TypeOfDance,
        ];

        public const string Region = "Region";
        public const string TypeOfDance = "Type of dance";

        public static bool IsKnown(string category) => All.Contains(category, StringComparer.InvariantCultureIgnoreCase);
    }

    private record class Metadata(string Scores, string Categories);

    /// <summary>
    /// Generates and exports the metadata corresponding to the data in <paramref name="results"/>.
    /// </summary>
    public void ExportMetadataFor(IEnumerable<ExportedTarget> results)
    {
        Metadata metadata = GenerateMetadataFor(results);
        File.WriteAllText(config.MetadataFileName, metadata.Scores);
        File.WriteAllText(config.SearchCategoriesFileName, metadata.Categories);
    }

    public static string? GetTypeOfDanceFor(LabeledTarget target)
    {
        target.Labels.TryGetValue(Categories.TypeOfDance, out string? typeOfDance);
        return typeOfDance;
    }

    private Metadata GenerateMetadataFor(IEnumerable<ExportedTarget> results)
    {
        string[] GetFolderStructureFor(Target item)
        {
            string relativeMsczPath = Path.GetRelativePath(config.MasterDataFolder, item.Mscz);
            string dirName = Path.GetDirectoryName(relativeMsczPath)
                ?? throw new FolderException("Could not parse folder path into separate folder names.");
            string[] folders = dirName
                .Split(
                    [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                    StringSplitOptions.RemoveEmptyEntries
                );
            return folders
                // Ignore the first folder, "Danças portuguesas" / "Non-portuguese dances".
                .Skip(1)
                .ToArray();
        }
        object ProcessInfo(ExportedTarget item, int index)
        {
            string[] folderStructure = GetFolderStructureFor(item);
            string searchableName = NormalizeStringForSearch(item.ScoreName);
            var pages = item.ScoreImages.Select(score => Path.GetRelativePath(config.JamicionarioPublicFolder, score));

            // Get the region and type of dance for this score.
            item.Labels.TryGetValue(Categories.Region, out string? region);
            item.Labels.TryGetValue(Categories.TypeOfDance, out string? typeOfDance);

            var labels = item
                .Labels
                // Exclude the extracted properties: region, etc.
                .Where(kvp => !Categories.IsKnown(kvp.Key))
                .ToDictionary();

            return new
            {
                // We want indexed to 1, not to 0, as it will be user-facing: in the URL.
                number = index + 1,
                name = item.ScoreName,
                searchableName,

                pages,
                region,
                typeOfDance,
                labels,

                folderStructure,
            };
        }

        var scoresMetadata = results
                // Order scores alphabetically.
                .OrderBy(result => result.Mscz)
                .Select(ProcessInfo)
                .ToArray();
        var categoriesMetadata = results
            .SelectMany(item => item.Labels)
            .Where(item => Categories.IsKnown(item.Key))
            .GroupBy(item => item.Key, item => item.Value, StringComparer.InvariantCultureIgnoreCase)
            .Select(group => new
            {
                // Each category has a name...
                name = group.Key,
                // ... and a set of values being used.
                values = group
                    // Order category values alphabetically.
                    .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                    .ToArray(),
            });

        string scoresMetadataJson = JsonHelper.Serialize(scoresMetadata);
        string categoriesMetadataJson = JsonHelper.Serialize(categoriesMetadata);
        return new Metadata(scoresMetadataJson, categoriesMetadataJson);
    }

    private static string NormalizeStringForSearch(string scoreName)
    {
        return scoreName.ToLowerInvariant().Trim();
    }

    public static Dictionary<string, string> ProcessLabels(IEnumerable<(string name, string value)> matches)
    {
        return matches
            .Where(SeemsToHaveValue)
            .ToDictionary(
                match => match.name.Humanize(LetterCasing.Sentence),
                match => match.value
                );
    }

    // These labels/metaTags are of no interest to Jamicionário.
    private static readonly HashSet<string> LabelNamesToSkip = [
        "mscVersion",
        "platform",
        // No use in it. Last-update could be interesting, but not this.
        "creationDate",
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
