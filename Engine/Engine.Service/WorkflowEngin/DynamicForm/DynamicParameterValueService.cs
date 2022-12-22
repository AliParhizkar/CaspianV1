using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterValueService : SimpleService<DynamicParameterValue>
    {
        public DynamicParameterValueService(ServiceProvider provider)
            :base(provider)
        {

        }
    }
}
