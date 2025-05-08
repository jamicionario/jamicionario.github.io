
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace ScoresProcessor.Helpers;
public class PdfCompiler(ScoresConfig config, ILogger<PdfCompiler> logger)
{
    /// <summary>
    /// Compiles the existing scores into a merged PDF, and saves it in the public folder.
    /// </summary>
    /// <remarks>
    /// Includes a landing page, an index, and bookmarks for all scores.
    /// </remarks>
    public void CompileJamicionario(ExportedTarget[] targets)
    {
        using PdfDocument merged = new();
        foreach (ExportedTarget target in targets)
        {
            if (target.ScorePdf == null)
            {
                logger.LogWarning("No PDF found for score '{Score}'!", target.ScoreName);
                continue;
            }
            using PdfDocument pdf = PdfReader.Open(target.ScorePdf, PdfDocumentOpenMode.Import);
            foreach (var page in pdf.Pages)
            {
                merged.AddPage(page);
            }
        }
        merged.Save(config.JamicionarioPdfFileName);
    }
}