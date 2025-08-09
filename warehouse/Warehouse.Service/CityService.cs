using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class CityService : BaseService<City>
    {
        public CityService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("شهری با این عنوان در سیستم ثبت شده است");
        }
    }
}
