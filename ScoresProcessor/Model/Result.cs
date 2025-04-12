namespace ScoresProcessor.Model;

public record class Result : Target
{
	public string[] ScoreImages;
	public Result(Target original, string[] scoreImages)
		: base(original)
	{
		ScoreImages = scoreImages;
	}
}
