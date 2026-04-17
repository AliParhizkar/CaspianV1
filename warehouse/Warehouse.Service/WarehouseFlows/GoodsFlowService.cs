using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class GoodsFlowService : BaseService<GoodsFlow>
    {
        public GoodsFlowService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
