using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class StockRoomService : BaseService<StockRoom>
    {
        public StockRoomService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("شهری با این عنوان در سیستم ثبت شده است");
        }
    }
}
