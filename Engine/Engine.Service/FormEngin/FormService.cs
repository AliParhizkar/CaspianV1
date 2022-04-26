using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class FormService : SimpleService<Form>
    {
        public FormService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("فرمی با این عنوان در سیستم وجود دارد");
        }
    }
}
