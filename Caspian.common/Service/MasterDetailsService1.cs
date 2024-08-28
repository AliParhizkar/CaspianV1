using Caspian.Common.Extension;

namespace Caspian.Common.Service
{
    public class MasterDetailsService<TMaster, TDetails, TDetails1> : MasterDetailsService<TMaster, TDetails>, IMasterDetailsService<TMaster, TDetails, TDetails1>
        where TMaster : class where TDetails : class where TDetails1 : class
    {
        public MasterDetailsService(IServiceProvider provider):
            base(provider)
        {
            var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetails1));
            BatchServiceData.DetailPropertiesInfo.Add(detailsProperty);
        }

        public async Task<TMaster> UpdateDatabaseAsync(TMaster entity, IList<ChangedEntity<TDetails>> changedEntities, IList<ChangedEntity<TDetails1>> changedEntities1)
        {
            var masterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(entity));
            var changedList = new List<ChangedEntity<TDetails1>>();
            foreach (var changedEntity in changedEntities1)
            {
                var changedDetail = new ChangedEntity<TDetails1>();
                changedDetail.ChangeStatus = changedEntity.ChangeStatus;
                changedDetail.Entity = Activator.CreateInstance<TDetails1>();
                foreach (var info in typeof(TDetails1).GetProperties())
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
                        if (info.PropertyType.IsCollectionType(typeof(TDetails1)))
                        {
                            var details = changedList.Select(t => t.Entity).ToList();
                            info.SetValue(entity, details);
                        }
                }
                return await AddAsync(entity);
            }
            var insertedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Added).Select(t => t.Entity);
            if (insertedItems.Any())
                await Context.Set<TDetails1>().AddRangeAsync(insertedItems);
            var updatedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Updated).Select(t => t.Entity);
            if (updatedItems.Any())
                Context.Set<TDetails1>().UpdateRange(updatedItems);
            var deletedItems = changedList.Where(t => t.ChangeStatus == ChangeStatus.Deleted).Select(t => t.Entity);
            if (deletedItems.Any())
                Context.Set<TDetails1>().RemoveRange(deletedItems);
            return await base.UpdateDatabaseAsync(entity, changedEntities);
        }
    }
}
