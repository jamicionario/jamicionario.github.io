using Microsoft.Extensions.Logging;
using NodaTime;
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
    public VersionInfo CompileJamicionario(ExportedResult[] targets)
    {
        using PdfDocument jamicionario = new();

        AddAuthoringTo(jamicionario, out VersionInfo version);

        AddStartingPagesTo(jamicionario);

        // Add all the scores.
        var targetsByTypeOfDance = targets
            .OrderBy(item => item.ScoreName, StringComparer.InvariantCultureIgnoreCase)
            .GroupBy(item => MetadataBuilder.GetTypeOfDanceFor(item.Source))
            .OrderBy(group => group.Key, StringComparer.InvariantCultureIgnoreCase);
        foreach (var group in targetsByTypeOfDance)
        {
            string? typeOfDance = group.Key;
            foreach (ExportedResult target in group)
            {
                if (target.ScorePdf == null)
                {
                    logger.LogWarning("No PDF found for score '{Score}'!", target.ScoreName);
                    continue;
                }
                AddPdfTo(jamicionario, target.ScorePdf, bookmarkGroup: typeOfDance, bookmarkName: target.ScoreName);
            }
        }
        jamicionario.Save(config.JamicionarioPdfFileName);
        string jsonVersion = JsonHelper.Serialize(version);
        File.WriteAllText(config.JamicionarioMetadataFileName, jsonVersion);

        return version;
    }

    private void AddAuthoringTo(PdfDocument jamicionario, out VersionInfo versionInfo)
    {
        int previousVersion = 0;
        if (File.Exists(config.JamicionarioMetadataFileName))
        {
            string data = File.ReadAllText(config.JamicionarioMetadataFileName);
            VersionInfo? read = JsonHelper.Deserialize<VersionInfo>(data);
            if (read != null)
            {
                previousVersion = read.Version;
            }
        }
        versionInfo = new VersionInfo(
            previousVersion + 1,
            SystemClock.Instance.GetCurrentInstant()
            );

        jamicionario.Info.Creator = "";
        // jamicionario.Info.Producer = "";
        jamicionario.Info.Elements.SetString("/Producer", "");
        jamicionario.Info.Title = $"Jamicionário v{versionInfo.Version}";
        jamicionario.Info.CreationDate = versionInfo.GenerationDate.ToDateTimeUtc();
    }

    private void AddPdfTo(PdfDocument jamicionario, string path, string? bookmarkGroup, string bookmarkName)
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

        AddOutline(jamicionario, bookmarkGroup, bookmarkName, firstPage);
    }

    /// <summary>
    ///     Adds an outline/bookmark to the chosen <paramref name="targetPage"/>,
    ///     with the chosen name,
    ///     and under the header with the title of the <paramref name="bookmarkGroup"/>.
    /// </summary>
    private void AddOutline(PdfDocument jamicionario,
        string? bookmarkGroup, string bookmarkName,
        PdfPage targetPage)
    {

        PdfOutlineCollection GetOutlines()
        {
            if (bookmarkGroup == null)
            {
                return jamicionario.Outlines;
            }
            PdfOutline? existing = jamicionario.Outlines
                .FirstOrDefault(outline => outline.Title == bookmarkGroup);
            existing ??= jamicionario.Outlines.Add(bookmarkGroup, targetPage);
            return existing.Outlines;
        }

        var outlines = GetOutlines();
        outlines.Add(bookmarkName, targetPage);
    }

    private void AddStartingPagesTo(PdfDocument jamicionario)
    {
        foreach (IIntroPage item in JamicionarioIntroPages)
        {
            if (item is IntroPage extraPage)
            {
                string path = Path.Combine(config.MasterDataFolder, extraPage.RelativePath);
                AddPdfTo(jamicionario, path, null, extraPage.BookmarkName);
                continue;
            }

            // If it's not an IntroPage, it has to be a group.
            PageGroup group = (PageGroup)item;
            foreach (var page in group.Children)
            {
                string path = Path.Combine(config.MasterDataFolder, page.RelativePath);
                AddPdfTo(jamicionario, path, group.BookmarkName, page.BookmarkName);
            }
        }
    }
}