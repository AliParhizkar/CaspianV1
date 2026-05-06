using Procurement.Model;
using Caspian.Common.Service;
using Caspian.Common;

namespace Procurement.Service
{
    public class SellerService: BaseService<Seller>
    {
        public SellerService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required();
            RuleFor(t => t.IdCard).Required().UniqueAsync("فروشنده ای با این کد ملی در سیستم ثبت شده است");
            RuleFor(t => t.Tell).Required().ShortTelNumber();
            RuleFor(t => t.Tell2).ShortTelNumber();
            RuleFor(t => t.Fax).ShortTelNumber();
            RuleFor(t => t.AreaCode).AreaCode();
            RuleFor(t => t.EconomicCode).Required();
        }
    }
}
