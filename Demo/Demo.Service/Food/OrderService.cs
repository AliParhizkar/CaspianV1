using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using FluentValidation;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service
{
    [ReportClass]
    public class OrderService : MasterDetailsService<Order, OrderDeatil>, IBaseService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == default, "Please specify the order date");
            RuleFor(t => t.OrderDeatils).Custom(t => t.Id == 0 && (t.OrderDeatils == null || t.OrderDeatils.Count == 0), "The order must has at leasta products");
            RuleFor(t => t.OrderStatus).Custom(t => t.CourierId.HasValue && t.OrderStatus == OrderStatus.Canceled,
                "The order has a courier and it is not possible to cancel it.");
            
            RuleForEach(t => t.OrderDeatils).ChildRules(u => 
            {
                u.RuleFor(t => t.Price).CustomValue(t => t < 0, "The price cannot be negative");
                u.RuleFor(t => t.Quantity).CustomValue(t => t <= 0, "This must be greater than 0");
                u.RuleFor(t => t.ProductId).Custom(t => Source.Any(u => u.ProductId == t.ProductId && t != u), "This product has been added to the invoice");
            });

        }

        public override async Task<Order> UpdateDatabaseAsync(Order order, IList<ChangedEntity<OrderDeatil>> changedEntities)
        {
            /// Add to factor
            var sum = changedEntities.Where(t => t.ChangeStatus == ChangeStatus.Added).Sum(t => t.Entity.Price * t.Entity.Quantity);
            if (order.Id == 0)
            {
                var maxOrderNo = GetAll().Where(t => t.Date == order.Date).Max(t => (int?)t.OrderNo).GetValueOrDefault() + 1;
                order.OrderNo = maxOrderNo;
            }
            else
            {
                var details = await GetService<OrderDeatilService>().GetAll().Where(t => t.OrderId == order.Id).AsNoTracking().ToListAsync();
                foreach (var detail in details)
                {
                    var changed = changedEntities.SingleOrDefault(t => t.Entity.Id == detail.Id);
                    /// if not changed
                    if (changed == null)
                        sum += detail.Price * detail.Quantity;
                    else if (changed.ChangeStatus == ChangeStatus.Updated)
                        sum += changed.Entity.Price * changed.Entity.Quantity;
                }
            }
            order.TotalAmount = sum;
            return await base.UpdateDatabaseAsync(order, changedEntities);
        }
    }
}
