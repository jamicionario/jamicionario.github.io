
namespace ScoresProcessor;
public record class ScoresConfig
{
    public static ScoresConfig Default => new()
    {
        JamicionarioWebsiteFolder = "~/code/jamicionario.github.io",
        JamicionarioDataFolder = "~/code/jamicionario-data",
    };

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
    ///     The folder of the project for the Jamicionário's website, such as "~/code/jamicionario.github.io". <br />
    ///     The ScoresProcessor will put the generated data there, in the public folder.
    /// </summary>
    /// <remarks>
    ///     It can be a unix relative path such as "~/foo".
    /// </remarks>
    public required string JamicionarioWebsiteFolder { get; set; }

    /// <summary>
    ///     The folder of the jamicionario-data project, where the original MSCZ files are — the "master" files.
    /// </summary>
    /// <remarks>
    ///     It can be a unix relative path such as "~/code/jamicionario-data".
    /// </remarks>
    public required string JamicionarioDataFolder { get; set; }


    /// <summary>
    ///     The name of the folder where the processed score-files are placed.
    /// 
    ///     This value is not configurable.
    /// </summary>
    /// <remarks>
    ///     This folder is cleaned up during the deploy,
    ///     so make sure that this is a folder that does not contain other non-temporary data.
    /// </remarks>
    public const string TargetFolderName = "files";

    // The remaining properties are automatically calculated:
    #region Calculated properties

    /// <summary>
    /// The path for the "public" folder in the jamicionario's website repository.
    /// </summary>
    public string JamicionarioPublicFolder => Path.Combine(JamicionarioWebsiteFolder, "public");

    /// <summary>
    /// The path for the Jamicionário itself, the PDF with all the scores.
    /// </summary>
    public string JamicionarioPdfFileName => Path.Combine(JamicionarioPublicFolder, "Jamicionario.pdf");
    /// <summary>
    /// The path for the metadata about the Jamicionário PDF: version, generation date, etc.
    /// </summary>
    public string JamicionarioMetadataFileName => Path.Combine(JamicionarioPublicFolder, "Jamicionario.metadata.json");

    /// <summary>
    /// The path for zip with all the MSCZ files for the Jamicionário.
    /// </summary>
    public string JamicionarioZipFileName => Path.Combine(JamicionarioPublicFolder, "Jamicionario - all MSCZ.zip");


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
