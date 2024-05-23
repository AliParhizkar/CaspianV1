using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using FluentValidation;
using Caspian.Common.Service;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Caspian.Common.Extension;

namespace Demo.Service
{
    [ReportClass]
    public class OrderService : MasterDetailsService<Order, OrderDeatil>, IBaseService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == null, "Please specify the order date");
            RuleFor(t => t.OrderDeatils).Custom(t => t.Id == 0 && (t.OrderDeatils == null || t.OrderDeatils.Count == 0), "The order must has at leasta products");
            RuleForEach(t => t.OrderDeatils).SetValidator(new OrderDeatilService(provider));
            RuleFor(t => t.OrderStatus).Custom(t => t.CourierId.HasValue && t.OrderStatus == OrderStatus.Canceled,
                "The order has a courier and it is not possible to cancel it.");

        }

        public override Task<Order> AddAsync(Order order)
        {
            var maxOrderNo = GetAll().Where(t => t.Date == order.Date).Max(t => (int?)t.OrderNo).GetValueOrDefault() + 1;
            order.OrderNo = maxOrderNo;
            order.TotalAmount = order.OrderDeatils.Sum(t => t.Price * t.Quantity);
            return base.AddAsync(order);
        }

        public override async Task UpdateAsync(Order order, IEnumerable<int> deletedIds)
        {
            var old = await GetAll().Include(t => t.OrderDeatils).SingleAsync(order.Id);
            var sum = 0;
            foreach(var item in old.OrderDeatils.Where(t => !deletedIds.Contains(t.Id)))
            {
                var detail = order.OrderDeatils.SingleOrDefault(t => t.Id == item.Id);
                sum += detail == null ? item.Quantity * item.Price : detail.Quantity * detail.Price;
            }
            order.TotalAmount = sum;
            await base.UpdateAsync(order, deletedIds);
        }
    }
}
