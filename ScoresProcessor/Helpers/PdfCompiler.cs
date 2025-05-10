
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace ScoresProcessor.Helpers;

public class PdfCompiler(ScoresConfig config, ILogger<PdfCompiler> logger)
{
    private static readonly IIntroPage[] JamicionarioIntroPages = [
        new IntroPage("Index", "01 index.pdf"),
        new PageGroup("Intro", [
            new IntroPage("Português", "02 Intro/01a Intro ao Jamicionario (PT).pdf"),
            new IntroPage("English", "02 Intro/01b Intro to Jamictionary (EN).pdf"),
            new IntroPage("Français", "02 Intro/01c Intro au Jamiccionaire (FR).pdf"),
            new IntroPage("Deutsch", "02 Intro/01d Einfürung zum Jam-Worterbuch (DE).pdf"),
        ]),
    ];
    /// <summary>
    ///     Compiles the existing scores into a merged PDF, and saves it in the public folder.
    /// </summary>
    /// <remarks>
    ///     Includes a landing page, an index, and bookmarks for all scores.
    /// </remarks>
    public void CompileJamicionario(ExportedTarget[] targets)
    {
        using PdfDocument jamicionario = new();

        AddStartingPagesTo(jamicionario);

        // Add all the scores.
        foreach (ExportedTarget target in targets)
        {
            if (target.ScorePdf == null)
            {
                logger.LogWarning("No PDF found for score '{Score}'!", target.ScoreName);
                continue;
            }
            AddPdfTo(jamicionario, target.ScorePdf, bookmarkName: target.ScoreName);
        }
        jamicionario.Save(config.JamicionarioPdfFileName);
    }

    private void AddPdfTo(PdfDocument jamicionario, string path, string bookmarkName,
        Func<PdfPage, PdfOutlineCollection>? getOutlines = null
        )
    {
        using PdfDocument pdf = PdfReader.Open(path, PdfDocumentOpenMode.Import);

        PdfPage? firstPage = null;
        foreach (PdfPage page in pdf.Pages)
        {
            var added = jamicionario.AddPage(page);
            firstPage ??= added;
        }

        if (firstPage == null)
        {
            logger.LogError("The PDF has no pages: '{Path}'.", path);
            throw new ConfigurationException("The pdf has no pages.");
        }
        // Add outline/bookmark for the PDF added.
        var outlines = getOutlines?.Invoke(firstPage)
            ?? jamicionario.Outlines;
        outlines.Add(bookmarkName, firstPage);
    }

    private void AddStartingPagesTo(PdfDocument jamicionario)
    {
        foreach (IIntroPage item in JamicionarioIntroPages)
        {
            if (item is IntroPage extraPage)
            {
                string path = Path.Combine(config.MasterDataFolder, extraPage.RelativePath);
                AddPdfTo(jamicionario, path, extraPage.BookmarkName);
                continue;
            }

            // If it's not an IntroPage, it has to be a group.
            PageGroup group = (PageGroup)item;
            PdfOutline? groupOutline = null;
            PdfOutlineCollection getOutlines(PdfPage firstPage)
            {
                if (groupOutline == null)
                {
                    groupOutline = jamicionario.Outlines.Add(group.BookmarkName, firstPage);
                }
                return groupOutline.Outlines;
            }

            foreach (var page in group.Children)
            {
                string path = Path.Combine(config.MasterDataFolder, page.RelativePath);
                AddPdfTo(jamicionario, path, page.BookmarkName, getOutlines);
            }
        }
    }
}