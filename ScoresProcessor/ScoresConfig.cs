
namespace ScoresProcessor;
public record class ScoresConfig
{
    public static ScoresConfig Default => new()
    {
        JamictionaryWebsiteFolder = "~/code/jamictionary.github.io",
        JamictionaryDataFolder = "~/code/jamictionary-data",
    };

    /// <summary>
    ///     The executable path of the MuseScore application,
    ///     which is used to process the MSCZ files
    ///     to include their data in the Jamictionary.
    /// </summary>
    /// <remarks>
    ///     If not supplied, "mscore" is assumed to be on $PATH.
    /// </remarks>
    public string? MuseScoreExecutablePath { get; set; }

    /// <summary>
    ///     The folder of the project for the Jamictionary's website, such as "~/code/jamictionary.github.io". <br />
    ///     The ScoresProcessor will put the generated data there, in the public folder.
    /// </summary>
    /// <remarks>
    ///     It can be a unix relative path such as "~/foo".
    /// </remarks>
    public required string JamictionaryWebsiteFolder { get; set; }

    /// <summary>
    ///     The folder of the jamictionary-data project, where the original MSCZ files are — the "master" files.
    /// </summary>
    /// <remarks>
    ///     It can be a unix relative path such as "~/code/jamictionary-data".
    /// </remarks>
    public required string JamictionaryDataFolder { get; set; }


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
    /// The path for the "public" folder in the jamictionary's website repository.
    /// </summary>
    public string JamictionaryPublicFolder => Path.Combine(JamictionaryWebsiteFolder, "public");

    /// <summary>
    /// The path for the Jamictionary itself, the PDF with all the scores.
    /// </summary>
    public string JamictionaryPdfFileName => Path.Combine(JamictionaryPublicFolder, "Jamictionary.pdf");
    /// <summary>
    /// The path for the metadata about the Jamictionary PDF: version, generation date, etc.
    /// </summary>
    public string JamictionaryMetadataFileName => Path.Combine(JamictionaryPublicFolder, "Jamictionary.metadata.json");

    /// <summary>
    /// The path for zip with all the MSCZ files for the Jamictionary.
    /// </summary>
    public string JamictionaryZipFileName => Path.Combine(JamictionaryPublicFolder, "Jamictionary - all MSCZ.zip");


    /// <summary>
    /// The folder in Jamictionary where the generated data will be saved.
    /// </summary>
    public string TargetFolder => Path.Combine(JamictionaryPublicFolder, TargetFolderName);

    /// <summary>
    /// The path for the file where the scores' metadata is saved (number, name, images, etc).
    /// </summary>
    public string MetadataFileName => Path.Combine(JamictionaryPublicFolder, "score-metadata.json");

    /// <summary>
    /// The path for the file where the scores' search categories is saved.
    /// It saves the interesting categories (country, type, etc) and their values present in the data.
    /// </summary>
    public string SearchCategoriesFileName => Path.Combine(JamictionaryPublicFolder, "search-categories.json");

    #endregion
}
