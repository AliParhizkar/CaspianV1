using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace Caspian.Common.Service
{
    public interface IMasterDetailsService<TMaster, TDetails>: IBaseService<TMaster>
    {
        Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetails>> changedEntities);
    }

    public interface IMasterDetailsService<TMaster, TDetails, TDetails1>: IMasterDetailsService<TMaster, TDetails>
    {
        Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetails>> changedEntities, IList<ChangedEntity<TDetails1>> changedEntities1);
    }

    public interface IBaseService<TEntity>: IEntity, IValidator<TEntity>, IDisposable
    {
        IQueryable<TEntity> GetAll();

        Task UpdateAsync(TEntity entity);

        Task<int> SaveChangesAsync();

        Task<TEntity> AddAsync(TEntity entity);

        Task RemoveAsync(TEntity entity);

        Task<TEntity> SingleAsync(int id);

        Task<TEntity> SingleOrDefaultAsync(int id);

        void SetSource(IReadOnlyCollection<TEntity> source);

        Task<ValidationResult> ValidateRemoveAsync(TEntity entity);
        Type DetailType { get; set; }

        
    }

    public interface IBaseService
    {
        IQueryable GetAllRecords();

        Task<bool> AnyAsync(int id);
    }
}
