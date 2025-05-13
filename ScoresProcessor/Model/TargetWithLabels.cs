namespace ScoresProcessor.Model;

public record class TargetWithLabels(Target Original, Dictionary<string, string> Labels)
	: Target(Original)
{
}
