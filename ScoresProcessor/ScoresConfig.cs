
namespace ScoresProcessor;
public record class ScoresConfig()
{
    /// <summary>
    ///     The executable path of the MuseScore application,
    ///     which is used to process the MSCZ files
    ///     to include their data in the Jamicionário.
    /// </summary>
    /// <remarks>
    ///     If not supplied, "mscore" is assumed to be on $PATH.
    /// </remarks>
    public string? MuseScoreExecutablePath { get; set; }

    /// <summary>
    ///     The "public" folder in the Jamicionário project, such as "~/code/jamicionario".
    /// </summary>
    /// <remarks>
    ///     It can be a unix relative path such as "~/foo/public".
    /// </remarks>
    public required string JamicionarioPublicFolder { get; set; }

    /// <summary>
    ///     The folder where the original MSCZ files are.
    /// </summary>
    public required string MasterDataFolder { get; set; }


    // The remaining properties are automatically calculated:


    public string TargetFolder => Path.Combine(JamicionarioPublicFolder, "scores");
    public string MetadataFileName => Path.Combine(JamicionarioPublicFolder, "score-metadata.json");
}
