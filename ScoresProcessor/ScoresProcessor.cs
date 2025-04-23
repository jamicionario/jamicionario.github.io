using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor;
internal class ScoresProcessor
{
    internal static ILoggerFactory LogFactory { get; } = LoggerFactory.Create(
        builder => builder
            .AddConsole()
            .AddFilter("ScoresProcessor", LogLevel.Debug)
        );
    private static ILogger Logger { get; } = LogFactory.CreateLogger<ScoresProcessor>();
    private static void Main(string[] args)
    {
        Stopwatch counter = Stopwatch.StartNew();

        ScoresConfig config = ConfigurationHelper.ReadConfig();

        Target[] targets = DataFinder.FindData(config);
        Logger.LogDebug("Found {Count} files.", targets.Length);

        Exporter exporter = new(config, LogFactory.CreateLogger<Exporter>());
        Result[] results = exporter.Export(targets)
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
        MetadataBuilder metaBuilder = new(config, LogFactory.CreateLogger<MetadataBuilder>());
        metaBuilder.ExportMetadataFor(results);

        counter.Stop();
        Logger.LogInformation("✅ Finished. Processed {Count} scores in {Time}.", targets.Length, counter.Elapsed);
    }
}