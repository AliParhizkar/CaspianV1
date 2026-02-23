using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class BarcodePatternService : BaseService<BarcodePattern>
    {
        public BarcodePatternService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("الگوی کدینگی با این کد در سیستم تعریف شده است");
            RuleFor(t => t.Name).Required().UniqueAsync("الگوی کدینگی با این نام در سیستم تعریف شده است");
        }
    }
}
