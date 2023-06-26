using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class MarriagePropertiesService : BaseService<MarriageProperties>, IBaseService<MarriageProperties>
    {
        public MarriagePropertiesService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
