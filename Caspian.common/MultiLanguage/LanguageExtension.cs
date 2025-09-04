using System.Text.Json;
using System.Reflection;
using System.Globalization;

namespace Caspian.Common.Extension
{
    public static class LanguageExtension
    {
        static EntityLangData[] Datas { get; set; }

        public static string GetTitle(this Type type, string memberName)
        {
            var subsystem =  type.Assembly.GetSystemKind();
            if (subsystem == null)
                return null;
            if (Datas == null)
            {
                var path = $"{type.Assembly.GetMapPath()}/Languages/{CultureInfo.CurrentCulture.Name}.json";
                var jsonText = File.ReadAllText(path);
                Datas = JsonSerializer.Deserialize<EntityLangData[]>(jsonText);
            }
            return Datas.SingleOrDefault(t => t.SubSystemKind == subsystem && t.TypeName == type.Name && t.MemberName == memberName)?.Title;
        }

        public static SubSystemKind? GetSystemKind(this Assembly assembly)
        {
            foreach(var field in typeof(SubSystemKind).GetFields().Where(t => !t.IsSpecialName))
            {
                var value = (SubSystemKind)field.GetValue(null);
                if (value.GetEntityAssembly().FullName == assembly.FullName)
                    return value;
            }
            return null;
        }

        //public static string GetTitle(this SubSystemKind subSystem, int pageId, string controlId)
        //{
        //    if (Datas == null)
        //    {
        //        var path = $"{type.Assembly.GetMapPath()}/Languages/{CultureInfo.CurrentCulture.Name}.json";
        //        var jsonText = File.ReadAllText(path);
        //        Datas = JsonSerializer.Deserialize<EntityLangData[]>(jsonText);
        //    }
        //}
    }
}
