
namespace ScoresProcessor;
public record class ScoresConfig
{
    public const string TargetFolderName = "files";

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
    #region Calculated properties

    /// <summary>
    /// The path for the Jamicionário itself, the PDF with all the scores.
    /// </summary>
    public string JamicionarioPdfFileName => Path.Combine(JamicionarioPublicFolder, "Jamicionario.pdf");
    /// <summary>
    /// The path for the metadata about the Jamicionário PDF: version, generation date, etc.
    /// </summary>
    public string JamicionarioMetadataFileName => Path.Combine(JamicionarioPublicFolder, "Jamicionario.metadata.json");

    /// <summary>
    /// The folder in Jamicionario where the generated data will be saved.
    /// </summary>
    public string TargetFolder => Path.Combine(JamicionarioPublicFolder, TargetFolderName);

    /// <summary>
    /// The path for the file where the scores' metadata is saved (number, name, images, etc).
    /// </summary>
    public string MetadataFileName => Path.Combine(JamicionarioPublicFolder, "score-metadata.json");

    /// <summary>
    /// The path for the file where the scores' search categories is saved.
    /// It saves the interesting categories (country, type, etc) and their values present in the data.
    /// </summary>
    public string SearchCategoriesFileName => Path.Combine(JamicionarioPublicFolder, "search-categories.json");

    #endregion
}
