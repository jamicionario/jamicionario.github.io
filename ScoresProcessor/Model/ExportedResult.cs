
namespace ScoresProcessor.Model;

public record class ExportedResult(TargetWithLabels Source, string[] ScoreImages, string? ScorePdf)
    : ITarget
{
  public string ScoreName => Source.ScoreName;

  public string Mscz => Source.Mscz;

  public string Pdf => Source.Pdf;
}
