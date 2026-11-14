using FluentValidation;
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
            UserId = provider.GetService<CaspianDataService>().UserId;
            Detail2Service = provider.GetService<IBaseService<TDetail1>>() as DetailService<TDetail1>;
            if (Detail2Service == null)
                throw new CaspianException($"There are any service that implement IDetailService<{typeof(TDetail1)}>");
            var lambda = CreateRuleForEachExpression();
            Detail2Service.SetMasterType(typeof(TMaster));
            RuleForEach(lambda).SetValidator(Detail2Service as IBaseService<TDetail1>);
        }

        Expression<Func<TMaster, IEnumerable<TDetail1>>> CreateRuleForEachExpression()
        {
            var detailsProperty = typeof(TMaster).GetProperties().SingleOrDefault(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail1));
            if (detailsProperty == null)
                throw new CaspianException($"In Master-Details Service({this.GetType().Name}) Type Master({typeof(TMaster)}) must have a property of  type Details(ICollection<{typeof(TDetail1)}>)");
            var parameter = Expression.Parameter(typeof(TMaster), "t");
            Expression expr = Expression.Property(parameter, detailsProperty);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(TDetail1));
            var body = detailsProperty.PropertyType == enumerableType ? expr : Expression.Convert(expr, enumerableType);
            return Expression.Lambda(body, parameter) as Expression<Func<TMaster, IEnumerable<TDetail1>>>;
        }

        public DetailService<TDetail1> Detail2Service { get; private set; }

        protected override void BeforeRemoveValidation(ValidationContext<TMaster> context)
        {
            context.RootContextData["__Details1Property"] = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            base.BeforeRemoveValidation(context);
        }

        protected override void SetAsDeleted(TMaster entity)
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            var details = detailsProperty.GetValue(entity) as ICollection<TDetail1>;
            foreach (var detail in details)
                Context.Entry(detail).State = EntityState.Deleted;
            base.SetAsDeleted(entity);
        }

        protected override IQueryable<TMaster> GetQueryForRemove()
        {
            var query = base.GetQueryForRemove();
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            query = query.Include(detailsProperty.Name);
            return query;
        }

        public override async Task<TMaster> RemoveAsync(int id)
        {
            var old = await base.RemoveAsync(id);
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            var details = detailsProperty.GetValue(old) as IEnumerable<TDetail1>;
            foreach (var detail in details)
                Context.Entry(detail).State = EntityState.Deleted;
            return old;
        }

        public override async Task UpdateAsync(TMaster entity)
        {
            await base.UpdateAsync(entity);
            var value = typeof(TMaster).GetPrimaryKey().GetValue(entity);
            await Detail2Service.SubmitChangesForUpdateAsync();
        }

        protected override async Task EntityInitialize(TMaster entity)
        {
            var details = await Detail2Service.GetDetailsForInsertAsync(typeof(TMaster));
            var property = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            property.SetValue(entity, details);
            await base.EntityInitialize(entity);
        }
    }
}
