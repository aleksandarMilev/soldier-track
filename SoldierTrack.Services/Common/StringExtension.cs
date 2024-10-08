namespace SoldierTrack.Services.Common
{
    using System.Text.RegularExpressions;

    public static class StringExtension
    {
        public static string SplitPascalCase(this string input) => Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }
}
