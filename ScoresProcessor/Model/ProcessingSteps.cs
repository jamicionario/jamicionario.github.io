
namespace ScoresProcessor.Model;
[Flags]
public enum ProcessingSteps
{
    None = 0,
    ExportScores = 1,
    RebuildMetadata = 2,
    All = ExportScores
        | RebuildMetadata,
}
