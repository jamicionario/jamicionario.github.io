using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor;
internal class ScoresProcessor
{
    internal static ILoggerFactory LogFactory {get;} = LoggerFactory.Create(builder => builder.AddConsole());
    private static ILogger Logger { get; } = LogFactory.CreateLogger<ScoresProcessor>();
    private static void Main(string[] args)
    {
        ScoresConfig config = new();
        Target[] targets = DataFinder.FindData(config);
        Logger.LogDebug("Found {Count} files. Processing.", targets.Length);
        Exporter exporter = new(config, LogFactory.CreateLogger<Exporter>());
        Result[] results = exporter.Export(targets)
            .ToArray();
        Logger.LogInformation("Exported {Count} scores successfully.", results.Length);
        MetadataBuilder metaBuilder = new(config);
        metaBuilder.ExportMetadata(results);
        Logger.LogTrace("Finished.");
    }
}