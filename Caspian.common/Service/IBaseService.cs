using FluentValidation;
using FluentValidation.Results;

namespace Caspian.Common.Service
{
    public interface IMasterDetailsService<TMaster, TDetails>: IBaseService<TMaster>
    {
        Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetails>> changedEntities);

        Task SetChangedEntitiesAsync(TMaster master, IList<ChangedEntity<TDetails>> changedEntities);

        void SetChangedEntities(IList<ChangedEntity<TDetails>> changedEntities);
    }

    public interface IMasterDetailsService<TMaster, TDetails, TDetails1>: IBaseService<TMaster>
    {
        Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetails>> changedEntities, IList<ChangedEntity<TDetails1>> changedEntities1);

        Task SetChangedEntities(TMaster master, IList<ChangedEntity<TDetails>> changedEntities, IList<ChangedEntity<TDetails1>> changedEntities1);

        void SetChangedEntities(IList<ChangedEntity<TDetails>> changedEntities, IList<ChangedEntity<TDetails1>> changedEntities1);
    }

    public interface IBaseService<TEntity>: IEntity, IValidator<TEntity>, IDisposable
    {
        IQueryable<TEntity> GetAll();

        Task UpdateAsync(TEntity entity);

        Task<int> SaveChangesAsync();

        Task<TEntity> AddAsync(TEntity entity);

        void Remove(TEntity entity);

        Task<TEntity> SingleAsync(int id);

        Task<TEntity> SingleOrDefaultAsync(int id);

        void SetSource(IReadOnlyCollection<TEntity> source);

        Task<ValidationResult> ValidateRemoveAsync(TEntity entity);

        Type OtherTypeIn1To1Relationship { get; set; }

        
    }

    public interface IBaseService
    {
        IQueryable GetAllRecords();

        Task<bool> AnyAsync(int id);
    }
}
