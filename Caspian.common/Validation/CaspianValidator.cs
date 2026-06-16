using Caspian.Common.Extension;
using Caspian.Common.Service;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Caspian.Common
{
    public class CaspianValidator<TModel> : AbstractValidator<TModel>, ICaspianValidator, IEntity where TModel : class
    {
        public CaspianValidator(IServiceProvider provider)
        {;
            BatchServiceData = provider.GetService<BatchServiceData>();
            RuleLevelCascadeMode = CascadeMode.Stop;
            ServiceProvider = provider;
            
            var contextType = new AssemblyInfo().GetDbContextType(typeof(TModel));
            if (contextType.Namespace == "Demo.Model")
                Language = Language.En;
            else
                Language= Language.Fa;
            Context = provider.GetService(contextType) as CaspianContext;
            foreach (var info in typeof(TModel).GetProperties())
            {
                var type = info.PropertyType;
                var param = Expression.Parameter(typeof(TModel), "t");
                if (type.IsEnum || new Type[] { typeof(DateOnly), typeof(DateTime) }.Contains(type.GetUnderlyingType()))
                {
                    Expression expr = Expression.Property(param, info);
                    expr = Expression.Convert(expr, typeof(object));
                    expr = Expression.Lambda(expr, param);
                    if (type.IsEnum)
                        RuleFor(expr as Expression<Func<TModel, object>>).CheckEnum(info);
                    else
                        RuleFor(expr as Expression<Func<TModel, object>>).CheckDate(info);
                }
                if (type.Name != "PersianDateTable")
                {
                    var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (attr != null)
                    {
                        var infoId = typeof(TModel).GetProperty(attr.Name);
                        Expression expr = Expression.Property(param, infoId);
                        expr = Expression.Convert(expr, typeof(object));
                        expr = Expression.Lambda(expr, param);
                        RuleFor(expr as Expression<Func<TModel, object>>).CheckForeignKeyAsync(info, infoId);
                    }
                }
            }
            foreach (var info in typeof(TModel).GetProperties())
            {
                if (info.Is1To1Relation())
                {
                    RuleSet(info.Name, () =>
                    {
                        foreach (var info1 in info.PropertyType.GetProperties())
                        {
                            var type = info1.PropertyType;
                            var param = Expression.Parameter(typeof(TModel), "t");
                            Expression expr = Expression.Property(param, info.Name);
                            if (type.IsEnum)
                            {
                                expr = Expression.Property(expr, info1);
                                expr = Expression.Convert(expr, typeof(object));
                                var lambda = Expression.Lambda(expr, param) as Expression<Func<TModel, object>>;
                                RuleFor(lambda).CheckEnum(info1);
                            }
                            var attr = info1.GetCustomAttribute<ForeignKeyAttribute>();
                            if (attr != null)
                            {
                                var infoId = info.PropertyType.GetProperty(attr.Name);
                                expr = Expression.Property(expr, infoId);
                                expr = Expression.Convert(expr, typeof(object));
                                var lambda = Expression.Lambda(expr, param) as Expression<Func<TModel, object>>;
                                RuleFor(lambda).CheckForeignKeyAsync(info1, infoId);
                            }
                        }
                    });
                }
            }

            RuleSet("remove", () =>
            {
                var pKey = typeof(TModel).GetPrimaryKey();
                var param = Expression.Parameter(typeof(TModel), "t");
                Expression expr = Expression.Property(param, pKey);
                expr = Expression.Convert(expr, typeof(object));
                expr = Expression.Lambda(expr, param);
                RuleFor(expr as Expression<Func<TModel, object>>).CheckForeignKeyOnRemove();
            });
        }

        public BatchServiceData BatchServiceData { get; private set; }

        internal void SetBatchServiceData(int masterId, Type masterType)
        {
            BatchServiceData.MasterType = masterType;
            BatchServiceData.MasterId = masterId;
        }

        public int UserId { get; internal set; }

        internal PropertyInfo ThirdLevelProperty { get; set; }

        

        public async virtual Task<ValidationResult> ValidateRemoveAsync(TModel model)
        {
            
            var list = new List<string>()
            {
                "remove"
            };
            var result = await ValidateAsync(new ValidationContext<TModel>(model, new PropertyChain(), new RulesetValidatorSelector(list)));
            return result;
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }

        public override Task<ValidationResult> ValidateAsync(ValidationContext<TModel> context, CancellationToken cancellation = default)
        {
            context.RootContextData["__ServiceProvider"] = ServiceProvider;
            context.RootContextData["__BatchServiceData"] = BatchServiceData;
            return base.ValidateAsync(context, cancellation);
        }

        protected IRuleBuilderInitial<TModel, object> RuleForRemove()
        {
            var pKey = typeof(TModel).GetPrimaryKey();
            var param = Expression.Parameter(typeof(TModel), "t");
            Expression expr = Expression.Property(param, pKey);
            expr = Expression.Convert(expr, typeof(object));
            var lambda = Expression.Lambda(expr, param) as Expression<Func<TModel, object>>;
            IRuleBuilderInitial<TModel, object> rule = null;
            RuleSet("remove", () =>
            {
                rule = RuleFor(lambda);
            });
            return rule;
        }

        public Language Language { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public CaspianContext Context { get; private set; }
    }
}
