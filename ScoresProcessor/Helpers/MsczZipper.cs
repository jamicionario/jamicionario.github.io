
using System.IO.Compression;

namespace ScoresProcessor.Helpers;

public class MsczZipper(ScoresConfig config)
{
    public void CompileAllMsczFilesIntoZip(ITarget[] targets, VersionInfo version)
    {
        // First, delete it in case it exists - to ensure it is empty.
        File.Delete(config.JamictionaryZipFileName);

        using var archive = ZipFile.Open(config.JamictionaryZipFileName, ZipArchiveMode.Create);
        archive.Comment = $"Jamictionary v{version.Version} — all MSCZ files";
        foreach (var target in targets)
        {
            archive.CreateEntryFromFile(target.Mscz, $"{target.ScoreName}.mscz", CompressionLevel.Optimal);
        }
    }
}
