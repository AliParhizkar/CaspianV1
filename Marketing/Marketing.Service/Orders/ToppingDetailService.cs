using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Service
{
    public class OrderToppingService: MasterDetailsService<OrderDetail, OrderDetailTopping>, IBaseService<OrderDetail>
    {
        public OrderToppingService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.ToppingAmount).CustomAsync(async t =>
            {
                var sumAddedToppings = ChangedEntities.Where(t => t.ChangeStatus == ChangeStatus.Added)
                .Sum(t => t.Entity.Quantity * t.Entity.Price);
                /// In update Mode (we have added, updated & deleted toppings)
                if (t.Id > 0)
                {
                    /// updated and deleted changes
                    var oldIds = ChangedEntities.Where(t => t.ChangeStatus != ChangeStatus.Added).Select(t => t.Entity.Id).ToList();
                    /// sum of toppings that not changed(deleted or updated)
                    var sumUnChangedToppings = await provider.GetCaspianService<OrderDetailToppingService>().GetAll()
                    .Where(u => u.OrderDetailId == t.Id && !oldIds.Contains(u.Id)).SumAsync(u => u.Price * u.Quantity);
                    
                    ///sum updated toppings
                    var sumUpdateToppings = ChangedEntities.Where(t => t.ChangeStatus == ChangeStatus.Updated)
                    .Sum(t => t.Entity.Quantity * t.Entity.Price);
                    return sumUnChangedToppings + sumUpdateToppings + sumUpdateToppings != t.ToppingAmount;
                }
                /// In insert Mode (we only have added toppings)
                return t.ToppingAmount != sumAddedToppings;
            }, "جمع تاپینگ درست محاسبه نشده است");
        }
    }
}
