using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class CurrencyService: BaseService<Currency>
    {
        public CurrencyService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("ارزی با این عنوان در سیستم تعریف شده است");
        }
    }
}
