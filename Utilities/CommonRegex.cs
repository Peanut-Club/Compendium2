using System.Text.RegularExpressions;

namespace Compendium.Utilities
{
    public static class CommonRegex
    {
        public static readonly Regex CamelCaseRegex = new Regex("([A-Z])([A-Z]+)($|[A-Z])", RegexOptions.Compiled);
    }
}