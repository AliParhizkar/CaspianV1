using Procurement.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class UrgentPurchaseService: BaseService<UrgentPurchase>
    {
        public UrgentPurchaseService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("فوریت خریدی با این عنوان در سیستم ثبت شده است");
        }
    }
}
