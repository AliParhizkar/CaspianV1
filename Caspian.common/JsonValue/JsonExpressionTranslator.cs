using System.Linq.Expressions;

namespace Caspian.Common.JsonValue
{
    public static class JsonExpressionTranslator
    {
        public static Expression Translate(IReadOnlyCollection<Expression> expressions)
        {
            var items = expressions.ToArray();
            return new JsonExpression(items[0], items[1]);
        }
    }
}
