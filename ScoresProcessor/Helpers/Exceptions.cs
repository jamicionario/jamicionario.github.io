namespace ScoresProcessor.Helpers;

public class FileNameException(string message) : ArgumentException(message)
{
}

public class LaunchException(string message) : InvalidOperationException(message)
{
}
