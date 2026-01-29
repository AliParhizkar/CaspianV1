using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class StandardChangeUnitService : BaseService<StandardChangeUnit>
    {
        public StandardChangeUnitService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("تبدیل واحدی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.MainUnitId).Custom(t => t.MainUnitId == t.OtherUnitId, "واحد اصلی و تبدیلی نمی توانند یکسان باشند");
            RuleFor(t => t.Rate).Custom(t => t.Rate <= 0, "نسبت در تبدیل واحد باید بزرگتر از صفر باشد.");
        }
    }
}
