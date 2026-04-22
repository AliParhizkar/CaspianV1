using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplyingScopeService:BaseService<SupplyingScope>
    {
        public SupplyingScopeService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.ProcurementItemGroupId).Custom(t =>
            {
                if (t.ProcurementItemGroupId is null && t.ProcurementItemId is null)
                    return "یکی از مقادیر اقلام خریدنی یا گروه اقلام خریدنی باید مشخص باشد";
                if (t.ProcurementItemGroupId.HasValue && t.ProcurementItemId.HasValue)
                    return "تنها یکی از مقادیر اقلام خریدنی یا گروه اقلام خریدنی باید مشخص شود";
                return null;
            }).UniqueAsync(t => t.ProcurementItemGroupId, "این گروه قلم خریدنی به حوزه ی تامین اضافه شده است");
        }
    }
}
