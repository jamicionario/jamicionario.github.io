
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ScoresProcessor.Helpers;

public class Exporter(ScoresConfig config, ILogger<Exporter> logger)
{
    public IEnumerable<Result> Export(Target[] targets)
    {
        // Generate the JSON job file.
        var conversionInstructions = targets
            // Order by title, to make it easier for the frontend.
            .OrderBy(target => target.ScoreName)
            .Select(target => new
            {
                @in = target.Mscz,
                @out = target.FullDestination
            });
        string json = JsonConvert.SerializeObject(conversionInstructions, Formatting.Indented);
        string jobFileName = Path.GetTempFileName();
        File.WriteAllText(jobFileName, json);

        // Ask MuseScore to do the work in that job file.
        logger.LogInformation("⚙️ Starting export of {Count} scores.", targets.Length);
        Process process = Process.Start(config.MuseScoreExecutablePath, arguments: [
            "-F", // Use factory settings - this avoids that user configs affect this script.
            "-j",
            jobFileName,
        ])
            ?? throw new LaunchException($"Could not start file conversion.");

        // Wait for MuseScore to finish. Around 1 minute...
        process.WaitForExit();
        logger.LogInformation("✅ Exported {Count} scores.", targets.Length);

        // Compile the produced files, and return them.
        foreach (var target in targets)
        {
            Result result = DataFinder.GatherExportsFor(target);
            yield return result;
        }
    }

    private readonly HashSet<string> checkedFolders = [];
    private readonly HashSet<string> previousScoreNamesChecked = [];
    private void EnsureFolderExistsFor(Target target)
    {
        // Note: we must check for this collision _even_ if the folder already exists.
        if (previousScoreNamesChecked.Contains(target.ScoreName))
        {
            logger.LogError(
                "There is already a score named {ScoreName}. Cannot process '{Duplicate file name}'.",
                target.ScoreName,
                target.Mscz
                );
            throw new FileNameException($"There is already a score named '{target.ScoreName}'.");
        }
        previousScoreNamesChecked.Add(target.ScoreName);

        if (checkedFolders.Contains(target.DestinationFolder))
        {
            return;
        }
        Directory.CreateDirectory(target.DestinationFolder);
        checkedFolders.Add(target.DestinationFolder);
    }
}