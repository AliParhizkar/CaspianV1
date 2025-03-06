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

        public LambdaExpression GroupBy(Type mainType, IList<AggregateReportGroupParameter> parameters)
        {
            var paramExpr = Expression.Parameter(mainType, "t");
            IList<MemberExpression> list = new List<MemberExpression>();
            foreach (var parameter in parameters)
            {
                if (parameter.AggregateParameterType == AggregateParameterType.Grouping)
                {
                    if (parameter.ParentParameterId.HasValue)
                    {
                        ///Group by with Persian Date items
                        parameter.Path = GetDatePath(mainType, parameter);
                    }
                    list.Add(paramExpr.CreateMemberExpresion(parameter.Path));
                }
            }
            return paramExpr.CreateLambdaExpresion(list);
        }

        string GetDatePath(Type entityType, AggregateReportGroupParameter parameter)
        {
            var path = parameter.ParentParameter.Path;

            var propertyInfo = entityType.GetMyProperty(path);
            propertyInfo = propertyInfo.DeclaringType.GetProperties().Single(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == propertyInfo.Name);
            var index = path.LastIndexOf(".");
            if (index != -1)
                path = path.Substring(0, index);
            return $"{path}.{propertyInfo.Name}.{parameter.Path}";
        }

        public LambdaExpression SelectForGroupBy(Type mainType, Type type)
        {
            throw new NotImplementedException();
        }

        Type GetTSourceType(Type mainType, IList<ReportParam> reportParams)
        {
            var list = new List<DynamicProperty>();
            foreach (var param in reportParams)
            {
                var info = mainType.GetMyProperty(param.ReportGroupParameter.PropertyPath);
                var type = info.PropertyType;
                
                var name = param.ReportGroupParameter.PropertyPath.Replace('.', '_');
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
                //if (info.DeclaringType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                //{
                //    var attr = info.GetCustomAttribute<ReportFieldAttribute>();
                //    int startIndex = attr.StartIndex, length = attr.Length;
                //    info = info.DeclaringType.GetProperties().Single(t => t.CanWrite);
                //    expr = Expression.Property(expr, info);
                //    var method = typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
                //    expr = Expression.Call(expr, method, Expression.Constant(startIndex), Expression.Constant(length));
                //}
                //else
                    expr = Expression.Property(expr, info);
                type = info.PropertyType;
            }
            return expr;
        }

        MethodInfo GetMethodInfo(Type mainType, AggregateReportGroupParameter param)
        {
            var tempType  = mainType.GetMyProperty(param.Path).PropertyType;
            var type = tempType;
            if (param.AggregateFunctionType == AggregateFunctionType.Average)
            {
                if (type == typeof(int) || type == typeof(long))
                    type = typeof(double);
                if (type == typeof(int?) || type == typeof(long?))
                    type = typeof(double?);
            }
            string methodName = null;
            switch (param.AggregateFunctionType.Value)
            {
                case AggregateFunctionType.Sum:
                    methodName = "Sum"; break;
                case AggregateFunctionType.Average:
                    methodName = "Average"; break;
                case AggregateFunctionType.Maximum:
                    methodName = "Max"; break;
                case AggregateFunctionType.Minimum:
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
            foreach (var str in param.ReportGroupParameter.PropertyPath.Split('.'))
                expr = Expression.Property(u, str);
            expr = Expression.Lambda(expr, u);
            var method = GetMethodInfo(mainType, null);
            return Expression.Call(null, method, parameter, expr);
        }

        public IList GetValues(IQueryable values, IList<ReportParam> reportParams, Type type)
        {
            IList list = new ArrayList();
            var flag = reportParams.Any(t => t.RuleId.HasValue);
            var result = values.ToDynamicList();
            foreach (var value in values.AsQueryable().ToIList())
            {
                IEnumerable<object> dynamicItemsValue = new List<object>();
                var obj = Activator.CreateInstance(type);
                foreach (var param in reportParams)
                {
                    string name = null;
                    object tempValue = null;
                    if (param.RuleId.HasValue)
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
                            }
                        }
                    }
                    else
                    {
                        name = GetEqualFieldName(param.ReportGroupParameter.PropertyPath);
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
                        var propertyInfo = type.GetProperty(name.Replace(".", ""));
                        propertyInfo.SetValue(obj, tempValue);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public Type GetStimuType(Type mainType, IList<ReportParam> reportParams, int level)
        {
            var list = new List<DynamicProperty>();
            foreach (var param in reportParams.Where(t => t.DataLevel == level))
            {
                var name = param.ReportGroupParameter.PropertyPath.Replace(".", "");
                var type = mainType.GetProperty(name).PropertyType;
                if (type.GetUnderlyingType().IsEnum)
                    type = typeof(string);
                list.Add(new DynamicProperty(name, type));
            }
            if (level > 1)
            {
                var type = GetStimuType(mainType, reportParams, level - 1);
                type = typeof(IList<>).MakeGenericType(type);
                list.Add(new DynamicProperty("__Details", type));
            }
            return DynamicClassFactory.CreateType(list);
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
            //using var context = new Context();
            //var expr = context.DynamicParametersValues.Select(u => new
            //{
            //    u.DynamicParameterId,
            //    u.RuleId,
            //    OptionTitle = u.DynamicParameterOption.FaTitle,
            //    u.Value,
            //    Text = u.RuleId.HasValue ? u.Rule.Title : u.DynamicParameter.Title
            //}).Expression;
            //expr = (expr as MethodCallExpression).Arguments[1];
            //return (expr as UnaryExpression).Operand as LambdaExpression;
            throw new NotImplementedException();
        }

        private Expression DynamicItemSelectExpr(Type dynamicItemType, string enTitle)
        {
            //var type = GetDynamicItemProperty(dynamicItemType, enTitle);
            //Expression expr = paramExpr;
            //if (enTitle.HasValue())
            //    expr = Expression.Property(paramExpr, enTitle);
            //Expression expr1 = Expression.MakeMemberAccess(expr, type);
            //var oftypeMethod = typeof(Enumerable).GetMethod("OfType").MakeGenericMethod(typeof(DynamicParameterValue));
            //expr1 = Expression.Call(null, oftypeMethod, new Expression[] { expr1 });
            //var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Select").ElementAt(0);
            //var expr2 = DynamicItemSelect();
            //method = method.MakeGenericMethod(new Type[] { typeof(DynamicParameterValue), expr2.Body.Type });
            //return Expression.Call(null, method, new Expression[] { expr1, expr2 });
            throw new NotImplementedException();
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

    public class ReportLevelData
    {
        public int? Key { get; set; }

        public object Master { get; set; }

        public IList Values { get; set; }

        public IList<ReportLevelData> Details { get; set; }
    }
}
