
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;

public class Exporter(ScoresConfig config, ILogger<Exporter> logger)
{
    public void Export(Target[] targets)
    {
        // TODO: convert in batch via JSON file.

        foreach (var target in targets)
        {
            string imageName = target.Mscz
                .Replace(".mscz", ".png")
                .Replace(config.DataFolder, config.TargetFolder);
            if (!imageName.EndsWith(".png"))
            {
                logger.LogError(
                    "Target does not seem to be a PNG? Got '{PNG file}' for '{Mscz file}'.",
                    imageName,
                    target.Mscz
                    );
                throw new FileNameException(
                    $"Target does not seem to be a PNG?"
                    );
            }

            // Thanks MuseScore. >_> I would expect at least an error like "could not create file"!
            EnsureFolderExistsFor(imageName);

            // mscore -o camposa.png camposa.mscz
            ProcessStartInfo startInfo = new(config.MuseScoreExecutablePath, [
                "-F", // Use factory settings - this avoids that user configs affect this script.
                "-o",
                imageName,
                target.Mscz,
            ])
            {
                // WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process process = Process.Start(startInfo)
                ?? throw new LaunchException($"Could not start file conversion for {target.Mscz}");

            bool completed = process.WaitForExit(config.ConversionTimeout);
            if (!completed)
            {
                logger.LogWarning("Conversion timeout reached, file might not have been converted: {Mscz}", target.Mscz);
            }
        }
    }

    private readonly HashSet<string> CheckedFolders = [];
    private void EnsureFolderExistsFor(string imageName)
    {
        string folder = Path.GetDirectoryName(imageName)
            ?? throw new FileNameException($"Could not get folder name for '{imageName}'.");
        if (CheckedFolders.Contains(folder))
        {
            return;
        }
        Directory.CreateDirectory(folder);
        CheckedFolders.Add(folder);
    }
}