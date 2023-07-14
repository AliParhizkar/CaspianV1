using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class ExceptionDataService : BaseService<ExceptionData>, IBaseService<ExceptionData>
    {
        public ExceptionDataService(IServiceProvider provider)
            :base(provider)
        {
             
        }
    }
}
