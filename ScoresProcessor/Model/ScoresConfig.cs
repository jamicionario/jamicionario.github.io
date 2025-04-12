
namespace ScoresProcessor.Model;

// TODO: read from configs.
public record class ScoresConfig()
{
    public string MuseScoreExecutablePath => "mscore";
    public string DataFolder => Path.GetFullPath(@"~/code/jamicionario/public/data");
    public string TargetFolder => Path.GetFullPath(@"~/code/jamicionario/public/scores");
    public TimeSpan ConversionTimeout => TimeSpan.FromSeconds(5);
}
