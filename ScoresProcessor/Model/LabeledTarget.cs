namespace ScoresProcessor.Model;

public record class LabeledTarget(Target original, Dictionary<string, string> Labels)
	: Target(original)
{
}
