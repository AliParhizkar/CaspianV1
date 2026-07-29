using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class BankService: BaseService<Bank>
    {
        public BankService(IServiceProvider provider) 
            : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("بانکی با این عنوان در سیستم تعریف شده است");
            RuleFor(t => t.Code).UniqueAsync("بانکی با این کد در سیستم تعریف شده است");
            RuleFor(t => t.Tell).TelNumber();
            RuleFor(t => t.WebSite).CheckUrl("آدرس سایت نامعتبر است");
        }
    }
}
