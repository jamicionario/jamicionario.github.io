// using System.Reflection;
// using Microsoft.Extensions.Logging;

namespace ScoresProcessor.Helpers;

public static class DataFinder
{
	// private static ILogger Logger { get; } =
	// 	LoggerFactory
	// 		.Create(builder => builder.AddConsole())
	// 		.CreateLogger(typeof(DataFinder));

	// public static Target[] FindAll()
	// {

	// 	string dataLocation = DataFinder.GetDataLocation();
	// 	Target[] targets = FindAllIn(dataLocation);
	// 	return targets;
	// }

	// public static string GetDataLocation()
	// {
	// 	// Check if the script received a location via args, like "--source ~/code/jamicionario/data
	// 	var args = Environment.GetCommandLineArgs();
	// 	var folderRequested = args
	// 		.SkipWhile(arg => arg.ToLowerInvariant() != "--source")
	// 		.Skip(1)
	// 		.FirstOrDefault();
	// 	if (folderRequested != null)
	// 	{
	// 		return folderRequested;
	// 	}

	// 	const string dataLocation = "../public/data";
	// 	string location = Assembly.GetExecutingAssembly().Location;
	// 	Logger.LogDebug("Location is:      {path}", location);
	// 	string path = Path.GetDirectoryName(location);
	// 	string rootLocation = Path.GetDirectoryName(path)!;
	// 	string targetPath = Path.Join(rootLocation, dataLocation);
	// 	string cleanPath = Path.GetFullPath(targetPath);

	// 	Logger.LogDebug("Path is:          {path}", path);
	// 	Logger.LogDebug("Root location is: {path}", rootLocation);
	// 	Logger.LogDebug("targetPath is:    {path}", targetPath);
	// 	Logger.LogDebug("cleanPath is:     {path}", cleanPath);
	// 	return cleanPath;
	// }

	public static Target[] FindData(ScoresConfig config)
	{
		string[] files = Directory.GetFiles(config.DataFolder, "*.mscz", SearchOption.AllDirectories);
		return files
			.Select(file => Target.For(file, config))
			.ToArray();
	}

	/// <summary>
	/// Find the generated PNG files for the <paramref name="target"/>.
	/// </summary>
	/// <remarks>
	/// This step is necesssary because MuseScore generates files "my score-1.png" when asked to generate "my score.png".
	/// <br />
	/// This is sensible: a score may have multiple pages for multiple instruments etc;
	/// forcing us to process a different filename in the general case makes us not trip on the special case.
	/// </remarks>
	/// <param name="target">
	/// The expected requested data,
	/// for which we will look for the <strong>actual</strong> result exports.
	/// </param>
	/// <returns>The actual data that was found to be exported.</returns>
	public static Result GatherExportsFor(Target target)
	{
		string searchPattern = target.GetPngSearchPattern();
		string[] scoreImages = Directory.GetFiles(target.DestinationFolder, searchPattern);
		return new Result(target, scoreImages);
	}
}