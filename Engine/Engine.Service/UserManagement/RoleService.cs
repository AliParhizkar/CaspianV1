using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class RoleService: BaseService<Role>
    {
        public RoleService(IServiceProvider provider) :
            base(provider)
        {
            RuleFor(t => t.Name).Required().UniqAsync("نقشی با این عنوان در سیستم وجود دارد");
        }
    }
}
