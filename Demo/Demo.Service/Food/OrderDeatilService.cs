using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Xml;
using System;

namespace Demo.Service
{
    [ReportClass]
    public class OrderDeatilService : SimpleService<OrderDeatil>, ISimpleService<OrderDeatil>
    {
        public OrderDeatilService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Price).CustomValue(t => t < 0, "The price cannot be negative");
            RuleFor(t => t.Count).CustomValue(t => t <= 0, "The unit must be greater than zero");
            RuleFor(t => t.ProductId).Custom(t => Source != null && Source.Any(u => u.ProductId == t.ProductId && t != u), "This product has been added to the invoice");
        }

        [ReportMethod("گزارش فروش")]
        public IQueryable<OrderDeatil> GetReportOrderDeatils(OrderDeatil deatil)
        {
            return GetAll(deatil);
        }
    }
}
