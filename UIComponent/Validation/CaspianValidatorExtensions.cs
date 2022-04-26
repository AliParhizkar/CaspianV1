using System;
using FluentValidation;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public static class CaspianValidatorExtensions
    {
        public static IRuleBuilderInitial<TModel, TProperty> Custom<TModel, TProperty>(this IRuleBuilder<TModel, TProperty> ruleBuilder,
            Expression<Func<TModel, bool>> expression, string message)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                if (expression.Compile().Invoke((TModel)context.InstanceToValidate))
                    context.AddFailure(message);
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
