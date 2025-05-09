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
            RuleFor(t => t.ToppingAmount).Custom(t =>
            {
                if (t.OrderDetailToppings == null)
                    return false;
                return t.OrderDetailToppings.Sum(u => u.Quantity * u.Price) != t.ToppingAmount;
            }, "جمع کل تاپینگ درست مقداردهی نشده است");
        }
    }
}
