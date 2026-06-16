using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class CashierService : BaseService<Cashier>
    {
        public CashierService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.RelatedUserId).UniqueAsync("این کاربر در حال حاضر بعنوان صندوقدار می باشد");
        }
    }
}
