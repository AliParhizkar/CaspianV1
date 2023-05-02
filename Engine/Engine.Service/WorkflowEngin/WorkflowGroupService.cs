using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class WorkflowGroupService : BaseService<WorkflowGroup>
    {
        public WorkflowGroupService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گروهی با عنوان در سیستم ثبت شده است");
        }
    }
}
