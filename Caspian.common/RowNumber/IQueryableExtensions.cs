using System.Reflection;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Common.RowNumber
{
    public static class IQueryableExtensions
    {
        public static Type CreateType()
        {
            var properties = new List<DynamicProperty>();
            properties.Add(new DynamicProperty("Id", typeof(int)));
            properties.Add(new DynamicProperty("RowNumber", typeof(long)));
            return DynamicClassFactory.CreateType(properties);
        }

        private static IQueryable CreateSelectManyExpr(this IQueryable source)
        {
            var t = Expression.Parameter(source.ElementType, "t");
            var u = Expression.Parameter(source.ElementType, "u");
            bool orderbyIsEmpty = false;
            var rowNumberExpr = CreateRowNumberExpression(source, u, out orderbyIsEmpty);
            Expression exprId = Expression.Property(u, source.ElementType.GetPrimaryKey());
            if (orderbyIsEmpty)
            {
                source = source.OrderBy(Expression.Lambda(exprId, u));
            }
            var type = CreateType();
            var memberExprList = new List<MemberAssignment>();
            memberExprList.Add(Expression.Bind(type.GetProperty("Id"), exprId));
            memberExprList.Add(Expression.Bind(type.GetProperty("RowNumber"), rowNumberExpr));
            var memberInit = Expression.MemberInit(Expression.New(type), memberExprList);
            var innerLambda = Expression.Lambda(memberInit, u);
            var body = source.Select(innerLambda).Expression;
            Type inputType = source.Expression.Type.GetGenericArguments()[0];
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(type);
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            var lambda = Expression.Lambda(delegateType, body, t);
            var result = source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "SelectMany",
                new Type[] { source.ElementType, type },
                source.Expression,
                Expression.Quote(lambda)));
            return result;
        }

        private static Expression CreateRowNumberExpression(this IQueryable query, ParameterExpression innerParameter, out bool orderbyIsEmpty)
        {
            var OrderByExprlist = new List<Expression>();
            var desc = new List<bool>();
            GetOrderbyExpression(query.Expression, OrderByExprlist, desc);
            orderbyIsEmpty = desc.Count == 0;
            if (desc.Count == 0)
            {
                desc.Add(true);
                OrderByExprlist.Add(Expression.Property(innerParameter, query.ElementType.GetPrimaryKey()));
            }

            var method = typeof(DbFunctionsExtensions).GetMethods().Where(t => t.Name == "RowNumber")
                .ToArray()[desc.Count - 1];
            var args = new List<Expression>();
            args.Add(Expression.Property(null, typeof(EF).GetProperty("Functions")));
            var type = query.ElementType;
            OrderByExprlist.Reverse();
            foreach (var param in OrderByExprlist)
            {
                Expression memberExpr = innerParameter.ReplaceParameter(param as MemberExpression);
                if (((memberExpr as MemberExpression).Member as PropertyInfo).PropertyType != typeof(string))
                    memberExpr = Expression.Convert(memberExpr, typeof(object));
                args.Add(memberExpr);
            }
            args.Add(Expression.Constant(desc.ToArray()));
            var rowNumberExpr = Expression.Call(null, method, args);
            return rowNumberExpr;
        }

        public async static Task<int?> GetRowNumber(this IQueryable query, int id)
        {
            var selectManyQuery = CreateSelectManyExpr(query);
            var param = Expression.Parameter(selectManyQuery.ElementType, "t");
            Expression whereExpr = Expression.Property(param, "Id");
            whereExpr = Expression.Equal(whereExpr, Expression.Constant(id));
            whereExpr = Expression.Lambda(whereExpr, param);
            dynamic rowNumber = (await selectManyQuery.Where_(whereExpr).Take(1).ToDynamicListAsync()).FirstOrDefault();
            if (rowNumber == null)
                return null;
            return Convert.ToInt32(rowNumber.RowNumber);
        }

        private static void GetOrderbyExpression(Expression expr, IList<Expression> OrderByExprlist, IList<bool> desc)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Call:
                    var callExpr = (expr as MethodCallExpression);
                    var name = callExpr.Method.Name;
                    if (name == "OrderBy" || name == "ThenBy")
                    {
                        var updateExpr = UpdateOrderbyExpr(callExpr.Arguments[1]);
                        OrderByExprlist.Add(updateExpr);
                        desc.Add(true);
                    }
                    if (name == "OrderByDescending" || name == "ThenByDescending")
                    {
                        var updateExpr = UpdateOrderbyExpr(callExpr.Arguments[1]);
                        OrderByExprlist.Add(updateExpr);
                        desc.Add(false);
                    }
                    GetOrderbyExpression(callExpr.Arguments[0], OrderByExprlist, desc);
                    break;
                case ExpressionType.Constant:
                case ExpressionType.Extension:
                    break;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }

        private static Expression UpdateOrderbyExpr(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    var body = (expression as LambdaExpression).Body;
                    return UpdateOrderbyExpr(body);
                case ExpressionType.MemberAccess:
                    return expression;
                case ExpressionType.Quote:
                    var unaryExpr = (expression as UnaryExpression).Operand;
                    return UpdateOrderbyExpr(unaryExpr);
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }
    }

    public class CaspianRowNumber
    {
        public int Id { get; set; }

        public long RowNumber { get; set; }
    }
}
