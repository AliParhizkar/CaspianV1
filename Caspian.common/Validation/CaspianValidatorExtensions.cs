using FluentValidation;
using System.Reflection;
using System.ComponentModel;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace Caspian.Common
{
    public static class CaspianValidatorExtensions
    {
        public static IRuleBuilderOptionsConditions<TModel, TProperty> Custom<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Func<TModel, bool> func, string message)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                if (func.Invoke((TModel)context.InstanceToValidate))
                    context.AddFailure(message);
            });
        }

        public static IRuleBuilderOptionsConditions<TModel, TProperty> CustomAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Func<TModel, Task<bool>> func, string message)
        {
            return ruleBuilder.CustomAsync(async (value, context, token) =>
            {
                if (await func.Invoke((TModel)context.InstanceToValidate))
                    context.AddFailure(message);
            });
        }

        public static IRuleBuilderOptionsConditions<TModel, TProperty> CustomValue<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Func<TProperty, bool> func, string message)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                if (func.Invoke(value))
                    context.AddFailure(message);
            });
        }

        public static Language GetLanguage<TModel>(this ValidationContext<TModel> context)
        {
            Language language = Language.En;
            if (context.RootContextData.ContainsKey("__ServiceScope"))
            {
                var provider = context.RootContextData["__ServiceScope"] as IServiceProvider;
                var service = provider.GetService<CaspianDataService>();
                language = service.Language ?? Language.Fa;
            }
            return language;
        }

        public static IRuleBuilderOptionsConditions<TModel, TProperty> Required<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, 
            Func<TModel, bool> func = null, string message = null)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                
                var language = context.GetLanguage();
                if (value is string && Convert.ToString(value) == "")
                    value = default(TProperty);
                if ((value == null) && func?.Invoke((TModel)context.InstanceToValidate) != false)
                {
                    var attr = typeof(TModel).GetMyProperty(context.PropertyPath).GetCustomAttribute<DisplayNameAttribute>();
                    var name = attr == null ? context.DisplayName : attr.DisplayName;
                    if (language == Language.Fa)
                        message = message ?? $"لطفا {name} را مشخص نمایید";
                    else
                        message = message ?? $"Please specify the {name}";
                    context.AddFailure(message);
                }
            });
        }

        public static IRuleBuilderOptionsConditions<TModel, TProperty> Range<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, TProperty min, TProperty max, string message = null) where TProperty : IComparable
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

        public static IRuleBuilderOptionsConditions<TModel, string> MobileNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
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

        public static IRuleBuilderOptionsConditions<TModel, string> TelNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
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

        public static IRuleBuilderOptionsConditions<TModel, TProperty> ShortTelNumber<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder)
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

        public static IRuleBuilderOptionsConditions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            string errorMessage) where TModel: class
        {
            return ruleBuilder.UniqAsync(null, null, null, errorMessage);
        }

        public static IRuleBuilderOptionsConditions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Expression<Func<TModel, object>> expr1, string errorMessage) where TModel: class
        {
            return ruleBuilder.UniqAsync(expr1, null, null, errorMessage);
        }

        public static IRuleBuilderOptionsConditions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Expression<Func<TModel, object>> expr1, Expression<Func<TModel, object>> expr2, string errorMessage) where TModel: class
        {
            return ruleBuilder.UniqAsync(expr1, expr2, null, errorMessage);
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

        public static IRuleBuilderOptionsConditions<TModel, TProperty> UniqAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, 
            Expression<Func<TModel, object>> expr1, Expression<Func<TModel, object>> expr2,
            Expression<Func<TModel, object>> expr3, string errorMessage) where TModel:class
        {
            return ruleBuilder.CustomAsync(async (value, context, token) =>
            {
                if (value == null)
                    return;
                var model = context.InstanceToValidate;
                
                var scope1 = (IServiceScope)context.RootContextData["__ServiceScope"];
                var param = Expression.Parameter(typeof(TModel), "t");
                var path = context.PropertyPath;
                if (context.PropertyChain.Count > 0)
                    path = path.Substring(context.PropertyChain.ToString().Length + 1);
                Expression expr = param.CreateMemberExpresion(path);
                Expression left = await CreateExpression(param, model, expr, scope1);
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
                var isValid = false;
                Expression<Func<TModel, bool>> lambda = Expression.Lambda(left, param) as Expression<Func<TModel, bool>>;
                if (context.RootContextData.ContainsKey("__ServiceScopeFactory"))
                {
                    using var scope = ((IServiceScopeFactory)context.RootContextData["__ServiceScopeFactory"])
                        .CreateScope();
                    var service = scope.GetService<BaseService<TModel>>();
                    isValid = !await service.GetAll(default(TModel)).AnyAsync(lambda);
                }
                var service1 = scope1.GetService<BaseService<TModel>>();
                isValid = !await service1.GetAll(default(TModel)).AnyAsync(lambda);
                if (!isValid)
                    context.AddFailure(errorMessage);
            });
        }

        public static void CheckForeignKeyOnRemove<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder)
        {
            ruleBuilder.CustomAsync(async (value, context, token) =>
            {
                BatchServiceData batchService = null;
                if (context.RootContextData.ContainsKey("__BatchServiceData"))
                    batchService = context.RootContextData["__BatchServiceData"] as BatchServiceData;
                foreach (var info in typeof(TModel).GetProperties())
                {
                    if (info.PropertyType.IsEnumerableType() && info.PropertyType != typeof(string) && info.PropertyType != typeof(byte[]))
                    {
                        var attr = info.GetCustomAttribute<CheckOnDeleteAttribute>();
                        if (attr == null)
                            throw new CaspianException("خطا: On type " + info.DeclaringType.Name + " property " + info.Name + " must has CheckOnDelete Attribute", 5);
                        if (!attr.Check)
                            continue;
                        if (batchService != null && batchService.DetailPropertiesInfo.Contains(info))
                            continue;
                        var type = info.PropertyType.GetGenericArguments()[0];
                        var pkey = type.GetPrimaryKey();
                        var foreignKey = pkey.GetCustomAttribute<ForeignKeyAttribute>();
                        if (foreignKey != null)
                        {
                            type = type.GetProperty(foreignKey.Name).PropertyType;
                        }
                        else
                        {
                            var info1 = type.GetForeignKey(typeof(TModel));
                            var paramExpr = Expression.Parameter(type);
                            Expression expr = Expression.Property(paramExpr, info1);
                            if (info1.PropertyType.IsNullableType())
                                expr = Expression.Property(expr, "Value");
                            expr = Expression.Equal(expr, Expression.Constant(value));
                            var lambda = Expression.Lambda(expr, paramExpr);
                            var serviceType = typeof(BaseService<>).MakeGenericType(type);
                            var scope1 = (IServiceScope)context.RootContextData["__ServiceScope"];
                            var service = Activator.CreateInstance(serviceType, scope1.ServiceProvider) as IBaseService;
                            var hasDetails = await service.GetAllRecords().Where(lambda).OfType<object>().AnyAsync();
                            if (hasDetails)
                            {
                                context.AddFailure(attr.ErrorMessage);
                                break;
                            }
                        }
                    }
                }
            });
        }

        public static void CheckEnum<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, PropertyInfo info)
        {
            ruleBuilder.Custom((value, context) =>
            {
                if (value != null)
                {
                    bool isValid = false, isBitwise = false; ; var tempValue = Convert.ToInt64(value);
                    if (Enum.IsDefined(info.PropertyType, value))
                        isValid = true;
                    else
                    {
                        var attr = info.PropertyType.GetUnderlyingType().GetCustomAttribute<EnumTypeAttribute>();
                        isBitwise = attr?.IsBitwise == true;
                        if (isBitwise)
                        {
                            var fields = info.PropertyType.GetFields().Where(t => !t.IsSpecialName);
                            var max = Convert.ToInt64(fields.Max(t => t.GetValue(null)));
                            if (tempValue >= 0 && tempValue < 2 * max)
                                isValid = true;
                        }
                    }
                    if (!isValid)
                    {
                        var attr = info.GetCustomAttribute<DisplayNameAttribute>();
                        var name = attr?.DisplayName ?? info.Name;
                        string message = null;
                        var language = context.GetLanguage();
                        if (tempValue == 0)
                        {
                            if (!isBitwise)
                            {
                                if (language == Language.En)
                                    message = attr == null ? $"Please specify the value of the field{name}" : $"Please specify {name}";
                                else
                                    message = attr == null ? "" : $"لطفا {name} را مشخص نمایید.";
                            }
                        }
                        else
                        {
                            if (language == Language.Fa)
                                message = $"مقدار {tempValue} برای نوع شمارشی {info.PropertyType.Name} نامعتبر است";
                            else
                                message = $"Value {tempValue} is invalid for {info.PropertyType.Name}";
                        }
                        context.AddFailure(message);
                    }
                }
            });
        }

        public static void CheckForeignKeyAsync<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder, PropertyInfo info, PropertyInfo infoId) where TModel : class
        {
            ruleBuilder.CustomAsync(async (value, context, token) =>
            {
                if (value != null)
                {
                    var language = context.GetLanguage();
                    var displayAttr = info.GetCustomAttribute<DisplayNameAttribute>() ?? infoId.GetCustomAttribute<DisplayNameAttribute>();
                    string message = null;
                    if (value.Equals(0))
                    {
                        var flag = false;
                        if (context.RootContextData.ContainsKey("__BatchServiceData"))
                        {
                            var serviceData = context.RootContextData["__BatchServiceData"] as BatchServiceData;
                            if (serviceData.MasterId == 0)
                            {
                                var MasterInfo = typeof(TModel).GetProperties().SingleOrDefault(t => t.PropertyType == serviceData.MasterType);
                                if (MasterInfo != null && MasterInfo == info)
                                    flag = true;
                            }
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
                        if (context.RootContextData.ContainsKey("__ServiceScopeFactory"))
                        {
                            using var scope = ((IServiceScopeFactory)context.RootContextData["__ServiceScopeFactory"]).CreateScope();
                            var serviceType = typeof(IBaseService<>).MakeGenericType(info.PropertyType);
                            var service = scope.ServiceProvider.GetService(serviceType) as IBaseService;
                            result = await service.AnyAsync(Convert.ToInt32(value));
                        }
                        else
                        {
                            var scope = (IServiceScope)context.RootContextData["__ServiceScope"];
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

        public static IRuleBuilderOptionsConditions<TModel, string> CallNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
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

        public static IRuleBuilderOptionsConditions<TModel, TProperty> Unless<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
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
