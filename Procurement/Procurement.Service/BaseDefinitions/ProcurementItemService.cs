using Caspian.Common;
using Warehouse.Service;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class ProcurementItemService : BaseService<ProcurementItem>
    {
        public ProcurementItemService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.GoodsId).Required(t => t.ProcurementItemType == ProcurementItemType.Goods)
                .UniqueAsync("کالا در حال حاضر بعنوان قلم خریدنی ثبت شده است");
            RuleFor(t => t.ServiceId).Required(t => t.ProcurementItemType == ProcurementItemType.Service)
                .UniqueAsync("این خدمت در حال حاضر بعنوان قلم خریدنی ثبت شده است");
            RuleFor(t => t.GoodsId).CustomAsync(async t =>
            {
                if (t.GoodsId.HasValue)
                {
                    var goods = await provider.GetCaspianService<GoodsService>().SingleAsync(t.GoodsId.Value);
                    if (!goods.IsActive)
                        return "فقط کالاهای فعال می توانند بعنوان قلم خریدنی باشند";
                    if (goods.GoodsType != Warehouse.Model.GoodsType.Salable)
                        return "فقط کالاهای خریدنی می توانند بعنوان قلم خریدنی باشند";
                }
                return null;
            });
        }
    }
}
