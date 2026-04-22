using Procurement.Model;
using Caspian.Common.Service;
using Caspian.Common;

namespace Procurement.Service
{
    public class SupplyingUnitService: MasterDetailsService<SupplyingUnit, SupplyingUnitMembership>
    {

        public SupplyingUnitService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required()
        }
    }
}
