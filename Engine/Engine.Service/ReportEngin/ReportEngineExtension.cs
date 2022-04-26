using System.Reflection;
using Caspian.Common.Extension;
using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common;

namespace Caspian.Engine
{
    public static class ReportPrintEngineExtension
    {

        public static IList<Type>GetEnumeType(this Type type)
        {
            var list = new List<Type>();
            GetEnumeType(type, list);
            return list;
        }

        private static void GetEnumeType(this Type type, IList<Type> list)
        {
            foreach (var info in type.GetProperties())
            {
                var tempType = info.PropertyType;
                if (tempType.IsNullableType())
                    tempType = Nullable.GetUnderlyingType(tempType);
                if (tempType.IsEnum)
                {
                    var attr = tempType.GetCustomAttribute<EnumTypeAttribute>();
                    if (attr != null)
                        list.Add(tempType);
                }
                else
                    if (tempType != type && !tempType.IsGenericType && !tempType.IsPrimitive && tempType != typeof(string))
                        GetEnumeType(tempType, list);
            }
        }

        //public static Type GetEnumeType(this Type type, string enTitle)
        //{
        //    var tempType = type.GetMyProperty(enTitle).PropertyType;
        //    if (tempType.IsNullable())
        //        tempType = Nullable.GetUnderlyingType(tempType);
        //    return tempType;
        //}

        //public static string CompleteTitleFa(this Type type, ReportParam reportParam)
        //{ 
        //    var control = new ReportControl()
        //    {
        //        EnTitle = reportParam.TitleEn,
        //        RuleId = reportParam.RuleId,
        //    };
        //    return CompleteTitleFa(type, control, reportParam.CompositionMethodType);
        //}

        //public static string CompleteTitleFa(this Type type, string enTitle)
        //{
        //    var control = new ReportControl()
        //    {
        //        EnTitle = enTitle
        //    };
        //    return CompleteTitleFa(type, control);
        //}

        //public static string CompleteTitleFa(this Type type, ReportNode node)
        //{
        //    var control = new ReportControl();
        //    if (node.RuleId.HasValue)
        //    {
        //        control.RuleId = node.RuleId;
        //    }
        //    control.EnTitle = node.TitleEn;
        //    return type.CompleteTitleFa(control);
        //}

        //public static string CompleteTitleFa(this Type type, int? ruleId, string enTitle, CompositionMethodType? methodType = null)
        //{
        //    if (ruleId.HasValue)
        //        return control.GetRuleTitle(type);
        //    Type tempType = type;
        //    string complexTypeTitle = null;
        //    PropertyInfo info = null;
        //    foreach (var str in control.EnTitle.Split('.'))
        //    {
        //        info = tempType.GetProperty(str);
        //        tempType = info.PropertyType;
        //        var complexTypeAttr = tempType.GetCustomAttribute<ComplexTypeAttribute>();
        //        if (complexTypeAttr != null)
        //            complexTypeTitle = info.GetCustomAttribute<ReportFieldAttribute>().FaTitle;
        //    }
        //    var reportFieldAttr = info.GetCustomAttribute<ReportFieldAttribute>();
        //    if (reportFieldAttr == null)
        //        return null;
        //    string title = reportFieldAttr.Title;
        //    if (!title.HasValue())
        //    {
        //        var display = info.GetCustomAttribute<DisplayNameAttribute>();
        //        if (display != null)
        //            title = display.DisplayName;
        //    }
        //    if (complexTypeTitle.HasValue())
        //        return string.Format(title + " {0}", complexTypeTitle);
        //    switch(methodType)
        //    {
        //        case CompositionMethodType.Sum: return "مجموع(" + title + ')';
        //        case CompositionMethodType.Avg: return "میانگین(" + title + ')';
        //        case CompositionMethodType.Max: return "ماکزیمم(" + title + ')';
        //        case CompositionMethodType.Min: return "مینیمم(" + title + ')';
        //    }
        //    return title;
        //}

