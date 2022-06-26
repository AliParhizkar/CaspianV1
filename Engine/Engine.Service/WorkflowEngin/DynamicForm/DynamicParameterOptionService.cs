using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicParameterOptionService : SimpleService<DynamicParameterOption>
    {
        public DynamicParameterOptionService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
