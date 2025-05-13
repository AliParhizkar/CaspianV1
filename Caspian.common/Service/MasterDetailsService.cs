using FluentValidation;
using System.Reflection;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common.Service
{
    public class MasterDetailsService<TMaster, TDetail>: BaseService<TMaster>, IMasterDetailsService<TMaster, TDetail> where TMaster : class where TDetail : class 
    {
        public MasterDetailsService(IServiceProvider provider)
            : base(provider)
        {
            BatchServiceData = new BatchServiceData();
            BatchServiceData.MasterType = typeof(TMaster);
            BatchServiceData.DetailPropertiesInfo = new List<PropertyInfo>();
            var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            BatchServiceData.DetailPropertiesInfo.Add(detailsProperty);
            Details = new List<TDetail>();
        }

        protected bool FillDetail { get; set; } = true;

        protected IList<ChangedEntity<TDetail>> ChangedEntities { get; private set; }

        protected IEnumerable<TDetail> Details { get; private set; }

        public async Task SetChangedEntities(TMaster master, IList<ChangedEntity<TDetail>> changedEntities)
        {
            ChangedEntities = changedEntities;
            if (FillDetail)
            {
                Details = await GetDetailsAfterAddChangesAsync(master);
                typeof(TMaster).GetDetailsProperty(typeof(TDetail)).SetValue(master, Details);
            }
        }

        async Task<IList<TDetail>> GetDetailsAfterAddChangesAsync(TMaster master)
        {
            var masterId = typeof(TMaster).GetPrimaryKey().GetValue(master);
            if (masterId.Equals(0))
                return ChangedEntities.Select(t => t.Entity).ToList();
            var fKey = typeof(TDetail).GetForeignKey(typeof(TMaster));
            var parameter = Expression.Parameter(typeof(TDetail), "t");
            Expression expr = Expression.Property(parameter, fKey);
            if (fKey.PropertyType.IsNullableType())
                expr = Expression.Property(parameter, "Value");
            var constantExpr = Expression.Constant(Convert.ChangeType(masterId, fKey.PropertyType.GetUnderlyingType()));
            expr = Expression.Equal(expr, constantExpr);
            expr = Expression.Lambda(expr, parameter);
            using var service = ServiceProvider.CreateScope().GetService<IBaseService<TDetail>>();
            var result = await service.GetAll().Where(expr).ToListAsync();
            var key = typeof(TDetail).GetPrimaryKey();
            ///Remove changed Entities (updated or deleted)
            foreach (var item in ChangedEntities.Where(t => t.ChangeStatus != ChangeStatus.Added))
            {
                var value = key.GetValue(item.Entity);
                for (var index = 0;  index < result.Count; index++)
                {
                    var entity = result[index];
                    if (key.GetValue(entity).Equals(value))
                        result.Remove(entity);
                }
            }
            ///Add added and updated entities
            foreach (var item in ChangedEntities.Where(t => t.ChangeStatus != ChangeStatus.Deleted))
                result.Add(item.Entity);
            return result;
        }

        public async override Task<TMaster> AddAsync(TMaster entity)
        {
            PropertyInfo detailsInfo = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            var details = detailsInfo.GetValue(entity) as IEnumerable<TDetail>;
            Details = details;
            if (details == null || details.Count() == 0)
                return await base.AddAsync(entity);
            foreach (var info in typeof(TMaster).GetProperties())
            {
                var type = info.PropertyType;
                if ( info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    info.SetValue(entity, default);
                else if (type.IsCollectionType())
                {
                    if (type != typeof(string))
                    {
                        if (info.PropertyType != detailsInfo)
                            info.SetValue(entity, default);
                    }
                }
            }
            var context = new ValidationContext<TMaster>(entity);
            var result = await ValidateAsync(context);
            if (result.Errors.Count > 0)
                throw new CaspianException(result.Errors[0].ErrorMessage);
            var newEntity = entity.CreateNewSimpleEntity();
            var detailsList = new List<TDetail>();
            detailsInfo.SetValue(newEntity, detailsList);
            var detailsPKey = typeof(TDetail).GetPrimaryKey();
            foreach (var detail in details)
            {
                var item = Activator.CreateInstance<TDetail>();
                foreach (var info in typeof(TDetail).GetProperties().Where(t => t.CanWrite))
                {
                    var type = info.PropertyType;
                    if (type.IsValueType || type.IsNullableType() || type == typeof(string) || type == typeof(byte[]))
                        info.SetValue(item, info.GetValue(detail));
                }
                var id = Convert.ToInt32(detailsPKey.GetValue(item));
                if (id < 0)
                    detailsPKey.SetValue(item, 0);
                detailsList.Add(item);
            }

            var result1 = await Context.Set<TMaster>().AddAsync(newEntity);
            return result1.Entity;
        }

        public virtual async Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetail>> changedEntities)
        {
            var masterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(entity));
            var changedList = new List<ChangedEntity<TDetail>>();
            foreach (var changedEntity in changedEntities)
            {
                var changedDetail = new ChangedEntity<TDetail>();
                changedDetail.ChangeStatus = changedEntity.ChangeStatus;
                changedDetail.Entity = Activator.CreateInstance<TDetail>();
                foreach (var info in typeof (TDetail).GetProperties().Where(t => t.CanWrite))
                {
                    var type = info.PropertyType;
                    if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                    {
                        var value = info.GetValue(changedEntity.Entity);
                        info.SetValue(changedDetail.Entity, value);
                    }
                }
                changedList.Add(changedDetail);
            }
            if (masterId == 0)
            {
                if (changedList.Any())
                {
                    foreach (var info in typeof(TMaster).GetProperties())
                        if (info.PropertyType.IsCollectionType(typeof(TDetail)))
                        {
                            var details = changedList.Select(t => t.Entity).ToList();
                            info.SetValue(entity, details);
                        }
                }
                return await AddAsync(entity);
            }
            var insertedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Added).Select(t => t.Entity).ToArray();
            if (insertedItems.Any())
            {
                var detailsKey = typeof(TDetail).GetPrimaryKey();
                foreach(var item in insertedItems)
                {
                    var id = Convert.ToInt32(detailsKey.GetValue(item));
                    if (id <  0)
                        detailsKey.SetValue(item, 0);
                }
                await Context.Set<TDetail>().AddRangeAsync(insertedItems);
            }
            var updatedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Updated).Select(t => t.Entity);
            if (updatedItems.Any())
                Context.Set<TDetail>().UpdateRange(updatedItems);
            var deletedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Deleted).Select(t => t.Entity);
            if (deletedItems.Any())
                Context.Set<TDetail>().RemoveRange(deletedItems);
            var detailsInfo = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            if (Details == null || !Details.Any())
                Details = detailsInfo.GetValue(entity) as IEnumerable<TDetail>;
            await UpdateAsync(entity);
            detailsInfo.SetValue(entity, insertedItems);
            return entity;
        }

        public async Task DeleteMasterAndDetails(TMaster master)
        {
            var info = typeof(TDetail).GetForeignKey(typeof(TMaster));
            var parameter = Expression.Parameter(typeof(TDetail), "t");
            Expression expr = Expression.Property(parameter, info);
            var value = typeof(TMaster).GetPrimaryKey().GetValue(master);
            expr = Expression.Equal(expr, Expression.Constant(value));
            var lambda = Expression.Lambda(expr, parameter);
            var service = GetService<BaseService<TDetail>>();
            var details = await service.GetAll().Where(lambda).ToListAsync();
            await service.RemoveRange(details);
            await base.RemoveAsync(master);
        }
    }
}
