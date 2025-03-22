using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class OrderService : BaseService<Order>, IBaseService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider) 
        {
            
        }
    }
}
