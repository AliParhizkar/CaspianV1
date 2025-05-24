using Caspian.Common;
using System.Collections;
using System.Linq.Expressions;
using Caspian.Common.Extension;

namespace Caspian.UI
{
    public class EnumSearch<TValue> : IEnumSearch<TValue> where TValue : Enum
    {
        string propertyPath;
        IDictionary<string, ICollection> dictionary;
        public EnumSearch(Expression expression, IDictionary<string, ICollection> dictionary)
        {
            this.dictionary = dictionary;
            var propertyPath = "";
            var expr = expression;
            while (expr.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpr = expr as MemberExpression;
                if (propertyPath.HasValue())
                    propertyPath = $".{propertyPath}";
                if (!memberExpr.Member.DeclaringType.IsNullableType())
                    propertyPath += memberExpr.Member.Name;
                expr = memberExpr.Expression;
            }
            this.propertyPath = propertyPath;
        }

        public void SetValues(params TValue[] values)
        {
            if (dictionary == null || !dictionary.ContainsKey(propertyPath))
            {
                if (values != null && values.Length > 0)
                    dictionary.Add(propertyPath, values);
            }
            else
            {
                if (values == null || values.Length == 0)
                    dictionary.Remove(propertyPath);
                else
                    dictionary[propertyPath] = values;
            }
        }
    }
}
