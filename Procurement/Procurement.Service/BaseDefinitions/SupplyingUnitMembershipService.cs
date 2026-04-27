using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplyingUnitMembershipService : BaseService<SupplyingUnitMembership>
    {
        public SupplyingUnitMembershipService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.PurchasingSpecialistId).UniqueAsync(t => t.SupplyingUnitId, "این کارشناس خرید در خال حاضر عضو واحد تامین می باشد");
        }
    }
}
