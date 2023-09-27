using FluentValidation;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common
{
    public class CaspianValidator<TModel> : AbstractValidator<TModel>, ICaspianValidator, IEntity where TModel : class
    {
        public CaspianValidator(IServiceProvider provider)
        {
            ServiceProvider = provider;
            var contextType = new AssemblyInfo().GetDbContextType(typeof(TModel));
            if (contextType.Namespace == "Demo.Model")
                Language = Language.En;
            else
                Language= Language.Fa;
            Context = provider.GetService(contextType) as MyContext;
            var data = provider.GetService(typeof(CaspianDataService)) as CaspianDataService;
            UserId = data.UserId;
            if (!data.Language.HasValue)
                data.Language = Language;
           
            foreach (var info in typeof(TModel).GetProperties())
            {
                var type = info.PropertyType;
                var param = Expression.Parameter(typeof(TModel), "t");
                if (type.IsEnum)
                {
                    Expression expr = Expression.Property(param, info);
                    expr = Expression.Convert(expr, typeof(object));
                    expr = Expression.Lambda(expr, param);
                    RuleFor(expr as Expression<Func<TModel, object>>).CheckEnum(info);
                }
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
            RuleSet("remove", () =>
            {
                var pkey = typeof(TModel).GetPrimaryKey();
                var param = Expression.Parameter(typeof(TModel), "t");
                Expression expr = Expression.Property(param, pkey);
                expr = Expression.Convert(expr, typeof(object));
                expr = Expression.Lambda(expr, param);
                RuleFor(expr as Expression<Func<TModel, object>>).CheckForeignKeyOnRemove();
            });
        }

        protected IRuleBuilderInitial<TModel, object> RuleForRemove()
        {
            throw new NotImplementedException();
            //var pkey = typeof(TModel).GetPrimaryKey();
            //var param = Expression.Parameter(typeof(TModel), "t");
            //Expression expr = Expression.Property(param, pkey);
            //expr = Expression.Convert(expr, typeof(object));
            //var lambda = Expression.Lambda(expr, param) as Expression<Func<TModel, object>>;
            //var rule = PropertyRule.Create(lambda);
            //rule.RuleSets = new string[] { "remove" };
            //AddRule(rule);
            //return new RuleBuilder<TModel, object>(rule, this);
        }

        public Language Language { get; private set; }

        public int UserId { get; set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public MyContext Context { get; private set; }

        void CheckOnDelete(Expression<Func<TModel, object>> expression)
        {
            //var rule = PropertyRule.Create(expression);
            //AddRule(rule);
            //var ruleBuilder = new RuleBuilder<TModel, object>(rule, this);

        }
    }
}
