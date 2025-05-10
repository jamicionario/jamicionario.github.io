namespace ScoresProcessor.Helpers;

/// <summary>
/// An introductory page to be added at the start of the Jamicionário PDF.
/// </summary>
public interface IIntroPage
{
    string BookmarkName { get; }
}

/// <inheritdoc cref="IIntroPage" />
/// <param name="RelativePath">The path to the PDF, relative to <see cref="ScoresConfig.MasterDataFolder"/> .</param>
public record class IntroPage(string BookmarkName, string RelativePath) : IIntroPage;

/// <summary>
///     A group of pages,
///     with a <paramref name="BookmarkName"/>
///     and a set of <paramref name="Children"/> pages.
/// </summary>
public record class PageGroup(string BookmarkName, IntroPage[] Children) : IIntroPage;
