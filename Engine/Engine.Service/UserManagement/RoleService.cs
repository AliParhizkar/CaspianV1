using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class RoleService: SimpleService<Role>
    {
        public RoleService(IServiceScope scope):
            base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("نقشی با این عنوان در سیستم وجود دارد");
        }
    }
}
