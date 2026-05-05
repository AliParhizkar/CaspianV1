using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PolicyParameterConditionService : BaseService<PolicyParameterCondition>
    {
        public PolicyParameterConditionService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.ParameterId).UniqueAsync(t => t.PolicyId, "این پارامتر قبلا به این سیاست خرید اضافه شده است");
        }
    }
}
