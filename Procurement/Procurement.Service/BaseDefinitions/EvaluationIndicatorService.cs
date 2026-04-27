using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class EvaluationIndicatorService: BaseService<EvaluationIndicator>
    {
        public EvaluationIndicatorService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("شاخص ارزیابی با این عنوان در سیستم وجود دارد");
            RuleFor(t => t.Factor).CustomValue(t => t <= 0, "وزن/ضریب شاخص ارزیابی باید بزرگتر از صفر باشد");
            RuleFor(t => t.Minimum).CustomValue(t => t < 0, "حداقل امتیاز نباید منفی باشد");
            RuleFor(t => t.Maximum).Custom(t => t.Maximum <= t.Minimum, "حداکثر امتیاز باید بزرگتر از حداقل امتیاز باشد");
        }
    }
}
