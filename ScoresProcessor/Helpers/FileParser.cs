using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;

public class FileParser(Exporter exporter, ILogger<FileParser> logger)
{
    private static readonly Regex MetadataParser = new(@"<metaTag name=""(?<name>[\w\s]+)"">(?<value>[^<]*)</metaTag>", RegexOptions.Compiled);
    public TargetWithLabels[] LoadLabelInfoFor(Target[] targets)
    {
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory("jamicionario");

        Dictionary<Target, string> locations = targets.ToDictionary(x => x, target => $"{target.FilenameForExporting}.mscx");
        exporter.ExportFilesAs(targets, target => [Path.Combine(tempDir.FullName, locations[target])]);

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
}
