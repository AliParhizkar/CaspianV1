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
            RuleForEach(t => t.OrderDetails).SetValidator(t => new OrderDetailService(provider));
            RuleFor(t => t.OrderDetails).Custom(t => t.OrderDetails == null || !t.OrderDetails.Any(), "سفارش باید حداقل یک محصول داشته باشد.");
            RuleFor(t => t.ProductAmount).Custom(t =>
            {
                return Details.Sum(u => u.Quantity * (u.Price - u.Discount + u.ToppingAmount)) != t.ProductAmount;
            }, "جمع محصول درست محاسبه نشده است");
            RuleFor(t => t.DiscountAmount).Custom(t =>
            {
                if (t.DiscountType == DiscountType.Amount)
                    return false;
                return t.DiscountAmount != t.PercentDiscount * t.ProductAmount / 100;
            }, "تخفیف درصدی درست محاسبه نشده است");
            RuleFor(t => t.RoundAmount).Custom(t =>
            {
                if (ConfigService.Config.RoundType == null)
                    return false;
                return t.RoundAmount != ConfigService.GetRoundValue(t.ProductAmount - t.DiscountAmount);
            }, "مبلغ رند درست محاسبه نشده است");
            RuleFor(t => t.PaymentAmount).Custom(t =>
            {
                return t.PaymentAmount != t.ProductAmount - t.DiscountAmount + t.RoundAmount.GetValueOrDefault();
            }, "جمع کل پرداختی درست محاسبه نشده است");
            RuleFor(t => t.PaymentAmount).Custom(t =>
            {
                if (!t.IsSettled)
                    return false;
                return t.PaymentAmount != t.CardAmount + t.CashAmount + t.AccountingAmount;
            }, "پرداخت بصورت کامل انجام نشده است");
        }

        public override Task<Order> UpdateDatabaseAsync(Order entity, IList<ChangedEntity<OrderDetail>> changedEntities)
        {
            return base.UpdateDatabaseAsync(entity, changedEntities);

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
