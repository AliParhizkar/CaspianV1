using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ConnectorService : SimpleService<Connector>
    {
        public ConnectorService(IServiceProvider provider)
            :base(provider)
        {

        }
        /// <summary>
        /// این متد تمامی لینک های گردش را حذف می کند
        /// </summary>
        /// <param name="workflowId"></param>
        public void RemoverWorkflowConnectors(int workflowId)
        {
            var connectors = GetAll().Where(t => t.Activity.WorkflowId == workflowId);
            RemoveRange(connectors);
        }
    }
}
