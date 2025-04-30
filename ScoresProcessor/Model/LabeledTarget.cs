namespace ScoresProcessor.Model;

public record class LabeledTarget(Target Original, Dictionary<string, string> Labels)
	: Target(Original)
{
}
