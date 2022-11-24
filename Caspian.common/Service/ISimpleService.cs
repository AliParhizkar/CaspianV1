using System.Linq;
using FluentValidation;
using System.Threading.Tasks;
using System;

namespace Caspian.Common.Service
{
    public interface ISimpleService<TEntity>: IEntity, IValidator<TEntity>
    {
        IQueryable<TEntity> GetAll(TEntity entity = default);

        Task UpdateAsync(TEntity entity);

        Task<int> SaveChangesAsync();

        Task<TEntity> AddAsync(TEntity entity);

        void Remove(TEntity entity);

        Task<TEntity> SingleAsync(int id);

        Task<TEntity> SingleOrDefaultAsync(int id);
    }

    internal interface ISimpleService
    {
        IQueryable GetAllRecords();
    }
}
