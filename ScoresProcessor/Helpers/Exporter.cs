
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;

public class Exporter(ScoresConfig config, ILogger<Exporter> logger)
{
    public IEnumerable<Result> Export(Target[] targets)
    {
        // TODO: convert in batch via JSON file.

        foreach (var target in targets)
        {
            // Thanks MuseScore. >_> I would expect at least an error like "could not create file"!
            EnsureFolderExistsFor(target);

            // mscore -o camposa.png camposa.mscz
            ProcessStartInfo startInfo = new(config.MuseScoreExecutablePath, [
                "-F", // Use factory settings - this avoids that user configs affect this script.
                "-o",
                target.FullDestination,
                target.Mscz,
            ])
            {
                // WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process process = Process.Start(startInfo)
                ?? throw new LaunchException($"Could not start file conversion for {target.Mscz}");

            bool completed = process.WaitForExit(config.ConversionTimeout);
            if (!completed)
            {
                logger.LogWarning("Conversion timeout reached, file might not have been converted: {Mscz}", target.Mscz);
                continue;
            }

            Result result = DataFinder.GatherExportsFor(target);
            logger.LogTrace(
                "Exported successfully the score {ScoreName}. {PNG Count} exported PNGs found.",
                result.ScoreName,
                result.ScoreImages.Length
                );
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