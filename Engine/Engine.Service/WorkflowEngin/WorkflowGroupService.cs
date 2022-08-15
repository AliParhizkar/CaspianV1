using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class WorkflowGroupService : SimpleService<WorkflowGroup>
    {
        public WorkflowGroupService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گروهی با عنوان در سیستم ثبت شده است");
        }
    }
}
