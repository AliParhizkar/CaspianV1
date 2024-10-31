using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class ReportGroupParameterService: BaseService<ReportGroupParameter>, IBaseService<ReportGroupParameter>
    {
        public ReportGroupParameterService(IServiceProvider provider)
            : base(provider) 
        {
        
        }

        public ReportGroupParameterService(IServiceProvider provider, IList<ReportGroupParameter> source):
            base(provider) 
        {
            RuleFor(t => t.TitleEn).Required();
            RuleFor(t => t.Alias).Required().Custom(t => source != null && source.Any(u => u.Alias == t.Alias && u.TitleEn != t.TitleEn), "Alise must be uniqu");
        }
    }
}
