using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;

namespace Demo.Service
{
    [ReportClass]
    public class OrderDeatilService : BaseService<OrderDeatil>, IBaseService<OrderDeatil>
    {
        public OrderDeatilService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Price).CustomValue(t => t < 0, "The price cannot be negative");
            RuleFor(t => t.Quantity).CustomValue(t => t <= 0, "The unit must be greater than zero");
            RuleFor(t => t.ProductId).Custom(t => Source != null && Source.Any(u => u.ProductId == t.ProductId && t != u), "This product has been added to the invoice");
        }

        public IQueryable<OrderDeatil> GetReportOrderDeatils(OrderDeatil orderDeatil)
        {
            return GetAll();
        }
    }
}
