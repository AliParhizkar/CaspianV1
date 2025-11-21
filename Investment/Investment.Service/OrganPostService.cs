using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class OrganPostService : BaseService<OrganPost>
    {
        public OrganPostService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("پست سازمانی با این عنوان در سیستم تعریف شده است");
            RuleFor(t => t.Code).UniqueAsync("پست سازمانی با این کد در سیستم تعریف شده است");
        }
    }
}
