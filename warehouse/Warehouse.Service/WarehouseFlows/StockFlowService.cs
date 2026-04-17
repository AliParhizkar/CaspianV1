using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class StockFlowService: MasterDetailsService<StockFlow, GoodsFlow>
    {
        public StockFlowService(IServiceProvider provider)
            :base(provider)
        {
            RuleForEach(t => t.GoodsFlows).SetValidator(t => new GoodsFlowService(provider));
        }
    }
}
