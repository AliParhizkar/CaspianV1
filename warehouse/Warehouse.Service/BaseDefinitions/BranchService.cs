using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class BranchService : BaseService<Branch>
    {
        public BranchService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("شعبه ای با این نام در سیستم ثبت شده است");
        }
    }
}
