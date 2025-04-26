using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class OrderDetailService : BaseService<OrderDetail>, IBaseService<OrderDetail>
    {
        public OrderDetailService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.ProductId).UniqueAsync(t => t.OrderId, "این محصول در حال حاضر به سفارش اضافه شده است");
            
        }
    }
}
