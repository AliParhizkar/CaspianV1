using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class MarriagePropertiesService : SimpleService<MarriageProperties>, ISimpleService<MarriageProperties>
    {
        public MarriagePropertiesService(ServiceProvider provider)
            :base(provider)
        {

        }
    }
}
