using FluentValidation;
using System.Reflection;
using System.ComponentModel;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Engine;
using System.Runtime.CompilerServices;
using System.Linq.Dynamic.Core;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace Caspian.Common
{
    public static class CaspianValidatorExtensions
    {
        public static IRuleBuilderInitial<TModel, TProperty> Custom<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Func<TModel, bool> func, string message)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                if (func.Invoke((TModel)context.InstanceToValidate))
                    context.AddFailure(message);
            });
        }

        public static IRuleBuilderInitial<TModel, TProperty> CustomAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Func<TModel, Task<bool>> func, string message)
        {
            return ruleBuilder.CustomAsync(async (value, context, token) =>
            {
                if (await func.Invoke((TModel)context.InstanceToValidate))
                    context.AddFailure(message);
            });
        }

        public static IRuleBuilderInitial<TModel, TProperty> CustomValue<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Func<TProperty, bool> func, string message)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                if (func.Invoke(value))
                    context.AddFailure(message);
            });
        }

        //public static IRuleBuilderInitial<TModel, TProperty> IgnoreForeignKey<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
        //    Action<ForeignKeyValidationConfig<TModel, TProperty>> config) where TModel : class
        //{
            
        //    return ruleBuilder.Custom((value, context) =>
        //    {
        //        var validatiorConfig = new ForeignKeyValidationConfig<TModel, TProperty>();
        //        config.Invoke(validatiorConfig);
        //        if (!context.ParentContext.RootContextData.ContainsKey("__IgnoreForeignKey"))
        //            context.ParentContext.RootContextData.Add("__IgnoreForeignKey", validatiorConfig);
        //    });
        //}

        public static Language GetLanguage(this CustomContext context)
        {
            Language language = Language.Fa;
            if (context.ParentContext.RootContextData.ContainsKey("__ServiceScope"))
            {
                var provider = context.ParentContext.RootContextData["__ServiceScope"] as IServiceProvider;
                var service = provider.GetService<CaspianDataService>();
                language = service.Language ?? Language.Fa;
            }
            return language;
        }

        public static IRuleBuilderInitial<TModel, TProperty> Required<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, 
            Func<TModel, bool> func = null, string message = null)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                
                var language = context.GetLanguage();
                if (value is string && Convert.ToString(value) == "")
                    value = default(TProperty);
                if ((value == null) && func?.Invoke((TModel)context.InstanceToValidate) != false)
                {
                    var attr = typeof(TModel).GetProperty(context.PropertyName).GetCustomAttribute<DisplayNameAttribute>();
                    var name = attr == null ? context.DisplayName : attr.DisplayName;
                    if (language == Language.Fa)
                        message = message ?? $"لطفا {name} را مشخص نمایید";
                    else
                        message = message ?? $"Please specify the {name}";
                    context.AddFailure(message);
                }
            });
        }

        public static IRuleBuilderInitial<TModel, TProperty> Range<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, TProperty min, TProperty max, string message = null) where TProperty : IComparable
        {
            return ruleBuilder.Custom((value, context) =>
            {
                var language = context.GetLanguage();
                if (value != null && (value.CompareTo(min) == -1 || value.CompareTo(max) == 1))
                {
                    if (language == Language.Fa)
                        message = message ?? "مقدار " + context.DisplayName + " باید بین {0} و {1} باشد";
                    else
                        message = message ?? "The value of " + context.DisplayName + " should be between {0} and {1} ";
                    context.AddFailure(string.Format(message, min, max));
                }
            });
        }

        public static IRuleBuilderInitial<TModel, string> MobileNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
        {
            return ruleBuilder.Custom((mobileNumber, context) =>
            {
                if (mobileNumber != null)
                {
                    var strMobileNumber = mobileNumber.ToString();
                    if (!Regex.IsMatch(strMobileNumber, "09\\d{9}"))
                    {
                        var language = context.GetLanguage();
                        if (language == Language.Fa)
                            context.AddFailure("شماره همراه باید 11 رقم باشد و با 09 شروع شود");
                        else
                            context.AddFailure("Mobile number must be 11 digits and start with 09(09125845632)");
                    }
                }
            });
        }

        public static IRuleBuilderInitial<TModel, string> TelNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
        {
            return ruleBuilder.Custom((telNumber, context) =>
            {
                if (telNumber != null)
                {
                    var strTelNumber = telNumber.ToString();
                    if (strTelNumber.Length != 11)
                        context.AddFailure("شماره تلفن باید 11 رقم باشد.");
                    if (!strTelNumber.StartsWith("0") || strTelNumber.StartsWith("09"))
                        context.AddFailure("فرمت شماره تلفن نادرست است");
                }
            });
        }

        public static IRuleBuilderInitial<TModel, TProperty> ShortTelNumber<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder)
        {
            return ruleBuilder.Custom((telNumber, context) =>
            {
                if (telNumber != null)
                {
                    var strTelNumber = telNumber.ToString();
                    if (strTelNumber.Length > 8 || strTelNumber.StartsWith("0"))
                        context.AddFailure("شماره تلفن باید 8 رقم باشد و با صفر شروع نشود.");
                }
            });
        }


        static async Task<Expression> CreateExpression<TModel>(ParameterExpression param, TModel model, Expression expr, IServiceScope scope)
        {
            if (expr == null)
                return null;
            if (expr.NodeType == ExpressionType.Lambda)
                expr = (expr as LambdaExpression).Body;
            if (expr.NodeType == ExpressionType.Convert)
                expr = (expr as UnaryExpression).Operand;
            var info = (expr as MemberExpression).Member as PropertyInfo;
            if (info.PropertyType.IsNullableType())
                expr = Expression.Property(expr, "Value");
            var value = await GetValue<TModel>(model, expr as MemberExpression, scope);
            if (value != null)
                return Expression.Equal(param.ReplaceParameter(expr), Expression.Constant(value));
            return null;
        }

        static async Task<object> GetValue<TModel>(TModel model, MemberExpression expr, IServiceScope scope)
        {
            var str = expr.ToString();
            str = str.Substring(str.IndexOf('.') + 1);
            var type = typeof(TModel);
            var array = str.Split('.');
            if (array.Length == 1)
                return model.GetMyValue(str);
            var info = type.GetProperty(array[0]);
            if (info.PropertyType.IsNullableType())
                return model.GetMyValue(array[0]);
            var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
            if (attr == null)
                throw new CaspianException("خطای عدم پیاده سازی");
            var fkId = model.GetMyValue(attr.Name);
            Expression conditionExpr = null;
            var param = Expression.Parameter(info.PropertyType, "t");
            if (fkId != null)
            {
                conditionExpr = Expression.Property(param, info.PropertyType.GetPrimaryKey());
                conditionExpr = Expression.Equal(conditionExpr, Expression.Constant(fkId));
                conditionExpr = Expression.Lambda(conditionExpr, param);
            }
            Expression memberExpr = param.CreateMemberExpresion(str.Substring(str.IndexOf('.') + 1));
            var selectExpr = Expression.Lambda(memberExpr, param);
            var serviceType = typeof(BaseService<>).MakeGenericType(info.PropertyType);
            var service = Activator.CreateInstance(serviceType, scope) as IBaseService;
            var query = service.GetAllRecords();
            if (conditionExpr != null)
                query = query.Where(conditionExpr);
            var result = await query.Select(selectExpr).ToDynamicListAsync(selectExpr.ReturnType);
            return result.FirstOrDefault();
        }

        public static IRuleBuilderOptions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            string errorMessage) where TModel: class
        {
            return ruleBuilder.UniqAsync(null, null, null, errorMessage);
        }

        public static IRuleBuilderOptions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Expression<Func<TModel, object>> expr1, string errorMessage) where TModel: class
        {
            return ruleBuilder.UniqAsync(expr1, null, null, errorMessage);
        }

        public static IRuleBuilderOptions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Expression<Func<TModel, object>> expr1, Expression<Func<TModel, object>> expr2, string errorMessage) where TModel: class
        {
            return ruleBuilder.UniqAsync(expr1, expr2, null, errorMessage);
        }

        public static IRuleBuilderOptions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, 
            Expression<Func<TModel, object>> expr1, Expression<Func<TModel, object>> expr2,
            Expression<Func<TModel, object>> expr3, string errorMessage) where TModel:class
        {
            return ruleBuilder.MustAsync(async (model, pro, contex, token) =>
            {
                if (pro == null)
                    return true;
                var scope1 = (IServiceScope)contex.ParentContext.RootContextData["__ServiceScope"];
                var param = contex.Rule.Expression.Parameters[0];
                Expression left = await CreateExpression(param, model, contex.Rule.Expression.Body, scope1);
                var tempexpr = await CreateExpression(param, model, expr1, scope1);
                if (tempexpr != null)
                {
                    if (left == null)
                        left = tempexpr;
                    else
                        left = Expression.And(left, tempexpr);
                }
                tempexpr = await CreateExpression(param, model, expr2, scope1);
                if (tempexpr != null)
                {
                    if (left == null)
                        left = tempexpr;
                    else
                        left = Expression.And(left, tempexpr);
                }
                tempexpr = await CreateExpression(param, model, expr3, scope1);
                if (tempexpr != null)
                {
                    if (left == null)
                        left = tempexpr;
                    else
                        left = Expression.And(left, tempexpr);
                }
                var pKey = typeof(TModel).GetPrimaryKey();
                if (!pKey.GetValue(model).Equals(0))
                {
                    Expression keyExpr = Expression.Property(param, pKey);
                    keyExpr = Expression.NotEqual(keyExpr, Expression.Constant(pKey.GetValue(model)));
                    left = Expression.And(left, keyExpr);
                }
                Expression<Func<TModel, bool>> lambda = Expression.Lambda(left, param) as Expression<Func<TModel, bool>>;
                if (contex.ParentContext.RootContextData.ContainsKey("__ServiceScopeFactory"))
                {
                    using var scope = ((IServiceScopeFactory)contex.ParentContext.RootContextData["__ServiceScopeFactory"])
                        .CreateScope();
                    var service = new BaseService<TModel>(scope.ServiceProvider);
                    return !await service.GetAll(default(TModel)).AnyAsync(lambda);
                }
                var service1 = new BaseService<TModel>(scope1.ServiceProvider);
                return !await service1.GetAll(default(TModel)).AnyAsync(lambda);
            }).WithMessage(errorMessage);
        }

        public static IRuleBuilder<TModel, TProperty> CheckForeignKeyAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, PropertyInfo info, PropertyInfo infoId) where TModel : class
        {
            return ruleBuilder.CustomAsync(async (value, context, token) => 
            {
                if (value != null)
                {
                    var language = context.GetLanguage();
                    var displayAttr = info.GetCustomAttribute<DisplayNameAttribute>() ?? infoId.GetCustomAttribute<DisplayNameAttribute>();
                    string message = null;
                    if (value.Equals(0))
                    {
                        var flag = false;
                        if (context.ParentContext.RootContextData.ContainsKey("__IgnorePropertyInfo"))
                        {
                            var info = context.ParentContext.RootContextData["__IgnorePropertyInfo"] as PropertyInfo;
                            var MastreId = Convert.ToInt32(context.ParentContext.RootContextData["__MasterId"]);
                            if (info == infoId && MastreId == 0)
                                flag = true;
                        }
                        if (!flag)
                        {
                            if (language == Language.Fa)
                                message = $"لطفا {(displayAttr?.DisplayName ?? infoId.Name)} را مشخص نمایید";
                            else
                                message = $"Please specify the {(displayAttr?.DisplayName ?? infoId.Name)}";
                        }
                    }
                    else
                    {
                        bool result = false;
                        if (context.ParentContext.RootContextData.ContainsKey("__ServiceScopeFactory"))
                        {
                            using var scope = ((IServiceScopeFactory)context.ParentContext.RootContextData["__ServiceScopeFactory"]).CreateScope();
                            var serviceType = typeof(IBaseService<>).MakeGenericType(info.PropertyType);
                            var service = scope.ServiceProvider.GetService(serviceType) as IBaseService;
                            result = await service.AnyAsync(Convert.ToInt32(value));
                        }
                        else
                        {
                            var scope = (IServiceScope)context.ParentContext.RootContextData["__ServiceScope"];
                            var serviceType = typeof(IBaseService<>).MakeGenericType(info.PropertyType);
                            var service = scope.ServiceProvider.GetService(serviceType) as IBaseService;
                            result = await service.AnyAsync(Convert.ToInt32(value));
                        }
                        if (!result)
                        {
                            if (language == Language.Fa)
                                message = (displayAttr?.DisplayName ?? infoId.Name) + "ی با کد " + value + " وجود ندارد.";
                            else
                                message = $"There is no {displayAttr?.DisplayName ?? infoId.Name} with Id {value}";
                        }
                    }
                    if (message.HasValue())
                        context.AddFailure(message);
                }
            });
        }

        public static IRuleBuilderInitial<TModel, string> CallNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
        {
            return ruleBuilder.Custom((callNumber, context) =>
            {
                if (callNumber != null)
                {
                    var strCallNumber = callNumber.ToString();
                    if (strCallNumber.Length != 11 || !strCallNumber.StartsWith("0"))
                        context.AddFailure("شماره تماس باید 11 رقم باشد و با صفر شروع شود.");
                }
            });
        }

        public static IRuleBuilderInitial<TModel, TProperty> Unless<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Expression<Func<TModel, bool>> expression, string message)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                if (!expression.Compile().Invoke((TModel)context.InstanceToValidate))
                    context.AddFailure(message);
            });
        }


    }
}
