using Caspian.Engine;
using System.Reflection;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class AssemblyInfo
    {
        public string RelatedPath { get; private set; }

        public AssemblyInfo()
        {
            RelatedPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
        }

        public Dictionary<string, string> GetRuleTypes(SubsystemKind subsystemKind)
        {
            var types = GetModelTypes(subsystemKind);
            var dic = new Dictionary<string, string>();
            foreach(var type in types)
            {
                var attr = type.GetCustomAttribute<RuleTypeAttribute>();
                if (attr != null)
                    dic.Add(type.Name, attr.Title);
            }
            return dic;
        }

        public Type GetDbContextType(SubsystemKind subsystemKind)
        {
            var contextType = GetModelTypes(subsystemKind).SingleOrDefault(t => t.BaseType == typeof(CaspianContext));
            if (contextType == null)
                throw new CaspianException("خطا: Model must has DbContext that inherited from MyContext");
            return contextType;
        }

        public Type GetDbContextType(Type type)
        {
            var contextType = type.Assembly.GetTypes().SingleOrDefault(t => t.BaseType == typeof(CaspianContext) ||
                t.BaseType?.BaseType == typeof(CaspianContext));
            if (contextType == null)
                throw new CaspianException("خطا: Model must has DbContext that inhirite from MyContext");
            return contextType;
        }

        public Type[] GetServiceTypes(SubsystemKind subsystemKind)
        {
            var path = $"{RelatedPath}\\{subsystemKind}.Service.dll";
            return Assembly.LoadFile(path).GetTypes();
        }

        public Type[] GetModelTypes(SubsystemKind subsystemKind)
        {
            return Assembly.Load(subsystemKind.ToString() + ".Model").GetTypes();
        }

        public Type[] GetWebTypes(SubsystemKind subsystemKind)
        {
            return Assembly.Load(subsystemKind.ToString() + ".Web").GetTypes();
        }

        public Type[] GetAllServiceTypes(SubsystemKind subsystemKind)
        {
            var types = GetServiceTypes(subsystemKind);
            return types.Where(t => t.CustomAttributes.Any(u => u.AttributeType == typeof(ReportClassAttribute))).ToArray();
        }

        public Type GetModelType(SubsystemKind subsystemKind, string typeName)
        {
            var type = GetModelTypes(subsystemKind).SingleOrDefault(t => t.Name == typeName);
            if (type == null)
                throw new CaspianException("خطا: " + "There are no type with name " + typeName + " in assembly");
            return type;
        }

        public Type GetModelType(SubsystemKind subsystemKind, string @namespace, string className)
        {
            var type = GetModelTypes(subsystemKind).SingleOrDefault(t => t.Namespace == @namespace && t.Name == className);
            if (type == null)
                throw new CaspianException("خطا: " + "There are no type in namespace " + @namespace + " with name " + className);
            return type;
        }

        public IEnumerable InvokeReportMethod(SubsystemKind subsystemKind, string className, string methodName, IServiceScope scope)
        {
            var type = GetAllServiceTypes(subsystemKind).SingleOrDefault(t => t.Name == className);
            var method = type.GetMethod(methodName);
            var obj = Activator.CreateInstance(type, scope);
            var contextType = GetDbContextType(subsystemKind);
            //(obj as IEntity).Context = Activator.CreateInstance(contextType) as MyContext;
            return (IEnumerable)method.Invoke(obj, new object[]{ null});
        }
    }
}