        //public static string TitleFa(this Type type, ReportNode node)
        //{
        //    if (node.RuleId != null)
        //        return node.Rule.Title;
        //    Type tempType = type;
        //    PropertyInfo info = null;
        //    foreach (var str in node.TitleEn.Split('.'))
        //    {
        //        info = tempType.GetProperty(str);
        //        tempType = info.PropertyType;
        //    }
        //    return info.GetCustomAttribute<ReportFieldAttribute>().Title;
        //}

        public static PropertyInfo GetComplexPropertyInfo(this Type mainType, string str)
        {
            var parentType = mainType;
            PropertyInfo info = null;
            foreach (var item in str.Split('.'))
            {
                info = parentType.GetProperty(item);
                if (info.GetCustomAttribute<ForeignKeyAttribute>() == null)
                    break;
                parentType = info.PropertyType;
            }
            return info;
        }

        /// <summary>
        ///این متد پسوند یک مسیر را برمی گرداند
        /// </summary>
        /// <param name="mainType">نوع که می خواهیم براساس آن گزارش تعیین نمائیم</param>
        /// <param name="str">مسیری که می خواهیم پسوند آن را تعیین نمائیم</param>
        /// <returns>پسوند مسیر</returns>
        public static string GetExtend(this Type mainType, string str)
        {
            var parentType = mainType;
            string tempStr = "";
            foreach (var item in str.Split('.'))
            {
                var info = parentType.GetProperty(item);
                if (info.GetCustomAttribute<ForeignKeyAttribute>() == null)
                    break;
                if (tempStr.HasValue())
                    tempStr += '.';
                tempStr += info.Name;
                parentType = info.PropertyType;
            }
            if (tempStr == str || !tempStr.HasValue())
                return null;
            return str.Substring(tempStr.Length + 1, str.Length - tempStr.Length - 1);
        }

        public static string GetPrefix(this Type mainType, string path)
        {
            var parentType = mainType;
            string tempStr = "";
            foreach (var item in path.Split('.'))
            {
                var info = parentType.GetProperty(item);
                if (tempStr.HasValue())
                    tempStr += '.';
                tempStr += info.Name;
                if (info.GetCustomAttribute<ForeignKeyAttribute>() == null)
                    break;
                parentType = info.PropertyType;
            }
            return tempStr;
        }

        /// <summary>
        /// این متد نوع <see cref="Entity"/> یک مسیر را برمی گرداند.
        /// </summary>
        /// <param name="mainType">نوعی که می خواهیم براساس ان گزارش تهیه نمائیم</param>
        /// <param name="enTitle">مسیر فیلد</param>
        /// <returns>نوع <see cref="Entity"/></returns>
        public static Type GetEntityType(this Type mainType, string enTitle)
        {
            var parentType = mainType;
            foreach (var item in enTitle.Split('.'))
            {
                var info = parentType.GetProperty(item);
                if (info.GetCustomAttribute<ForeignKeyAttribute>() == null)
                    break;
                parentType = info.PropertyType;
            }
            return parentType;
        }

        private static string GetDependency(this Type mainType, Type type, string enTitle)
        {
            string result = null;
            if (mainType == type)
                return enTitle;
            else
            {
                foreach (var info in mainType.GetProperties())
                {
                    if (info.CustomAttributes.Any(t => t.AttributeType == typeof(ForeignKeyAttribute)))
                    {
                        var name = enTitle;
                        if (name.HasValue())
                            name += '.';
                        name += info.Name;
                        var temp = GetDependency(info.PropertyType, type, name);
                        if (temp.HasValue())
                            result = temp;
                    }
                }
            }
            return result;
        }

        public static string GetDependencyOfType(this Type mainType, Type type, string enTitle)
        {
            var extend = mainType.GetExtend(enTitle);
            var entityType = mainType.GetEntityType(enTitle);
            var dependency = type.GetDependency(entityType, null);
            var result = dependency;
            if (extend.HasValue())
            {
                if (result.HasValue())
                    result += '.';
                result += extend;
            }
            return result;
        }

        //public static void OrderByExpr(this Type type, int? orderById1)
        //{
        //    if (orderById1.HasValue)
        //    {
        //        ReportTree tree = new ReportTree();
        //        //var enTitle = tree.GetOrderByList(type).Single(t => t.Id == orderById1.Value).TitleEn;
                
        //    }
        //}
    }
}
