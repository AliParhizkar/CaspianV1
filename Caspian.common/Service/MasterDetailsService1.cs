using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common.Service
{
    public class MasterDetailsService<TMaster, TDetail, TDetail1> : MasterDetailsService<TMaster, TDetail>, IMasterDetailsService<TMaster, TDetail, TDetail1>
        where TMaster : class where TDetail : class where TDetail1 : class
    {
        public MasterDetailsService(IServiceProvider provider):
            base(provider)
        {
            var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail1));
            BatchServiceData.DetailPropertiesInfo.Add(detailsProperty);
        }

        protected bool FillDetail { get; set; } = true;

        protected IList<TDetail1> Details1 { get; private set; }

        protected internal IList<ChangedEntity<TDetail1>> ChangedEntities1 { get; internal set; }

        async Task<IList<TDetail1>> GetDetailsAfterAddChangesAsync(TMaster master)
        {
            var masterId = typeof(TMaster).GetPrimaryKey().GetValue(master);
            if (masterId.Equals(0))
                return ChangedEntities1.Select(t => t.Entity).ToList();
            var fKey = typeof(TDetail1).GetForeignKey(typeof(TMaster));
            var parameter = Expression.Parameter(typeof(TDetail1), "t");
            Expression expr = Expression.Property(parameter, fKey);
            if (fKey.PropertyType.IsNullableType())
                expr = Expression.Property(parameter, "Value");
            var constantExpr = Expression.Constant(Convert.ChangeType(masterId, fKey.PropertyType.GetUnderlyingType()));
            expr = Expression.Equal(expr, constantExpr);
            expr = Expression.Lambda(expr, parameter);
            using var service = ServiceProvider.CreateScope().GetService<IBaseService<TDetail1>>();
            var result = await service.GetAll().Where(expr).ToListAsync();
            var key = typeof(TDetail1).GetPrimaryKey();
            ///Remove changed Entities (updated or deleted)
            foreach (var item in ChangedEntities1.Where(t => t.ChangeStatus != ChangeStatus.Added))
            {
                var value = key.GetValue(item.Entity);
                foreach (var entity in result)
                {
                    if (key.GetValue(entity) == value)
                        result.Remove(entity);
                }
            }
            ///Add added and updated entities
            foreach (var item in ChangedEntities1.Where(t => t.ChangeStatus != ChangeStatus.Deleted))
                result.Add(item.Entity);
            return result;
        }

        public async Task SetChangedEntities(TMaster master, IList<ChangedEntity<TDetail>> changedEntities, IList<ChangedEntity<TDetail1>> changedEntities1)
        {
            base.FillDetail = FillDetail;
            await SetChangedEntities(master, changedEntities);
            ChangedEntities1 = changedEntities1;
            if (FillDetail)
            {
                Details1 = await GetDetailsAfterAddChangesAsync(master);
                typeof(TMaster).GetDetailsProperty(typeof(TDetail)).SetValue(master, Details);
            }
        }

        public async Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetail>> changedEntities, IList<ChangedEntity<TDetail1>> changedEntities1)
        {
            var masterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(entity));
            var changedList = new List<ChangedEntity<TDetail1>>();
            foreach (var changedEntity in changedEntities1)
            {
                var changedDetail = new ChangedEntity<TDetail1>();
                changedDetail.ChangeStatus = changedEntity.ChangeStatus;
                changedDetail.Entity = Activator.CreateInstance<TDetail1>();
                foreach (var info in typeof(TDetail1).GetProperties())
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
                        if (info.PropertyType.IsCollectionType(typeof(TDetail1)))
                        {
                            var details = changedList.Select(t => t.Entity).ToList();
                            info.SetValue(entity, details);
                        }
                }
                return await AddAsync(entity);
            }
            var insertedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Added).Select(t => t.Entity);
            if (insertedItems.Any())
            {
                var detailsKey = typeof(TDetail1).GetPrimaryKey();
                foreach(var item in insertedItems)
                {
                    var id = Convert.ToInt32(detailsKey.GetValue(item));
                    if (id < 0)
                        detailsKey.SetValue(item, 0);
                }
                await Context.Set<TDetail1>().AddRangeAsync(insertedItems);
            }
            var updatedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Updated).Select(t => t.Entity);
            if (updatedItems.Any())
                Context.Set<TDetail1>().UpdateRange(updatedItems);
            var deletedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Deleted).Select(t => t.Entity);
            if (deletedItems.Any())
                Context.Set<TDetail1>().RemoveRange(deletedItems);
            if (changedEntities == null)
                return entity;
            return await base.UpdateDatabaseAsync(entity, changedEntities);
        }


    }
}
