using System.Text.RegularExpressions;

namespace Playground.Wasm;

public static partial class GeneratedRegex
{
    [GeneratedRegex("^(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])$")]
    public static partial Regex PascalToKebabRegex();
}