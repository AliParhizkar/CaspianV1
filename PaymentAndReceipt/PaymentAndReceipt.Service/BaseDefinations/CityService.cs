using System;
using Caspian.Common;
using Caspian.Common.Service;
using PaymentAndReceipt.Model;

namespace PaymentAndReceipt.Service
{
    public class CityService : BaseService<City>
    {
        public CityService(IServiceProvider provider) 
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync(t => t.ProvinceId, "شهری با این عنوان در این استان تعریف شده است");
        }
    }
}
