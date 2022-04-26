using Caspian.Engine;
using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ReportService : SimpleService<Report>
    {
        public ReportService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("گزارشی با این عنوان در سیستم ثبت شده است");
        }

        public override IQueryable<Report> GetAll(Report report = null)
        {
            var query = base.GetAll();
            //if (ServiceData.SubSystem.HasValue)
            //    query = query.Where(t => t.SubSystem == ServiceData.SubSystem.Value);
            return query;
        }


        public bool IsExist(int id)
        {
            return GetAll().Any(t => t.Id == id);
        }

        public IDictionary<string, string> GetReportMethod(SubSystemKind subSystemKind)
        {
            var dic = new Dictionary<string, string>();
            foreach (var type in new AssemblyInfo().GetServiseTypes(subSystemKind).Where(t => t.CustomAttributes.Any(u => u.AttributeType == typeof(ReportClassAttribute))))
            {
                var methods = type.GetMethods().Where(t => t.CustomAttributes.Any(u => u.AttributeType == typeof(ReportMethodAttribute)));
                foreach (var method in methods)
                    dic.Add(type.Namespace + ',' + type.Name + ',' + method.Name, method.GetCustomAttribute<ReportMethodAttribute>().FaTitle);
            }
            return dic;
        }

        public IQueryable<Report> GetSubSystemReports()
        {
            return GetAll().Where(t => t.FilteringFileName != null && t.PrintFileName != null);
        }

        public void CheckExist(int id)
        {
            if (!IsExist(id))
                throw new Exception("گزارشی با کد " + id + " وجود ندارد.", null);
        }

        async public Task RemoveReportFile(string relatedPath, int reportId)
        {
            var report = await SingleAsync(reportId);
            if (report.PrintFileName.HasValue())
            {
                var path = relatedPath + report.PrintFileName + ".mrt";
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                report.PrintFileName = null;
            }
            else
                throw new Exception("گزارش فاقد فایل می باشد.", null);
            await SaveChangesAsync();
        }
    }
}
