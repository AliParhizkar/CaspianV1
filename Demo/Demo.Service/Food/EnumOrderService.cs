using System;
using Demo.Model;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class EnumOrderService : BaseService<Order>, IBaseService<Order>
    {
        public EnumOrderService(IServiceProvider provider)
            :base(provider)
        {
            
        }
    }
}
