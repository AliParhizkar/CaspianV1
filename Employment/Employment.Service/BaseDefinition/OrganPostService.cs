using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class OrganPostService : SimpleService<OrganPost>, ISimpleService<OrganPost>
    {
        public OrganPostService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("پست سازمانی با این عنوان در سیستم وجود دارد");
            RuleFor(t => t.Code).Required().UniqAsync("پست سازمانی با این کد در سیستم وجود دارد");
        }
    }
}
