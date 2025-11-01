using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class InvestmentUnitService : BaseService<InvestmentUnit>
    {
        public InvestmentUnitService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).UniqueAsync("کدی با این عنوان ثبت شده است");
        }
    }
}
