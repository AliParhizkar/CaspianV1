using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class SellerService : BaseService<Seller>
    {
        public SellerService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.EconomicCode).Required().UniqueAsync("فروشنده ای با این کد اقتصادی در سیستم ثبت شده است");
        }
    }
}
