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
            RuleFor(t => t.Title).Required().UniqueAsync("واحد مالی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Code).UniqueAsync("واحد مالی با این کد در سیستم ثبت شده است");
        }
    }
}
