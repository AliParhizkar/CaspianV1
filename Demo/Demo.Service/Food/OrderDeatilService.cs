using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    [ReportClass]
    public class OrderDeatilService : SimpleService<OrderDeatil>, ISimpleService<OrderDeatil>
    {
        public OrderDeatilService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Price).CustomValue(t => t < 0, "قیمت نمی تواند منفی باشد");
            RuleFor(t => t.Count).CustomValue(t => t <= 0, "واحد باید بزرگتر از صفر باشد");
            RuleFor(t => t.ProductId).UniqAsync(t => t.OrderId, "این محصول در حواله وجود دارد");
        }

        [ReportMethod("گزارش فروش")]
        public IQueryable<OrderDeatil> GetReportOrderDeatils(OrderDeatil deatil)
        {
            return GetAll(deatil);
        }
    }
}
