using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PurchaseTypeService: BaseService<PurchaseType>
    {
        public PurchaseTypeService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("نوع خریدی با این کد در سیستم ثبت شده است");
            RuleFor(t => t.Title).Required().UniqueAsync("نوع خریدی با این عنوان در سیستم ثبت شده است");
        }
    }
}
