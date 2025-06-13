
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

## Features

- Search should ignore accents and other diacritics.
    Searching for "bourree" and "bourrée" gets us different results. It shouldn't.
    Ideally, search would be ASCII-equivalent:
        Search term converted to ASCII-only;
        And filtering against ASCII-only names of scores and groups.
    This should not just remove the accented characters! That will fail the example of "bourrée".
