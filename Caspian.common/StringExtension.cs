using System.Text.RegularExpressions;

namespace Caspian.Common
{
    public static class StringExtension
    {
        public static string SplitPascalCase(this string str)
        {
            var regex = new Regex(
            @"(?<=[a-z])(?=[A-Z])|
              (?<=[A-Z])(?=[A-Z][a-z])|
              (?<=[A-Za-z])(?=[0-9])|
              (?<=[0-9])(?=[A-Za-z])",
            RegexOptions.IgnorePatternWhitespace);

            return regex.Replace(str, " ");
        }

        public static bool HasValue(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static string NormalizePropertyPath(this string str, bool isReport)
        {
            if (isReport)
                return str.Replace(".", "");
            return str;
        }
    }
}
