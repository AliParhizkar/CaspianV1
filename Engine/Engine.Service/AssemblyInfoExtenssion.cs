using Caspian.Common;
using Caspian.Common.Extension;

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

    public static class TypeExtensions
    {
        public static ControlType GetControlType(this Type type)
        {
            var type1 = type.GetUnderlyingType();
            if (type1.IsMultiSelectEnum())
                return ControlType.CheckListBox;
            if (type1.IsEnumType())
                return ControlType.DropdownList;
            if (type1 == typeof(string))
                return ControlType.String;
            if (type1 == typeof(DateTime))
                return ControlType.Date;
            if (type1 == typeof(TimeSpan))
                return ControlType.Time;
            if (type1 == typeof(long) || type1 == typeof(int) || type1 == typeof(short) || type1 == typeof(byte))
                return ControlType.Integer;
            if (type1 == typeof(bool))
                return ControlType.CheckBox;
            if (type1 == typeof(double) || type1 == typeof(decimal) || type1 == typeof(float) || type1 == typeof(Single))
                return ControlType.Numeric;
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }
    }
}
