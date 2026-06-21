using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;

public class FileParser(Exporter exporter, ILogger<FileParser> logger)
{
    private static readonly Regex MetadataParser = new(@"<metaTag name=""(?<name>[\w\s]+)"">(?<value>[^<]*)</metaTag>", RegexOptions.Compiled);
    public TargetWithLabels[] LoadLabelInfoFor(Target[] targets)
    {
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory("jamictionary");
        string[] path = [tempDir.FullName];

        exporter.ExportFilesAs(targets, target =>
        {
            string path = Path.Combine(
                tempDir.FullName,
                // We need each file in its own folder.
                // They have some dependent files such as "score_style.mss" that we don't want to clash.
                target.FilenameForExporting,
                $"{target.FilenameForExporting}.mscx"
                );
            // Ensure path exists, otherwise the process just fails.
            Directory.CreateDirectory(path);
            return [path];
        });

        Dictionary<string, List<Target>> styleProblems = [];
        Dictionary<string, string> GetLabelsFor(Target target)
        {
            // Load file as text.
            string mscxText = File.ReadAllText(Path.Combine(tempDir.FullName, target.FilenameForExporting, $"{target.FilenameForExporting}.mscx"));
            string scoreStyleText = File.ReadAllText(Path.Combine(tempDir.FullName, target.FilenameForExporting, "score_style.mss"));

            // Check for problems.
            CheckForStyleProblems(styleProblems, target, mscxText, scoreStyleText);

            // Parse labels.
            IEnumerable<(string name, string value)> matches = MetadataParser
                    .Matches(mscxText)
                    .Select(match => (name: match.Groups["name"].Value, value: match.Groups["value"].Value));
            return MetadataBuilder.ProcessLabels(matches);
        }
        var labeled = targets
            .Select(target => new TargetWithLabels(target, GetLabelsFor(target)))
            .ToArray();

        foreach((string problem, List<Target> problematicTargets) in styleProblems)
        {
            logger.LogError(
                "Problem found in {Count} scores: '{Title}'\n Scores: \n -> {Scores}\n",
                problematicTargets.Count,
                problem,
                string.Join("\n -> ", problematicTargets)
                );
        }
        return labeled;
    }

    private void CheckForStyleProblems(Dictionary<string, List<Target>> styleProblems, Target target, string mscxText, string scoreStyleText)
    {
        string[] problemsFound = CheckForStyleProblems(mscxText, scoreStyleText)
                          .ToArray();
        foreach (var problem in problemsFound)
        {
            if (styleProblems.TryGetValue(problem, out var targetsWithThisProblem))
            {
                targetsWithThisProblem.Add(target);
            }
            else
            {
                styleProblems.Add(problem, [target]);
            }
        }
    }

    /// <summary>
    /// Checks the <paramref name="scoreStyleText"/> for known problems.
    /// </summary>
    /// <param name="scoreStyleText">The text contents of an MSS file (the style for a given score).</param>
    /// <returns>Any problems found.</returns>
    private static IEnumerable<string> CheckForStyleProblems(string mscxText, string scoreStyleText)
    {
        if (mscxText.Contains("<Spanner type=\"Volta\""))
        {
            if (scoreStyleText.Contains("<voltaLineWidth>0</voltaLineWidth>"))
            {
                yield return "Voltas for this score have zero width!";
            }
        }
    }
}
