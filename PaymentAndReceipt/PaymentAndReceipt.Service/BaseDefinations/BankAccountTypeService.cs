using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class BankAccountTypeService: BaseService<BankAccountType>
    {
        public BankAccountTypeService(IServiceProvider service)
            : base(service) 
        {
            RuleFor(t => t.Title).Required().UniqueAsync("نوع حسابی با این عنوان در سیستم ثبت شده است");
        }
    }
}
