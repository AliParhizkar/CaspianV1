using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class GoodsOrderingService:BaseService<GoodsOrdering>
    {
        public GoodsOrderingService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.KeepCenterId).CustomAsync(async t =>
            {
                var service = provider.GetCaspianService<GoodsPlacementService>();
                if (!await service.AnyAsync(u => u.StockRoom.KeepCenterId == t.KeepCenterId && u.GoodsId == t.GoodsId))
                    return "این کالا در این مرکز نگهداری، نگهداری نمی شود";
                return null;
            });
            RuleFor(t => t.MaximumInventory).Custom(t =>
            {
                if (t.MaximumInventory < t.MinimumInventory)
                    return "مداکثر موجودی نباید کوچکتر از حداقل موجودی باشد";
                return null;
            });
        }
    }
}
