using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class OrderService : MasterDetailsService<Order, OrderDetail>, IBaseService<Order>
    {
        public OrderService(IServiceProvider provider)
            :base(provider) 
        {
            RuleForEach(t => t.OrderDetails).SetValidator(new OrderDetailService(provider));
        }

        public override Task<Order> AddAsync(Order entity)
        {
            ///
            entity.OrderDate = DateTime.Now.GetDateOnly();
            var date = DateTime.Now.TimeOfDay > ConfigService.Config.OpenTime.ToTimeSpan() ? entity.OrderDate :
                entity.OrderDate.AddDays(-1);
            var orderNumberId = GetAll().Where(t => t.OrderDate == date).Max(t => (int?)t.OrderNumber).GetValueOrDefault() + 1;
            entity.OrderNumber = orderNumberId;
            return base.AddAsync(entity);
        }

    }
}
