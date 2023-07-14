using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class ExceptionDetailService : BaseService<ExceptionDetail>, IBaseService<ExceptionDetail>
    {
        public ExceptionDetailService(IServiceProvider provider)
            :base(provider)
        {
             
        }
    }
}
