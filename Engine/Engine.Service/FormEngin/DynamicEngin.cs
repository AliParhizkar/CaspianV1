using System.Reflection;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common;

namespace Caspian.Engine
{
    /// <summary>
    /// این کلاس برای ایجاد <see cref="Expression" فیلدهای پویا مورد استفاده قرار می گیرد/>
    /// </summary>
    public class DynamicFieldExpressionEngin
    {
        private ParameterExpression _ParamExpr;
        private MyContext Context;

        public DynamicFieldExpressionEngin(MyContext context, ParameterExpression paramExpr)
        {
            Context = context;
            _ParamExpr = paramExpr;
        }

        private ControlType GetControlType(string str)
        {
            int i = 0;
            if ( int.TryParse(str, out i))
                return ControlType.Integer;
            double d = 0;
            if (double.TryParse(str, out d))
                return ControlType.Numeric;
            return ControlType.String;
        }

        private Expression GetValueExpression(object value, ExpressionType exprType, Expression valueExpr)
        {
            MethodInfo method = null;
            var controlType = GetControlType(value.ToString());
            if (controlType == ControlType.Integer)
            {
                method = typeof(MyContext).GetMethod("ConvertToInteger");
                value = Convert.ToInt32(value);
            }
            else
                if (controlType == ControlType.Numeric)
                {
                    method = typeof(MyContext).GetMethod("ConvertToDecimal");
                    value = Convert.ToDecimal(value);
                }
            if (controlType == ControlType.String)
            {
                var compareMethod = typeof(string).GetMethod("CompareTo", new Type[] { typeof(string) });
                Expression comapreExpr = Expression.Call(valueExpr, compareMethod, Expression.Constant(value));
                switch(exprType)
                {
                    case ExpressionType.GreaterThanOrEqual:
                        return Expression.GreaterThanOrEqual(comapreExpr, Expression.Constant(0));
                    case ExpressionType.LessThanOrEqual:
                        return Expression.LessThanOrEqual(comapreExpr, Expression.Constant(0));
                    default:
                        throw new NotImplementedException("خطای عدم پیاده سازی");
                }
            }
            else
            {
                Expression contextExpr = Expression.Constant(Context);
                valueExpr = Expression.Call(contextExpr, method, valueExpr);
                valueExpr = Expression.Property(valueExpr, "Value");
                switch (exprType)
                {
                    case ExpressionType.GreaterThanOrEqual:
                        return Expression.GreaterThanOrEqual(valueExpr, Expression.Constant(value));
                    case ExpressionType.LessThanOrEqual:
                        return Expression.LessThanOrEqual(valueExpr, Expression.Constant(value));
                    default:
                        throw new NotImplementedException("خطای عدم پیاده سازی");
                }
            }
        }

        private LambdaExpression CountExpr(IEnumerable<ReportValue> values, PropertyInfo itemIdInfo, PropertyInfo valueInfo)
        {
            ParameterExpression paramExpr = Expression.Parameter(valueInfo.DeclaringType, "u");
            Expression valueExpr = Expression.Property(paramExpr, valueInfo);
            var itemIdExpr = Expression.Property(paramExpr, itemIdInfo);
            Expression leftOrExpr = null;
            foreach (var value in values)
            {
                var expr = Expression.Equal(itemIdExpr, Expression.Constant(value.DynamicItemId.Value));
                if (value.From != null)
                {
                    var fromValueExpr = GetValueExpression(value.From, ExpressionType.GreaterThanOrEqual, valueExpr);
                    expr = Expression.And(expr, fromValueExpr);
                }
                if (value.To != null)
                {
                    var toValueExpr = GetValueExpression(value.To, ExpressionType.LessThanOrEqual, valueExpr);
                    expr = Expression.And(expr, toValueExpr);
                }
                if (value.Values != null && value.Values.Count() > 0)
                {
                    var tempExpr = Expression.Property(paramExpr, "Value");
                    var obj = new
                    {
                        Values = value.Values.Select(t => t.ToString())
                    };
                    var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Contains").ElementAt(0);
                    var field = obj.GetType().GetMember("Values")[0];
                    method = method.MakeGenericMethod(new Type[] { typeof(string) });
                    var constant = Expression.Constant(obj);
                    Expression containExpr = Expression.MakeMemberAccess(constant, field);
                    containExpr = Expression.Call(null, method, new Expression[] { containExpr, tempExpr });
                    expr = Expression.And(expr, containExpr);
                }
                if (leftOrExpr == null)
                    leftOrExpr = expr;
                else
                    leftOrExpr = Expression.Or(leftOrExpr, expr);
            }
            return Expression.Lambda(leftOrExpr, paramExpr);
        }

        private Expression WhereExpr(IEnumerable<ReportValue> values, PropertyInfo itemsInfo, PropertyInfo itemIdInfo,
            PropertyInfo valueInfo)
        {
            var name = values.FirstOrDefault().EnTitle;
            Expression itemsExpr = _ParamExpr;
            if (name.HasValue())
                itemsExpr = Expression.Property(itemsExpr, name);
            itemsExpr = Expression.Property(itemsExpr, itemsInfo);
            var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Count").ElementAt(1);
            method = method.MakeGenericMethod(new Type[] { itemIdInfo.DeclaringType });
            var countExpr = CountExpr(values, itemIdInfo, valueInfo);
            var callExpr = Expression.Call(null, method, new Expression[] { itemsExpr, countExpr });
            return Expression.Equal(callExpr, Expression.Constant(values.Count()));
        }

        public Expression CreateDynamicExpr(IEnumerable<ReportValue> values, Type parentType)
        {
            var enTitle = values.First(t => t.DynamicItemId.HasValue).EnTitle;
            if (enTitle.HasValue())
                parentType = parentType.GetMyProperty(enTitle).PropertyType;
            var otherType = parentType.GetCustomAttribute<DynamicFieldAttribute>().OtherType;
            var parameterType = typeof(DynamicParameter);
            var name = otherType.GetProperties().Single(t => t.PropertyType == parameterType)
                .GetCustomAttribute<ForeignKeyAttribute>().Name;
            var itemsInfo = parentType.GetProperties().Single(t => t.PropertyType.IsGenericType &&
                t.PropertyType.GenericTypeArguments[0] == otherType);
            var valueInfo = otherType.GetProperty("Value");
            var itemIdInfo = otherType.GetProperty(name);
            return WhereExpr(values, itemsInfo, itemIdInfo, valueInfo);
        }
    }
}
