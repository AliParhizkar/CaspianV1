using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class InvestmentUnitAccessService : BaseService<InvestmentUnitAccess>
    {
        public InvestmentUnitAccessService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.InvestmentUnitId).UniqueAsync(t => t.UserId, "این کاربر به این واحد اموال دسترسی دارد");
        }
    }
}
