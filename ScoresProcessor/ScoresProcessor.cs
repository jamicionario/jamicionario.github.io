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
        MetadataBuilder metaBuilder = new(config, loggerFactory.CreateLogger<MetadataBuilder>());

        if (instructions.HasFlag(ProcessingSteps.ExportScores))
        {
            exporter.ExportImagesFor(targets);
        }
        if (instructions.HasFlag(ProcessingSteps.ExportInfo))
        {
            LabeledTarget[] labelInfo = exporter.LoadLabelInfoFor(targets);
            metaBuilder.ExportLabels(labelInfo);
        }

        RebuildMetadata(metaBuilder, exporter, targets);

        counter.Stop();
        Logger.LogInformation("✅ Finished. Processed {Count} scores in {Time}.", targets.Length, counter.Elapsed);
    }

    private void RebuildMetadata(MetadataBuilder metaBuilder, Exporter exporter, Target[] targets)
    {
        if (!instructions.HasFlag(ProcessingSteps.RebuildMetadata))
        {
            return;
        }

        Result[] results = exporter.GatherExportResultsFor(targets)
            .ToArray();
        Logger.LogDebug("Exported {Count} scores.", results.Length);
        if (results.Length != targets.Length)
        {
            Logger.LogWarning(
                "The number of exported scores does not match the number of scores!"
                    + " Got {Count results} for {Count targets}.",
                results.Length,
                targets.Length
                );
        }

        Logger.LogDebug("Generating and exporting metadata.");
        metaBuilder.ExportMetadataFor(results);
    }
}