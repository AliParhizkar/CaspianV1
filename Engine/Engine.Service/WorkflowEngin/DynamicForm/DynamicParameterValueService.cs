using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterValueService : BaseService<DynamicParameterValue>
    {
        public DynamicParameterValueService(ServiceProvider provider)
            :base(provider)
        {

        }
    }
}
