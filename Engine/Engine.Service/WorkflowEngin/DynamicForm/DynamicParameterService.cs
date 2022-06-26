using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterService : SimpleService<DynamicParameter>
    {
        public DynamicParameterService(IServiceScope scope)
            :base(scope)
        {

        }

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
