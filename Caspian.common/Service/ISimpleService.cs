using FluentValidation;
using FluentValidation.Results;

namespace Caspian.Common.Service
{
    public interface ISimpleService<TEntity>: IEntity, IValidator<TEntity>
    {
        IQueryable<TEntity> GetAll(TEntity entity = default);

        Task UpdateAsync(TEntity entity);

        Task<int> SaveChangesAsync();

        Task<TEntity> AddAsync(TEntity entity);

        Task RemoveAsync(TEntity entity);

        Task<TEntity> SingleAsync(int id);

        Task<TEntity> SingleOrDefaultAsync(int id);
    }

    public interface ISimpleService
    {
        IQueryable GetAllRecords();

        Task<bool> AnyAsync(int id);
    }
}
