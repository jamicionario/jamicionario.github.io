

using Microsoft.Extensions.Logging;

namespace ScoresProcessor;
public record class ScoresConfig
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
    #region Calculated properties

    /// <summary>
    /// The folder in Jamicionario where the generated data will be saved.
    /// </summary>
    public string TargetFolder => Path.Combine(JamicionarioPublicFolder, "scores");
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


    // Next we have helper methods that are used by other classes.
    #region Helper methods

    /// <summary>
    /// The full path to export a target to, such as "/.../scores/Hanter Dro.mscz".
    /// </summary>
    public string GetDestinationFor(Target target) => Path.Combine(TargetFolder, target.ImageName);

    #endregion
}
