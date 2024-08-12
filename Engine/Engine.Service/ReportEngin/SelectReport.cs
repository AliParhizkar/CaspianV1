using Caspian.Common;
using System.Reflection;
using System.Collections;
using Caspian.Engine.Model;
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
        }

        IEnumerable<ReportParam> ComplexTypeFilter(IEnumerable<ReportParam> fields)
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

        public LambdaExpression SimpleSelect(IList<ReportParam> reportParams, out Type dynamicType)
        {
            var flag = reportParams.Any(t => t.RuleId.HasValue || t.DynamicParameterId.HasValue);
            var fields = ComplexTypeFilter(reportParams.Where(t => t.RuleId == null && t.DynamicParameterId == null)).ToArray();
            var dynamicFields = reportParams.Where(t => t.RuleId.HasValue || t.DynamicParameterId.HasValue);
            Type dynamicItemType = null, dynamicTypeOfDynamicItem = null;
            if (flag)
            {
                var enTitle = dynamicFields.First().TitleEn;
                dynamicItemType = GetDynamicItemType(enTitle);
                dynamicTypeOfDynamicItem = GetDynamicTypeOfDynamicItem(enTitle);
            }
            dynamicType = GetDynamicType(fields, dynamicTypeOfDynamicItem);
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
                var titleEn = dynamicFields.First().TitleEn;
                var tempExpr = DynamicItemSelectExpr(dynamicItemType, titleEn);
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
            if (type.IsValueType && !type.IsNullableType())
            {
                bool canbeNull = false;
                var tempType = paramExpr.Type;
                foreach (var item in field.Split("."))
                {
                    var info = tempType.GetProperty(item);
                    var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (attr != null)
                    {
                        var propertyType = tempType.GetProperty(attr.Name).PropertyType;
                        if (propertyType.IsNullableType())
                        { 
                            canbeNull = true; 
                            break;
                        }
                    }
                    tempType = info.PropertyType;
                }
                if (canbeNull)
                    expr = Expression.Convert(expr, typeof(Nullable<>).MakeGenericType(type));
            }
            return expr;
        }

        public Type GetDynamicType(IEnumerable<ReportParam> fields, Type dynamicTypeOfDynamicItem)
        {
            var list = new List<DynamicProperty>();
            foreach (var field in fields)
            {
                Type tempType = null;
                string name = GetEqualFieldName(field.TitleEn, field.CompositionMethodType);
                var info = paramExpr.Type.GetMyProperty(field.TitleEn);
                if (info.DeclaringType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                    tempType = typeof(string);
                else
                    tempType = info.PropertyType;
                if (tempType.IsValueType && !tempType.IsNullableType() && TypeMaybeWasNull(paramExpr.Type, name))
                    tempType = typeof(Nullable<>).MakeGenericType(tempType);
                list.Add(new DynamicProperty(name.Replace(".", ""), tempType));
            }
            if (dynamicTypeOfDynamicItem != null)
            {
                var type = typeof(IEnumerable<>);
                type = type.MakeGenericType(new Type[] { dynamicTypeOfDynamicItem });
                list.Add(new DynamicProperty("DynamicItems", type));
            }
            return DynamicClassFactory.CreateType(list, false);
        }

        bool TypeMaybeWasNull(Type type, string path)
        {
            var tempType = type;
            foreach(var section in path.Split('.'))
            {
                var property = tempType.GetProperty(section);
                var attr = property.GetCustomAttribute<ForeignKeyAttribute>();
                if (attr != null)
                {
                    var info = tempType.GetProperty(attr.Name);
                    if (info.PropertyType.IsNullableType())
                        return true;
                }
                tempType = property.PropertyType;
            }
            return false;
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

        Type GetTSourceType(Type mainType, IList<ReportParam> reportParams)
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
            return DynamicClassFactory.CreateType(list, false);
        }

        Expression PropertyExpr(Expression parameter, string path)
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

        MethodInfo GetMethodInfo(Type mainType, ReportParam param)
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

        Expression MethodExpr(Type mainType, Expression parameter, ReportParam param)
        {
            var u = Expression.Parameter(mainType, "u");
            Expression expr = u;
            foreach (var str in param.TitleEn.Split('.'))
                expr = Expression.Property(u, str);
            expr = Expression.Lambda(expr, u);
            var method = GetMethodInfo(mainType, param);
            return Expression.Call(null, method, parameter, expr);
        }

        public IList GetValues(IQueryable values, IList<ReportParam> reportParams, Type type)
        {
            IList list = new ArrayList();
            var flag = reportParams.Any(t => t.RuleId.HasValue || t.DynamicParameterId.HasValue);
            var result = values.ToDynamicList();
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
                    if (param.RuleId.HasValue || param.DynamicParameterId.HasValue)
                    {
                        foreach (var dynamicItem in dynamicItemsValue)
                        {
                            if (param.RuleId.HasValue)
                            {
                                if (Convert.ToInt32(dynamicItem.GetMyValue("Rule")) == param.RuleId.Value)
                                {
                                    //var text = Convert.ToString(dynamicItem.GetMyValue("Text"));
                                    //if (text.HasValue())
                                    //    tempValue = text;
                                    //else
                                    tempValue = dynamicItem.GetMyValue("Value");
                                    name = "DynamicParam" + param.RuleId.Value;
                                }
                                
                            }
                            else
                            {
                                if (Convert.ToInt32(dynamicItem.GetMyValue("DynamicParameterId")) == param.DynamicParameterId.Value)
                                {
                                    //var text = Convert.ToString(dynamicItem.GetMyValue("Text"));
                                    //if (text.HasValue())
                                    //    tempValue = text;
                                    //else
                                    var optionTitle = dynamicItem.GetMyValue("OptionTitle");
                                    if (optionTitle == null)
                                    {
                                        var decimalValue = dynamicItem.GetMyValue("Value");
                                        if (decimalValue != null)
                                        {
                                            if (param.DynamicParameter.ControlType == ControlType.CheckBox)
                                            {
                                                if (Convert.ToDecimal(decimalValue) == 0)
                                                    tempValue = "خیر";
                                                else
                                                    tempValue = "بله";
                                            }
                                            else
                                                tempValue = Convert.ToDecimal(decimalValue).Seprate3Digit();
                                        }
                                    }
                                    else
                                        tempValue = optionTitle ?? dynamicItem.GetMyValue("Value");
                                    name = "DynamicParam" + param.DynamicParameterId.Value;
                                }
                            }
                        }
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
                        tempValue = type.GetProperty(name.Replace(".", "")).GetValue(value);

                    }
                    if (name.HasValue())
                    {
                        var propertyInfo = type.GetProperty(name);
                        propertyInfo.SetValue(obj, tempValue);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        IList GetStimulData(IList firstLevelSource, Type mainType, Type type)
        {
            var newList = new ArrayList(firstLevelSource.Count);
            foreach (var item in firstLevelSource)
            {
                var newItem = Activator.CreateInstance(type);
                foreach (var property in mainType.GetProperties().Where(t => t.IsCollectible))
                {
                    object value = null;
                    if (property.PropertyType.GetUnderlyingType().IsEnum)
                        value = (property.GetValue(item) as Enum).EnumText();
                    else
                        value = property.GetValue(item);
                    type.GetProperty(property.Name).SetValue(newItem, value);
                }
                newList.Add(newItem);
            }
            return newList;
        }

        object GetObjectFromData(Type type, IList list)
        {
            var item = Activator.CreateInstance(type);
            var old = list[0];
            var oldType = old.GetType();
            foreach(var info in type.GetProperties().Where(t => t.IsCollectible))
            {
                var tempType = info.PropertyType;
                if(info.Name == "Details" && tempType.IsCollectionType() && tempType != typeof(string) && tempType != typeof(byte[]))
                {
                    var detailsType = tempType.GenericTypeArguments[0];
                    var details = Activator.CreateInstance(typeof(List<>).MakeGenericType(detailsType)) as IList;
                    
                    foreach(var oldDeatil in list)
                    {
                        var detail = Activator.CreateInstance(detailsType);
                        foreach (var info1 in detailsType.GetProperties().Where(t => t.IsCollectible))
                        {
                            var tempInfo = oldType.GetProperty(info1.Name);
                            var value = oldType.GetProperty(info1.Name).GetValue(oldDeatil);
                            if (tempInfo.PropertyType.GetUnderlyingType().IsEnum)
                                value = (value as Enum).EnumText();
                            info1.SetValue(detail, value);
                        }
                        details.Add(detail);
                    }
                    info.SetValue(item, details);
                }
                else
                {
                    var tempInfo = oldType.GetProperty(info.Name);
                    var value = oldType.GetProperty(info.Name).GetValue(old);
                    if (tempInfo.PropertyType.GetUnderlyingType().IsEnum)
                        value = (value as Enum).EnumText();
                    info.SetValue(item, value);
                }
            }
            return item;
        }

        object GetObjectFromData(Type type, IList list, string keyName)
        {
            var item = Activator.CreateInstance(type);
            var old = list[0];
            var oldType = old.GetType();
            foreach (var info in type.GetProperties().Where(t => t.IsCollectible))
            {

            }
            return null;
        }

        public IList GetStumlData(IList source, IList<ReportParam> reportParams, Type mainType, int level)
        {
            Type type = GetStimuType(mainType, reportParams, level);
            if (level == 1)
                return GetStimulData(source, mainType, type);
            var secondLevelSource = GetDataOfLevel(source, reportParams, mainType, level);
            if (level == 2)
            {
                IList list = new ArrayList();
                foreach (var  dic in secondLevelSource)
                {
                    var value = GetObjectFromData(type, dic.Value);
                    list.Add(value);
                }
                return list;
            }
            foreach(var dic in secondLevelSource)
            {
                GetObjectFromData(type, dic.Value, "");
            }
            throw new NotImplementedException();

        }

        Dictionary<int?, IList> GetDataOfLevel(IList source,IList<ReportParam> reportParams, Type type, int level)
        {
            var keyparam = reportParams.Single(t => t.DataLevel == level && t.IsKey);
            var keyInfo = type.GetProperty(keyparam.TitleEn.Replace(".", ""));
            Dictionary<int?, IList> dictionary = new Dictionary<int?, IList>();
            dictionary.Add(null, source);
            foreach (var item in source)
            {
                var keyvalue = (int?)keyInfo.GetValue(item);
                if (keyvalue != null && dictionary.ContainsKey(keyvalue))
                {
                    var list = dictionary[keyvalue];
                    list.Add(item);
                }
                else
                {
                    var list = new ArrayList
                    {
                        item
                    };
                    dictionary.Add(keyvalue, list);       
                }
            }
            return dictionary;
        }

        public Type GetStimuType(Type mainType, IList<ReportParam> reportParams, int level)
        {
            var list = new List<DynamicProperty>();
            foreach (var param in reportParams.Where(t => t.DataLevel == level))
            {
                var name = param.TitleEn.Replace(".", "");
                var type = mainType.GetProperty(name).PropertyType.GetUnderlyingType();
                if (type.IsEnum)
                    type = typeof(string);
                list.Add(new DynamicProperty(name, type));
            }
            if (level > 1)
            {
                var type = GetStimuType(mainType, reportParams, level - 1);
                type = typeof(IList<>).MakeGenericType(type);
                list.Add(new DynamicProperty("Details", type));
            }
            return DynamicClassFactory.CreateType(list);

        }

        public Type GetEqualType(IList<ReportParam> reportParams)
        {
            var list = new List<DynamicProperty>();
            foreach (var param in reportParams)
            {
                Type type = null;
                string name = null;
                if (param.RuleId.HasValue || param.DynamicParameterId.HasValue)
                {
                    type = typeof(string);
                    name = "DynamicParam" + (param.RuleId ?? param.DynamicParameterId.Value);
                }
                else
                {
                    type = GetEqualType(param.TitleEn, false);
                    if (type.IsEnum)
                        type = typeof(string);
                    name = GetEqualFieldName(param.TitleEn);
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
            return DynamicClassFactory.CreateType(list, false);
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
            if (methodType.HasValue)
            {
                switch(methodType.Value)
                {
                    case CompositionMethodType.Sum: 
                        return "Sum_" + field;
                    case CompositionMethodType.Avg: 
                        return "Avg_" + field;
                    case CompositionMethodType.Max: 
                        return "Max_" + field;
                    case CompositionMethodType.Min: 
                        return "Min_" + field;
                }
            }
            return field;
        }

        public LambdaExpression DynamicItemSelect()
        {
            using var context = new Context();
            var expr = context.DynamicParametersValues.Select(u => new
            {
                u.DynamicParameterId,
                u.RuleId,
                OptionTitle = u.DynamicParameterOption.FaTitle,
                u.Value,
                Text = u.RuleId.HasValue ? u.Rule.Title : u.DynamicParameter.Title
            }).Expression;
            expr = (expr as MethodCallExpression).Arguments[1];
            return (expr as UnaryExpression).Operand as LambdaExpression;
        }

        private Expression DynamicItemSelectExpr(Type dynamicItemType, string enTitle)
        {
            var type = GetDynamicItemProperty(dynamicItemType, enTitle);
            Expression expr = paramExpr;
            if (enTitle.HasValue())
                expr = Expression.Property(paramExpr, enTitle);
            Expression expr1 = Expression.MakeMemberAccess(expr, type);
            var oftypeMethod = typeof(Enumerable).GetMethod("OfType").MakeGenericMethod(typeof(DynamicParameterValue));
            expr1 = Expression.Call(null, oftypeMethod, new Expression[] { expr1 });
            var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Select").ElementAt(0);
            var expr2 = DynamicItemSelect();
            method = method.MakeGenericMethod(new Type[] { typeof(DynamicParameterValue), expr2.Body.Type });
            return Expression.Call(null, method, new Expression[] { expr1, expr2 });
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
        /// This method return the type inherite from DynamicParameterValue
        /// </summary>
        private Type GetDynamicItemType(string enTitle)
        {
            var type = paramExpr.Type;
            if (enTitle.HasValue())
                type = type.GetMyProperty(enTitle).PropertyType;
            var dynamicTypeAttr = type.GetCustomAttribute<DynamicTypeAttribute>();
            if (dynamicTypeAttr == null)
                return null;
            return dynamicTypeAttr.Type;
        }

        /// <summary>
        /// این متد 
        /// </summary>
        private Type GetDynamicTypeOfDynamicItem(string enTitle)
        {
            if (enTitle.HasValue())
                throw new NotImplementedException("خطای عدم پیاده سازی");
            return DynamicItemSelect().Body.Type;
        }
    }
}
