using System.Linq.Expressions;

namespace Capian.Dynamicform.Component
{
    public class ComboBox<TEntity> : InputControl
    {
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        public void ReloadData()
        {

        }

        public void EnableLoading()
        {

        }
    }
}