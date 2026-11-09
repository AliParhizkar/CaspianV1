using FluentValidation;
using System.Collections;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using FluentValidation.Results;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Common.Service
{
    public class MasterDetailsService<TMaster, TDetail>: BaseService<TMaster>, IMasterDetailsService<TMaster, TDetail> where TMaster : class where TDetail : class 
    {
        public MasterDetailsService(IServiceProvider provider)
            : base(provider)
        {
            //Details = new List<TDetail>();
            UserId = provider.GetService<CaspianDataService>().UserId;
            DetailService = provider.GetService<IBaseService<TDetail>>() as DetailService<TDetail>;
            if (DetailService == null)
                throw new CaspianException($"There are any service that implement IDetailService<{typeof(TDetail)}>");
            var lambda = CreateRuleForEachExpression();
            DetailService.SetMasterType(typeof(TMaster));
            RuleForEach(lambda).SetValidator(DetailService as IBaseService<TDetail>);
        }

        Expression<Func<TMaster, IEnumerable<TDetail>>> CreateRuleForEachExpression()
        {
            var detailsProperty = typeof(TMaster).GetProperties().SingleOrDefault(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            if (detailsProperty == null)
                throw new CaspianException($"In Master-Details Service({this.GetType().Name}) Type Master({typeof(TMaster)}) must have a property of  type Details(ICollection<{typeof(TDetail)}>)");
            var parameter = Expression.Parameter(typeof(TMaster), "t");
            Expression expr = Expression.Property(parameter, detailsProperty);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(TDetail));
            var body = detailsProperty.PropertyType == enumerableType ? expr : Expression.Convert(expr, enumerableType);
            return Expression.Lambda(body, parameter) as Expression<Func<TMaster, IEnumerable<TDetail>>>;
        }

        public DetailService<TDetail> DetailService { get; private set; }

        protected override void BeforeRemoveValidation(ValidationContext<TMaster> context)
        {
            context.RootContextData["__DetailsProperty"] = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            base.BeforeRemoveValidation(context);
        }

        public override Task<ValidationResult> ValidateAsync(ValidationContext<TMaster> context, CancellationToken cancellation = default)
        {
            var masterId = typeof(TMaster).GetPrimaryKey().GetValue(context.InstanceToValidate);
            context.RootContextData["__MasterId"] = masterId;
            context.RootContextData["__MasterType"] = typeof(TMaster);
            return base.ValidateAsync(context, cancellation);
        }

        protected override void SetAsDeleted(TMaster entity)
        {
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            var details = detailsProperty.GetValue(entity) as ICollection<TDetail>;
            foreach ( var detail in details)
                Context.Entry(detail).State = EntityState.Deleted;
            base.SetAsDeleted(entity);
        }

        protected override IQueryable<TMaster> GetQueryForRemove()
        {
            var query = base.GetQueryForRemove();
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            query = query.Include(detailsProperty.Name);
            return query;
        }

        public override async Task<TMaster> RemoveAsync(int id)
        {
            var old = await base.RemoveAsync(id);
            var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            var details = detailsProperty.GetValue(old) as IEnumerable<TDetail>;
            foreach (var detail in details)
                Context.Entry(detail).State = EntityState.Deleted;
            return old;
        }

        public override async Task UpdateAsync(TMaster entity)
        {
            await base.UpdateAsync(entity);
            var value = typeof(TMaster).GetPrimaryKey().GetValue(entity);
            await DetailService.SubmitChangesForUpdateAsync();
        }

        protected override async Task EntityInitialize(TMaster entity)
        {
            var details = await DetailService.GetDetailsForInsertAsync(typeof(TMaster));
            var property = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            property.SetValue(entity, details);
            await base.EntityInitialize(entity);
        }
    }
}
