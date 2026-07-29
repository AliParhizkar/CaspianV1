using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class BankAccountService: BaseService<BankAccount>
    {
        public BankAccountService(IServiceProvider provider):
            base(provider) 
        {
            RuleFor(t => t.AccountNumber).Required().UniqueAsync("حسابی با این شماره در سیستم تعریف شده است");
            RuleFor(t => t.Shaba).Custom(t => t.Shaba != null && t.Shaba.Length != 26, "شماره ی شبا باید 26 رقم باشد");
            RuleFor(t => t.ReserveBalance).Custom(t => t.ReserveBalance > 0 && t.CreditLimit > 0, "فقط یکی از فیلدهای موجودی ذخیره و مبلغ اعتبار حساب می تواند بزرگتر از صفر باشند");
            RuleFor(t => t.CreditLimit).Custom(t => t.ReserveBalance > 0 && t.CreditLimit > 0, "فقط یکی از فیلدهای موجودی ذخیره و مبلغ اعتبار حساب می تواند بزرگتر از صفر باشند");
        }
    }
}
