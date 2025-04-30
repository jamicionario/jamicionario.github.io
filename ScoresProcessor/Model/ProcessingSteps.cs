
namespace ScoresProcessor.Model;
[Flags]
public enum ProcessingSteps
{
    None = 0,
    ExportScores = 1,
    ExportInfo = 2,
    RebuildMetadata = 4,
    All = ExportScores
        | ExportInfo
        | RebuildMetadata,
}
