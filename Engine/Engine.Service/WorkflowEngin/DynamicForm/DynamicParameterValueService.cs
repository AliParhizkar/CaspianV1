using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterValueService : SimpleService<DynamicParameterValue>
    {
        public DynamicParameterValueService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
