using FluentValidation;

namespace Caspian.Common.Service
{
    public interface IMasterDetailsService<TMaster, TDetails>: IBaseService<TMaster>
    {
        Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetails>> changedEntities);
    }

    public interface IBaseService<TEntity>: IEntity, IValidator<TEntity>, IDisposable
    {
        IQueryable<TEntity> GetAll(TEntity entity = default);

        Task UpdateAsync(TEntity entity);

        Task<int> SaveChangesAsync();

        Task<TEntity> AddAsync(TEntity entity);

        Task RemoveAsync(TEntity entity);

        Task<TEntity> SingleAsync(int id);

        Task<TEntity> SingleOrDefaultAsync(int id);
    }

    public interface IBaseService
    {
        IQueryable GetAllRecords();

        Task<bool> AnyAsync(int id);

        void SetSource(object source);
    }
}
