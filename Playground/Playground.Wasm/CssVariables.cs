using System.Text;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Utils;

namespace Playground.Wasm;

public static class CssVariables
{
    public static string Create(Scheme<uint> scheme, Scheme<uint> dark = null, string selector = ":root")
    {
        StringBuilder builder = new();

        builder.Append(selector);
        builder.Append(" {\n");
        foreach (var color in scheme.Enumerate())
        {
            builder.Append("    --md-sys-color-");
            builder.Append(PascalToKebab(color.Key));
            builder.Append(": ");
            string hex = StringUtils.HexFromArgb(color.Value);
            builder.Append(hex);
            builder.Append(";\n");
        }
        builder.Append('}');

        if (dark != null)
        {
            builder.Append("\n\n@media (prefers-color-scheme: dark) {\n");
            builder.Append("    ");
            builder.Append(selector);
            builder.Append(" {\n");
            foreach (var color in dark.Enumerate())
            {
                builder.Append("        --md-sys-color-");
                builder.Append(PascalToKebab(color.Key));
                builder.Append(": ");
                string hex = StringUtils.HexFromArgb(color.Value);
                builder.Append(hex);
                builder.Append(";\n");
            }
            builder.Append("    }\n}");
        }

        return builder.ToString();
    }

    private static string PascalToKebab(ReadOnlySpan<char> pascalCase)
    {
        if (pascalCase.Length == 0) return string.Empty;

        Span<char> kebabCase = stackalloc char[pascalCase.Length + pascalCase.Length / 2];
        int kebabCaseIndex = 0;
        kebabCase[kebabCaseIndex++] = char.ToLower(pascalCase[0]);

        for (int i = 1; i < pascalCase.Length; i++)
        {
            char currentChar = pascalCase[i];

            if (char.IsUpper(currentChar))
            {
                kebabCase[kebabCaseIndex++] = '-';
                kebabCase[kebabCaseIndex++] = char.ToLower(currentChar);
            }
            else
            {
                kebabCase[kebabCaseIndex++] = currentChar;
            }
        }

        return new(kebabCase[..kebabCaseIndex]);
    }
}