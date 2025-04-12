namespace ScoresProcessor.Model;

/// <param name="Mscz">The path to the original MSCZ file, such as "/.../data/.../Hanter Dro.mscz".</param>
/// <param name="ScoreName">The clean name of the score, such as "Hanter Dro".</param>
/// <param name="DestinationFolder">
/// The folder path for the <paramref name="DestinationPng"/>, such as "/.../scores/".<br />
/// This would respect the original folder structure before, and be something such as "/.../scores/foo/", but no longer.
/// </param>
public record class Target(string Mscz, string ScoreName, string DestinationFolder)
{
	/// <summary>
	/// The name of the PNG file to create, such as "Hanter Dro.png".
	/// </summary>
	public string PngName = $"{ScoreName}.png";
	/// <summary>
	/// Gets the file-search pattern, to find files such as "Hanter Dro-1.png".
	/// </summary>
	public string GetPngSearchPattern() => $"{ScoreName}*.png";
	/// <summary>
	/// The full path to export to, such as "/.../scores/Hanter Dro.mscz".
	/// </summary>
	public string FullDestination => Path.Combine(DestinationFolder, PngName);

	public static Target For(string mscz, ScoresConfig config)
	{
		string scoreName = Path.GetFileNameWithoutExtension(mscz);
		if (!mscz.EndsWith(".mscz"))
		{
			throw new FileNameException(
				$"File does not seem to be an mscz: {scoreName}. Failing to avoid unexpected problems with '{scoreName}'."
				);
		}
		return new(mscz, scoreName, config.TargetFolder);
	}

	public override string ToString() => ScoreName;
}
