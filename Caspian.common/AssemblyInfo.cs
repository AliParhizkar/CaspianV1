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

        public Dictionary<string, string> GetRuleTypes(SubSystemKind subSystemKind)
        {
            var types = GetModelTypes(subSystemKind);
            var dic = new Dictionary<string, string>();
            foreach(var type in types)
            {
                var attr = type.GetCustomAttribute<RuleTypeAttribute>();
                if (attr != null)
                    dic.Add(type.Name, attr.Title);
            }
            return dic;
        }

        public Type GetDbContextType(SubSystemKind subSystemKind)
        {
            var contextType = GetModelTypes(subSystemKind).SingleOrDefault(t => t.BaseType == typeof(MyContext));
            if (contextType == null)
                throw new CaspianException("خطا: Model must has DbContext that inhirite from MyContext");
            return contextType;
        }

        public Type GetDbContextType(Type type)
        {
            var contextType = type.Assembly.GetTypes().SingleOrDefault(t => t.BaseType == typeof(MyContext));
            if (contextType == null)
                throw new CaspianException("خطا: Model must has DbContext that inhirite from MyContext");
            return contextType;
        }

        public Type[] GetServiseTypes(SubSystemKind subSystemKind)
        {
            return Assembly.LoadFile(RelatedPath + "\\" + subSystemKind.ToString() + ".Service.dll" ).GetTypes();
        }

        public Type[] GetModelTypes(SubSystemKind subSystemKind)
        {
            return Assembly.Load(subSystemKind.ToString() + ".Model").GetTypes();
        }

        public Type[] GetWebTypes(SubSystemKind subSystemKind)
        {
            return Assembly.Load(subSystemKind.ToString() + ".Web").GetTypes();
        }

        public Type[] GetAllServiceTypes(SubSystemKind subSystemKind)
        {
            var types = GetServiseTypes(subSystemKind);
            return types.Where(t => t.CustomAttributes.Any(u => u.AttributeType == typeof(ReportClassAttribute))).ToArray();
        }

        public Type GetModelType(SubSystemKind subSystemKind, string typeName)
        {
            var type = GetModelTypes(subSystemKind).SingleOrDefault(t => t.Name == typeName);
            if (type == null)
                throw new CaspianException("خطا: " + "There are no type with name " + typeName + " in assembly");
            return type;
        }

        public Type GetModelType(SubSystemKind subSystemKind, string @namespace, string className)
        {
            var type = GetModelTypes(subSystemKind).SingleOrDefault(t => t.Namespace == @namespace && t.Name == className);
            if (type == null)
                throw new CaspianException("خطا: " + "There are no type in namespace " + @namespace + " with name " + className);
            return type;
        }

        public IEnumerable InvokeReportMethod(SubSystemKind subSystemKind, string className, string methodName, IServiceScope scope)
        {
            var type = GetAllServiceTypes(subSystemKind).SingleOrDefault(t => t.Name == className);
            var method = type.GetMethod(methodName);
            var obj = Activator.CreateInstance(type, scope);
            var contextType = GetDbContextType(subSystemKind);
            //(obj as IEntity).Context = Activator.CreateInstance(contextType) as MyContext;
            return (IEnumerable)method.Invoke(obj, new object[]{ null});
        }
    }
}
