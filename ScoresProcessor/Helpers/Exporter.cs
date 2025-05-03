
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ScoresProcessor.Helpers;
public class Exporter(ScoresConfig config, DataFinder dataFinder)
{
    public IEnumerable<Result> GatherExportResultsFor(LabeledTarget[] targets)
    {
        // Compile the produced files, and return them.
        foreach (var target in targets)
        {
            string[] scoreImages = dataFinder.FindExportedImagesFor(target);
            yield return new Result(target, target.Labels, scoreImages);
        }
    }

    public void ExportImagesFor(Target[] targets)
    {
        // Ensure folder exists. Otherwise MuseScore fails silently.
        Directory.CreateDirectory(config.TargetFolder);

        ExportFor(targets, config.GetDestinationFor);
    }

    public LabeledTarget[] LoadLabelInfoFor(Target[] targets)
    {
        Stopwatch sw = Stopwatch.StartNew();
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory("jamicionario");

        Dictionary<Target, string> locations = targets.ToDictionary(x => x, target => $"{target.ScoreName}.mscx");
        ExportFor(targets, target => Path.Combine(tempDir.FullName, locations[target]));

        Regex metadataParser = new(@"<metaTag name=""(?<name>[\w\s]+)"">(?<value>[^<]*)</metaTag>", RegexOptions.Compiled);
        Dictionary<string, string> GetLabelsFor(Target target)
        {
            string fileName = locations[target];
            string mscxText = File.ReadAllText(Path.Combine(tempDir.FullName, locations[target]));
            IEnumerable<(string name, string value)> matches = metadataParser
                    .Matches(mscxText)
                    .Select(match => (name: match.Groups["name"].Value, value: match.Groups["value"].Value));
            return MetadataBuilder.ProcessLabels(matches);
        }
        var labeled = targets
            // TODO: humanize the label names.
            .Select(target => new LabeledTarget(target, GetLabelsFor(target)))
            .ToArray();
        return labeled;
    }


    private void ExportFor(Target[] targets, Func<Target, string> getOutFileName)
    {
        // Generate the JSON job file.
        var conversionInstructions = targets
            // Order by title, to make it easier for the frontend.
            .OrderBy(target => target.ScoreName)
            .Select(target => new
            {
                @in = target.Mscz,
                @out = getOutFileName(target),
            });
        string json = JsonConvert.SerializeObject(conversionInstructions, Formatting.Indented);
        string jobFileName = Path.GetTempFileName();
        File.WriteAllText(jobFileName, json);

        // Ask MuseScore to do the work in that job file.
        string museScoreExecutable = config.MuseScoreExecutablePath ?? "mscore";
        Process process = Process.Start(
            museScoreExecutable,
            arguments: [
                // Use factory settings - this avoids that user configs affect this script.
                // Confront with -F, which uses the factory settings AND deletes user preferences.
                "-R",
                "--job",
                jobFileName,
            ])
            ?? throw new LaunchException($"Could not start file conversion.");

        // Wait for MuseScore to finish.
        // Around 1 minute when generating the PNGs, around 7-10s when generating the metadata (MSCX).
        process.WaitForExit();
    }
}