namespace ScoresProcessor.Model;

public record class Result(Target original, string[] ScoreImages)
	: Target(original)
{
}
