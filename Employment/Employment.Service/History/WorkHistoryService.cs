using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class WorkHistoryService : SimpleService<WorkHistory>, ISimpleService<WorkHistory>
    {
        public WorkHistoryService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
