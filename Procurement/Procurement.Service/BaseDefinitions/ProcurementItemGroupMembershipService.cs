using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class ProcurementItemGroupingService: BaseService<ProcurementItemGrouping>
    {
        public ProcurementItemGroupingService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.ProcurementItemGroupId).UniqueAsync(t => t.ProcurementItemId, "در حال حاضر این قلم خریدنی عضو گروه می باشد");
        }
    }
}
