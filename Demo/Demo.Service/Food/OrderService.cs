using Demo.Model;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.Validators;
using System.Xml;
using System;

namespace Demo.Service
{
    [ReportClass]
    public class OrderService : SimpleService<Order>, ISimpleService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Date).CustomValue(t => t == null, "لطفا تاریخ سفارش را مشخص نمائید");
            RuleFor(t => t.OrderDeatils).CustomValue(t => t == null || t.Count == 0, "سفارش باید حداقل یک محصول داشته باشد");
            RuleForEach(t => t.OrderDeatils)
                ///In order and orderdetails insert OrderId for type orderdetail is not recognized and for ForeignKey must ignored
                ///Important: First must Ignore Foreign Key then set validator
                .IgnoreForeignKey(t => t.Property(u => u.OrderId).Condition(u => u.Id == 0))
                .SetValidator(new OrderDeatilService(provider));
        }
    }
}
