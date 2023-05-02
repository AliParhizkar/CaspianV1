using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class WorkHistoryService : BaseService<WorkHistory>, IBaseService<WorkHistory>
    {
        public WorkHistoryService(ServiceProvider provider)
            :base(provider)
        {

        }
    }
}
