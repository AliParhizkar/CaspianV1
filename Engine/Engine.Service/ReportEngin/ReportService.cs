using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class ReportService : MasterDetailsService<Report, ReportParam, AggregateReportParameter>
    {
        public ReportService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.ReportGroup.SubSystem, "گزارشی با این عنوان در سیستم ثبت شده است");
            RuleForEach(t => t.ReportParams).SetValidator(report => new ReportParamService(provider, report));
        }
    }
}
