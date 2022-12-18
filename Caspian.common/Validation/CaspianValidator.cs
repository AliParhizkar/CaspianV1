using Caspian.common;
using FluentValidation;
using System.Reflection;
using System.ComponentModel;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using FluentValidation.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common
{
    public class CaspianValidator<TModel> : AbstractValidator<TModel>, ICaspianValidator, IEntity where TModel : class
    {
        public CaspianValidator(IServiceScope scope)
        {
            if (scope != null)
            {
                ServiceScope = scope;
                var contextType = new AssemblyInfo().GetDbContextType(typeof(TModel));
                Context = scope.ServiceProvider.GetService(contextType) as MyContext;
                var data = scope.ServiceProvider.GetService(typeof(CaspianDataService)) as CaspianDataService;
                UserId = data.UserId;
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
                        RuleFor(expr as Expression<Func<TModel, object>>).CheckForeignKeyAsync(info, infoId, MasterInfo);
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

        public int UserId { get;private set; }
        /// <summary>
        /// In Master-Details insert MasterInfo should not Validate for Foreign Key
        /// </summary>
        internal PropertyInfo MasterInfo { get; set; }

        /// <summary>
        /// In Master-Details insert MasterInfo should not Validate for Foreign Key
        /// </summary>
        public bool IgnoreDetailsProperty { get; set; }

        public IServiceScope ServiceScope { get; private set; }

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
                            var serviceType = typeof(SimpleService<>).MakeGenericType(type);
                            var service = Activator.CreateInstance(serviceType, ServiceScope);
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
                    var fields = info.PropertyType.GetFields().Where(t => !t.IsSpecialName);
                    var index = 0;
                    foreach (var field in fields)
                    {
                        if (Convert.ToInt32(field.GetValue(null)) == Math.Pow(2, index))
                            index++;
                        else
                            break;
                    }
                    var tempValue = Convert.ToInt64(value);
                    var attr = info.GetCustomAttribute<DisplayNameAttribute>();
                    string message = null;
                    if (index < fields.Count() || fields.Count() < 3)
                    {
                        if (tempValue == 0)
                            message = attr == null ? "لطفا مقدار فیلد را مشخص نمایید" : "لطفا " + attr.DisplayName + " را مشخص نمایید.";
                        else
                            message = attr == null ? "خطا: In type " + info.PropertyType.Name + " value " + value + " is invalid" : "لطفا " + attr.DisplayName + " را مشخص نمایید";
                    }
                    else if (tempValue < 0 || tempValue > Math.Pow(2, index) - 1)
                        message = "مقدار " + tempValue + " برای نوع شمارشی " + info.PropertyType.Name + " نامعتبر است";
                    if (message.HasValue())                        
                        context.AddFailure(message);
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
