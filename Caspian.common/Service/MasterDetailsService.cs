using FluentValidation;
using Caspian.Common.Service;
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
            //Details = new List<TDetail>();
            UserId = provider.GetService<CaspianDataService>().UserId;
            DetailService = provider.GetService<IBaseService<TDetail>>() as DetailService<TDetail>;
            if (DetailService == null)
                throw new CaspianException($"There are any service that implement IDetailService<{typeof(TDetail)}>");
            var lambda = CreateRuleForEachExpression();
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

        public override async Task UpdateAsync(TMaster entity)
        {
            await base.UpdateAsync(entity);
            var value = typeof(TMaster).GetPrimaryKey().GetValue(entity);
            await DetailService.SubmitChangesForUpdateAsync();
        }

        public override async Task<TMaster> AddAsync(TMaster entity)
        {
            var result = await base.AddAsync(entity);
            var value = typeof(TMaster).GetPrimaryKey().GetValue(entity);
            //await DetailService.SaveChangeAsync(Convert.ToInt32(value), typeof(TMaster));
            return result;
        }
    }
}
