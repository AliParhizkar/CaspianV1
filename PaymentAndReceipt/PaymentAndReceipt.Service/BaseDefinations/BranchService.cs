using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class BranchService : BaseService<Branch>
    {
        public BranchService(IServiceProvider provider): 
            base(provider) 
        {
            RuleFor(t => t.Name).Required().UniqueAsync("شعبه ای با این عنوان در سیستم ثبت شده است");
        }
    }
}
