using System.Linq.Expressions;

namespace Caspian.Dynamicform.Component
{
    public class Lookup<TEntity> : InputControl
    {
        public Expression<Func<TEntity, string>> TextExpression { get; set; }
    }
}