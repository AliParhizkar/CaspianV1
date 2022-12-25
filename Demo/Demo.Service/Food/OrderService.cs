using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;

namespace Demo.Service
{
    [ReportClass]
    public class OrderService : SimpleService<Order>, ISimpleService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == null, "لطفا تاریخ سفارش را مشخص نمائید");
            RuleFor(t => t.OrderDeatils).CustomValue(t => t == null || !t.Any(), "سفارش باید حداقل یک محصول داشته باشد");
            RuleForEach(t => t.OrderDeatils).SetValidator(new OrderDeatilService(provider));
        }
    }
}
