using System.Linq.Expressions;

namespace Caspian.UI
{
    internal interface IListViewer<TEntity> where TEntity : class
    {
        void AddDataField(Expression expression);
    }
}
