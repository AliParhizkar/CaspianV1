using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CustomerGroupService : BaseService<CustomerGroup>, IBaseService<CustomerGroup>
    {
        public CustomerGroupService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("گروه مشتری با این عنوان در سیستم ثبت شده است.");
        }
    }
}
