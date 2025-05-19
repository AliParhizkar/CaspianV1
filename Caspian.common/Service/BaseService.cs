using System.Data;
using FluentValidation;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using FluentValidation.Results;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Service
{
    public class BaseService<TEntity> : CaspianValidator<TEntity>, IBaseService, IDisposable, IBaseService<TEntity> where TEntity : class
    {
        protected string GuId = Guid.NewGuid().ToString();
        object otherEntityIn1To1Relationship;
        public BaseService(IServiceProvider provider)
            :base(provider)
        {
            Source = new List<TEntity>();
        }

        public bool CheckValidation { get; set; } = true;

        protected async Task<T> GetEntity<T>(Expression<Func<T, bool>> expression)
        {
            using var service = ServiceProvider.CreateScope().GetService<IBaseService<T>>();
            return await service.GetAll().SingleOrDefaultAsync(expression);
        }

        protected async Task<T> GetEntity<T>(int id, T entity)
        {
            if (entity == null || Convert.ToInt32(typeof(T).GetPrimaryKey().GetValue(entity)) != id)
            {
                using var service = ServiceProvider.CreateScope().GetService<IBaseService<T>>();
                entity = await service.SingleAsync(id);
            }
            return entity;
        }

        internal protected IReadOnlyCollection<TEntity> Source { get; set; }

        public void SetSource(IReadOnlyCollection<TEntity> source)
        {
            Source = source;
        }

        public Type OtherTypeIn1To1Relationship { get; set; }

        public TService GetService<TService>() where TService : class 
        {
            return ServiceProvider.GetCaspianService<TService>();
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

        public virtual IQueryable<TEntity> Search(TEntity entity, IDictionary<string, SearchType> searchData, IDictionary<string, ICollection> enumValues)
        {
            return GetAll().Search(entity, searchData, enumValues);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            ValidationResult result = null;
            if (OtherTypeIn1To1Relationship == null || OtherTypeIn1To1Relationship == typeof(TEntity))
                result = await ValidateAsync(entity);
            else
                result = await this.ValidateAsync(entity, OtherTypeIn1To1Relationship);
            if (result.Errors.Count > 0)
                throw new CaspianException(result.Errors[0].ErrorMessage);
            if (Context.Entry(entity).State != EntityState.Modified)
            {
                var query = GetAll();
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                PropertyInfo info = null;
                if (OtherTypeIn1To1Relationship != null && OtherTypeIn1To1Relationship != typeof(TEntity))
                {
                    info = typeof(TEntity).GetOneToOnePropertyInfo(OtherTypeIn1To1Relationship);
                    if (info != null)
                        query = query.Include(info.Name);
                }

                var old = await query.SingleAsync(id);
                if (info != null)
                {
                    var oldOneToOne = info.GetValue(old);
                    var oneToOne = info.GetValue(entity);
                    if (oldOneToOne == null)
                        info.SetValue(old, oneToOne);
                    else
                    {
                        if (oneToOne == null)
                            info.SetValue(old, null);
                        else
                        {
                            foreach (var info1 in oneToOne.GetType().GetProperties())
                            {
                                var type = info1.PropertyType;
                                if (info1.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() == null)
                                {
                                    if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                                    {
                                        var value1 = info1.GetValue(oneToOne);
                                        info1.SetValue(oldOneToOne, value1);
                                    }
                                }
                            }
                        }
                    }
                }
                if (old != null)
                    old.CopySimpleProperty(entity);
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            if (CheckValidation)
            {
                var result = await ValidateAsync(entity);
                if (!result.IsValid)
                    throw new CaspianException(result.Errors.First().ErrorMessage);
            }
            if (OtherTypeIn1To1Relationship != null)
                otherEntityIn1To1Relationship = typeof(TEntity).GetOneToOnePropertyInfo(OtherTypeIn1To1Relationship).GetValue(entity);
            /// All properties that are entity should be null except One-To-One relationship properties
            /// Note: Onet-To-One relationship properties hasven't any ForeignKeyAttribute
            foreach (var info in typeof(TEntity).GetProperties())
            {
                var type = info.PropertyType;
                if (!type.IsValueType && type != typeof(string) && type != typeof(byte[]) )
                    info.SetValue(entity, default);
            }
            EntityInitialize(entity);
            var result1 = await Context.Set<TEntity>().AddAsync(entity);
            return result1.Entity;
        }

        protected virtual void EntityInitialize(TEntity entity)
        {
            /// Initialize Onet-To-One relationship 
            if (OtherTypeIn1To1Relationship != null)
                typeof(TEntity).GetOneToOnePropertyInfo(OtherTypeIn1To1Relationship).SetValue(entity, otherEntityIn1To1Relationship);
        }


        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any()) 
                return;
            foreach(var entity in entities)
            {
                var result = await ValidateAsync(entity);
                if (!result.IsValid)
                    throw new CaspianException(result.Errors.First().ErrorMessage);
            }
            foreach (var info in typeof(TEntity).GetProperties())
            {
                if (info.GetCustomAttribute<ForeignKeyAttribute>() != null || (info.PropertyType.IsCollectionType() && info.PropertyType != typeof(string)))
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

        public async virtual Task RemoveAsync(int id)
        {
            var old = await GetAll().SingleOrDefaultAsync(id);
            if (old != null)
                await RemoveAsync(old);
        }
        
        async public Task<TEntity> SingleOrDefaultAsync(int id)
        {
            var type = typeof(TEntity);
            var t = Expression.Parameter(type, "t");
            var info = type.GetPrimaryKey();
            Expression expr = Expression.Property(t, info);
            expr = Expression.Equal(expr, Expression.Constant(Convert.ChangeType(id, info.PropertyType)));
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

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expr = null)
        {
            if (expr == null)
                return await GetAll().AnyAsync();
            return await GetAll().AnyAsync(expr);
        }

        public async Task RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                return;
            foreach (var entity in entities)
            {
                var result = await ValidateRemoveAsync(entity);
                if (!result.IsValid)
                    throw new CaspianException(result.Errors.First().ErrorMessage);
            }
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
            var pKey = typeof(TEntity).GetPrimaryKey();
            Expression expr = Expression.Property(param, pKey);
            expr = Expression.Equal(expr, Expression.Constant(Convert.ChangeType(id, pKey.PropertyType)));
            return await GetAll().Where(Expression.Lambda(expr, param)).AnyAsync();
        }
    }
}
