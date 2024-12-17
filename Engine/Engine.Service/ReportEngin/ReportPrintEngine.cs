using System.Collections;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ReportPrintEngine
    {
        IServiceProvider ServiceProvider;

        public ReportPrintEngine(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }


        public IList GetData(int reportId, IQueryable data)
        {
            var service = ServiceProvider.GetService<ReportParamService>();
            var reportParams = service.GetAll().Include(t => t.DynamicParameter).Include(t => t.Rule)
                .Include(t => t.ReportGroupParameter).Where(t => t.ReportId == reportId).ToList();
            var report = new SelectReport(data.ElementType);
            Type type = null;
            if (reportParams.Any(t => t.CompositionMethodType.HasValue))
            {
                var lambda = report.SelectForGroupBy(reportParams);
                data = data.GroupBy(report.GroupBy(reportParams)).Select(lambda);
            }
            else
            {
                var lambda = report.SimpleSelect(reportParams, out type);
                data = data.Select(lambda);
            }
            var list = data.ToIList();
            return report.GetStumlData(list, reportParams, type, reportParams.Max(t => t.DataLevel));
            //return report.GetValues(data, reportParams);
        }

        private string GetPropertyListName(string name, int dataLevel)
        {
            var str = name;
            if (dataLevel > 1)
            {
                var index = str.LastIndexOf('.');
                str = str.Substring(0, index);
            }
            if (dataLevel > 2)
            {
                var index = str.LastIndexOf('.');
                str = str.Substring(0, index);
            }
            return str + 's';
        }
    }
}
