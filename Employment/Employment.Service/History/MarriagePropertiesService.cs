using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class MarriagePropertiesService : BaseService<MarriageProperties>, IBaseService<MarriageProperties>
    {
        public MarriagePropertiesService(ServiceProvider provider)
            :base(provider)
        {

        }
    }
}
