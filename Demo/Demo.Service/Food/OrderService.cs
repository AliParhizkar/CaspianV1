using Demo.Model;
using Caspian.Common;
using Caspian.Engine;
using FluentValidation;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    [ReportClass]
    public class OrderService : SimpleService<Order>, ISimpleService<Order>
    {
        public OrderService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Date).CustomValue(t => { return t == null; }, "لطفا تاریخ سفارش را مشخص نمائید");
            RuleForEach(t => t.OrderDeatils).SetValidator(new OrderDeatilService(scope))
            .When(t =>
            {
                return !IgnoreDetailsProperty;
            });
        }

        /// <summary>
        /// مشتری مبلغ زیادی خرید نموده است
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [Task("مشتری VIP")]
        public bool CheckOrder(Order order)
        {
            return true;
        }

        /// <summary>
        /// مشتری سفارشهای زیادی داشته است
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [Task("مشتری پرخرید")]
        public bool CheckOrder1(Order order)
        {
            return false;
        }
    }
}
