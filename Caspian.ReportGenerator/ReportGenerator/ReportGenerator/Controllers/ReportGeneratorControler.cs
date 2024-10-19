using Demo.Service;
using Caspian.Report;
using Caspian.Common;
using System.Text.Json;
using Stimulsoft.Report;
using Caspian.Report.Data;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using ReportGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Caspian.Engine.Model;
using Stimulsoft.Base.Helpers;

namespace ReportGenerator.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    //[Authorize]
    public class ReportGeneratorController : ControllerBase
    {
        IWebHostEnvironment environment;
        IServiceProvider provider;
        public ReportGeneratorController(IWebHostEnvironment environment, IServiceProvider provider)
        {
            this.environment = environment;
            this.provider = provider;
        }

        [HttpGet]
        public async Task<PageData> GetReportData(int reportId)
        {
            var report = await GetService<ReportService>().SingleAsync(reportId);
            if (report.PrintFileName.HasValue())
            {
                var path = $"{environment.ContentRootPath}/Report/View/{report.PrintFileName}.json";
                var content = System.IO.File.ReadAllText(path);
                try
                {
                    return JsonSerializer.Deserialize<PageData>(content);

                }
                catch (Exception ex)
                {
                    
                    new DateTime().ToShortDateString();
                }
            }
            var parameters = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId).ToListAsync();
            var maxDataLevel = parameters.Max(t => t.DataLevel);
            var page = new PageData()
            {
                Setting = new ReportSetting()
                {
                    PageType = Caspian.Report.ReportPageType.A4,
                    PageWidth = 21,
                    PageHeight = 29.7
                },
                Bound = new BoundData()
                {
                    DataLevel = maxDataLevel,
                    Items = new List<BoundItemData>()
                },
                ReportId = reportId
            };
            for (var level = 1; level <= maxDataLevel; level++)
            {
                page.Bound.Items.Add(new BoundItemData()
                {
                    BondType = (BondType)(level + 2),
                    Height = 30
                });
            }
            return page;
        }

        TService GetService<TService>()
        {
            return provider.GetService<TService>();
        }

        [HttpGet]
        public async Task<IList<string>> GetFonts()
        {
            return await GetService<CaspianFontService>().GetAll().Select(t => t.Name).ToListAsync();
        }

        [HttpGet]
        public async  Task<IList<DataField>> GetReportParameters(int reportId, int dataLevel)
        {
            var report = await provider.GetService<ReportService>().GetAll().Include(t => t.ReportParams).Include(t => t.ReportGroup).SingleAsync(reportId);
            var mainType = new AssemblyInfo().GetReturnType(report.ReportGroup);
            var parameters = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId && (t.DataLevel == dataLevel)).ToListAsync();
            var fields = new List<DataField>();
            foreach(var param in parameters)
            {
                DataFieldType? dataFieldType = null;
                var type = mainType.GetMyProperty(param.TitleEn).PropertyType.GetUnderlyingType();
                if (type.IsIntegerType())
                    dataFieldType = DataFieldType.Integer;
                else if (type.IsNumericType())
                    dataFieldType = DataFieldType.Number;
                else if (type.IsDateType())
                    dataFieldType = DataFieldType.Date;
                fields.Add(new DataField()
                {
                    Name = param.TitleEn,
                    Title = param.Alias ?? param.TitleEn,
                    DataFieldType = dataFieldType
                });
            }
            return fields;
        }

        public async Task SaveReport(PageData page)
        {
            try
            {
                var service = GetService<ReportService>();
                var report = await service.SingleAsync(page.ReportId);
                if (report.PrintFileName == null)
                {
                    report.PrintFileName = Path.GetRandomFileName();
                    await service.SaveChangesAsync();
                }
                ReportComponentExtension.PPC = page.PixelsPerCentimetre;

                var path = $"{environment.ContentRootPath}/Report/View/{report.PrintFileName}.json";
                var json = JsonSerializer.Serialize(page);
                System.IO.File.WriteAllText(path, json);
                ///mrt file
                var doc = await page.GetXMLDocument(provider);
                path = $"{environment.ContentRootPath}/Report/Print/{report.PrintFileName}.mrt";
                doc.Save(path);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<FileContentResult> GetReport(int reportId)
        {
            var report = await GetService<ReportService>().SingleAsync(reportId);
            var path = $"{environment.ContentRootPath}/Report/Print/{report.PrintFileName}.mrt";
            var stiReport = new StiReport();
            stiReport["@ReportDate"] = DateTime.Now;
            stiReport["@FirstName"] = "Ali";
            stiReport["FirstName"] = "Ali";
            stiReport["@LastName"] = "Parhizkar";
            stiReport["FullName"] = "Ali Parhizkar";
            stiReport["@PersonalCode"] = "123456";
            stiReport.Variables["FullName"] = "Ali Parhizkar";
            var query = provider.GetService<OrderDeatilService>().GetAll();
            try
            {
                var list = new ReportPrintEngine(provider).GetData(reportId, query);
                stiReport.RegBusinessObject("list", list);
                stiReport.Load(path);
                stiReport.Render(false);
                var stream = new MemoryStream();
                stiReport.ExportDocument(StiExportFormat.Pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
            //return File(stream.ToArray(), "HTML");
        }
    }
}
