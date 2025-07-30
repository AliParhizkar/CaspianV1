using Caspian.Common;
using System.Reflection;
using Caspian.Engine.Model;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    public static class ReportEngineExtension
    {
        internal static IQueryable SelectSimpleReport(this IQueryable query, IEnumerable<ReportGroupParameter> parameters)
        {
            var paramExpr = Expression.Parameter(query.ElementType, "t");
            IList<MemberExpression> list = new List<MemberExpression>();
            foreach (var param in parameters)
            {
                MemberExpression memberExpression = paramExpr.CreateMemberExpresion(param.PropertyPath);
                list.Add(memberExpression);
            }
            var lambda = paramExpr.CreateLambdaExpresion(list, true);
            return query.Select(lambda);
        }

        internal static IQueryable CreateGroupByQuery(this IQueryable query, IEnumerable<AggregateReportGroupParameter> parameters)
        {
            var paramExpr = Expression.Parameter(query.ElementType, "t");
            IList<MemberExpression> list = new List<MemberExpression>();
            foreach (var parameter in parameters)
            {
                if (parameter.AggregateParameterType == AggregateParameterType.Grouping)
                {
                    if (parameter.ParentParameterId.HasValue)
                    {
                        ///Group by with Persian Date items
                        parameter.Path = GetDatePath(query.ElementType, parameter);
                    }
                    list.Add(paramExpr.CreateMemberExpresion(parameter.Path));
                }
                else if (parameter.AggregateParameterType != AggregateParameterType.AggregateFunction)
                    list.Add(paramExpr.CreateMemberExpresion(parameter.Path));
            }
            var lambda = paramExpr.CreateLambdaExpresion(list, true);
            return query.GroupBy(lambda).CreateSelectForGroupBy(parameters, paramExpr.Type);
        }

        internal static IQueryable CreateSelectForGroupBy(this IQueryable query, IEnumerable<AggregateReportGroupParameter> parameters, Type mainType)
        {
            var paramExpr = Expression.Parameter(query.ElementType, "t");
            IList<MemberAssignment> list = new List<MemberAssignment>();
            var type = query.ElementType.GetProperty("Key").PropertyType;
            var extendedType = type.ExtendTypeForAggrigateParameter(mainType, parameters.Where(t => t.AggregateParameterType == AggregateParameterType.AggregateFunction));
            foreach (var parameter in parameters)
            {
                if (parameter.AggregateParameterType != AggregateParameterType.AggregateFunction)
                {
                    var path = parameter.Path.NormalizePropertyPath(true);
                    Expression expr = Expression.Property(paramExpr, "Key");
                    expr = Expression.Property(expr, path);
                    var property = extendedType.GetProperty(path);
                    if (mainType.GetMyProperty(parameter.Path).PropertyType.GetUnderlyingType().IsEnum)
                    {
                        var method = typeof(Common.OtherExtension).GetMethod("EnumText");
                        expr = Expression.Convert(expr, typeof(Enum));
                        expr = Expression.Call(null, method, expr);
                    }
                    list.Add(Expression.Bind(property, expr));
                }
                else if (parameter.AggregateParameterType == AggregateParameterType.AggregateFunction)
                {
                    var methodExpr = MethodExpr(paramExpr, mainType, parameter);
                    var path = parameter.ParentParameter.Path.NormalizePropertyPath(true) + parameter.AggregateFunctionType.ToString();
                    var property = extendedType.GetProperty(path);
                    list.Add(Expression.Bind(property, methodExpr));
                }
            }
            var memberInit = Expression.MemberInit(Expression.New(extendedType), list);
            var lambda = Expression.Lambda(memberInit, paramExpr);
            return query.Select(lambda);
        }

        static Type ExtendTypeForAggrigateParameter(this Type type, Type mainType, IEnumerable<AggregateReportGroupParameter> parameters)
        {
            var list = new List<DynamicProperty>();
            foreach (var property in type.GetProperties())
                if (property.IsCollectible)
                {
                    var type1 = property.PropertyType;
                    if (type1.GetUnderlyingType().IsEnum) 
                        type1 = typeof(string);
                    list.Add(new DynamicProperty(property.Name, type1));
                }
            foreach (var parameter in parameters)
            {
                var propertyPath = parameter.ParentParameter.Path;
                var paramType = mainType.GetMyProperty(propertyPath).PropertyType;
                if (parameter.AggregateFunctionType == AggregateFunctionType.Average)
                {
                    if (paramType == typeof(int) || paramType == typeof(long))
                        paramType = typeof(double);
                    if (paramType == typeof(int?) || paramType == typeof(long?))
                        paramType = typeof(double?);
                }
                propertyPath += parameter.AggregateFunctionType.ToString();

                list.Add(new DynamicProperty(propertyPath.NormalizePropertyPath(true), paramType));
            }
            return DynamicClassFactory.CreateType(list, false);
        }

        static Expression MethodExpr(ParameterExpression paramExpr, Type mainType, AggregateReportGroupParameter param)
        {
            var u = Expression.Parameter(mainType, "u");
            Expression expr = u;
            foreach (var str in param.ParentParameter.Path.Split('.'))
                expr = Expression.Property(u, str);
            expr = Expression.Lambda(expr, u);
            var method = GetMethodInfo(mainType, param);
            return Expression.Call(null, method, paramExpr, expr);
        }

        static MethodInfo GetMethodInfo(Type mainType, AggregateReportGroupParameter param)
        {
            var tempType = mainType.GetMyProperty(param.ParentParameter.Path).PropertyType;
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
            foreach (var item in methods)
            {
                var parameters = item.GetParameters();
                Type argumentType = null;
                if (parameters.Length == 2)
                    argumentType = parameters[1].ParameterType.GetGenericArguments().Last();
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

        static string GetDatePath(Type entityType, AggregateReportGroupParameter parameter)
        {
            var path = parameter.ParentParameter.Path;

            var propertyInfo = entityType.GetMyProperty(path);
            propertyInfo = propertyInfo.DeclaringType.GetProperties().Single(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == propertyInfo.Name);
            var index = path.LastIndexOf(".");
            if (index != -1)
                path = path.Substring(0, index);
            return $"{path}.{propertyInfo.Name}.{parameter.Path}";
        }
    }
}
