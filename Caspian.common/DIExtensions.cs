using Caspian.Common.Service;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common
{
    public static class DIExtensions
    {
        public static TService GetService<TService>(this IServiceScope scope) where TService : class
        {
            return scope.ServiceProvider.GetCaspianService<TService>();
        }

        public static TService GetCaspianService<TService>(this IServiceProvider serviceProvider) where TService : class
        {
            var type = typeof(TService);
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
                            return serviceProvider.GetService(interfaceType) as TService;
                        }
                    }
                    if (type.GenericTypeArguments.Length == 1)
                    {
                        var genericType = type.GenericTypeArguments[0];
                        if (type == typeof(IBaseService<>).MakeGenericType(genericType))
                            return serviceProvider.GetService(type) as TService;
                        if (type == typeof(BaseService<>).MakeGenericType(genericType))
                        {
                            var interfaceType = typeof(IBaseService<>).MakeGenericType(genericType);
                            return serviceProvider.GetService(interfaceType) as TService;
                        }
                        if (type == typeof(AbstractValidator<>).MakeGenericType(genericType))
                        {
                            var interfaceType = typeof(IValidator<>).MakeGenericType(genericType);
                            return serviceProvider.GetService(interfaceType) as TService;
                            
                        }
                    }

                }
                type = type.BaseType;
            }
            var service = serviceProvider.GetService(typeof(TService)) as TService;
            if ( service == null)
                throw new CaspianException($"Service of type {typeof(TService).Name} not registered");
            return service;
        }
    }
}
