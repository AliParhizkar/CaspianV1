using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marketing.Service
{
    public class OrderService : MasterDetailsService<Order, OrderDetail>, IBaseService<Order>
    {
        Order order;
        Cashier cashier;
        Customer customer;
        IList<Product> products;

        async Task<IList<Product>> GetProductsAsync()
        {
            if (products == null)
            {
                using var service = ServiceProvider.CreateScope().GetService<ProductService>();
                products = await service.GetAll().ToListAsync();
            }
            return products;
        }

        public OrderService(IServiceProvider provider)
            :base(provider) 
        {
            
            RuleFor(t => t.OrderType).CustomAsync(async t =>
            {
                if (t.CustomerId > 0)
                    customer = await GetEntity(t.CustomerId.Value, customer);
                var products = await GetProductsAsync();

                if (customer?.IsSpecial == true)
                {
                    if (Details.Any(t => products.Single(u => u.Id == t.ProductId).SpecialCustomerPrice != t.Price))
                        return "قیمت غذاهای سالن درست انتخاب نشده است";
                }
                else
                {
                    if (t.OrderType == OrderType.Salon)
                    {
                        if (Details.Any(d => products.Single(u => u.Id == d.ProductId).Price != d.Price))
                            return "قیمت غذاهای سالن درست انتخاب نشده است";
                    }
                    else
                    {
                        if (Details.Any(t => products.Single(u => u.Id == t.ProductId).TakeOutPrice != t.Price))
                            return "قیمت غذاهای بیرون بر درست انتخاب نشده است";
                    }
                }
                    return null;
            });
            RuleFor(t => t.CashierId).Custom(t =>
            {
                if (t.CashierId.HasValue && t.CashAmount == 0)
                    return "برای سفارش های غیرنقدی، صندوقدار باید خالی باشد.";
                return null;
            });
            RuleFor(t => t.CashAmount).CustomAsync(async t =>
            {
                if (t.Id == 0)
                {
                    if (t.CashAmount > 0)
                    {
                        if (t.CashierId == null)
                        {
                            cashier = await GetEntity<Cashier>(t => t.RelatedUserId == UserId && t.ActiveType == ActiveType.Active);
                            if (cashier == null)
                                return "برای ثبت سفارش نقد، کاربر باید بعنوان صندوقدار فعال باشد";
                            t.CashierId = cashier.Id;
                        }
                        else
                        {
                            cashier = await GetEntity(t.CashierId.Value, cashier);
                            if (cashier.RelatedUserId != UserId)
                                return "کاربر صندوقدار و کاربر جاری یکسان نیست";
                            if (cashier.ActiveType != ActiveType.Active)
                                return "صندوقدار فعال نیست";
                        }
                    }
                }
                else
                {
                    order = await GetEntity(t.Id, order);
                    if (order.CashierId.HasValue && order.CashierId != t.CashierId)
                        return "در ویرایش صندوقدار نمی تواند تغییر کند";
                    if (order.CashAmount != t.CashAmount)
                    {
                        cashier = await GetEntity(t.CashierId.Value, cashier);
                        if (cashier.RelatedUserId != UserId)
                            return "فقط صندوقدار ثبت کننده ی مبلغ نقد، می تواند مبلغ نقد را تغییر دهد.";
                        if (cashier.ActiveType != ActiveType.Active)
                            return "صندوق دار فعال نیست";
                    }
                }
                return null;
            });
            RuleFor(t => t.IsSpecialCustomer).CustomAsync(async t =>
            {
                if (t.CustomerId > 0)
                {
                    customer = await GetEntity(t.CustomerId.Value, customer);
                    if (t.IsSpecialCustomer != customer.IsSpecial)
                        return "فیلد مشتری خاص درست مقداردهی نشده است";
                    if (customer.IsSpecial)
                    {
                        var products = await GetProductsAsync();
                        if (t.IsSpecialCustomer)
                        {
                            if (Details.Any(u => u.Price != products.Single(v => v.Id == u.ProductId).SpecialCustomerPrice))
                                return "مبلغ غذا برای مشتری خاص درست مقداردهی نشده است";
                        }
                    }
                }
                else if (t.IsSpecialCustomer)
                    return "برای مشتری خاص مشتری باید مشخص باشد";
                return null;
            });
            RuleForEach(t => t.OrderDetails).SetValidator(t => new OrderDetailService(provider));
            RuleFor(t => t.OrderDetails).Custom(t => !Details.Any(), "سفارش حداقل باید یک محصول داشته باشد.");
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
