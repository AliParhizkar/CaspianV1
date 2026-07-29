using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class BankBranchService : BaseService<BankBranch>
    {
        public BankBranchService(IServiceProvider provider) 
            :base(provider) 
        {
            RuleFor(t => t.Name).Required().UniqueAsync("شعبه ای با این نام در سیستم تعریف شده است");
            RuleFor(t => t.Code).UniqueAsync("شعبه ای با این کد در سیستم تعریف شده است");
            RuleFor(t => t.Tel).TelNumber();
            RuleFor(t => t.Website).CheckUrl();
        }
    }
}
