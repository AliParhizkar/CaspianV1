using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PurchaseRequestService : MasterDetailsService<PurchaseRequest, PurchaseRequestGoods>
    {
        public PurchaseRequestService(IServiceProvider provider)
            :base(provider)
        {
            RuleForEach(t => t.Goods).SetValidator(new PurchaseRequestGoodsService(provider));
        }
    }
}
