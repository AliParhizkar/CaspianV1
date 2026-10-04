using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;
using System.Collections.Generic;

namespace Demo.Service
{
    [ReportClass]
    public class OrderDetailService : DetailService<OrderDetail>
    {
        public OrderDetailService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Price).CustomValue(t => t < 0, "The price cannot be negative");
            RuleFor(t => t.Quantity).CustomValue(t => t <= 0, "This must be greater than 0");
            RuleFor(t => t.ProductId).Custom(t =>
            {
                if (Source != null && Source.Any(u => u.ProductId == t.ProductId && t.Id != u.Id))
                    return "This product has been added to the invoice";
                return null;
            });
        }

        public IQueryable<OrderDetail> GetReportOrderDetails(OrderDetail orderDetail)
        {
            return GetAll();
        }
    }
}
