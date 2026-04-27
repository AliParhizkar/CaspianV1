using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class PurchasingSpecialistService: BaseService<PurchasingSpecialist>
    {
        public PurchasingSpecialistService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("مسئول خریدی با این کد در سیستم ثبت شده است");
            RuleFor(t => t.UserId).UniqueAsync("این کاربر قبلا بعنوان مسئول خرید تعریف شده است");
        }
    }
}
