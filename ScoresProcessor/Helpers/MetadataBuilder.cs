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

        string filePath = Path.Combine(config.TargetFolder, "score-metadata.json");
        File.WriteAllText(filePath, metadata);
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

        // string SubfolderToPublic(string ab);
        var information = results.Select((item, index) => new
        {
            // We want indexed to 1, not to 0, as it will be user-facing: in the URL.
            Number = index + 1,
            Name = item.ScoreName,
            Mscz = PathAsRelativeToPublic(item.Mscz),
            Pages = item.ScoreImages.Select(PathAsRelativeToPublic),
        });
        return JsonConvert.SerializeObject(information, Formatting.Indented);
    }

    private string GetAbsolutePathToPublic()
    {
        const string publicFolder = "/public/";
        int publicIndex = config.TargetFolder.IndexOf(publicFolder);

        // TODO 1/2: make these two checks when the Config is loaded.
        if (publicIndex <= 0)
        {
            logger.LogError("Cannot generate metadata as the configured TargetFolder is not in .../public/ .");
            throw new ConfigurationException("Error: TargetFolder must be in Jamicionário's /public/ folder.");
        }

        string pathToPublic = config.TargetFolder[..(publicIndex + publicFolder.Length)];

        // TODO 2/2: make these two checks when the Config is loaded.
        if (!config.DataFolder.StartsWith(pathToPublic))
        {
            logger.LogError("Cannot generate metadata as the configured DataFolder is not in .../public/ .");
            throw new ConfigurationException("Error: DataFolder is assumed to be in Jamicionário's /public/ folder.");
        }
        return pathToPublic;
    }
}
