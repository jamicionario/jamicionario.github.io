
namespace ScoresProcessor.Model;

public record class ExportedTarget(Target Original, Dictionary<string, string> Labels,
	string[] ScoreImages,
	string? ScorePdf)
	: LabeledTarget(Original, Labels)
{
}
