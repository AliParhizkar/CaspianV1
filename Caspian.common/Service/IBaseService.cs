using FluentValidation;
using FluentValidation.Results;

namespace Caspian.Common.Service
{
    public interface IMasterDetailsService<TMaster, TDetail>: IBaseService<TMaster> where TDetail : class
    {
        DetailService<TDetail> DetailService { get; }
    }

    public interface IMasterDetailsService<TMaster, TDetails, TDetails1>: IBaseService<TMaster>
    {

    }

    public interface IBaseService<TEntity>: IEntity, FluentValidation.IValidator<TEntity>, IDisposable
    {
        IQueryable<TEntity> GetAll();

        Task UpdateAsync(TEntity entity);

        Task<int> SaveChangesAsync();

        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity> RemoveAsync(int id);

        void Remove(TEntity entity);

        Task<TEntity> SingleAsync(int id);

        Task<TEntity> SingleOrDefaultAsync(int id);

        Task<ValidationResult> ValidateRemoveAsync(TEntity entity);

        Type OtherTypeIn1To1Relationship { get; set; }
    }

    public interface IBaseService
    {
        IQueryable GetAllRecords();

        Task<bool> AnyAsync(int id);
    }
}
