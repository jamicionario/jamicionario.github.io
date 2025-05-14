using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Model;

public interface ITarget
{
	string ScoreName { get; }
	string Mscz { get; }
	string Pdf { get; }
}

/// <param name="ScoreName">The clean name of the score, such as "Hanter Dro".</param>
/// <param name="Mscz">The path to the original MSCZ file, such as "/.../data/.../Hanter Dro.mscz".</param>
/// <param name="Pdf">The path to the PDF that is expected to be next to the original <paramref name="Mscz"/> file.</param>
public record class Target(string ScoreName, string Mscz, string Pdf) : ITarget
{
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

	/// <summary>
	/// Builds a <see cref="Target"/> from a file path. 
	/// </summary>
	/// <param name="mscz">The file path to an MSCZ file.</param>
	/// <exception cref="FileNameException">Thrown if the file does not appear to be an MSCZ — a MuseScore file.</exception>
	public static Target For(string mscz, ILogger logger)
	{
		string scoreName = Path.GetFileNameWithoutExtension(mscz);
		if (!mscz.EndsWith(".mscz"))
		{
			throw new FileNameException(
				$"File does not seem to be an mscz: {scoreName}. Failing, to avoid unexpected problems with '{scoreName}'."
				);
		}
		string pdf = mscz[..^".mscz".Length] + ".pdf";
		string cleanScoreName = FileHelper.CleanNameForWeb(scoreName, logger);
		return new(cleanScoreName, mscz, pdf);
	}

	public override string ToString() => ScoreName;
}
