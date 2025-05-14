
using System.IO.Compression;

namespace ScoresProcessor.Helpers;

public class MsczZipper(ScoresConfig config)
{
    public void CompileAllMsczFilesIntoZip(ITarget[] targets, VersionInfo version)
    {
        // First, delete it in case it exists - to ensure it is empty.
        File.Delete(config.JamicionarioZipFileName);

        using var archive = ZipFile.Open(config.JamicionarioZipFileName, ZipArchiveMode.Create);
        archive.Comment = $"Jamicionário v{version.Version} — all MSCZ files";
        foreach (var target in targets)
        {
            archive.CreateEntryFromFile(target.Mscz, $"{target.ScoreName}.mscz", CompressionLevel.Optimal);
        }
    }
}
