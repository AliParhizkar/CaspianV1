using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Caspian.Client.Extension
{
    public static class Extension
    {
        public static int? ConvertToInt(this Enum currentEnum)
        {
            if (currentEnum == null)
                return null;
            return Convert.ToInt32(currentEnum);
        }

        public static void AddRange<TEntity>(this ICollection<TEntity> entities, params TEntity[] items)
        {
            foreach (var item in items)
                entities.Add(item);
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


        public static bool HasValue(this string str) => !string.IsNullOrEmpty(str);

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
}
