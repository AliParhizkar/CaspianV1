using Caspian.Common;
using System.Reflection;

namespace Caspian.Engine.Service
{
    public class DynamicTypeManager
    {
        public Dictionary<string, string> GetDynamicType(SubSystemKind subSystem)
        {
            var dic = new Dictionary<string, string>();
            var types = new AssemblyInfo().GetModelTypes(subSystem);
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<DynamicTypeAttribute>();
                if (attr != null)
                    dic.Add(type.Name, attr.Title);
            }
            return dic;
        }
    }
}
