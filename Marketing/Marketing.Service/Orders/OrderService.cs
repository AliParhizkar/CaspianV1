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
            RuleFor(t => t.OrderDetails).Custom(t => !Details.Any(), "سفارش باید حداقل یک محصول داشته باشد.");
            RuleFor(t => t.ProductAmount).Custom(t =>
            {
                var sum = Details.Sum(u => u.Quantity * (u.Price - u.Discount + u.ToppingAmount));
                return sum != t.ProductAmount;
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
            RuleFor(t => t.IsSettled).Custom(t =>
            {
                if (!t.IsSettled)
                    return false;
                return t.PaymentAmount != t.CardAmount + t.CashAmount + t.AccountingAmount;
            }, "پرداخت بصورت کامل انجام نشده است");
            RuleFor(t => t.AccountingAmount).CustomAsync(async t =>
            {
                if (t.AccountingAmount > 0)
                {
                    if (t.CustomerId == null)
                        return "برای پرداخت از حساب، مشتری باید مشخص باشد.";
                    var customer = await provider.GetCaspianService<CustomerService>().SingleAsync(t.Id);
                    if (customer.AccountBalance < t.AccountingAmount && !customer.NegativeBalance)
                        return "موجودی مشتری کافی نیست، و مشتری نمی تواند موجودی منفی داشته باشد";
                }
                return null;
            });
        }

        public override Task<Order> UpdateDatabaseAsync(Order entity, IList<ChangedEntity<OrderDetail>> changedEntities)
        {
            return base.UpdateDatabaseAsync(entity, changedEntities);

        }

        public override Task<Order> AddAsync(Order entity)
        {
            entity.OrderDate = DateTime.Now.GetDateOnly();
            entity.OrderTime = DateTime.Now.ToTimeOnly();
            var query = GetAll();

            if (DateTime.Now.TimeOfDay > ConfigService.Config.OpenTime.ToTimeSpan())
                query = query.Where(t => t.OrderDate == entity.OrderDate && t.OrderTime > ConfigService.Config.OpenTime);
            else // from midnight to open time order number set for yesterday 
            {
                var yesterday = entity.OrderDate.AddDays(-1);
                query = GetAll().Where(t => t.OrderDate >= yesterday);
            }
            entity.OrderNumber = query.Max(t => (int?)t.OrderNumber).GetValueOrDefault() + 1;
            return base.AddAsync(entity);
        }

    }
}
