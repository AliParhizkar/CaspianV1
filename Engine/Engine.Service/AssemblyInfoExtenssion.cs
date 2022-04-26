using Caspian.Common;

namespace Caspian.Engine.Service
{
    public static class AssemblyInfoExtenssion
    {
        /// <summary>
        /// با استفاده از مشخصات گزارش مشخصات <see cref="Type"/>گزارش را برمی گرداند
        /// </summary>
        public static Type GetReturnType(this AssemblyInfo assemblyInfo, ReportGroup group)
        {
            var type = assemblyInfo.GetAllServiceTypes(group.SubSystem).SingleOrDefault(t => t.Namespace == group.NameSpace && t.Name == group.ClassTitle);
            if (type == null)
                throw new CaspianException("خطا: In namespace " + group.NameSpace + " type with name " + group.ClassTitle + " that has ReportClassAttribute not exist");
            var method = type.GetMethod(group.MethodName);
            if (method == null)
                throw new CaspianException("خطا: In Type " + type.Name + " method with name " + group.MethodName + " not exists");
            return method.ReturnType.GenericTypeArguments[0];
        }
    }
}
