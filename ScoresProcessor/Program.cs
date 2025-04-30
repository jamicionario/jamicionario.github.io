
using Microsoft.Extensions.Logging;

namespace ScoresProcessor;
public static class Program
{
    /// <summary>
    /// Main entry-point for the Scores Processor console application.
    /// </summary>
    public static void Main()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(
            builder => builder
                .AddConsole()
                .AddFilter("ScoresProcessor", LogLevel.Debug)
            );

        ILogger readerLogger = loggerFactory.CreateLogger<ConfigurationReader>();

        ScoresConfig config = ConfigurationReader.ReadConfig(readerLogger);
        ILogger logger = loggerFactory.CreateLogger<ScoresProcessor>();

        ScoresProcessor processor = new(ProcessingSteps.ExportInfo, config, logger, loggerFactory);
        processor.ProcessJamicionarioData();
    }
}
