using Caspian.Common;
using System.Reflection;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    public class GenrateWhereExpression
    {
        private ParameterExpression paramExpr = null;
        private MyContext Context = null;


        public GenrateWhereExpression(MyContext context, Type type)
        {
            Context = context; 
            paramExpr = Expression.Parameter(type, "t");
        }

        public LambdaExpression WhereExpression(IEnumerable<ReportValue> values)
        {
            Expression expr = null;
            var list = values.Where(t => t.DynamicItemId.HasValue);
            if (list.Count() > 0)
                expr = new DynamicFieldExpressionEngin(Context, paramExpr).CreateDynamicExpr(list, paramExpr.Type);
            foreach (var value in values.Where(t => !t.DynamicItemId.HasValue))
            {
                if (expr == null)
                    expr = WhereExpression(value);
                else
                    expr = Expression.And(expr, WhereExpression(value));
            }
            return Expression.Lambda(expr, new ParameterExpression[] { paramExpr });
        }

        private Expression CallWhereExpr(Expression expr)
        {
            var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Where").ElementAt(0);
            method = method.MakeGenericMethod(new Type[] { paramExpr.Type });
            return Expression.Call(null, method, new Expression[] { expr });
        }

        private Expression WhereExpression(ReportValue value)
        {
            if (Convert.ToString(value.From).HasValue() || Convert.ToString(value.To).HasValue())
                return FromToExpr(value);

            return ContainExpression(value);
        }

        private object ConvertToEnum(Type type, IEnumerable<int?> values)
        {
            var array = Array.CreateInstance(type, values.Count());
            var tempType = type;
            if (tempType.IsNullableType())
                tempType = Nullable.GetUnderlyingType(tempType);
            var fields = tempType.GetFields().Where(t => !t.IsSpecialName && values.Contains((int)t.GetValue(100)));
            for (int i = 0; i < values.Count(); i++)
            {
                var fieldName = fields.Single(t => (int)t.GetValue(100) == values.ElementAt(i)).Name;
                var value = tempType.GetField(fieldName).GetValue(100);
                array.SetValue(value, i);
            }
            IList<DynamicProperty> dpList = new List<DynamicProperty>();
            dpList.Add(new DynamicProperty("Values", type.MakeArrayType()));
            var newType = DynamicClassFactory.CreateType(dpList);
            var obj = Activator.CreateInstance(newType);
            newType.GetProperty("Values").SetValue(obj, array);
            return obj;
        }

        private object ConvertToString(Type type, IEnumerable<int?> values, int length)
        {
            var array = new string[values.Count()];
            for (int i = 0; i < values.Count(); i++)
            {
                var temp = values.ElementAt(i).ToString();
                while (temp.Length < length)
                    temp = '0' + temp;
                array[i] = temp;
            }
            return new
            {
                Values = array
            };
        }

        private Expression ContainExpression(ReportValue value)
        {
            int length;
            var propertyExpr = PropertyExpression(value.EnTitle, out length);
            object obj = GetObject(value.Values, propertyExpr.Type, length);
            var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Contains").ElementAt(0);
            var field = obj.GetType().GetMember("Values")[0];
            method = method.MakeGenericMethod(new Type[] { propertyExpr.Type });
            var constant = Expression.Constant(obj);
            var expr = Expression.MakeMemberAccess(constant, field);
            return Expression.Call(null, method, new Expression[] { expr, propertyExpr });
        }

        private object GetObject(IEnumerable<int?> values, Type type, int length)
        {
            if (length > 0)
                return ConvertToString(type, values, length);
            var tempType = type;
            if (tempType.IsNullableType())
                tempType = Nullable.GetUnderlyingType(tempType);
            if (tempType.IsEnum)
                return ConvertToEnum(type, values);
            if (type.IsNullableType())
            {
                return new
                {
                    Values = values
                };
            }
            else
            {
                var temp = values.Where(t => t.HasValue).Select(t => t.Value);
                return new
                {
                    Values = temp
                };
            }
        }

        /// <summary>
        /// این متد برای ساخت Expression برای حالت از تا مورد استفاده قرار می گیرد
        /// </summary>
        /// <param name="value">پارامتر حاوی از تا</param>
        /// <returns>Expression از تا</returns>
        private Expression FromToExpr(ReportValue value)
        {
            int length;
            var expr = PropertyExpression(value.EnTitle, out length);
            Expression fromExpr = null, toExpr = null; ;
            var tempType = expr.Type;
            if (tempType.IsNullableType())
            {
                expr = Expression.Property(expr, "Value");
                tempType = Nullable.GetUnderlyingType(tempType);
            }
            if (Convert.ToString(value.From).HasValue())
            {
                if (expr.Type == typeof(string))
                {
                    var valueStr = value.From.ToString();
                    while (valueStr.Length < length)
                        valueStr = '0' + valueStr;
                    var constExpr = Expression.Constant(valueStr);
                    fromExpr = Expression.Call(expr, typeof(string).GetMethod("CompareTo", new Type[] { typeof(string) }), new Expression[] { constExpr });
                    fromExpr = Expression.GreaterThanOrEqual(fromExpr, Expression.Constant(0));
                }
                else
                {
                    var val = Convert.ChangeType(value.From, tempType);
                    fromExpr = Expression.GreaterThanOrEqual(expr, Expression.Constant(val));
                }
            }
            if (Convert.ToString(value.To).HasValue())
            {
                if (expr.Type == typeof(string))
                {
                    var valueStr = value.To.ToString();
                    while (valueStr.Length < length)
                        valueStr = '0' + valueStr;
                    var constExpr = Expression.Constant(valueStr);
                    toExpr = Expression.Call(expr, typeof(string).GetMethod("CompareTo", new Type[] { typeof(string) }), new Expression[] { constExpr });
                    toExpr = Expression.LessThanOrEqual(toExpr, Expression.Constant(0));
                }
                else
                {
                    var val = Convert.ChangeType(value.To, tempType);
                    toExpr = Expression.LessThanOrEqual(expr, Expression.Constant(val));
                }
            }
            if (fromExpr != null && toExpr != null)
                return Expression.And(fromExpr, toExpr);
            if (fromExpr != null)
                return fromExpr;
            return toExpr;
        }

        private Expression CallCompareToExpr(Expression expr, object value, CompareToType compareType)
        {
            var method = typeof(string).GetMethod("CompareTo", new Type[] { typeof(object) });
            var compare = Expression.Call(expr, method, new Expression[] { Expression.Constant(value) });
            if (compareType == CompareToType.GTEZero)
                return Expression.GreaterThanOrEqual(compare, Expression.Constant(0));
            return Expression.LessThanOrEqual(compare, Expression.Constant(0));
        }

        /// <summary>
        /// این متد با توجه به رشته ی ورودی یک Expression تولید می کند
        /// </summary>
        /// <param name="str">رشته ورودی بصورت Order.Customer.FName</param>
        /// <param name="length"></param>
        /// <returns></returns>
        private Expression PropertyExpression(string str, out int length)
        {
            Expression expr = paramExpr;
            var tempType = paramExpr.Type;
            var startIndex = 0;
            length = 0;
            foreach (var item in str.Split('.'))
            {
                var PInfo = tempType.GetProperty(item);
                tempType = PInfo.PropertyType;
                expr = Expression.Property(expr, PInfo);
                ///if type of Property has complex type attribute 
                if (PInfo.PropertyType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                {
                    ///property of complex type that has set method(only a property is set)
                    var complexTypeInfo = PInfo.PropertyType.GetProperties().Single(t => t.SetMethod != null);
                    expr = Expression.Property(expr, complexTypeInfo);
                    var strName = str.Split('.').Last();
                    var reportField = PInfo.PropertyType.GetProperty(strName).GetCustomAttribute<ReportFieldAttribute>();
                    startIndex = reportField.StartIndex;
                    length = reportField.Length;
                    if (length > 0)
                        expr = CallSubString(expr, startIndex, length);
                    break;
                }
            }
            return expr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private Expression CallSubString(Expression expr, int start, int length)
        {
            var method = typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
            var startExpr = Expression.Constant(start);
            var lengthExpr = Expression.Constant(length);
            return Expression.Call(expr, method, new Expression[] { startExpr, lengthExpr });
        }
    }
}
