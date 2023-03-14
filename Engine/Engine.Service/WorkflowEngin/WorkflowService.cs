using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class WorkflowService : SimpleService<Workflow>, ISimpleService<Workflow>
    {
        public WorkflowService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گردش کاری با این عنوان در سیستم تعریف شده است");
        }

        public void CheckExist(int workflowId)
        {
            if (!GetAll().Any(t => t.Id == workflowId))
                throw new Exception("این گردش کار از سیستم حذف شده است.", null);
        }
    }
}
