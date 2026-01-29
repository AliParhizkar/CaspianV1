using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class PropertyListService: BaseService<PropertyList>
    {
        public PropertyListService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().Custom(t => Source.Any(u => u.Name == t.Name && u.Id != t.Id), 
                "آیتمی با این عنوان در لیست وجود دارد");
        }
    }
}
