using Microsoft.Extensions.DependencyInjection;

namespace Capian.Dynamicform.Component
{
    public static class Extension
    {
        public static bool HasValue(this string str)
        {
            if (str == null || str == "")
                return false;
            return true;
        }

        public static int? ConvertToInt(this Enum curentEnum)
        {
            if (curentEnum == null)
                return null;
            return Convert.ToInt32(curentEnum);
        }
    }
}
