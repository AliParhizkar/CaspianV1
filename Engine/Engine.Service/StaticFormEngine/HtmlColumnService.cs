using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class HtmlColumnService : BaseService<HtmlColumn>, IBaseService<HtmlColumn>
    {
        public HtmlColumnService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
