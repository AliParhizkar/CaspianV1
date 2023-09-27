using System.Linq.Expressions;

namespace Caspian.Dynamicform.Component
{
    public class ComboBox<TEntity> : InputControl
    {
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        public void EnableLoading()
        {
            throw new NotImplementedException();
        }
    }
}