using FluentValidation;
using System.Reflection;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Service
{
    public class MasterDetailsService<TMaster, TDetails>: BaseService<TMaster>, IMasterDetailsService<TMaster, TDetails> where TMaster : class where TDetails : class 
    {
        public MasterDetailsService(IServiceProvider provider)
            : base(provider)
        {
            if (BatchServiceData == null)
                BatchServiceData = new BatchServiceData();
            BatchServiceData.MasterType = typeof(TMaster);
            if (BatchServiceData.DetailPropertiesInfo == null)
                BatchServiceData.DetailPropertiesInfo = new List<PropertyInfo>();
            var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetails));
            BatchServiceData.DetailPropertiesInfo.Add(detailsProperty);
        }

        public async override Task<TMaster> AddAsync(TMaster entity)
        {
            PropertyInfo detailsInfo = typeof(TMaster).GetDetailsProperty(typeof(TDetails));
            var details = detailsInfo.GetValue(entity) as IEnumerable<TDetails>;
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
            var detailsList = new List<TDetails>();
            detailsInfo.SetValue(newEntity, detailsList);
            var detailsPKey = typeof(TDetails).GetPrimaryKey();
            foreach (var detail in details)
            {
                var item = Activator.CreateInstance<TDetails>();
                foreach (var info in typeof(TDetails).GetProperties().Where(t => t.CanWrite))
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

        public virtual async Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetails>> changedEntities)
        {
            var masterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(entity));
            var changedList = new List<ChangedEntity<TDetails>>();
            foreach (var changedEntity in changedEntities)
            {
                var changedDetail = new ChangedEntity<TDetails>();
                changedDetail.ChangeStatus = changedEntity.ChangeStatus;
                changedDetail.Entity = Activator.CreateInstance<TDetails>();
                foreach (var info in typeof (TDetails).GetProperties().Where(t => t.CanWrite))
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
                        if (info.PropertyType.IsCollectionType(typeof(TDetails)))
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
                var detailsKey = typeof(TDetails).GetPrimaryKey();
                foreach(var item in insertedItems)
                {
                    var id = Convert.ToInt32(detailsKey.GetValue(item));
                    if (id <  0)
                        detailsKey.SetValue(item, 0);
                }
                await Context.Set<TDetails>().AddRangeAsync(insertedItems);
            }
            var updatedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Updated).Select(t => t.Entity);
            if (updatedItems.Any())
                Context.Set<TDetails>().UpdateRange(updatedItems);
            var deletedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Deleted).Select(t => t.Entity);
            if (deletedItems.Any())
                Context.Set<TDetails>().RemoveRange(deletedItems);
            await UpdateAsync(entity);
            typeof(TMaster).GetDetailsProperty(typeof(TDetails)).SetValue(entity, insertedItems);
            return entity;
        }

        public async Task DeleteMasterAndDetails(TMaster master)
        {
            var info = typeof(TDetails).GetForeignKey(typeof(TMaster));
            var paraameter = Expression.Parameter(typeof(TDetails), "t");
            Expression expr = Expression.Property(paraameter, info);
            var value = typeof(TMaster).GetPrimaryKey().GetValue(master);
            expr = Expression.Equal(expr, Expression.Constant(value));
            var lambda = Expression.Lambda(expr, paraameter);
            var service = GetService<BaseService<TDetails>>();
            var details = await service.GetAll().Where(lambda).ToListAsync();
            await service.RemoveRange(details);
            await base.RemoveAsync(master);
        }
    }
}
