using FluentValidation;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using FluentValidation.Results;
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
            var detailsProperty = typeof(TMaster).GetProperties().SingleOrDefault(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            if (detailsProperty == null)
                throw new CaspianException($"In Master-Details Service({this.GetType().Name}) Type Master({typeof(TMaster)}) must have a property of  type Details(ICollection<{typeof(TDetail)}>)");
            Details = new List<TDetail>();
            UserId = provider.GetService<CaspianDataService>().UserId;
            ChangedEntities = new List<ChangedEntity<TDetail>>();
        }

        public override Task<ValidationResult> ValidateRemoveAsync(TMaster model)
        {
            BatchServiceData.DetailPropertiesInfo.Add(typeof(TMaster).GetDetailsProperty(typeof(TDetail)));
            return base.ValidateRemoveAsync(model);
        }

        protected bool FillDetail { get; set; } = true;

        protected IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        protected IList<TDetail> Details { get; private set; }

        public void SetChangedEntitiesAsync(TMaster master, IList<ChangedEntity<TDetail>> changedEntities)
        {
            ChangedEntities = changedEntities;
            if (FillDetail)
            {
                var result = GetDetailsAfterAddChangesAsync(master);
                Details.Clear();
                Details.AddRange(result.ToArray());
            }
        }

        public void SetChangedEntities(IList<ChangedEntity<TDetail>> details)
        {
            ChangedEntities.Clear();
            ChangedEntities.AddRange(details.ToArray());
        }

        [Obsolete("This Method is Changed in future")]
        public IList<TDetail> GetDetailsAfterAddChangesAsync(TMaster master)
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
            var result = query.ToIList();
            //var result = new List<TDetail>();
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
            return result.ToDynamicList<TDetail>();
        }

        protected override void EntityInitialize(TMaster entity)
        {
            /// Clear all entities
            var details = new List<TDetail>();
            var pKey = typeof(TDetail).GetPrimaryKey();
            foreach(var change in ChangedEntities)
            {
                var detail = Activator.CreateInstance<TDetail>();
                detail.CopySimpleProperty(change.Entity);
                pKey.SetValue(detail, 0);
                details.Add(detail);
            }
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            detailsProperty.SetValue(entity, details);
            base.EntityInitialize(entity);
        }

        protected override IQueryable<TMaster> GetQueryForUpdate(TMaster entity)
        {
            var query = base.GetQueryForUpdate(entity);
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            return query.Include(detailsProperty.Name);
        }

        protected override IQueryable<TMaster> GetQueryForRemove()
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            return base.GetQueryForRemove().Include(detailsProperty.Name);
        }

        protected override void SetAsDeleted(TMaster entity)
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            var details = detailsProperty.GetValue(entity) as IEnumerable<TDetail>;
            if (details != null)
            {
                foreach(var detail in details)
                    Context.Entry(detail).State = EntityState.Deleted;
            }
            base.SetAsDeleted(entity);
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
    }
}
