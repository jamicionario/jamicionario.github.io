namespace ScoresProcessor.Model;

public record class Result(Target Original, Dictionary<string, string> Labels, string[] ScoreImages)
	: LabeledTarget(Original, Labels)
{
}
