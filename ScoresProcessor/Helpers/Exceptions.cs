
namespace ScoresProcessor.Helpers;

public class ConfigurationException(string message) : ArgumentException(message)
{
}

public class PdfCompilationException(string message) : Exception(message)
{
}

public class FileNameException(string message) : ArgumentException(message)
{
}

public class FolderException(string message) : ArgumentException(message)
{
}

public class LaunchException(string message) : InvalidOperationException(message)
{
}
