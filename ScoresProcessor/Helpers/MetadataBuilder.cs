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
            var categories = SelectCategoriesFor(item);
            bool isPortuguese = categories[0].Contains("Danças portuguesas", StringComparison.InvariantCultureIgnoreCase);
            string category = categories[1];
            string[] subcategories = categories.Skip(2).ToArray();
            return new
            {
                // We want indexed to 1, not to 0, as it will be user-facing: in the URL.
                number = index + 1,
                name = item.ScoreName,
                searchableName = NormalizeStringForSearch(item.ScoreName),
                pages = item.ScoreImages.Select(score => Path.GetRelativePath(config.JamicionarioPublicFolder, score)),
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

}
