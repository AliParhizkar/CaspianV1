using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Dynamicform.Component
{
    public static class Extension
    {
        public static bool HasValue(this string str)
        {
            throw new NotImplementedException();
        }

        public static int? ConvertToInt(this Enum curentEnum)
        {
            throw new NotImplementedException();
        }
    }

    public static class DIExtensions
    {
        public static TService GetService<TService>(this IServiceScope scope) where TService : class
        {
            throw new NotImplementedException();
        }
    }
}
