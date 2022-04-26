using System.Threading.Tasks;

namespace UIComponent.Page
{
    internal interface ISimplePage<TEntity>
    {
        Task<int> Post(TEntity entity);

        Task Delete(TEntity entity);
    }
}
