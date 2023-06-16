using FluentValidation;
using System.Reflection;
using System.ComponentModel;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using FluentValidation.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common.Attributes;

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
                if (type.IsEnum)
                    RuleForEnum(info);
                var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
                if (attr != null)
                {
                    //if (info.GetValue())
                    var infoId = typeof(TModel).GetProperty(attr.Name);
                    var param = Expression.Parameter(typeof(TModel), "t");
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
                CheckOnDelete(expr as Expression<Func<TModel, object>>);
            });
        }

        protected IRuleBuilderInitial<TModel, object> RuleForRemove()
        {
            var pkey = typeof(TModel).GetPrimaryKey();
            var param = Expression.Parameter(typeof(TModel), "t");
            Expression expr = Expression.Property(param, pkey);
            expr = Expression.Convert(expr, typeof(object));
            var lambda = Expression.Lambda(expr, param) as Expression<Func<TModel, object>>;
            var rule = PropertyRule.Create(lambda);
            rule.RuleSets = new string[] { "remove" };
            AddRule(rule);
            return new RuleBuilder<TModel, object>(rule, this);
        }

        public Language Language { get; private set; }

        public int UserId { get; set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public MyContext Context { get; private set; }

        void CheckOnDelete(Expression<Func<TModel, object>> expression)
        {
            var rule = PropertyRule.Create(expression);
            AddRule(rule);
            var ruleBuilder = new RuleBuilder<TModel, object>(rule, this);
            ruleBuilder.Custom((value, context) =>
            {
                foreach (var info in typeof(TModel).GetProperties())
                {
                    if (info.PropertyType.IsEnumerableType() && info.PropertyType != typeof(string) && info.PropertyType != typeof(byte[]))
                    {
                        var attr = info.GetCustomAttribute<CheckOnDeleteAttribute>();
                        if (attr == null)
                            throw new CaspianException("خطا: On type " + info.DeclaringType.Name + " property " + info.Name + " must has CheckOnDelete Attribute", 5);
                        var type = info.PropertyType.GetGenericArguments()[0];
                        var pkey = type.GetPrimaryKey();
                        var foreignKey = pkey.GetCustomAttribute<ForeignKeyAttribute>();
                        if (foreignKey != null)
                        {
                            type = type.GetProperty(foreignKey.Name).PropertyType;
                        }
                        else
                        {
                            var serviceType = typeof(BaseService<>).MakeGenericType(type);
                            var service = Activator.CreateInstance(serviceType, ServiceProvider);
                            IQueryable query = serviceType.GetMethod("GetAll").Invoke(service, new object[] { null }) as IQueryable;
                            var name = type.GetProperties().First(t => t.PropertyType == typeof(TModel)).GetCustomAttribute<ForeignKeyAttribute>().Name;
                            var fkIdInfo = type.GetProperties().Single(t => t.Name == name);
                            var param = Expression.Parameter(type, "t");
                            Expression expr = Expression.Property(param, fkIdInfo);
                            if (fkIdInfo.PropertyType.IsNullableType())
                                expr = Expression.Property(expr, "Value");
                            expr = Expression.Equal(expr, Expression.Constant(value));
                            var lamda = Expression.Lambda(expr, param);
                            if (query.Any(lamda))
                            {
                                context.AddFailure(attr.ErrorMessage);
                                break;
                            }
                        }
                    }
                }
            });
        }

        private IRuleBuilderInitial<TModel, object> RuleForEnum(PropertyInfo info)
        {
            var param = Expression.Parameter(typeof(TModel), "t");
            Expression expr = Expression.Property(param, info);
            expr = Expression.Convert(expr, typeof(object));
            expr = Expression.Lambda(expr, param);
            var rule = PropertyRule.Create(expr as Expression<Func<TModel, object>>);
            AddRule(rule);
            var ruleBuilder = new RuleBuilder<TModel, object>(rule, this);
            ruleBuilder.Custom((value, context) =>
            {
                
                if (value != null && !Enum.IsDefined(info.PropertyType, value))
                {
                    var tempValue = Convert.ToInt64(value);
                    var enumIsValid = false;
                    var isbitwise = info.PropertyType.GetUnderlyingType().GetCustomAttribute<EnumTypeAttribute>()?.Isbitw == true;
                    var fields = info.PropertyType.GetFields().Where(t => !t.IsSpecialName);
                    if (isbitwise)
                    {
                        var max = Convert.ToInt64(fields.Max(t => t.GetValue(null)));
                        if (tempValue >= 0 && tempValue < 2 * max) 
                            enumIsValid = true;
                    }
                    else
                    {
                        foreach (var field in fields)
                        {
                            if (field.GetValue(null) == value)
                            {
                                enumIsValid = true;
                                break;
                            }
                        }
                    }

                    if (!enumIsValid)
                    {
                        
                        var attr = info.GetCustomAttribute<DisplayNameAttribute>();
                        string message = null;
                        if (tempValue == 0)
                        {
                            if (Language == Language.En)
                                message = attr == null ? "Please specify the value of the field" : "Please specify " + attr.DisplayName;
                            else
                                message = attr == null ? "" : "لطفا " + attr.DisplayName + " را مشخص نمایید.";
                        }
                        else
                        {
                            if (Language == Language.Fa)
                                message = $"مقدار {tempValue} برای نوع شمارشی {info.PropertyType.Name} نامعتبر است";
                            else
                                message = $"Value {tempValue} is invalid for {info.PropertyType.Name}";
                        }
                            
                        if (message.HasValue())
                            context.AddFailure(message);
                    }
                }
            });
            return ruleBuilder;
        }
    }

    public class ForeignKeyValidationConfig<TMaster, TDetail> :  IForeignKeyValidationConfig where TMaster : class
    {
        public Func<TMaster, bool> ConditionFunc { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public ForeignKeyValidationConfig<TMaster, TDetail> Condition(Func<TMaster, bool> func)
        {
            ConditionFunc = func;
            return this;
        }

        public bool HasCondition(object obj)
        {
            return ConditionFunc.Invoke(obj as TMaster);
        }

        public ForeignKeyValidationConfig<TMaster, TDetail> Property<TProperty>(Expression<Func<TDetail, TProperty>> expr)
        {
            PropertyInfo = (expr.Body as MemberExpression).Member as PropertyInfo;
            return this;
        }
    }

    public interface IForeignKeyValidationConfig
    {
        bool HasCondition(object obj);

        PropertyInfo PropertyInfo { get; }
    }

}
