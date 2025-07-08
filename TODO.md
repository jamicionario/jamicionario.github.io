
# TODO

## Bugs

- Images and other files aren't served if their filenames include non-ASCII characters: as à é å etc.
  - Test if UTF8-normalizing the filename fixes it.
  - Add a front-end test, a hidden page with a more complex URL like "Bourrée 2T - à Malochet.txt".
        This will allow testing without regenerating the data.
        Then it's easier to play-test with possible solutions.
        Ideally we deploy not to live, but to an alternative URL like https://jamicionario.github.io/jamicionario-staging/ or wherever else.

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
