
namespace ScoresProcessor.Model;

// TODO: read from configs.
public record class ScoresConfig()
{
    public string MuseScoreExecutablePath => "mscore";
    public string DataFolder => @"/???/jamicionario/public/data";
    public string TargetFolder => @"/???/jamicionario/public/scores";
}
