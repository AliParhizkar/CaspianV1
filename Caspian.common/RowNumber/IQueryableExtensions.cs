using System.Reflection;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;

namespace Caspian.Common.RowNumber
{
    public static class IQueryableExtensions
    {
        public static Type CreateType()
        {
            var properties = new List<DynamicProperty>
            {
                new DynamicProperty("Id", typeof(int)),
                new DynamicProperty("RowNumber", typeof(int))
            };
            return DynamicClassFactory.CreateType(properties, false);
        }

        private static IQueryable CreateSelectExpr(this IQueryable source)
        {
            var t = Expression.Parameter(source.ElementType, "t");
            var rowNumberExpr = CreateRowNumberExpression(source, t);
            Expression exprId = Expression.Property(t, source.ElementType.GetPrimaryKey());
            var type = CreateType();
            var memberExprList = new List<MemberAssignment>
            {
                Expression.Bind(type.GetProperty("Id"), exprId),
                Expression.Bind(type.GetProperty("RowNumber"), rowNumberExpr)
            };
            var memberInit = Expression.MemberInit(Expression.New(type), memberExprList);
            return source.Select(Expression.Lambda(memberInit, t));   
        }

        private static Expression CreateRowNumberExpression(this IQueryable query, ParameterExpression innerParameter)
        {
            var OrderByExprlist = new List<Expression>();
            var desc = new List<bool>();
            GetOrderbyExpression(query.Expression, OrderByExprlist, desc);
            if (desc.Count == 0)
            {
                desc.Add(true);
                OrderByExprlist.Add(Expression.Property(innerParameter, query.ElementType.GetPrimaryKey()));
            }

            var method = typeof(DbFunctionsExtensions).GetMethods().Where(t => t.Name == "RowNumber")
                .ToArray()[desc.Count - 1];
            var args = new List<Expression>
            {
                Expression.Property(null, typeof(EF).GetProperty("Functions"))
            };
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

        public async static Task<int?> GetRowNumber(this IQueryable query, MyContext context, int id)
        {
            var selectQuery = CreateSelectExpr(query).Take(10_000_1000).ToQueryString();
            var index = selectQuery.LastIndexOf(";");
            string str = "";
            if (index > -1)
                str = selectQuery.Substring(0, index + 1);
            str += $"select RowNumber from({selectQuery.Substring(index + 1)}) Entity where Entity.Id = {id}";
            var result = await context.Database.SqlQueryRaw<long?>(str).ToListAsync();
            var item = result.FirstOrDefault();
            if (item == null)
                return null;
            return Convert.ToInt32(item.Value);
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
}
