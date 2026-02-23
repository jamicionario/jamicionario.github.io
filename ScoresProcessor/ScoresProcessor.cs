using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor;

public class ScoresProcessor(ProcessingSteps instructions, ScoresConfig config, ILogger Logger, ILoggerFactory loggerFactory)
{
    private ILogger<T> CreateLogger<T>() => loggerFactory.CreateLogger<T>();

    public void ProcessJamicionarioData()
    {
        Stopwatch counter = Stopwatch.StartNew();

        Logger.LogDebug("Starting process. Instructions received: {instructions}", instructions);
        DataFinder dataFinder = new(config, CreateLogger<DataFinder>());
        Target[] targets = dataFinder.FindData();
        Logger.LogDebug("Found {Count} MSCZ files.", targets.Length);

        Exporter exporter = new(config, dataFinder, CreateLogger<Exporter>());
        FileParser parser = new(exporter, CreateLogger<FileParser>());

        if (instructions.HasFlag(ProcessingSteps.ExportScores))
        {
            Stopwatch exportingCounter = Stopwatch.StartNew();
            Logger.LogDebug("Exporting available scores, as images pdfs and mscz.");
            try
            {
                exporter.ExportToPublicFolder(targets);
            }
            finally
            {
                exportingCounter.Stop();
                Logger.LogDebug("Time taken to export: {time}.", exportingCounter.Elapsed);
            }
        }

        Lazy<ExportedResult[]> exportedResults = new(() => CompileExportedResults(parser, exporter, targets));

        if (instructions.HasFlag(ProcessingSteps.RebuildMetadata))
        {
            Logger.LogDebug("Generating and exporting metadata.");
            MetadataBuilder metaBuilder = new(config);
            metaBuilder.ExportMetadataFor(exportedResults.Value);
        }

        if (instructions.HasFlag(ProcessingSteps.ExportJamicionarioPdf))
        {
            Logger.LogDebug("Generating and exporting Jamicionário PDF.");
            PdfCompiler pdfCompiler = new(config, CreateLogger<PdfCompiler>());
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

    private ExportedResult[] CompileExportedResults(FileParser fileParser, Exporter exporter, Target[] targets)
    {
        TargetWithLabels[] labeledTargets = fileParser.LoadLabelInfoFor(targets);
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