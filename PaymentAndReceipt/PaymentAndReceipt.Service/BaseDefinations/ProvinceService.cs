using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class ProvinceService : BaseService<Province>
    {
        public ProvinceService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("استانی با این عنوان در سیستم ثبت شده است");
        }
    }
}
