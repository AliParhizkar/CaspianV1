using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Warehouse.Service
{
    public class GoodsPlacementService: BaseService<GoodsPlacement>
    {
        public GoodsPlacementService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.LocationId).CustomAsync(async t =>
            {
                if (t.LocationId == null)
                    return null;
                if (await GetAll().AnyAsync(u => u.GoodsId == t.GoodsId && u.LocationId == t.LocationId && u.Id != t.Id))
                    return "کالا قبلا به این محل فیزیکی تخصیص داده شده است";
                var old = await GetService<MaterialLocationService>().SingleAsync(t.LocationId.Value);
                if (old.StockroomId != t.StockRoomId)
                    return "محل ذخیره کالا به این انبار تعلق ندارد";
                return null;
            });
            RuleFor(t => t.StockRoomId).CustomAsync(async t =>
            {
                if (t.LocationId != null)
                    return null;
                if (await GetAll().AnyAsync(u => u.LocationId == null && u.StockRoomId == t.StockRoomId && u.GoodsId == t.GoodsId))
                    return "کالا قبلا به این انبار تخصیص داده شده است";
                return null;
            });
        }
    }
}
