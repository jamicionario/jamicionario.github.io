using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Model;

public interface ITarget
{
	string ScoreName { get; }
	string FilenameForExporting { get; }
	string Mscz { get; }
}

/// <param name="ScoreName">The clean name of the score, such as "Hanter Dro".</param>
/// <param name="Mscz">The path to the original MSCZ file, such as "/.../data/.../Hanter Dro.mscz".</param>
/// <param name="Pdf">The path to the PDF that is expected to be next to the original <paramref name="Mscz"/> file.</param>
public record class Target(string ScoreName, string FilenameForExporting, string Mscz) : ITarget
{
	/// <summary>
	/// 	The name of the metadata file to create, such as "Hanter Dro.json".
	/// </summary>
	/// <remarks>
	/// 	This file will contain all the properties of the MSCZ file that MuseScore exports.
	/// </remarks>
	public readonly string MetadataFileName = $"{FilenameForExporting}.metajson";

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
		string cleanScoreName = FileHelper.ClearSuffixFrom(scoreName);

		cleanScoreName = FileHelper.CleanNameForUri(cleanScoreName);
		// We have character encoding issues when serving files with non-ASCII characters.
		// Even if we HTML-encode it, something is not right and we get a 404.
		// See for example https://jamicionario.github.io/scores/12
		// Its named "Bourrée 2T - à Malochet", and this URL would fail to download from github... but it works locally.
		// https://jamicionario.github.io/files/Bourrée%202T%20-%20%C3%A0%20Malochet.pdf
		// So we're opting to implify the filename down to ASCII, removing diacritics and then all special characters.
		string filenameForExporting = FileHelper.SimplifyToUseAsWebFilename(cleanScoreName);
		return new(cleanScoreName, filenameForExporting, mscz);
	}

	public override string ToString() => ScoreName;
}
