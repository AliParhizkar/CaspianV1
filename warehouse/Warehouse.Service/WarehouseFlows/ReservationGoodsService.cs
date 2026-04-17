using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class ReservationGoodsService: BaseService<ReservationGoods>
    {
        public ReservationGoodsService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Quantity).CustomValue(t => t < 0, "تعداد/مقدار کالا باید بزرگتر از صفر باشد");
            RuleFor(t => t.GoodsId).Custom(t => Source.Any(u => u.GoodsId == t.GoodsId && t.Id != u.Id), "این کالا قبلا به این رزرو اضافه شده است");
        }
    }
}
