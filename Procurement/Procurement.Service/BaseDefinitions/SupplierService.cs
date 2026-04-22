using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplierService:BaseService<Supplier>
    {
        public SupplierService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("تامین کننده ای با این کد  در سیستم ذخیره شده است");
            RuleFor(t => t.StartDate).Required();
        }
    }
}
