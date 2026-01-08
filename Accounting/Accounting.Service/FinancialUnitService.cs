using Caspian.Common;
using Accounting.Model;
using Caspian.Common.Service;

namespace Accounting.Service
{
    public class FinancialUnitService : BaseService<FinancialUnit>
    {
        public FinancialUnitService(IServiceProvider provider) : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(" عنوان واحد مالی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Image != null && t.Image.Length > 100_000);
        }
    }
}
