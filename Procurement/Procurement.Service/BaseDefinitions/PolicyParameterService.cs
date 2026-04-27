using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PolicyParameterService: BaseService<PolicyParameter>
    {
        public PolicyParameterService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
