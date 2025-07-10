
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScoresProcessor.Helpers;
public class Exporter(ScoresConfig config, DataFinder dataFinder)
{
    public IEnumerable<ExportedResult> GatherExportResultsFor(TargetWithLabels[] targets)
    {
        // Compile the produced files, and return them.
        foreach (var target in targets)
        {
            string[] scoreImages = dataFinder.FindExportedImagesFor(target);
            string? pdfFile = dataFinder.FindExportedPdfFor(target);
            yield return new ExportedResult(target, scoreImages, pdfFile);
        }
    }

    public void ExportFilesFor(Target[] targets)
    {
        // Ensure folder exists. Otherwise MuseScore fails silently.
        Directory.CreateDirectory(config.TargetFolder);

        string[] GetDestinationsFor(Target target)
        {
            string[] extensions = [
                ".png",
                ".pdf",
                ".mscz",
            ];
            return extensions
                .Select(ext => Path.Combine(config.TargetFolder, $"{target.FilenameForExporting}{ext}"))
                .ToArray();
            
        }
        ExportFor(targets, GetDestinationsFor);
    }

    private static readonly Regex MetadataParser = new(@"<metaTag name=""(?<name>[\w\s]+)"">(?<value>[^<]*)</metaTag>", RegexOptions.Compiled);
    public TargetWithLabels[] LoadLabelInfoFor(Target[] targets)
    {
        Stopwatch sw = Stopwatch.StartNew();
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory("jamicionario");

        Dictionary<Target, string> locations = targets.ToDictionary(x => x, target => $"{target.FilenameForExporting}.mscx");
        ExportFor(targets, target => [Path.Combine(tempDir.FullName, locations[target])]);

        Dictionary<string, string> GetLabelsFor(Target target)
        {
            string fileName = locations[target];
            string mscxText = File.ReadAllText(Path.Combine(tempDir.FullName, locations[target]));
            IEnumerable<(string name, string value)> matches = MetadataParser
                    .Matches(mscxText)
                    .Select(match => (name: match.Groups["name"].Value, value: match.Groups["value"].Value));
            return MetadataBuilder.ProcessLabels(matches);
        }
        var labeled = targets
            .Select(target => new TargetWithLabels(target, GetLabelsFor(target)))
            .ToArray();
        return labeled;
    }


    private void ExportFor(Target[] targets, Func<Target, string[]> getOutFileNames)
    {
        // Generate the JSON job file.
        var conversionInstructions = targets
            // Order by title, to make it easier for the frontend.
            .OrderBy(target => target.ScoreName)
            .Select(target => new
            {
                @in = target.Mscz,
                @out = getOutFileNames(target),
            });
        string json = JsonHelper.Serialize(conversionInstructions);
        string jobFileName = Path.GetTempFileName();
        File.WriteAllText(jobFileName, json, System.Text.Encoding.UTF8);

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