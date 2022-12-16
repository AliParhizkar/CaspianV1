using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class ChildrenPropertiesService : SimpleService<ChildrenProperties>, ISimpleService<ChildrenProperties>
    {
        public ChildrenPropertiesService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
