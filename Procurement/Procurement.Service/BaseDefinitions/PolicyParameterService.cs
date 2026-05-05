using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PolicyParameterService: BaseService<PolicyParameter>
    {
        public PolicyParameterService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("پارامتری با این عنوان در سیستم تعریف شده است");
        }
    }
}
