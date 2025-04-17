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
        // The absolute path to the "public" folder.
        string pathToPublic = GetAbsolutePathToPublic();
        string PathAsRelativeToPublic(string absoluteLocalPath)
        {
            string relative = Path.GetRelativePath(pathToPublic, absoluteLocalPath);
            return relative;
        }

        string[] SelectCategoriesFor(Target item)
        {
            string relativeMsczPath = Path.GetRelativePath(config.DataFolder, item.Mscz);
            string dirName = Path.GetDirectoryName(relativeMsczPath)
                ?? throw new FolderException("Could not parse folder path into separate folder names.");
            string[] folders = dirName
                .Split(
                    [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                    StringSplitOptions.RemoveEmptyEntries
                );
            return folders;
        }
        // string SubfolderToPublic(string ab);
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
                // mscz = PathAsRelativeToPublic(item.Mscz),
                pages = item.ScoreImages.Select(PathAsRelativeToPublic),
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

    private string GetAbsolutePathToPublic()
    {
        const string publicFolder = "/public/";
        int publicIndex = config.TargetFolder.IndexOf(publicFolder);

        // TODO: make this check when the Config is loaded.
        if (publicIndex <= 0)
        {
            logger.LogError("Cannot generate metadata as the configured TargetFolder is not in .../public/ .");
            throw new ConfigurationException("Error: TargetFolder must be in Jamicionário's /public/ folder.");
        }

        string pathToPublic = config.TargetFolder[..(publicIndex + publicFolder.Length)];
        return pathToPublic;
    }
}
