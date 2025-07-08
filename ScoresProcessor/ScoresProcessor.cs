using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor;
public class ScoresProcessor(ProcessingSteps instructions, ScoresConfig config, ILogger Logger, ILoggerFactory loggerFactory)
{
    public void ProcessJamicionarioData()
    {
        Stopwatch counter = Stopwatch.StartNew();

        Logger.LogDebug("Starting process. Instructions received: {instructions}", instructions);
        DataFinder dataFinder = new(config, loggerFactory.CreateLogger<DataFinder>());
        Target[] targets = dataFinder.FindData();
        Logger.LogDebug("Found {Count} MSCZ files.", targets.Length);

        Exporter exporter = new(config, dataFinder, loggerFactory.CreateLogger<Exporter>());

        if (instructions.HasFlag(ProcessingSteps.ExportScores))
        {
            Logger.LogDebug("Exporting available scores, as images pdfs and mscz.");
            exporter.ExportFilesFor(targets);
        }

        Lazy<ExportedResult[]> exportedResults = new(() => CompileExportedResults(exporter, targets));

        if (instructions.HasFlag(ProcessingSteps.RebuildMetadata))
        {
            Logger.LogDebug("Generating and exporting metadata.");
            MetadataBuilder metaBuilder = new(config);
            metaBuilder.ExportMetadataFor(exportedResults.Value);
        }

        if (instructions.HasFlag(ProcessingSteps.ExportJamicionarioPdf))
        {
            Logger.LogDebug("Generating and exporting Jamicionário PDF.");
            PdfCompiler pdfCompiler = new(
                config,
                loggerFactory.CreateLogger<PdfCompiler>()
                );
            VersionInfo version = pdfCompiler.CompileJamicionario(exportedResults.Value);

            Logger.LogDebug("Generating and exporting Jamicionário MSCZ zip.");
            MsczZipper zipper = new(
                config
                );
            zipper.CompileAllMsczFilesIntoZip(exportedResults.Value, version);
        }

        counter.Stop();
        Logger.LogInformation("✅ Finished. Processed {Count} scores in {Time}.", targets.Length, counter.Elapsed);
    }

    private ExportedResult[] CompileExportedResults(Exporter exporter, Target[] targets)
    {
        TargetWithLabels[] labeledTargets = exporter.LoadLabelInfoFor(targets);
        ExportedResult[] results = exporter
            .GatherExportResultsFor(labeledTargets)
            .ToArray();
        Logger.LogDebug("Found {Count} exported scores.", results.Length);

        if (results.Length != targets.Length)
        {
            Logger.LogWarning(
                "The number of exported scores does not match the number of scores!"
                    + " Got {Count results} for {Count targets}.",
                results.Length,
                targets.Length
                );
        }
        return results;
    }
}