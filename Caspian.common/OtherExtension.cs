using System.Reflection;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Caspian.Common.Attributes;


namespace Caspian.Common
{
    public static class OtherExtension
    {
        internal static void CopySimpleProperty<TModel>(this TModel model, TModel newModel)
        {
            foreach (var info in typeof(TModel).GetProperties().Where(t => t.CanWrite))
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
            var type = field.GetType();
            if (type.GetCustomAttribute<EnumTypeAttribute>()?.IsBitwise == true)
            {
                var value = field.ConvertToInt();
                var text = string.Empty;
                foreach(var field1 in type.GetFields())
                {
                    if (!field1.IsSpecialName)
                    {
                        var value1 = Convert.ToInt32(field1.GetValue(null));
                        if ((value & value1) == value1)
                        {
                            var attr = field1.GetCustomAttribute<DisplayAttribute>();
                            if (text != string.Empty)
                                text += ", ";
                            text += attr?.Name ?? Convert.ToString(field1);
                        }
                    }
                }
                return text;
            }
            var fi = field.GetType().GetField(field.ToString());
            if (fi == null)
                throw new Exception("هیچ فیلدی برای " + field.GetType().Name + " با مقدار " + field + " تعریف نشده است.");
            da = fi.GetCustomAttribute<DisplayAttribute>();
            return da?.Name ?? Convert.ToString(field);
        }

        public static int? ConvertToInt(this Enum currentEnum)
        {
            if (currentEnum == null)
                return null;
            return Convert.ToInt32(currentEnum);
        }

        internal static void FullCopy<TModel>(this TModel model, TModel newModel)
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
