using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Demo.Service
{
    public class CustomerGroupMembershipService : DetailService<CustomerGroupMembership>
    {
        public CustomerGroupMembershipService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.CustomerId).UniqueAsync(t => t.CustomerGroupId, "The customer is already a member of this group.");
        }
    }
}
