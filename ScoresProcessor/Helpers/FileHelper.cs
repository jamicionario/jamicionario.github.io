using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ScoresProcessor.Helpers;

public static partial class FileHelper
{
    private static readonly OrderedDictionary<string, string> NecessaryReplacements = new() {
        // This is just organizational and internal to the work of the "Data Team".
        { " (Jamicionario)", "" },
        { "(Jamicionario)", "" },
        // "#" in filenames creates problems when serving files in web.
        // URL "/files/image#41.png" will request "/files/image" only! :/
        { "#", "No." },
        // "?" in filenames will also create problems. Simply removed.
        { "?", "" },
        // Parenthesis in filenames create problems when serving files in web.
        // They are a special character, like ? and # and / and etc.
        // See e.g. https://webmasters.stackexchange.com/a/78114
        { " (", " " },
        { "(", " " },
        { ")", "" },
    };

    /// <summary>
    /// Cleans a <paramref name="scoreName"/> to be used in a URI,
    /// taking out special characters like # .
    /// </summary>
    public static string CleanNameForUri(string scoreName)
    {
        string clean = scoreName;
        foreach ((string find, string replace) in NecessaryReplacements)
        {
            clean = clean.Replace(find, replace);
        }
        clean = clean.Trim();
        return clean;
    }

    /// <summary>
    /// Removes the suffix "(Jamicionario)" from the end of the filename.
    /// </summary>
    public static string ClearSuffixFrom(string jamicionarioFileName)
    {
        // Remove "(Jamicionario)" from the end of the filename.
        return FilenameRegex().Replace(jamicionarioFileName, "");
    }
    /// <summary>
    /// Finds " (Jamicionário)" at the end of a string.
    /// </summary>
    /// <remarks>
    /// Allows variations:
    /// - Extra whitespace anywhere, from "(Jamicionario)" to " ( Jamicionario   ) ".
    /// - A dash before, e.g. " - Jamicionario".
    /// - Uppercase or lowercase J, with or without accent on the a: "Jamicionário", "jamicionario", etc.
    /// - With parenthesis or not.
    /// </remarks>
    [GeneratedRegex(@"\s*[-–—]\s*\(?\s*[Jj]amicion[aá]rio\s*\)?\s*$")]
    private static partial Regex FilenameRegex();

    /// <summary>
    /// Simplifies the <paramref name="text"/> to only have ASCII-like characters.
    /// This avoids issues with serving files on Github Pages.
    /// </summary>
    public static string SimplifyToUseAsWebFilename(string text)
    {
        var allowedCharacterTypes = new[] {
            UnicodeCategory.DecimalDigitNumber,
            UnicodeCategory.LowercaseLetter,
            UnicodeCategory.UppercaseLetter,
            UnicodeCategory.SpaceSeparator,
            UnicodeCategory.DashPunctuation,
        };

        string normalizedString = text.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new();

        foreach (var c in normalizedString.EnumerateRunes())
        {
            var unicodeCategory = Rune.GetUnicodeCategory(c);
            if (allowedCharacterTypes.Contains(unicodeCategory))
            {
                stringBuilder.Append(c);
            }
        }

        var compiled = stringBuilder.ToString();
        var renormalized = compiled.Normalize(NormalizationForm.FormC);

        return renormalized;
    }
}
