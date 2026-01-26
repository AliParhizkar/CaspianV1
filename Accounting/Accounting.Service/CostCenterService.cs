using Accounting.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Accounting.Service
{
    public class CostCenterService: BaseService<CostCenter>
    {
        public CostCenterService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("مرکز هزینه ای با این نام در سیستم وجود دارد");
            RuleFor(t => t.Code).UniqueAsync("مرکز هزینه ای با این کد در سیستم وجود دارد").CustomAsync(async t =>
            {
                if (t.ParentId.HasValue)
                {
                    var parent = await SingleAsync(t.ParentId.Value);
                    if (!parent.Code.HasValue() && t.Code.HasValue() || parent.Code.HasValue() && !t.Code.HasValue())
                        return "تمامی سطوح یا باید همگی کد داشته باشند یا هیچکدام کد نداشته باشند";
                    if (t.Code.Length <= parent.Code.Length || !t.Code.StartsWith(parent.Code))
                        return "کدها باید بصورت سلسله مراتبی تعریف شوند";
                }
                return null;
            });
        }
    }
}
