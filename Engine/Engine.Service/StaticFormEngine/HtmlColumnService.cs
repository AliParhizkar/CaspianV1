using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class HtmlColumnService : BaseService<HtmlColumn>, IBaseService<HtmlColumn>
    {
        public HtmlColumnService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.RowId)
                .Custom(t => t.RowId.HasValue && t.InnerRowId.HasValue, "Only RowId or InnerRowId can has value")
                .Custom(t => t.RowId == null && t.InnerRowId == null, "one of RowId or InnerRowId must has value");

        }
    }
}
