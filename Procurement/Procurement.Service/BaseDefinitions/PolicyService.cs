using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PolicyService: MasterDetailsService<Policy, PolicyParameter>
    {
        public PolicyService(IServiceProvider provider)
            :base(provider)
        {

            RuleForEach(t => t.Parameters).SetValidator(new PolicyParameterService(provider));
        }
    }
}
