using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PolicyService: BaseService<Policy>
    {
        public PolicyService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("سیاست خرید با این عنوان در سیستم وجود دارد");
            RuleFor(t => t.DocumentKind).Custom(t => t.DocumentKind.ConvertToInt() == 0, "حداقل باید یک نوع سند باید انتخاب شود");
        }
    }
}
