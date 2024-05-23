using System.Reflection;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Caspian.Common.RowNumber
{
    public class SqlServerRowNumberTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _rowNumberMethod = typeof(DbFunctionsExtensions).GetMethod(
                       nameof(DbFunctionsExtensions.RowNumber),
                       new[] { typeof(DbFunctions), typeof(object), typeof(bool[]) });
        private static ReadOnlyCollection<Expression> ExtractParams(Expression parameter)
        {
            if (parameter is ConstantExpression constant
                && constant.Value is IEnumerable<Expression> enumerable)
            {
                return enumerable.ToList().AsReadOnly();
            }
            if (parameter is SqlUnaryExpression sqlUnaryExpression)
                parameter = sqlUnaryExpression.Operand;
            return new List<Expression> { parameter }.AsReadOnly();
        }


        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (method != _rowNumberMethod)
                return null;
            var simpleList = new List<Expression>();
            for (int i = 1; i < arguments.Count - 1; i++)
                simpleList.AddRange(ExtractParams(arguments[i]));
            var direction = ((arguments[arguments.Count - 1] as SqlConstantExpression).Value as bool[]);
            if (direction.Length == 0)
                direction = new bool[]{true };
            if (direction != null)
            {
                direction = direction.Reverse().ToArray();
                var list = new List<OrderingExpression>();
                var index = 0;
                foreach (var item in simpleList)
                {
                    list.Add(new OrderingExpression(item as SqlExpression, direction[index]));
                    index++;
                }
                return new RowNumberExpression(new List<SqlExpression>().AsReadOnly(), list.AsReadOnly(), RelationalTypeMapping.NullMapping);
            }
            return null;
        }
    }
}
