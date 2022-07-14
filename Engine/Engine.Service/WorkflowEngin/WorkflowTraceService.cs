using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class WorkflowTraceService : SimpleService<WorkflowTrace>
    {
        public WorkflowTraceService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
