using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor;
public class ScoresProcessor(ProcessingSteps instructions, ScoresConfig config, ILogger Logger, ILoggerFactory loggerFactory)
{
    public void ProcessJamicionarioData()
    {
        Stopwatch counter = Stopwatch.StartNew();

        Logger.LogDebug("Starting process. Instructions: {instructions}", instructions);
        DataFinder dataFinder = new(config);
        Target[] targets = dataFinder.FindData();
        Logger.LogDebug("Found {Count} MSCZ files.", targets.Length);

        Exporter exporter = new(config, dataFinder);

        if (instructions.HasFlag(ProcessingSteps.ExportScores))
        {
            Logger.LogDebug("Exporting available scores as images.");
            exporter.ExportImagesFor(targets);
        }

        Lazy<ExportedTarget[]> exportedTargets = new(() => CompileExportedTargets(exporter, targets));

        if (instructions.HasFlag(ProcessingSteps.RebuildMetadata))
        {
            Logger.LogDebug("Generating and exporting metadata.");
            MetadataBuilder metaBuilder = new(config);
            metaBuilder.ExportMetadataFor(exportedTargets.Value);
        }

        if (instructions.HasFlag(ProcessingSteps.ExportJamicionarioPdf))
        {
            Logger.LogDebug("Generating and exporting Jamicionário PDF.");
            PdfCompiler pdfCompiler = new(
                config,
                loggerFactory.CreateLogger<PdfCompiler>()
                );
            pdfCompiler.CompileJamicionario(exportedTargets.Value);
        }

        counter.Stop();
        Logger.LogInformation("✅ Finished. Processed {Count} scores in {Time}.", targets.Length, counter.Elapsed);
    }

    private ExportedTarget[] CompileExportedTargets(Exporter exporter, Target[] targets)
    {
        LabeledTarget[] labeledTargets = exporter.LoadLabelInfoFor(targets);
        ExportedTarget[] results = exporter
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