using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class ProductClassService : BaseService<ProductClass>
    {
        public ProductClassService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("طبقه کالایی با این کد در سیستم وجود دارد");
            RuleFor(t => t.Name).Required().UniqueAsync("طبقه کالایی با این نام در سیستم وجود دارد.");
        }
    }
}
