using System.Text;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common
{
    public static class QueryableCommonExtension
    {
        public static async Task<int> ExecuteInsertAsync<TSource, TResult>(this IQueryable<TSource> source,
                Expression<Func<TSource, TResult>> selector, IServiceScope scope)
        {
            var expr = selector.Body as MemberInitExpression;
            var insertTable = expr.NewExpression.Type.GetCustomAttribute<TableAttribute>();
            var dic = new Dictionary<string, object>();
            var list = new List<DynamicProperty>();
            foreach (MemberAssignment expr1 in expr.Bindings)
            {
                var column = expr1.Member.GetCustomAttribute<ColumnAttribute>();
                var columnName = column?.Name ?? expr1.Member.Name;
                Expression expr2 = expr1.Expression;
                if (expr2.NodeType == ExpressionType.Convert)
                    expr2 = (expr2 as UnaryExpression).Operand;
                var propertyPath = "";
                while (expr2.NodeType == ExpressionType.MemberAccess)
                {
                    if (propertyPath.HasValue())
                        propertyPath = '.' + propertyPath;
                    propertyPath = (expr2 as MemberExpression).Member.Name + propertyPath;
                    expr2 = (expr2 as MemberExpression).Expression;
                }
                if (expr2.NodeType == ExpressionType.Constant)
                {
                    var constValue = (expr2 as ConstantExpression).Value;
                    foreach (var path in propertyPath.Split('.'))
                    {
                        var member = constValue.GetType().GetMember(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0];
                        if (member.MemberType == MemberTypes.Field)
                            constValue = (member as FieldInfo).GetValue(constValue);
                        else if (member.MemberType == MemberTypes.Property)
                            constValue = (member as PropertyInfo).GetValue(constValue);
                        else
                            throw new NotImplementedException("خطای عدم پیاده سازی");
                    }
                    dic.Add(columnName, constValue);
                }
                else if (expr2.NodeType == ExpressionType.Parameter)
                {
                    list.Add(new DynamicProperty(expr1.Member.Name, (expr1.Member as PropertyInfo).PropertyType));
                    dic.Add(columnName, expr1.Expression);
                }
                else
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
            var dynamicType = DynamicClassFactory.CreateType(list, false);
            var exprList = dic.Where(t => t.Value is Expression).Select(t => t.Value as Expression).ToList();
            var index = 0;
            var memberExprList = new List<MemberAssignment>();
            foreach (var info in dynamicType.GetProperties())
            {
                if (info.IsCollectible)
                {
                    var bindingExpr = Expression.Bind(info, exprList.ElementAt(index));
                    memberExprList.Add(bindingExpr);
                    index++;
                }
            }
            var memberInit = Expression.MemberInit(Expression.New(dynamicType), memberExprList);
            var lambda = Expression.Lambda(memberInit, selector.Parameters[0]);

            var cmdText = new StringBuilder("insert into ");
            if (insertTable.Schema.HasValue())
                cmdText.Append($"{insertTable.Schema}.");
            cmdText.Append($"{insertTable.Name}(");
            var firstTime = true;
            foreach (var item in dic)
            {
                if (firstTime)
                    firstTime = false;
                else
                    cmdText.Append(", ");
                cmdText.Append(item.Key);
            }
            cmdText.Append(")\nSELECT ");
            firstTime = true;
            foreach (var item in dic)
            {
                if (firstTime)
                    firstTime = false;
                else
                    cmdText.Append(", ");
                if (item.Value is Expression)
                    cmdText.Append(item.Key);
                else
                    cmdText.Append(item.Value);
            }
            cmdText.Append("\nfrom\n(\n\t");
            cmdText.Append(source.Select(lambda).ToQueryString().Replace("\n", "\n\t"));
            cmdText.Append("\n) as Temporary");
            return await scope.ServiceProvider.GetService<IBaseService<TSource>>().Context.Database.ExecuteSqlRawAsync(cmdText.ToString());
        }
    }
}
