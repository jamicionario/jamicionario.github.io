namespace ScoresProcessor.Model;

/// <param name="Mscz">The path to the original MSCZ file, such as "/.../data/.../Hanter Dro.mscz".</param>
/// <param name="ScoreName">The clean name of the score, such as "Hanter Dro".</param>
public record class Target(string Mscz, string ScoreName)
{
	/// <summary>
	/// The name of the image file to create, such as "Hanter Dro.png".
	/// </summary>
	public readonly string ImageName = $"{ScoreName}.png";

	/// <summary>
	/// 	The name of the metadata file to create, such as "Hanter Dro.json".
	/// </summary>
	/// <remarks>
	/// 	This file will contain all the properties of the MSCZ file that MuseScore exports.
	/// </remarks>
	public readonly string MetadataFileName = $"{ScoreName}.metajson";

	/// <summary>
	/// Gets the file-search pattern, to find files such as "Hanter Dro-1.png".
	/// </summary>
	public string GetPngSearchPattern() => $"{ScoreName}*.png";

	public static Target For(string mscz, ScoresConfig config)
	{
		string scoreName = Path.GetFileNameWithoutExtension(mscz);
		if (!mscz.EndsWith(".mscz"))
		{
			throw new FileNameException(
				$"File does not seem to be an mscz: {scoreName}. Failing, to avoid unexpected problems with '{scoreName}'."
				);
		}
		return new(mscz, scoreName);
	}

	public override string ToString() => ScoreName;
}
