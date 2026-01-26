using Caspian.Common;
using Caspian.Common.Service;
using Investment.Model;

namespace Investment.Service
{
    public class DepreciationLawService: BaseService<DepreciationLaw>
    {
        public DepreciationLawService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("قانونی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Rate).CustomValue(t => t <= 0 || t > 100, "نرخ استهلاک باید بین صفر تا صد باشد");
            RuleFor(t => t.UsefulLife).CustomValue(t => t <= 0, "عمر مفید باید بزگتر از صفر باشد");
        }
    }
}
