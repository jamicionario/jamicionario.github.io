
namespace ScoresProcessor.Helpers;
public class DataFinder(ScoresConfig config)
{
	public Target[] FindData()
	{
		string[] files = Directory.GetFiles(config.MasterDataFolder, "*.mscz", SearchOption.AllDirectories);
		return files
			.Select(file => Target.For(file, config))
			.ToArray();
	}

	/// <summary>
	/// 	Find the generated PNG files for the <paramref name="target"/>.
	/// </summary>
	/// <remarks>
	/// 	This step is necesssary because MuseScore generates files "my score-1.png" when asked to generate "my score.png".
	/// 	<br />
	/// 	This is sensible: a score may have multiple pages for multiple instruments etc;
	/// 	forcing us to process a different filename in the general case makes us not trip on the special case.
	/// </remarks>
	/// <param name="target">
	/// 	The expected requested data,
	/// 	for which we will look for the <strong>actual</strong> result exports.
	/// </param>
	/// <returns>
	/// 	The actual data that was found to be exported.
	/// </returns>
	public string[] FindExportedImagesFor(Target target)
	{
		string searchPattern = target.GetPngSearchPattern();
		string[] scoreImages = Directory.GetFiles(config.TargetFolder, searchPattern);
		return scoreImages;
	}
}