using Caspian.Common;
using FluentValidation;
using System.Reflection;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.UI;

namespace Caspian.Engine.Service
{
    public static class AssemblyInfoExtenssion
    {
        /// <summary>
        /// با استفاده از مشخصات گزارش مشخصات <see cref="Type"/>گزارش را برمی گرداند
        /// </summary>
        public static Type GetReturnType(this AssemblyInfo assemblyInfo, ReportGroup group)
        {
            var type = assemblyInfo.GetAllServiceTypes(group.SubSystem).SingleOrDefault(t => t.Namespace == group.NameSpace && t.Name == group.ClassTitle);
            if (type == null)
                throw new CaspianException("خطا: In namespace " + group.NameSpace + " type with name " + group.ClassTitle + " that has ReportClassAttribute not exist");
            var method = type.GetMethod(group.MethodName);
            if (method == null)
                throw new CaspianException("خطا: In Type " + type.Name + " method with name " + group.MethodName + " not exists");
            return method.ReturnType.GenericTypeArguments[0];
        }
    }

    public static class TypeExtensions
    {
        public static ControlType GetControlType(this PropertyInfo info)
        {
            var entityType = info.DeclaringType;
            if (entityType.GetProperties().Any(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == info.Name))
                return ControlType.List;
            var type =  info.PropertyType.GetUnderlyingType();
            if (type.IsMultiSelectEnum())
                return ControlType.CheckListBox;
            if (type.IsEnumType())
                return ControlType.DropdownList;
            if (type == typeof(string))
                return ControlType.String;
            if (type == typeof(DateTime))
                return ControlType.Date;
            if (type == typeof(TimeSpan))
                return ControlType.Time;
            if (type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(byte))
                return ControlType.Integer;
            if (type == typeof(bool))
                return ControlType.CheckBox;
            if (type == typeof(double) || type == typeof(decimal) || type == typeof(float) || type == typeof(Single))
                return ControlType.Numeric;
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }
    }

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
                        if (baseType.GenericTypeArguments.Length == 2)
                        {
                            Type type1 = baseType.GenericTypeArguments[0], type2 = baseType.GenericTypeArguments[1];
                            if (baseType == typeof(MasterDetailsService<,>).MakeGenericType(type1, type2))
                            {
                                var interfaceType = typeof(IMasterDetailsService<, >).MakeGenericType(type1, type2);
                                services.AddScoped(interfaceType, provider => Activator.CreateInstance(type, provider));
                                var batchServiceType = typeof(BatchService<,>).MakeGenericType(baseType.GenericTypeArguments);
                                services.AddScoped(batchServiceType, provider => Activator.CreateInstance(batchServiceType, provider));
                            }
                        }
                        if (baseType.GenericTypeArguments.Length == 1)
                        {
                            var genericType = baseType.GenericTypeArguments[0];
                            var serviceType = typeof(BaseService<>).MakeGenericType(genericType);
                            if (baseType == serviceType)
                            {
                                services.AddScoped(typeof(IBaseService<>).MakeGenericType(genericType), provider => Activator.CreateInstance(type, provider));
                                break;
                            }
                            else
                            {
                                var validatoeType = typeof(AbstractValidator<>).MakeGenericType(genericType);
                                if (baseType == validatoeType)
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
