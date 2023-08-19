using System.Linq.Expressions;

namespace Caspian.Common.JsonValue
{
    public class JsonExpression : Expression
    {
        public JsonExpression(Expression column, Expression path)
        {
            Column = column;
            Path = path;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public Expression Column { get; private set; }

        public Expression Path { get; private set; }


        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return new JsonExpression(visitor.Visit(Column), visitor.Visit(Path));
        }

        public override Type Type => typeof(string);

        public override string ToString() => $"JSON_VALUE([{Column.ToString().Trim('"')}], '{Path.ToString().Trim('"')}')";
    }
}
