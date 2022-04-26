using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ActivityFieldService : SimpleService<ActivityField>
    {
        public ActivityFieldService(IServiceScope scope)
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
