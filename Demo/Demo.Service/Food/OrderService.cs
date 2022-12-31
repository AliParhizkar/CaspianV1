﻿using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;
using System.Threading.Tasks;

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
            RuleFor(t => t.OrderStatus).Custom(t => t.DeliveryId.HasValue && t.OrderStatus == OrderStatus.Canceled,
                "سفارش دارای پیک می باشد و امکان لغو آن وجود ندارد");
        }

        public override Task<Order> AddAsync(Order entity)
        {
            var maxOrderNo = GetAll().Where(t => t.Date == entity.Date).Max(t => (int?)t.OrderNo).GetValueOrDefault() + 1;
            entity.OrderNo = maxOrderNo;
            return base.AddAsync(entity);
        }

        public override async Task UpdateAsync(Order entity)
        {
            var old = await SingleAsync(entity.Id);
            entity.OrderNo = old.OrderNo;
            await base.UpdateAsync(entity);
        }
    }
}
