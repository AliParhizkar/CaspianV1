using Caspian.Common;
using System.Xml.Linq;
using Caspian.Engine.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Server
{
    [ApiController]
    [Route("[controller]")]
    public class ReportGeneratorController : ControllerBase
    {
        IServiceProvider provider;
        IHostEnvironment host;
        public ReportGeneratorController(IServiceProvider provider, IHostEnvironment host)
        {
            this.host = host;
            this.provider = provider;
        }

        [HttpGet(Name = "GetReport")]
        public async Task<ObjectResult> GetReport(int reportId)
        {
            var report = await provider.GetService<ReportService>().GetAll().Include(t => t.ReportParams).SingleAsync(t => t.Id == reportId);
            if (!report.ReportParams.Any())
                throw new CaspianException("Please specify the report parameters first and then create the report file");
            if (report.PrintFileName.HasValue())
            {
                var path = host.ContentRootPath + "\\Data\\Report\\View\\" + report.PrintFileName + ".mrt";
                var doc = XDocument.Load(path);
                var pagesElement = doc.Element("StiSerializer").Element("Pages").Elements();
                var page = new ReportPrintPage()
                {
                    Report = new ReportPrint()
                    {
                        Pages = pagesElement.Select(t => new ReportPrintPage(t)).ToList()
                    }
                };
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
