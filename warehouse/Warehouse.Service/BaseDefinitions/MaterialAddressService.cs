using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class MaterialAddressService : BaseService<MaterialAddress>
    {
        public MaterialAddressService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("آدرس کالایی با این عنوان در سیستم ثبت شده است");
        }
    }
}
