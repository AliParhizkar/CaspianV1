using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Demo.Service
{
    public class CustomerGroupService : BaseService<CustomerGroup>, IBaseService<CustomerGroup>
    {
        public CustomerGroupService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("مشتری با این عنوان در سیستم ثبت شده است");
        }
    }
}
