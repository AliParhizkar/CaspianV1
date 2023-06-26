using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Caspian.Common
{
    public static class DIExtensions
    {
        public static TService GetService<TService>(this IServiceScope scope) where TService : class
        {
            var type = typeof(TService).BaseType;
            while(type != typeof(object))
            {
                if (type.IsGenericType)
                {
                    if (type.GenericTypeArguments.Length == 2)
                    {
                        Type type1 = type.GenericTypeArguments[0], type2 = type.GenericTypeArguments[1];
                        if (type == typeof(MasterDetailsService<,>).MakeGenericType(type1, type2))
                        {
                            var interfaceType = typeof(IMasterDetailsService<,>).MakeGenericType(type1, type2);
                            return scope.ServiceProvider.GetService(interfaceType) as TService;
                        }
                    }
                    if (type.GenericTypeArguments.Length == 1)
                    {
                        var genericType = type.GenericTypeArguments[0];
                        if (type == typeof(BaseService<>).MakeGenericType(genericType))
                        {
                            var interfaceType = typeof(IBaseService<>).MakeGenericType(genericType);
                            return scope.ServiceProvider.GetService(interfaceType) as TService;
                        }
                    }

                }
                type = type.BaseType;
            }
            throw new CaspianException("Service not registered");
        }
    }
}
