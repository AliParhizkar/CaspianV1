﻿using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class CustomerGroupMembershipService : BaseService<CustomerGroupMembership>, IBaseService<CustomerGroupMembership>
    {
        public CustomerGroupMembershipService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.CustomerId).UniqAsync(t => t.CustomerGroupId, "مشتری در حال حاضر عضو این گروه می باشد.");
        }
    }
}
