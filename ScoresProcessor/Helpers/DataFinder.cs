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
			.Select(file => new Target(file))
			.ToArray();
	}
}