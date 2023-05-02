using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class ChildrenPropertiesService : BaseService<ChildrenProperties>, IBaseService<ChildrenProperties>
    {
        public ChildrenPropertiesService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
