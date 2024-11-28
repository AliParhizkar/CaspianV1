using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class AggregateReportGroupParameterService : MasterDetailsService<AggregateReportGroupParameter, AggregateReportGroupParameter>, IBaseService<AggregateReportGroupParameter>
    {
        public AggregateReportGroupParameterService(IServiceProvider provider)
            : base(provider) 
        {
            
        }
    }
}
