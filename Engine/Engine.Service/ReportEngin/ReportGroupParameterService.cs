using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class ReportGroupParameterService: BaseService<ReportGroupParameter>
    {
        public ReportGroupParameterService(IServiceProvider provider)
            : base(provider) 
        {
        
        }

        public ReportGroupParameterService(IServiceProvider provider, IList<ReportGroupParameter> source):
            base(provider) 
        {
            RuleFor(t => t.PropertyPath).Required();
            RuleFor(t => t.Alias).Required().Custom(t => source != null && source.Any(u => u.Alias == t.Alias && u.PropertyPath != t.PropertyPath), "Alise must be uniqu");
        }
    }
}
