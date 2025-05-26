using System.Data;
using FluentValidation;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Service
{
    public class BaseService<TEntity> : CaspianValidator<TEntity>, IBaseService, IDisposable, IBaseService<TEntity> where TEntity : class
    {
        object otherEntityIn1To1Relationship;
        Type otherTypeIn1To1Relationship;
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

        public Type OtherTypeIn1To1Relationship
        {
            get { return otherTypeIn1To1Relationship; }
            set
            {
                if (value == typeof(TEntity))
                    otherTypeIn1To1Relationship = null;
                else
                    otherTypeIn1To1Relationship = value;
            }
        }

        public TService GetService<TService>() where TService : class 
        {
            return ServiceProvider.GetCaspianService<TService>();
        }

        public IQueryable GetAllRecords()
        {
            return GetAll();
        }

        public virtual IQueryable<TEntity> Search(TEntity entity, IDictionary<string, SearchType> searchData, IDictionary<string, ICollection> enumValues)
        {
            return GetAll().Search(entity, searchData, enumValues);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>();
        }

        #region Range CRUD
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                return;
            foreach (var entity in entities)
            {
                var result = await ValidateAsync(entity);
                if (!result.IsValid)
                    throw new CaspianException(result.Errors.First().ErrorMessage);
            }
            foreach (var info in typeof(TEntity).GetProperties())
            {
                if (info.GetCustomAttribute<ForeignKeyAttribute>() != null || (info.PropertyType.IsCollectionType() && info.PropertyType != typeof(string)))
                {
                    foreach (var entity in entities)
                        info.SetValue(entity, default);
                }
            }
            await Context.Set<TEntity>().AddRangeAsync(entities);
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
        #endregion

        #region LINQ Methods
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

        public async Task<bool> AnyAsync(int id)
        {
            var param = Expression.Parameter(typeof(TEntity), "t");
            var pKey = typeof(TEntity).GetPrimaryKey();
            Expression expr = Expression.Property(param, pKey);
            expr = Expression.Equal(expr, Expression.Constant(Convert.ChangeType(id, pKey.PropertyType)));
            return await GetAll().Where(Expression.Lambda(expr, param)).AnyAsync();
        }

        public bool Any(Expression<Func<TEntity, bool>> expr = null)
        {
            if (expr == null)
                return GetAll().Any();
            return GetAll().Any(expr);
        }
        #endregion

        #region For CRUD Operation For Entity
        #region For Add Entity 

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            if (CheckValidation)
            {
                var result = await ValidateAsync(entity);
                if (!result.IsValid)
                    throw new CaspianException(result.Errors.First().ErrorMessage);
            }
            if (OtherTypeIn1To1Relationship != null)
                otherEntityIn1To1Relationship = typeof(TEntity).GetOneToOneProperty(OtherTypeIn1To1Relationship).GetValue(entity);
            /// All properties that are entity should be null except One-To-One relationship properties
            /// Note: One-To-One relationship properties haven't any ForeignKeyAttribute
            entity = entity.ClearEntityProperties();
            EntityInitialize(entity);
            var result1 = await Context.Set<TEntity>().AddAsync(entity);
            return result1.Entity;
        }

        /// <summary>
        /// This Method Initialize all Entities that have relation with this entity
        /// </summary>
        protected virtual void EntityInitialize(TEntity entity)
        {
            /// Initialize One-To-One relationship 
            if (OtherTypeIn1To1Relationship != null)
                typeof(TEntity).GetOneToOneProperty(OtherTypeIn1To1Relationship).SetValue(entity, otherEntityIn1To1Relationship);
        }

        #endregion

        #region For Update Entity
        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (CheckValidation)
            {
                var result = await this.ValidateAsync(entity, OtherTypeIn1To1Relationship);
                if (result.Errors.Count > 0)
                    throw new CaspianException(result.Errors[0].ErrorMessage);
            }
            var query = GetQueryForUpdate(entity);
            var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
            var old = await query.SingleAsync(id);
            CopyEntityWithRelations(old, entity);
        }

        /// <summary>
        /// This method create Query for Fetch data base on entities relationship(Master-Other or master-Details)
        /// </summary>
        protected virtual IQueryable<TEntity> GetQueryForUpdate(TEntity entity)
        {
            var query = GetAll();
            if (OtherTypeIn1To1Relationship != null)
            {
                var otherProperty = typeof(TEntity).GetOneToOneProperty(OtherTypeIn1To1Relationship);
                if (otherProperty != null)
                    query = query.Include(otherProperty.Name);
            }
            return query;
        }

        /// <summary>
        /// This method Copy Current-Entity to Old-Entity base on entities relationship(Master-Other or master-Details)
        /// </summary>
        protected virtual void CopyEntityWithRelations(TEntity old, TEntity current)
        {
            old.CopySimpleProperty(current);
            if (OtherTypeIn1To1Relationship != null)
            {
                var otherProperty = typeof(TEntity).GetOneToOneProperty(OtherTypeIn1To1Relationship);
                var detail = otherProperty.GetValue(current);
                var oldDetail = otherProperty.GetValue(old);
                if (oldDetail == null)
                    otherProperty.SetValue(old, detail);
                else
                    oldDetail.CopyEntity(detail);
            }
        }

        #endregion

        #region Remove Entity

        /// <summary>
        /// This Method get query for delete. It's can be override in sub class and update query for sub class
        /// </summary>
        protected virtual IQueryable<TEntity> GetQueryForRemove()
        {
            var query = GetAll();
            ///For One-To-One relationship we should include all One-To-One relationship to cascade remove 
            foreach (var property in typeof(TEntity).GetOneToOneProperties())
                query = query.Include(property.Name);
            return query;
        }

        /// <summary>
        /// This method change entity-state to delete for Remove. It's can be override in sub class and change state of
        /// related properties in Master-Details properties
        /// </summary>
        protected virtual void SetAsDeleted(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public async virtual Task RemoveAsync(int id)
        {
            var old = await GetQueryForRemove().SingleOrDefaultAsync(id);
            if (old != null)
                SetAsDeleted(old);
        }

        public void Remove(TEntity entity)
        {
            Context.Remove(entity);
        }
        #endregion
        #endregion

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }        
    }
}
