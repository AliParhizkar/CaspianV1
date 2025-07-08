using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using FluentValidation;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service
{
    [ReportClass]
    public class OrderService : MasterDetailsService<Order, OrderDetail>, IBaseService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == default, "Please specify the order date");
            RuleFor(t => t.TotalAmount).Custom(t => Details.Sum(t => t.Quantity * t.Price) != t.TotalAmount, "Total amount is incorect");
            RuleFor(t => t.OrderDetails).Custom(t => 
            {
                return Details.Count() == 0;
            }, "The order must has at least products");
            RuleFor(t => t.OrderStatus).Custom(t => t.CourierId.HasValue && t.OrderStatus == OrderStatus.Canceled,
                "The order has a courier and it is not possible to cancel it.");
            RuleForEach(t => t.OrderDetails).SetValidator(new OrderDetailService(provider, Details.ToList()));
        }

        public override async Task<Order> AddAsync(Order order)
        {
            var todaysDate = DateTime.Now.ToDateOnly();
            var maxOrderNo = await GetAll().Where(t => t.Date == todaysDate).MaxAsync(t => t.OrderNo);
            order.OrderNo = maxOrderNo.GetValueOrDefault() + 1;
            order.TotalAmount = ChangedEntities.Where(t => t.ChangeStatus == ChangeStatus.Added)
                .Sum(t => t.Entity.Price * t.Entity.Quantity); ;
            return await base.AddAsync(order);
        }

        public override async Task UpdateAsync(Order order)
        {
            var sum = ChangedEntities.Where(t => t.ChangeStatus == ChangeStatus.Added).Sum(t => t.Entity.Price * t.Entity.Quantity);
            var details = await GetService<OrderDetailService>().GetAll().Where(t => t.OrderId == order.Id).AsNoTracking().ToListAsync();
            foreach (var detail in details)
            {
                var changed = ChangedEntities.SingleOrDefault(t => t.Entity.Id == detail.Id);
                /// if not changed
                if (changed == null)
                    sum += detail.Price * detail.Quantity;
                else if (changed.ChangeStatus == ChangeStatus.Updated)
                    sum += changed.Entity.Price * changed.Entity.Quantity;
            }
            order.TotalAmount = sum;
            await base.UpdateAsync(order);
        }
    }
}
