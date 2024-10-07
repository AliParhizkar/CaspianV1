using System.Reflection;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Common
{
    public static class OtherExtension
    {
        public static void CopySimpleProperty<TModel>(this TModel model, TModel newModel)
        {
            foreach (var info in typeof(TModel).GetProperties())
            {
                var type = info.PropertyType;
                if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                    info.SetValue(model, info.GetValue(newModel));
            }
        }

        public static string EnumText(this Enum field)
        {
            DisplayAttribute da;
            if (field == null)
                return null;
            var fi = field.GetType().GetField(field.ToString());
            if (fi == null)
                throw new Exception("هیچ فیلدی برای " + field.GetType().Name + " با مقدار " + field + " تعریف نشده است.");
            da = fi.GetCustomAttribute<DisplayAttribute>();
            if (da != null)
                return da.Name;
            return Convert.ToString(field);
        }

        public static int? ConvertToInt(this Enum curentEnum)
        {
            if (curentEnum == null)
                return null;
            return Convert.ToInt32(curentEnum);
        }

        public static void FullCopy<TModel>(this TModel model, TModel newModel)
        {
            foreach (var info in model.GetType().GetProperties())
            {
                var type = info.PropertyType;
                if (info.SetMethod != null)
                {
                    if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                        info.SetValue(model, info.GetValue(newModel));
                    else if (type.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                        FullCopy(info.GetValue(model), info.GetValue(newModel));
                }
            }
        }
    }

    public static class TypeExtension
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}