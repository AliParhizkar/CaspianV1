using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplierGroupMembershipService: BaseService<SupplierGroupMembership>
    {
        public SupplierGroupMembershipService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.SupplierGroupId).UniqueAsync(t => t.SupplierId, "در حال حاضر تامین کننده عضو گروه می باشد");
        }
    }
}
