using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    //public class GoodsRequestService : MasterDetailsService<GoodsRequest, RequestGoods>
    //{
    //    public GoodsRequestService(IServiceProvider provider)
    //        : base(provider)
    //    {
    //        RuleFor(t => t.CostCenterId).Required(t => t.OtherPartyType == OtherPartyType.CostCenter);
    //        RuleFor(t => t.SupplierId).Required(t => t.OtherPartyType == OtherPartyType.PersonOrCompany);
    //        RuleForEach(t => t.Goods).SetValidator(new RequestGoodsService(provider));
    //    }
    //}
}
