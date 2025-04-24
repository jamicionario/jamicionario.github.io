
using System.Diagnostics;
using Newtonsoft.Json;

namespace ScoresProcessor.Helpers;
public class Exporter(ScoresConfig config, DataFinder dataFinder)
{
    public IEnumerable<Result> GatherExportResultsFor(Target[] targets) {
        // Compile the produced files, and return them.
        foreach (var target in targets)
        {
            Result result = dataFinder.GatherExportsFor(target);
            yield return result;
        }
    }

    public void ExportImagesFor(Target[] targets)
    {
        // Ensure folder exists. Otherwise MuseScore fails silently.
        Directory.CreateDirectory(config.TargetFolder);

        // Generate the JSON job file.
        var conversionInstructions = targets
            // Order by title, to make it easier for the frontend.
            .OrderBy(target => target.ScoreName)
            .Select(target => new
            {
                @in = target.Mscz,
                @out = config.GetDestinationFor(target),
            });
        string json = JsonConvert.SerializeObject(conversionInstructions, Formatting.Indented);
        string jobFileName = Path.GetTempFileName();
        File.WriteAllText(jobFileName, json);

        // Ask MuseScore to do the work in that job file.
        string museScoreExecutable = config.MuseScoreExecutablePath ?? "mscore";
        Process process = Process.Start(
            museScoreExecutable,
            arguments: [
                "-F", // Use factory settings - this avoids that user configs affect this script.
                "--job",
                jobFileName,
            ])
            ?? throw new LaunchException($"Could not start file conversion.");

        // Wait for MuseScore to finish. Around 1 minute...
        process.WaitForExit();
    }

    // public void ExportInfoFor(Target[] targets)
    // {
    //     // TODO: export metajson.
    //     // TODO: export custom properties from MSCX ?
    // }
}