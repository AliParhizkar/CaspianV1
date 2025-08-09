using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class ScopeService : BaseService<Scope>
    {
        public ScopeService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("ناحیه ای با این عنوان در سیستم ثبت شده است");
        }
    }
}
