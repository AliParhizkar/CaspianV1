using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;


namespace Warehouse.Service
{
    public class SimpleDataService : BaseService<SimpleData>
    {
        public SimpleDataService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.DataType, "این عنوان تکراری است");
        }
    }
}
