namespace ScoresProcessor.Model;

public record class Target(string Mscz)
{
	public override string ToString()
	{
		return Mscz;
	}
}
