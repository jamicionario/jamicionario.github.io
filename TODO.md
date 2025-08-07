
# TODO

## Bugs

### Urgent

- The image/PDF for scores should have bars above the repeating sections "1" and "2". Example: 7-beat waltz.
    Check the PDF in Dropobox — it is correct.
    My Musescore is not showing the lines, but it does show some small markers. But JV does see them in their MuseScore (same OS).

### Other

- Bug: after navigating to a score's details, any page moves to a score details using arrows — and it should not.
    Then going to some score details does not allow navigating to the previous, it's polluted with the first one.
- Images and other files aren't served if their filenames include non-ASCII characters: as à é å etc.
  - Test if UTF8-normalizing the filename fixes it.
  - Add a front-end test, a hidden page with a more complex URL like "Bourrée 2T - à Malochet.txt".
        This will allow testing without regenerating the data.
        Then it's easier to play-test with possible solutions.
        Ideally we deploy not to live, but to an alternative URL like <https://jamicionario.github.io/jamicionario-staging/> or wherever else.
- Tests are not running. Fix that.
  - After that, add tests for TimeAgoPipe.

## Tasks

- Autofocus
    The search page's input has autofocus.
    But as angular loads that focus is lost.
- Styling :target
    If we open /download#footnote-1 , the footnote is styled with :target .
    But as angular loads, that styling is lost.
- Set page title in score details
    Extend the TitleStrategy to include score name in the page title.
    This improves browser usability.
- Improve UX of About page.
    Allow collapsing sections.
    Add a TOC, maybe floating to the right and collapsable.
- Style external links.
    External links should have some visual mark.
    They might or might not open in a new page — consider this option.
    We might style download links as well (MSCZ, PDF), with some download icon.
- Auto-generate the PDF's "index" page, that lists the available scores.
    At the moment we are using a static file from the data folder, "Index/01 index.pdf".
    Instead, we should generate it ourselves as part of the processing of the files.
    See PdfCompiler.cs that is using that file. Use PdfSharp to create the index.
  - After, let's consider adding links to that index.
        Then the user can click the scores in the index to navigate in the PDF, like they use the bookmarks/outline.
        Right in the first pages, without extra clicks or opening more menus — great UX!

## Features

- Allow reading MSCX
    Some master files might be in MSCX format.
    Allow reading those.
    Ensure that we don't read both MSCZ and MSCX for the same score.
        Decide what to do if there are clashes. Throw an error? Or always prefer one, and throw a warning? Prefer which, the most recent?...
- Only regenerate data that is new.
    Check the change date for the files, and only reprocess those that are not onlder than the date of the current version: fileDate <= versionDate.
- Multi-level categories
    We'd like to have multi-level categories, such as "Bourrée" > "2T" and "Círculo/Chappeloise" > "Jigs ABC".
    From category details, allow navigating to children or to parent if they exist.
- Create template MSCZ
    Create a template MSCZ with proper metadata fields (empty).
    It should use metadata placeholders like $composer and $modifiedDate in its style/layout.
    Share it with the team, explain how it works and the difference.
    Consider adding it to the website.
  - Afterwards, see if we can put the property "work title" in the document.
- Show composer of the score.
    Where we have that information, show the composer of the score.
