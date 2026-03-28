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
            if (Datas == null)
            {
                var path = $"{type.Assembly.GetMapPath()}/Languages/{CultureInfo.CurrentCulture.Name}.json";
                var jsonText = File.ReadAllText(path);
                Datas = JsonSerializer.Deserialize<EntityLangData[]>(jsonText);
            }
            return Datas.SingleOrDefault(t => t.SubsystemKind == subsystem && t.TypeName == type.Name && t.MemberName == memberName)?.Title;
        }

        public static SubsystemKind GetSystemKind(this Assembly assembly)
        {
            var subsystemName = assembly.GetName().Name.Split('.')[0];
            return (SubsystemKind)typeof(SubsystemKind).GetField(subsystemName).GetValue(null);
        }

        public static SubsystemMetaDataAttribute GetSubsystemMetaData(this Assembly assembly)
        {
            var subsystemName = assembly.GetName(true).Name.Split('.')[0];
            return typeof(SubsystemKind).GetField(subsystemName).GetCustomAttribute<SubsystemMetaDataAttribute>();
        }
    }
}
