using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class OrderService : MasterDetailsService<Order, OrderDetail>, IBaseService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider) 
        {
            RuleForEach(t => t.OrderDetails).SetValidator(new OrderDetailService(provider));
        }
    }
}
