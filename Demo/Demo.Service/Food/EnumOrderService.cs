using System;
using Demo.Model;
using Caspian.Common;

namespace Demo.Service
{
    public class EnumOrderService : CaspianValidator<Order>
    {
        public EnumOrderService(IServiceProvider provider)
            :base(provider)
        {
            
        }
    }
}
