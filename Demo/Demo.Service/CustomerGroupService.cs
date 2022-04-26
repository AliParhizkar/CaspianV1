using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class CustomerGroupService : SimpleService<CustomerGroup>, ISimpleService<CustomerGroup>
    {
        public CustomerGroupService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("مشتری با این عنوان در سیستم ثبت شده است");
        }
    }
}
