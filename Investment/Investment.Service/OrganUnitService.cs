using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class OrganUnitService : BaseService<OrganUnit>
    {
        public OrganUnitService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("واحد سازمانی با این عنوان در سیستم تعریف شده است");
        }
    }
}
