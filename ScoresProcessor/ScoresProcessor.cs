using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor;
internal class ScoresProcessor
{
    internal static ILoggerFactory LogFactory {get;} = LoggerFactory.Create(builder => builder.AddConsole());
    private static ILogger Logger { get; } = LogFactory.CreateLogger<ScoresProcessor>();
    private static void Main(string[] args)
    {
        Stopwatch counter = Stopwatch.StartNew();
        ScoresConfig config = new();
        Target[] targets = DataFinder.FindData(config);
        Logger.LogDebug("Found {Count} files. Processing.", targets.Length);
        Exporter exporter = new(config, LogFactory.CreateLogger<Exporter>());
        Result[] results = exporter.Export(targets)
            .ToArray();
        Logger.LogDebug("Exported {Count} scores successfully.", results.Length);
        Logger.LogDebug("Generating and exporting metadata.");
        MetadataBuilder metaBuilder = new(config, LogFactory.CreateLogger<MetadataBuilder>());
        metaBuilder.ExportMetadataFor(results);
        counter.Stop();
        Logger.LogInformation("✅ Finished. Time taken: {Count}.", counter.Elapsed);
    }
}