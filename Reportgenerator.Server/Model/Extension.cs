using System;

namespace Caspian.Model
{
    public static class Extension
    {
        public static TService GetService<TService>(this IServiceProvider provider) where TService: class
        {
            return provider.GetService(typeof(TService)) as TService;
        }
    }
}
