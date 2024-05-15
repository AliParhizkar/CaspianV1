using System.Linq.Expressions;

namespace Caspian.UI
{
    public interface IListViewer<TEntity> where TEntity : class
    {
        void AddDataField(Expression expression);
    }
}
