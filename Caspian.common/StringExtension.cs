using System.Text.RegularExpressions;

namespace Caspian.Common
{
    public static class StringExtension
    {
        public static string SplitPascalCase(this string str)
        {
            Regex r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
            return r.Replace(str, " ");
        }

        public static bool HasValue(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
    }
}
