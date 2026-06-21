
namespace ScoresProcessor.Model;
[Flags]
public enum ProcessingSteps
{
    None = 0,
    CleanPreviousData = 1,
    ExportScores = 2,
    RebuildMetadata = 4,
    ExportJamictionaryPdf = 8,
    All = 0
        | CleanPreviousData
        | ExportScores
        | RebuildMetadata
        | ExportJamictionaryPdf
        ,
}
