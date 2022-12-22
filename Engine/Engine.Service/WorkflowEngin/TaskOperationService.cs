using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class TaskOperationService : SimpleService<TaskOperation>
    {
        public TaskOperationService(IServiceProvider provider)
            : base(provider)
        {

        }
    }
}
