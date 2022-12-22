using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Demo.Service
{
    public class CustomerGroupMembershipService : SimpleService<CustomerGroupMembership>, ISimpleService<CustomerGroupMembership>
    {
        public CustomerGroupMembershipService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.CustomerId).UniqAsync(t => t.CustomerGroupId, "مشتری در حال حاضر عضو این گروه می باشد.");
        }
    }
}
