using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ConnectorService : BaseService<NodeConnector>
    {
        public ConnectorService(IServiceProvider provider)
            :base(provider)
        {

        }

        public IQueryable<NodeConnector> GetConnectors(int workflowId)
        {
            return GetAll().Where(t => t.Activity.WorkflowId == workflowId);
        }
    }
}
