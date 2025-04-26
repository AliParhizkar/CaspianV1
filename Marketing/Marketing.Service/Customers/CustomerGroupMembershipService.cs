using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CustomerGroupMembershipService : BaseService<CustomerGroupMembership>, IBaseService<CustomerGroupMembership>
    {
        public CustomerGroupMembershipService(IServiceProvider provider)
            : base(provider)
        {

        }
    }
}
