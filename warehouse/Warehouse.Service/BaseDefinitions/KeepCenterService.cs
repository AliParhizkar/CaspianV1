using Warehouse.Model;
using Caspian.Common.Service;
using Caspian.Common;

namespace Warehouse.Service
{
    public class KeepCenterService: BaseService<KeepCenter>
    {
        public KeepCenterService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("مرکز نگهداری با این کد در سیستم تعریف شده است");
            RuleFor(t => t.Name).Required().UniqueAsync("مرکز نگهداری با این نام در سیستم تعریف شده است");
            RuleFor(t => t.Tell).Required().CallNumber();
            RuleFor(t => t.Address).Required();
        }
    }
}
