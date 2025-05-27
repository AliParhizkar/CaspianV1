using System.Linq.Expressions;
using Caspian.Common.Extension;
using FluentValidation.Results;
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
            ChangedEntities1 = new List<ChangedEntity<TDetail1>>();
        }

        protected bool FillDetail { get; set; } = true;

        protected IList<TDetail1> Details1 { get; private set; }

        protected IList<ChangedEntity<TDetail1>> ChangedEntities1 { get; set; }

        public void SetChangedEntities(IList<ChangedEntity<TDetail>> details, IList<ChangedEntity<TDetail1>> details1)
        {
            base.SetChangedEntities(details);
            ChangedEntities1.Clear();
            ChangedEntities1.AddRange(details1.ToArray());
        }

        protected override void EntityInitialize(TMaster entity)
        {
            /// Clear all entities
            var details = ChangedEntities1.Select(t => t.Entity.ClearEntityProperties()).ToList();
            var pKey = typeof(TDetail1).GetPrimaryKey();
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            detailsProperty.SetValue(entity, details);
            base.EntityInitialize(entity);
        }

        protected override IQueryable<TMaster> GetQueryForRemove()
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            return base.GetQueryForRemove().Include(detailsProperty.Name);
        }

        protected override void SetAsDeleted(TMaster entity)
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            var details = detailsProperty.GetValue(entity) as IEnumerable<TDetail1>;
            if (details != null)
            {
                foreach (var detail in details)
                    Context.Entry(detail).State = EntityState.Deleted;
            }
            base.SetAsDeleted(entity);
        }

        public override Task<ValidationResult> ValidateRemoveAsync(TMaster model)
        {
            BatchServiceData.DetailPropertiesInfo.Add(typeof(TMaster).GetDetailsProperty(typeof(TDetail1)));
            return base.ValidateRemoveAsync(model);
        }

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
            SetChangedEntitiesAsync(master, changedEntities);
            ChangedEntities1 = changedEntities1;
            if (FillDetail)
            {
                Details1 = await GetDetailsAfterAddChangesAsync(master);
                typeof(TMaster).GetDetailsProperty(typeof(TDetail)).SetValue(master, Details);
            }
        }

        protected override IQueryable<TMaster> GetQueryForUpdate(TMaster entity)
        {
            var query = base.GetQueryForUpdate(entity);
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            return query.Include(detailsProperty.Name);
        }

        protected override void CopyEntityWithRelations(TMaster old, TMaster current)
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            var details = (detailsProperty.GetValue(old) as IEnumerable<TDetail1>).ToList();
            foreach (var detail in ChangedEntities1)
            {
                var id = Convert.ToInt32(typeof(TDetail1).GetPrimaryKey().GetValue(detail.Entity));
                var oldDetail = details.SingleById(id);
                switch (detail.ChangeStatus)
                {
                    case ChangeStatus.Added:
                        var newDetail = Activator.CreateInstance<TDetail1>();
                        newDetail.CopySimpleProperty(detail.Entity);
                        typeof(TDetail1).GetPrimaryKey().SetValue(newDetail, 0);
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
    }
}
