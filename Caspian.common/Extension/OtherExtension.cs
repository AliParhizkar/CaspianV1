using System;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Caspian.Common.Service;

namespace Caspian.Common.Extension
{
    public static class OtherExtension
    {
        public static void RegisterCommonServices(this IServiceCollection services)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes(); 
            foreach(var type in types)
            {
                if (type.IsClass && type.BaseType.IsGenericType && type.Name != "CaspianValidator`1")
                {
                    var type1 = type.BaseType.GetGenericArguments()[0];
                    if (type.BaseType == typeof(SimpleService<>).MakeGenericType(type1))
                    {
                        services.AddTransient(type);
                        services.AddTransient(typeof(ISimpleService<>).MakeGenericType(type1), type);
                    }
                }
            }
        }

        static void CopySimpleProperty1(object obj, object newObj)
        {
            foreach(var info in obj.GetType().GetProperties())
            {
                var type = info.PropertyType;
                if (type.IsValueType || type == typeof(string))
                    info.SetValue(newObj, info.GetValue(obj));
                else if (type.IsEnumerableType())
                {
                    var items = info.GetValue(obj) as IEnumerable<object>;
                    if (items != null)
                    {
                        var genericType = typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]);
                        var list = Activator.CreateInstance(genericType) as System.Collections.IList;
                        foreach (var item in items)
                        {
                            var newItem = Activator.CreateInstance(item.GetType());
                            CopySimpleProperty1(item, newItem);
                            list.Add(newItem);
                        }
                    }
                }
            }
        }

        public static TModel CopySimpleProperty<TModel>(this TModel model)where TModel : class
        {
            var newObj = Activator.CreateInstance<TModel>();
            CopySimpleProperty1(model, newObj);
            return newObj;
        }

        public static void CopySimpleProperty<TModel>(this TModel model, TModel newModel)
        {
            try
            {
                foreach (var info in typeof(TModel).GetProperties())
                {
                    try
                    {
                        var type = info.PropertyType;
                        if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                        {
                            info.SetValue(model, info.GetValue(newModel));
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public static object Copy(this object model)
        {
            var type = model.GetType();
            var data = Activator.CreateInstance(type);
            foreach (var info in type.GetProperties())
            {
                if (info.PropertyType.IsValueType || info.PropertyType == typeof(string))
                {
                    info.SetValue(data, info.GetValue(model));
                }
            }
            return data;
        }

        public static TModel Copy<TModel>(this TModel model)
        {
            var data = Activator.CreateInstance<TModel>();
            foreach(var info in typeof(TModel).GetProperties())
            {
                if (info.PropertyType.IsValueType || info.PropertyType == typeof(string))
                {
                    info.SetValue(data, info.GetValue(model));
                }
            }
            return data;
        }

        public static object GetMyValue(this object obj, MemberExpression expr, bool checkNull = true)
        {
            if (expr == null)
                return null;
            var str = expr.ToString();
            var index = str.IndexOf('.');
            str = str.Substring(index + 1);
            return obj.GetMyValue(str, checkNull);
        }

        public static object GetMyValue(this object obj, string strName, bool checkNull = true)
        {
            if (checkNull == false && obj == null)
                return null;
            object result = obj;
            var type = result.GetType();
            foreach (var str in strName.Split('.'))
            {
                var temp = type.GetProperty(str);
                result = temp.GetValue(result);
                if (result == null)
                {
                    var type1 = temp.PropertyType;
                    if (!checkNull)
                        return null;
                    else
                        if (!type1.IsNullableType() && type1 != typeof(string) && type1 != typeof(byte[]))
                            throw new CaspianException(null, 1);
                }
                type = temp.PropertyType;
            }
            return result;
        }
    }
}
