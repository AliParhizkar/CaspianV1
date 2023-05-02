using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class InnerRowService : BaseService<InnerRow>, IBaseService<InnerRow>
    {
        public InnerRowService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
