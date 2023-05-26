using System.Data;
using FluentValidation;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using FluentValidation.Results;
using FluentValidation.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Service
{
    public class BaseService<TEntity> : CaspianValidator<TEntity>, IBaseService, IDisposable, IBaseService<TEntity> where TEntity : class
    {
        protected ReadOnlyCollection<TEntity> Source;
        public BaseService(IServiceProvider provider)
            :base(provider)
        {
            
        }

        public void SetSource(object obj)
        {
            Source = (obj as List<TEntity>).AsReadOnly();
        }

        public IQueryable GetAllRecords()
        {
            return GetAll();
        }

        public bool Any(Expression<Func<TEntity, bool>> expr = null)
        {
            if (expr == null)
                return GetAll().Any();
            return GetAll().Any(expr);
        }

        public virtual IQueryable<TEntity> GetAll(TEntity search = null)
        {
            return Context.Set<TEntity>().Search(search);
        }

        async public virtual Task UpdateAsync(TEntity entity)
        {
            var result = await ValidateAsync(entity);
            if (result.Errors.Count > 0)
                throw new CaspianException(result.Errors[0].ErrorMessage);
            if (Context.Entry(entity).State != EntityState.Modified)
            {
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                var old = await SingleOrDefaultAsync(id);
                if (old != null)
                    old.CopySimpleProperty(entity);
            }
        }

        public IBaseService<T> GetEntityService<T>() where T: class
        {
            return new BaseService<T>(ServiceProvider);
        }

        public TService GetService<TService>() where TService : class
        {
            return (TService)Activator.CreateInstance(typeof(TService), ServiceProvider);
        }

        public async virtual Task<ValidationResult> ValidateRemoveAsync(TEntity entity)
        {
            return await ValidateAsync(new ValidationContext<TEntity>(entity, new PropertyChain(), new RulesetValidatorSelector("remove")));
        }

        public override Task<ValidationResult> ValidateAsync(ValidationContext<TEntity> context, CancellationToken cancellation = default)
        {
            context.RootContextData["__ServiceScope"] = ServiceProvider;
            if (!context.IsChildContext)
                context.RootContextData["__MasterInstanse"] = context.InstanceToValidate;
            
            return base.ValidateAsync(context, cancellation);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            foreach(var info in typeof(TEntity).GetProperties())
            {
                var type = info.PropertyType;
                if (info.GetCustomAttribute<ForeignKeyAttribute>() != null || (type.IsCollectionType() && type != typeof(string)))
                    info.SetValue(entity, default);
            }
            var result = await Context.Set<TEntity>().AddAsync(entity);
            return result.Entity;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().UpdateRange(entities);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var info in typeof(TEntity).GetProperties())
            {
                if (info.GetCustomAttribute<ForeignKeyAttribute>() != null || info.PropertyType.IsCollectionType())
                {
                    foreach(var entity in entities)
                        info.SetValue(entity, default);
                }
            }
            await Context.Set<TEntity>().AddRangeAsync(entities);
        }

        public async virtual Task RemoveAsync(TEntity entity)
        {
           
            Context.Set<TEntity>().Remove(entity);
        }

        public async virtual Task Remove(int id)
        {
            var old = await SingleOrDefaultAsync(id);
            if (old != null)
                await RemoveAsync(old);
        }
        
        async public Task<TEntity> SingleOrDefaultAsync(int id)
        {
            var type = typeof(TEntity);
            var t = Expression.Parameter(type, "t");
            Expression expr = Expression.Property(t, type.GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(id));
            expr = Expression.Lambda(expr, t);
            var entity = await GetAll().Where(expr).SingleOrDefaultAsync();
            return entity;
        }

        async public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> expr)
        {
            var result = await GetAll().MaxAsync(expr);
            return result;
        }

        async public Task<TEntity> SingleAsync(int id)
        {
            var old = await SingleOrDefaultAsync(id);
            if (old == null)
                throw new CaspianException("آیتم از سیستم حذف شده است");
            return old;
        }

        public TEntity Single(int id)
        {
            var type = typeof(TEntity);
            var t = Expression.Parameter(type, "t");
            Expression expr = Expression.Property(t, type.GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(id));
            expr = Expression.Lambda(expr, t);
            var entity = GetAll().Where(expr).SingleOrDefault();
            return entity;
        }

        async public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expr = null)
        {
            if (expr == null)
                return await GetAll().AnyAsync();
            return await GetAll().AnyAsync(expr);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.RemoveRange(entities);
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(int id)
        {
            var param = Expression.Parameter(typeof(TEntity), "t");
            Expression expr = Expression.Property(param, typeof(TEntity).GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(id));
            return await GetAll().Where(Expression.Lambda(expr, param)).AnyAsync();
        }

        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
        }
    }
}
