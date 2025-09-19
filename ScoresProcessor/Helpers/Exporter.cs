
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;

/// <summary>
/// Converts and exports scores using MuseScore via CLI.
/// </summary>
public class Exporter(ScoresConfig config, DataFinder dataFinder, ILogger<Exporter> logger)
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

    /// <summary>
    ///     Exports the <paramref name="targets"/> into the <see cref="ScoresConfig.JamicionarioPublicFolder"/>.
    ///     Exports as PNG, PDF, and MSCZ.
    /// </summary>
    public void ExportToPublicFolder(Target[] targets)
    {
        // Ensure folder exists. Otherwise MuseScore fails silently.
        Directory.CreateDirectory(config.TargetFolder);

        string[] GetDestinationsFor(Target target)
        {
            string[] extensions = [
                ".png",
                ".pdf",
                // TODO: copy the original file instead of re-exporting to MSCZ.
                //      - We're not keeping the original file dates, etc.
                //      - Not even the data is exactly the same - different file sizes.
                //        See the "Valsa 7T", the exported is 34316 bytes and the original is 34311 bytes.
                ".mscz",
            ];

            return extensions
                .Select(ext => Path.Combine(config.TargetFolder, $"{target.FilenameForExporting}{ext}"))
                .ToArray();

        }
        ExportFilesAs(targets, GetDestinationsFor);
    }

    /// <summary>
    ///     Converts the scores <paramref name="targets"/> and exports them,
    ///     according to the <paramref name="getOutFileNames"/>.
    /// </summary>
    /// <param name="targets">The scores to convert.</param>
    /// <param name="getOutFileNames">
    ///     A method to get the desired output filenames for each target.
    ///     The file will be in the format of its extension, as converted by MuseScore — e.g. PDF.
    /// </param>
    /// <exception cref="LaunchException">Raised if the conversion process could not be started.</exception>
    /// <exception cref="FileConversionException">Raised if the conversion process completes with an error.</exception>
    public void ExportFilesAs(Target[] targets, Func<Target, string[]> getOutFileNames)
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

        // Validate that the process completed properly.
        if (process.ExitCode != 0)
        {
            logger.LogError("Failed to export {Count} scores.", targets.Length);
            throw new FileConversionException($"Failed to export the files using MuseScore. Error code: {process.ExitCode}");
        }
    }
}