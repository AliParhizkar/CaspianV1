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
                return (t.OrderDetailToppings ?? new List<OrderDetailTopping>()).Sum(u => u.Quantity * u.Price) != t.ToppingAmount;
            }, "جمع کل تاپینگ درست محاسبه نشده است");
        }
    }
}
