
using Caspian.Common.Extension;
using Caspian.Engine.Model;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace Caspian.Engine
{
    public static class ReportPrintEngineExtension
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
            var lambda = paramExpr.CreateLambdaExpresion(list);
            return query.Select(lambda);
        }
    }
}
