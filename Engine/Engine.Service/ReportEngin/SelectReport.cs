using Caspian.Common;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectReport
    {
        private ParameterExpression paramExpr;

        public SelectReport(Type type)
        {
            paramExpr = Expression.Parameter(type, "t");
            var dynamicField = type.GetCustomAttribute<DynamicFieldAttribute>();

        }

        private IEnumerable<ReportParam> ComplexTypeFilter(IEnumerable<ReportParam> fields)
        {
            var list = new List<ReportParam>();
            foreach (var field in fields)
            {
                var type = paramExpr.Type;
                string str = "";
                foreach (var item in field.TitleEn.Split('.'))
                {
                    if (type.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                        break;
                    if (str != "")
                        str += '.';
                    str += item;
                    var info = type.GetProperty(item);
                    type = info.PropertyType;
                }
                if (!list.Any(t => t.TitleEn == str))
                    list.Add(field);
            }
            return list;
        }

        public LambdaExpression SimpleSelect(IList<ReportParam> reportParams)
        {
            var flag = reportParams.Any(t => t.RuleId.HasValue);
            var fields = ComplexTypeFilter(reportParams.Where(t => !t.RuleId.HasValue)).ToArray();
            var dynamicFields = reportParams.Where(t => t.RuleId.HasValue);
            Type dynamicItemType = null, dynamicTypeOfDynamicItem = null;
            if (flag)
            {
                var enTitle = dynamicFields.First().TitleEn;
                dynamicItemType = GetDynamicItemType(enTitle);
                dynamicTypeOfDynamicItem = GetDynamicTypeOfDynamicItem(dynamicItemType, enTitle);
            }
            var dynamicType = GetDynamicType(fields, dynamicTypeOfDynamicItem);
            int i = 0;
            var list = new List<MemberBinding>();
            foreach (var info in dynamicType.GetProperties())
            {
                if (info.Name != "DynamicItems" && i < fields.Length)
                {
                    var name = fields[i].TitleEn;
                    list.Add(Expression.Bind(info, GetMemberExpr(name)));
                }
                i++;
            }
            if (flag)
            {
                var info = dynamicType.GetMember("DynamicItems")[0];
                var tempExpr = DynamicItemSelectExpr(dynamicItemType, dynamicTypeOfDynamicItem, info, dynamicFields.First().TitleEn);
                list.Add(Expression.Bind(info, tempExpr));
            }
            var expr = Expression.MemberInit(Expression.New(dynamicType), list);
            return Expression.Lambda(expr, new ParameterExpression[] { paramExpr });
        }

        private Expression GetMemberExpr(string field)
        {
            Expression expr = paramExpr;
            var type = expr.Type;
            foreach (var item in field.Split('.'))
            {
                var info = type.GetProperty(item);
                if (info.DeclaringType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                {
                    var attr = info.GetCustomAttribute<ReportFieldAttribute>();
                    int startIndex = attr.StartIndex, length = attr.Length;
                    info = info.DeclaringType.GetProperties().Single(t => t.CanWrite);
                    expr = Expression.Property(expr, info);
                    var method = typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
                    expr = Expression.Call(expr, method, Expression.Constant(startIndex), Expression.Constant(length));
                }
                else
                    expr = Expression.Property(expr, info);
                type = info.PropertyType;
            }
            return expr;
        }

        private Type GetDynamicType(IEnumerable<ReportParam> fields, Type dynamicTypeOfDynamicItem)
        {
            var list = new List<DynamicProperty>();
            foreach (var field in fields)
            {
                Type tempType = null;
                string name = GetEqualFieldName(field.TitleEn, field.CompositionMethodType);
                var index = name.IndexOf('.');
                if (index > 0)
                {
                    name = name.Substring(0, index);
                    tempType = paramExpr.Type.GetComplexPropertyInfo(field.TitleEn).PropertyType;
                }
                else
                {
                    var info = paramExpr.Type.GetMyProperty(field.TitleEn);
                    if (info.DeclaringType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                        tempType = typeof(string);
                    else
                        tempType = info.PropertyType;
                }
                list.Add(new DynamicProperty(name, tempType));
            }
            if (dynamicTypeOfDynamicItem != null)
            {
                var type = typeof(IEnumerable<>);
                type = type.MakeGenericType(new Type[] { dynamicTypeOfDynamicItem });
                list.Add(new DynamicProperty("DynamicItems", type));
            }
            return DynamicClassFactory.CreateType(list);
        }

        public LambdaExpression GroupBy(IList<ReportParam> reportParams)
        {
            reportParams = reportParams.Where(t => !t.IsKey && !t.CompositionMethodType.HasValue).ToArray();
            var type = GetTSourceType(paramExpr.Type, reportParams);
            Expression parameterExpr = Expression.Parameter(paramExpr.Type, "t");
            var index = 0;
            var members = new List<MemberAssignment>();
            foreach (var info in type.GetProperties())
            {
                if (info.Name == "Item")
                    continue;
                members.Add(Expression.Bind(info, PropertyExpr(parameterExpr, reportParams[index].TitleEn)));
                index++;
            }
            var expr = Expression.MemberInit(Expression.New(type), members);
            return Expression.Lambda(expr, new ParameterExpression[] { parameterExpr as ParameterExpression });
        }

        public LambdaExpression SelectForGroupBy(IList<ReportParam> reportParams)
        {
            var sourceType = GetTSourceType(paramExpr.Type, reportParams.Where(t => !t.CompositionMethodType.HasValue).ToList());
            var type = typeof(IGrouping<,>).MakeGenericType(sourceType, paramExpr.Type);
            sourceType = GetTSourceType(paramExpr.Type, reportParams);
            Expression parameterExpr = Expression.Parameter(type, "t");
            var members = new List<MemberAssignment>();
            foreach (var param in reportParams)
            {
                Expression expr = null;
                var name = param.TitleEn.Replace('.', '_');
                if (param.CompositionMethodType.HasValue)
                {
                    switch (param.CompositionMethodType.Value)
                    {
                        case CompositionMethodType.Sum: name = "Sum_" + name; break;
                        case CompositionMethodType.Avg: name = "Avg_" + name; break;
                        case CompositionMethodType.Max: name = "Max_" + name; break;
                        case CompositionMethodType.Min: name = "Min_" + name; break;
                    }
                    expr = MethodExpr(paramExpr.Type, parameterExpr, param);
                }
                else
                {
                    expr = parameterExpr;
                    expr = Expression.Property(expr, "Key");
                    expr = Expression.Property(expr, name);
                }
                var info = sourceType.GetProperty(name);
                members.Add(Expression.Bind(info, expr));
            }
            var memberExpr = Expression.MemberInit(Expression.New(sourceType), members);
            return Expression.Lambda(memberExpr, new ParameterExpression[] { parameterExpr as ParameterExpression });
        }

        private Type GetTSourceType(Type mainType, IList<ReportParam> reportParams)
        {
            var list = new List<DynamicProperty>();
            foreach (var param in reportParams)
            {
                var info = mainType.GetMyProperty(param.TitleEn);
                var type = info.PropertyType;
                
                var name = param.TitleEn.Replace('.', '_');
                switch (param.CompositionMethodType)
                {
                    case CompositionMethodType.Sum: name = "Sum_" + name; break;
                    case CompositionMethodType.Avg: name = "Avg_" + name;
                        if (type == typeof(int) || type == typeof(long))
                            type = typeof(double);
                        else
                            if (type == typeof(int?) || type == typeof(long?))
                                type = typeof(double?);
                        break;
                    case CompositionMethodType.Max: name = "Max_" + name; break;
                    case CompositionMethodType.Min: name = "Min_" + name; break;
                }
                if (info.DeclaringType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                    type = typeof(string);
                list.Add(new DynamicProperty(name, type));
            }
            return DynamicClassFactory.CreateType(list);
        }

        private Expression PropertyExpr(Expression parameter, string path)
        {
            Expression expr = parameter;
            var type = parameter.Type;
            foreach (var str in path.Split('.'))
            {
                var info = type.GetProperty(str);
                if (info.DeclaringType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                {
                    var attr = info.GetCustomAttribute<ReportFieldAttribute>();
                    int startIndex = attr.StartIndex, length = attr.Length;
                    info = info.DeclaringType.GetProperties().Single(t => t.CanWrite);
                    expr = Expression.Property(expr, info);
                    var method = typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
                    expr = Expression.Call(expr, method, Expression.Constant(startIndex), Expression.Constant(length));
                }
                else
                    expr = Expression.Property(expr, info);
                type = info.PropertyType;
            }
            return expr;
        }

        private MethodInfo GetMethodInfo(Type mainType, ReportParam param)
        {
            var tempType  = mainType.GetMyProperty(param.TitleEn).PropertyType;
            var type = tempType;
            if (param.CompositionMethodType == CompositionMethodType.Avg)
            {
                if (type == typeof(int) || type == typeof(long))
                    type = typeof(double);
                if (type == typeof(int?) || type == typeof(long?))
                    type = typeof(double?);
            }
            string methodName = null;
            switch (param.CompositionMethodType.Value)
            {
                case CompositionMethodType.Sum:
                    methodName = "Sum"; break;
                case CompositionMethodType.Avg:
                    methodName = "Average"; break;
                case CompositionMethodType.Max:
                    methodName = "Max"; break;
                case CompositionMethodType.Min:
                    methodName = "Min"; break;
            }
            var methods = typeof(Enumerable).GetMethods().Where(t => t.Name == methodName && t.IsGenericMethod);
            MethodInfo method = null;
            foreach(var item in methods)
            {
                var parameters = item.GetParameters();
                Type argumentType = null;
                if (parameters.Length == 2)
                    argumentType = parameters[1].ParameterType.GetGenericArguments()[1];
                else
                    argumentType = parameters[0].ParameterType.GetGenericArguments()[0];
                if (tempType == argumentType)
                {
                    method = item;
                    break;
                }
            }
            return method.MakeGenericMethod(mainType);
        }

        private Expression MethodExpr(Type mainType, Expression parameter, ReportParam param)
        {
            var u = Expression.Parameter(mainType, "u");
            Expression expr = u;
            foreach (var str in param.TitleEn.Split('.'))
                expr = Expression.Property(u, str);
            expr = Expression.Lambda(expr, u);
            var method = GetMethodInfo(mainType, param);
            return Expression.Call(null, method, parameter, expr);
        }

        public IList GetValues(IEnumerable values, IList<ReportParam> reportParams)
        {
            //var fields = reportParams.Where(t => !t.DynamicItemId.HasValue).Select(t => t.TitleEn).ToList();
            var type = GetEqualType(reportParams, typeof(string));
            var listType = typeof(List<>);
            listType = listType.MakeGenericType(new Type[] { type });
            IList list = (IList)Activator.CreateInstance(listType);
            var flag = reportParams.Any(t => t.RuleId.HasValue);
            string dynamicFieldName = null;
            if (flag)
            {
                var enTitle = reportParams.First(t => t.RuleId.HasValue).TitleEn;
                dynamicFieldName = new DynamicFieldEngin().GetDynamicFieldForeignKey(paramExpr.Type, enTitle);
            }
            foreach (var value in values.AsQueryable().ToIList())
            {
                IEnumerable<object> dynamicItemsValue = new List<object>();
                if (value.GetType().GetProperty("DynamicItems") != null)
                    dynamicItemsValue = (IEnumerable<object>)value.GetMyValue("DynamicItems");
                var obj = Activator.CreateInstance(type);
                foreach (var param in reportParams)
                {
                    string name = null;
                    object tempValue = null;
                    if (param.RuleId.HasValue)
                    {
                        foreach (var dynamicItem in dynamicItemsValue)
                        {
                            if (Convert.ToInt32(dynamicItem.GetMyValue(dynamicFieldName)) == param.RuleId.Value)
                            {
                                var text = Convert.ToString(dynamicItem.GetMyValue("Text"));
                                if (text.HasValue())
                                    tempValue = text;
                                else
                                    tempValue = dynamicItem.GetMyValue("Value");
                            }
                                
                        }
                        name = "DynamicParam" + param.RuleId.Value;
                    }
                    else
                    {
                        name = GetEqualFieldName(param.TitleEn);
                        switch(param.CompositionMethodType)
                        {
                            case CompositionMethodType.Sum: name = "Sum_" + name; break;
                            case CompositionMethodType.Avg: name = "Avg_" + name; break;
                            case CompositionMethodType.Max: name = "Max_" +  name; break;
                            case CompositionMethodType.Min: name = "Min_" + name; break;
                        }
                        tempValue = value.GetMyValue(name.Replace('.', '_'), false);
                        var tempInfo = paramExpr.Type.GetMyProperty(param.TitleEn);
                        if (tempInfo.DeclaringType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                        {
                            var fieldType = paramExpr.Type.GetMyProperty(param.TitleEn).PropertyType;
                            if (fieldType.IsNullableType())
                                fieldType = Nullable.GetUnderlyingType(fieldType);
                            if (fieldType.IsEnumType())
                                tempValue = (Enum.ToObject(fieldType, Convert.ToInt32(tempValue)) as Enum).FaText();
                            else
                                tempValue = Convert.ChangeType(tempValue, fieldType);
                        }
                        else
                        {
                            var tempType = GetEqualType(param.TitleEn);
                            if (tempType.IsEnumType())
                                tempValue = (tempValue as Enum).FaText();
                            if (tempType.GetUnderlyingType() == typeof(DateTime) && tempValue != null)
                                tempValue = ((DateTime)tempValue).ToPersianDateString();
                        }
                    }
                    var propertyInfo = type.GetProperty(name.Replace('.', '_'));
                    propertyInfo.SetValue(obj, tempValue);
                }
                list.Add(obj);
            }
            return list;
        }

        public Type GetEqualType(IList<ReportParam> reportParams, Type dynamicValueType)
        {
            var list = new List<DynamicProperty>();
            foreach (var param in reportParams)
            {
                Type type = null;
                string name = null;
                if (param.RuleId.HasValue)
                {
                    type = typeof(string);
                    name = "DynamicParam" + param.RuleId.Value;
                }
                else
                {
                    type = GetEqualType(param.TitleEn, false);
                    if (type.IsEnum)
                        type = typeof(string);
                    if (type.GetUnderlyingType() == typeof(DateTime))
                        type = typeof(string);
                    name = GetEqualFieldName(param.TitleEn).Replace('.', '_');
                    switch(param.CompositionMethodType)
                    {
                        case CompositionMethodType.Sum: name = "Sum_" + name; break;
                        case CompositionMethodType.Avg: name = "Avg_" + name;
                            if (type == typeof(int) || type == typeof(long))
                                type = typeof(double);
                            else if (type == typeof(int?) || type == typeof(long?))
                                type = typeof(double?);
                            break;
                        case CompositionMethodType.Max: name = "Max_" + name; break;
                        case CompositionMethodType.Min: name = "Min_" + name; break;
                    }
                }
                list.Add(new DynamicProperty(name, type));
            }
            return DynamicClassFactory.CreateType(list);
        }

        private Type GetEqualType(string field, bool flag = true)
        {
            var info = paramExpr.Type.GetMyProperty(field);
            var type = info.PropertyType;
            if (type.IsNullableType())
                type = Nullable.GetUnderlyingType(type);
            return type;
        }

        private string GetEqualFieldName(string field, CompositionMethodType? methodType = null)
        {
            var tempType = paramExpr.Type.GetMyProperty(field);
            var name = field.Replace('.', '_');;
            if (methodType.HasValue)
            {
                switch(methodType.Value)
                {
                    case CompositionMethodType.Sum: 
                        return "Sum_" + name;
                    case CompositionMethodType.Avg: 
                        return "Avg_" + name;
                    case CompositionMethodType.Max: 
                        return "Max_" + name;
                    case CompositionMethodType.Min: 
                        return "Min_" + name;
                }
            }
            return name;
        }

        private LambdaExpression DynamicItemSelect(Type dynamicItemType, Type dynamicType, string enTitle)
        {
            ParameterExpression param = Expression.Parameter(dynamicItemType, "u");
            var dynamicItemIdExpr = Expression.Property(param, GetDynamicItemIdProperty(dynamicItemType, enTitle));
            var valueExpr = Expression.Property(param, dynamicItemType.GetProperty("Value"));
            var list = new List<MemberBinding>();
            foreach (var info in dynamicType.GetProperties())
            {
                if (info.Name == "Value")
                    list.Add(Expression.Bind(info, valueExpr));
                else if (info.Name == "Text")
                {
                    Expression textExpr = Expression.Property(param, "ProjectParameterValue");
                    textExpr = Expression.Property(textExpr, "Display");
                    list.Add(Expression.Bind(info, textExpr));
                }
                else
                    list.Add(Expression.Bind(info, dynamicItemIdExpr));
            }
            var expr = Expression.MemberInit(Expression.New(dynamicType), list);
            return Expression.Lambda(expr, new ParameterExpression[] { param });
        }

        private Expression DynamicItemSelectExpr(Type dynamicItemType, Type dynamicType, MemberInfo info, string enTitle)
        {
            var type = GetDynamicItemProperty(dynamicItemType, enTitle);
            Expression expr = paramExpr;
            if (enTitle.HasValue())
                expr = Expression.Property(paramExpr, enTitle);
            var expr1 = Expression.MakeMemberAccess(expr, type);
            var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Select").ElementAt(0);
            var expr2 = DynamicItemSelect(dynamicItemType, dynamicType, enTitle);
            method = method.MakeGenericMethod(new Type[] { dynamicItemType, dynamicType });
            return Expression.Call(null, method, new Expression[] { expr1, expr2 });
        }

        private PropertyInfo GetDynamicItemIdProperty(Type dynamicItemType, string enTitle)
        {
            var type = paramExpr.Type;
            if (enTitle.HasValue())
                type = type.GetMyProperty(enTitle).PropertyType;
            PropertyInfo tempInfo = null;
            foreach(var info in dynamicItemType.GetProperties())
            {
                if (info.PropertyType != type)
                {
                    var foreignKey = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKey != null)
                    {
                        tempInfo = dynamicItemType.GetProperty(foreignKey.Name);
                        break;
                    }
                }
            }
            return tempInfo;
        }

        /// <summary>
        /// این متد Property که بصورت مجموعه ای از پارامترهای پویا تعریف شده است را در صورت وجود برمی گرداند.
        /// </summary>
        private PropertyInfo GetDynamicItemProperty(Type dynamicItemType, string enTitle)
        {
            if (dynamicItemType == null)
                return null;
            var type = paramExpr.Type;
            if (enTitle.HasValue())
                type = type.GetMyProperty(enTitle).PropertyType;
            return type.GetProperties().SingleOrDefault(t => t.PropertyType.GenericTypeArguments.Count() > 0 && t.PropertyType.GenericTypeArguments[0] == dynamicItemType);
        }

        /// <summary>
        /// این متد Type پارامترهای پویا را برمی گرداند.
        /// </summary>
        private Type GetDynamicItemType(string enTitle)
        {
            var type = paramExpr.Type;
            if (enTitle.HasValue())
                type = type.GetMyProperty(enTitle).PropertyType;
            var dynamicFieldAttr = type.GetCustomAttribute<DynamicFieldAttribute>();
            if (dynamicFieldAttr == null)
                return null;
            return dynamicFieldAttr.ParameterValueType;
        }

        /// <summary>
        /// این متد 
        /// </summary>
        private Type GetDynamicTypeOfDynamicItem(Type dynamicItemType, string enTitle)
        {
            var dynamicProperty = GetDynamicItemIdProperty(dynamicItemType, enTitle);
            var list = new List<DynamicProperty>();
            list.Add(new DynamicProperty(dynamicProperty.Name, dynamicProperty.PropertyType));
            list.Add(new DynamicProperty("Value", dynamicItemType.GetProperty("Value").PropertyType));
            list.Add(new DynamicProperty("Text", typeof(string)));
            return DynamicClassFactory.CreateType(list);
        }
    }
}
