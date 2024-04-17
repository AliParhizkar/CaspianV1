using System.Text.RegularExpressions;

namespace Caspian.Common.Extension
{
    public static class StringExtension
    {
        public static string SplitPascalCase(this string str)
        {
            Regex r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
            return r.Replace(str, " ");
        }
    }
}
