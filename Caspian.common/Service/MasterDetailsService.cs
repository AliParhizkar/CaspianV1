using FluentValidation;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common.Service
{
    public class MasterDetailsService<TMaster, TDetail>: BaseService<TMaster>, IMasterDetailsService<TMaster, TDetail> where TMaster : class where TDetail : class 
    {
        public MasterDetailsService(IServiceProvider provider)
            : base(provider)
        {
            var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            BatchServiceData.DetailPropertiesInfo.Add(detailsProperty);
            Details = new List<TDetail>();
            UserId = provider.GetService<CaspianDataService>().UserId;
            ChangedEntities = new List<ChangedEntity<TDetail>>();
        }

        protected bool FillDetail { get; set; } = true;

        protected IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        protected IEnumerable<TDetail> Details { get; private set; }

        public async Task SetChangedEntitiesAsync(TMaster master, IList<ChangedEntity<TDetail>> changedEntities)
        {
            ChangedEntities = changedEntities;
            if (FillDetail)
            {
                Details = await GetDetailsAfterAddChangesAsync(master);
                typeof(TMaster).GetDetailsProperty(typeof(TDetail)).SetValue(master, Details);
            }
        }

        public void SetChangedEntities(IList<ChangedEntity<TDetail>> details)
        {
            ChangedEntities.Clear();
            ChangedEntities.AddRange(details.ToArray());
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
            var query = service.GetAll().Where(expr);
            if (BatchServiceData.ThirdLevelProperty != null)
                query = query.Include(BatchServiceData.ThirdLevelProperty.Name);
            var result = await query.ToListAsync();
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

        protected override void EntityInitialize(TMaster entity)
        {
            /// Clear all entities
            var details = ChangedEntities.Select(t => t.Entity.ClearEntityProperties()).ToList();
            var pKey = typeof(TDetail).GetPrimaryKey();
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            detailsProperty.SetValue(entity, details);
            base.EntityInitialize(entity);
        }

        public override Task<TMaster> AddAsync(TMaster entity)
        {
            BatchServiceData.MasterType = typeof(TMaster);
            return base.AddAsync(entity);
        }

        protected override IQueryable<TMaster> GetQueryForUpdate(TMaster entity)
        {
            var query = base.GetQueryForUpdate(entity);
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            return query.Include(detailsProperty.Name);
        }

        public override void Remove(TMaster entity)
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            var details = detailsProperty.GetValue(entity) as IEnumerable<TDetail>;
            Context.RemoveRange(details);
            base.Remove(entity);
        }

        protected override void CopyEntityWithRelations(TMaster old, TMaster current)
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            var details = (detailsProperty.GetValue(old) as IEnumerable<TDetail>).ToList();
            foreach (var detail in ChangedEntities)
            {
                var id = Convert.ToInt32(typeof(TDetail).GetPrimaryKey().GetValue(detail.Entity));
                var oldDetail = details.SingleById(id);
                switch (detail.ChangeStatus)
                {
                    case ChangeStatus.Added:
                        var newDetail = Activator.CreateInstance<TDetail>();
                        newDetail.CopySimpleProperty(detail.Entity);
                        typeof(TDetail).GetPrimaryKey().SetValue(newDetail, 0);
                        details.Add(newDetail);
                        break;
                    case ChangeStatus.Updated:
                        oldDetail.CopySimpleProperty(detail.Entity);
                        Context.Entry(oldDetail).State = EntityState.Modified;
                        break;
                    case ChangeStatus.Deleted:
                        Context.Entry(oldDetail).State = EntityState.Deleted;
                        break;
                }
            }
            detailsProperty.SetValue(old, details);
            base.CopyEntityWithRelations(old, current);
        }

        public virtual async Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetail>> changedEntities)
        {
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
            var masterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(entity));
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
            base.Remove(master);
        }

        public override async Task RemoveAsync(int id)
        {

            await base.RemoveAsync(id);
        }
    }
}
