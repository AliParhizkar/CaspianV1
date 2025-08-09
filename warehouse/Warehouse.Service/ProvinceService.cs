using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class ProvinceService : BaseService<Province>
    {
        public ProvinceService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("استانی با این عنوان در سیستم ثبت شده است");
        }
    }
}
