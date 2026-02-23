using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class SubstituteProductService : BaseService<SubstituteProduct>
    {
        public SubstituteProductService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.GoodsId).UniqueAsync(t => t.SubstituteGoodsId, "این حالت جایگزینی قبلا تعریف شده است");
        }
    }
}
