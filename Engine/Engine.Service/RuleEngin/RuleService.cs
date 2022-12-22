using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class RuleService : SimpleService<Rule>, ISimpleService<Rule>
    {
        public RuleService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("قانونی با این عنوان در سیستم ثبت شده است.");
            RuleFor(t => t.TypeName).Required();
            RuleFor(t => t.EnumTypeName).Required(t => t.ResultType == ValueTypeKind.Enum);
            RuleFor(t => t.Id).CustomAsync(async t => 
            {
                if (t.Id == 0 || !t.FormRule)
                    return false;
                var result = await new TokenService(ServiceProvider).GetAll().AnyAsync(u => u.RuleId == t.Id && !u.RuleValue.FormRule);
                return result;
            }, "قانون فرمی نباید شامل قانون غیرفرمی باشد");
        }

        public IQueryable<Rule> GetAll(SubSystemKind subSystemKind)
        {
            return GetAll().Where(t => t.SystemKind == subSystemKind);
        }

        public async Task<ValueTypeKind?> NextTokeValueType(IList<Token> tokens, int ruleId)
        {
            if (tokens == null || tokens.Count == 0)
            {
                var rule = await SingleAsync(ruleId);
                return rule.ResultType;
            }
            var lastToken = tokens[tokens.Count - 1];
            var beforeLastToke = tokens[tokens.Count - 2];
            switch(lastToken.OperatorKind)
            {
                case OperatorType.If:
                    return null;
                case OperatorType.Plus:
                case OperatorType.Minus:
                case OperatorType.Stra:
                case OperatorType.Slash:
                    return ValueTypeKind.Double;
                case OperatorType.BTE:
                case OperatorType.BT:
                case OperatorType.LT:
                case OperatorType.LTE:
                case OperatorType.Equ:
                case OperatorType.NotEqu:
                    if (beforeLastToke.ConstValueType == ValueTypeKind.Enum)
                        return ValueTypeKind.Enum;
                    return ValueTypeKind.Double;
                default:
                    throw new NotFiniteNumberException("خطای عدم پیاده سازی");

            }
        }

        public int? CalculateRulePriority(Rule rule)
        {
            int? maxPriority = null;
            foreach (var token in rule.Tokens)
            {
                switch (token.parameterType)
                {
                    case null:
                    case ParameterType.MainParameter:
                        break;
                    case ParameterType.DaynamicParameter:
                        var param = token.DynamicParameter;
                        switch(param.CalculationType)
                        {
                            case CalculationType.UserData:
                                break;
                            case CalculationType.FormData:
                                if (token.DynamicParameter.Priority == null)
                                    return null;
                                if (token.DynamicParameter.Priority.Value > maxPriority.GetValueOrDefault(0))
                                    maxPriority = token.DynamicParameter.Priority.Value; 
                                break;
                        }
                        break;
                    case ParameterType.RuleParameter:
                        if (token.RuleValue.Priority == null)
                            return null;
                        if (token.RuleValue.Priority.Value > maxPriority.GetValueOrDefault(0))
                            maxPriority = (int)token.RuleValue.Priority.Value;
                        break;

                }
            }
            if (maxPriority == null)
                return 1;
            return maxPriority.Value + 1;
        }
    }
}
