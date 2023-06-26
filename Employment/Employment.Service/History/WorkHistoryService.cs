using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class WorkHistoryService : BaseService<WorkHistory>, IBaseService<WorkHistory>
    {
        public WorkHistoryService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
