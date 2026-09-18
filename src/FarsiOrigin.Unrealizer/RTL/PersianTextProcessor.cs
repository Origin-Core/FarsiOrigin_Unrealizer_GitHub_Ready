using System.Globalization;
using System.Text;

namespace FarsiOrigin.Unrealizer.RTL;

public static class PersianTextProcessor
{
    public static string Normalize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input.Normalize(NormalizationForm.FormC)
            .Replace('\u064A', '\u06CC') // Arabic Yeh -> Persian Yeh
            .Replace('\u0643', '\u06A9') // Arabic Kaf -> Persian Kaf
            .Replace('\u200B', '\u200C'); // zero-width space -> ZWNJ
    }

    public static string PrepareLogicalRtl(string input) => Normalize(input);

    public static string SafePreview(string input)
    {
        var s = Normalize(input);
        return $"Graphemes={new StringInfo(s).LengthInTextElements} | {s}";
    }
}
