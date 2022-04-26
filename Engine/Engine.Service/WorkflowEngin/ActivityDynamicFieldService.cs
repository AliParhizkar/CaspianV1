using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine
{
    public class ActivityDynamicFieldService : SimpleService<ActivityDynamicField>
    {
        public ActivityDynamicFieldService(IServiceScope scope)
            :base(scope)
        {

        }

        public void RemoverWorkflowFields(int workflowId)
        {
            var fields = GetAll().Where(t => t.Activity.WorkflowId == workflowId);
            RemoveRange(fields);
        }
    }
}
