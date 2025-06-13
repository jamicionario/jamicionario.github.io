
# TODO

## Bugs

- Images and other files aren't served if their filenames include non-ASCII characters: as à é å etc.

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

## Features

- Better tiling for categories
    There's a way to tile the squares so they fit together, instead of being in fixed rows with variable space.
- Multi-level categories
    We'd like to have multi-level categories, such as "Bourrée" > "2T" and "Círculo/Chappeloise" > "Jigs ABC".
    From category details, allow navigating to children or to parent if they exist.
- Create template MSCZ
    Create a template MSCZ with proper metadata fields (empty).
    It should use metadata placeholders like $composer and $modifiedDate in its style/layout.
    Share it with the team, explain how it works and the difference.
    Consider adding it to the website.
- Show composer of the score.
    Where we have that information, show the composer of the score.
- Search should ignore accents and other diacritics.
    Searching for "bourree" and "bourrée" gets us different results. It shouldn't.
    Ideally, search would be ASCII-equivalent:
        Search term converted to ASCII-only;
        And filtering against ASCII-only names of scores and groups.
    This should not just remove the accented characters! That will fail the example of "bourrée".
