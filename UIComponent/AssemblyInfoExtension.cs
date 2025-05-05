using FluentValidation;
using System.Reflection;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI.Service
{
    public static class AssemblyExtension
    {
        public static void InjectServices(this Assembly assembly, IServiceCollection services)
        {
            var types = assembly.GetTypes();
            foreach(var type in types)
            {
                var baseType = type.BaseType;
                while(baseType != typeof(object) && baseType != null)
                {
                    if (baseType.IsGenericType)
                    {
                        if (baseType.GenericTypeArguments.Length == 3)
                        {
                            Type type1 = baseType.GenericTypeArguments[0], type2 = baseType.GenericTypeArguments[1],
                                type3 = baseType.GenericTypeArguments[2];
                            if (baseType == typeof(MasterDetailsService<,,>).MakeGenericType(type1, type2, type3))
                            {
                                var interfaceType = typeof(IMasterDetailsService<,,>).MakeGenericType(type1, type2, type3);
                                services.AddScoped(interfaceType, provider => Activator.CreateInstance(type, provider));
                                var batchServiceType = typeof(BatchService<,,>).MakeGenericType(baseType.GenericTypeArguments);
                                services.AddScoped(batchServiceType, provider => Activator.CreateInstance(batchServiceType, provider));
                                var searchType = typeof(ISearchService<>).MakeGenericType(type1);
                                services.AddScoped(searchType, provider => Activator.CreateInstance(batchServiceType, provider));
                            }
                        }
                        else if (baseType.GenericTypeArguments.Length == 2)
                        {
                            Type type1 = baseType.GenericTypeArguments[0], type2 = baseType.GenericTypeArguments[1];
                            if (baseType == typeof(MasterDetailsService<,>).MakeGenericType(type1, type2))
                            {
                                var interfaceType = typeof(IMasterDetailsService<, >).MakeGenericType(type1, type2);
                                services.AddScoped(interfaceType, provider => Activator.CreateInstance(type, provider));
                                var searchType = typeof(ISearchService<>).MakeGenericType(type1);
                                var batchServiceType = typeof(BatchService<,>).MakeGenericType(baseType.GenericTypeArguments);
                                services.AddScoped(batchServiceType, provider => Activator.CreateInstance(batchServiceType, provider));
                                services.AddScoped(searchType, provider => Activator.CreateInstance(batchServiceType, provider));
                            }
                        }
                        else if (baseType.GenericTypeArguments.Length == 1)
                        {
                            var genericType = baseType.GenericTypeArguments[0];
                            var serviceType = typeof(BaseService<>).MakeGenericType(genericType);
                            if (baseType == serviceType)
                            {
                                services.AddScoped(typeof(IBaseService<>).MakeGenericType(genericType), provider => Activator.CreateInstance(type, provider));
                                var simpleInterfaceService = typeof(IUIService<>).MakeGenericType(genericType);
                                var searchType = typeof(ISearchService<>).MakeGenericType(genericType);
                                var simpleServiceType = typeof(UIService<>).MakeGenericType(genericType);
                                services.AddScoped(simpleInterfaceService, provider => Activator.CreateInstance(simpleServiceType, provider));
                                services.AddScoped(searchType, provider => Activator.CreateInstance(simpleServiceType, provider));
                                break;
                            }
                            else
                            {
                                var validatorType = typeof(AbstractValidator<>).MakeGenericType(genericType);
                                if (baseType == validatorType)
                                {
                                    services.AddScoped(typeof(IValidator<>).MakeGenericType(genericType), provider => Activator.CreateInstance(type, provider));
                                    break;
                                }
                            }
                        }
                    }
                    baseType = baseType.BaseType;
                }
            }
        }
    }
}
