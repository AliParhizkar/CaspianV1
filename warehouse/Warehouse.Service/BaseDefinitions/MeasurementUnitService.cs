using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class MeasurementUnitService: BaseService<MeasurementUnit>
    {
        public MeasurementUnitService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("واحد سنجشی با این نام در سیستم ثبت شده است");
        }
    }
}
