using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class InnerRowService : SimpleService<InnerRow>, ISimpleService<InnerRow>
    {
        public InnerRowService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
