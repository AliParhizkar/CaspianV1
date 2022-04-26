using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterService : SimpleService<DynamicParameter>
    {
        public DynamicParameterService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
