using Warehouse.Model;
using Caspian.Common.Service;
using Caspian.Common;

namespace Procurement.Service
{
    public class InventoryControlAgentService:BaseService<InventoryControlAgent>
    {
        public InventoryControlAgentService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("کنترل موجودی با این عنوان در سیستم ثبت شده است");
        }
    }
}
