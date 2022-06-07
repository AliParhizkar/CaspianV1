﻿using Caspian.Common;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

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
        public static ControlType GetControlType(this PropertyInfo info)
        {
            var entityType = info.DeclaringType;
            if (entityType.GetProperties().Any(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == info.Name))
                return ControlType.ComboBox;
            var type =  info.PropertyType.GetUnderlyingType();
            if (type.IsMultiSelectEnum())
                return ControlType.CheckListBox;
            if (type.IsEnumType())
                return ControlType.DropdownList;
            if (type == typeof(string))
                return ControlType.String;
            if (type == typeof(DateTime))
                return ControlType.Date;
            if (type == typeof(TimeSpan))
                return ControlType.Time;
            if (type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(byte))
                return ControlType.Integer;
            if (type == typeof(bool))
                return ControlType.CheckBox;
            if (type == typeof(double) || type == typeof(decimal) || type == typeof(float) || type == typeof(Single))
                return ControlType.Numeric;
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }
    }
}
