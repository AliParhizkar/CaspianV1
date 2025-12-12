using Accounting.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Accounting.Service
{
    public class CodingLevelService : BaseService<CodingLevel>
    {
        public CodingLevelService(IServiceProvider provider) : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("سطح کدی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Length).Custom(t => t.Length < 1, "طول سطح نمی تواند کمتر از یک باشد");
            RuleFor(t => t.CodingLevelType).UniqueAsync("طول و عنوان این سطح قبلا مشخص شده است");
        }
    }
}
