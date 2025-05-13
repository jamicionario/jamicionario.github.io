
namespace ScoresProcessor.Model;

public record class ExportedResult(TargetWithLabels Source,
	string[] ScoreImages,
	string? ScorePdf)
{
}
