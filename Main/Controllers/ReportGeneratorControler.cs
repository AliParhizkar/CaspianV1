using Demo.Service;
using Caspian.Report;
using Caspian.Common;
using System.Text.Json;
using Stimulsoft.Report;
using Caspian.Report.Data;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ReportGenerator.Controllers
{
    [ApiController]
    [Route("ReportGenerator/[action]")]
    //[Authorize]
    public class ReportGeneratorController : ControllerBase
    {
        IServiceProvider provider;
        IWebHostEnvironment environment;
        public ReportGeneratorController(IWebHostEnvironment environment, IServiceProvider provider, IServiceScopeFactory scopeFactory)
        {
            this.provider = provider;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<ReportPageData> GetReportData(int reportId)
        {
            var report = await GetService<ReportService>().SingleAsync(reportId);
            if (report.PrintFileName.HasValue())
            {
                var path = $"{environment.ContentRootPath}/Report/View/{report.PrintFileName}.json";
                var content = System.IO.File.ReadAllText(path);
                return JsonSerializer.Deserialize<ReportPageData>(content);
            }
            var parameters = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId).ToListAsync();
            var maxDataLevel = parameters.Max(t => t.DataLevel);
            var page = new ReportPageData()
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

        TService GetService<TService>() where TService: class
        {
            return provider.GetCaspianService<TService>();
        }

        [HttpGet]
        public async Task<IList<string>> GetFonts()
        {
            return await GetService<CaspianFontService>().GetAll().Select(t => t.Name).ToListAsync();
        }

        [HttpGet]
        public async Task<IList<ParamtereData>> GetReportParameters(int reportId, int dataLevel)
        {
            var result = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId && (t.DataLevel == dataLevel))
                .Select(t => new SelectListItem
                {
                    Text = t.ReportGroupParameter.Alias ?? t.ReportGroupParameter.Alias,
                    Value = t.ReportGroupParameter.PropertyPath
                }).ToListAsync();
            var report = await GetService<ReportService>().GetAll().Include(t => t.ReportGroup).SingleAsync(reportId);
            var mainType = new AssemblyInfo().GetReturnType(report.ReportGroup);
            return result.Select(t => new ParamtereData 
            { 
                Value = t.Value,
                Text = t.Text,
                ParameterFormatType = GetParameterType(mainType.GetMyProperty(t.Value).PropertyType.GetUnderlyingType())
            }).ToList();
        }

        ParameterFormatType? GetParameterType(Type type)
        {
            if (type == typeof(string))
                return ParameterFormatType.String;
            if (type.IsEnum || type == typeof(byte[]) || type == typeof(bool))
                return null;
            if (new Type[] {typeof(byte), typeof(short), typeof(int), typeof(long) }.Contains(type))
                return ParameterFormatType.Integer;
            if (new Type[] {typeof(decimal), typeof(double), typeof(float) }.Contains(type))
                return ParameterFormatType.Number;
            if (type == typeof(DateOnly) || type == typeof(DateTime))
                return ParameterFormatType.Date;
            if (type == typeof(TimeOnly))
                return ParameterFormatType.Time;
            return null;
        }

        public async Task SaveReport(ReportPageData page)
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
                //ReportComponentExtension.PPC = page.PixelsPerCentimetre;

                var path = $"{environment.ContentRootPath}/Report/View/{report.PrintFileName}.json";
                var json = JsonSerializer.Serialize(page);
                System.IO.File.WriteAllText(path, json);
               
                ///mrt file
                //var doc = await page.GetXMLDocument(provider);
                path = $"{environment.ContentRootPath}/Report/Print/{report.PrintFileName}.mrt";
                //doc.Save(path);
            }
            catch (Exception ex)
            {

            }
        }

        [HttpGet]
        public async Task<FileContentResult> GetReport(int reportId)
        {

            try
            {
                var report = await GetService<ReportService>().SingleAsync(reportId);
                //report.PrintFileName = "report";
                var path = $"{environment.ContentRootPath}/Report/Print/{report.PrintFileName}.mrt";
                var stiReport = new StiReport();
                stiReport["@ReportDate"] = DateTime.Now;
                stiReport["@FirstName"] = "Ali";
                stiReport["FirstName"] = "Ali";
                stiReport["@LastName"] = "Parhizkar";
                stiReport["FullName"] = "Ali Parhizkar";
                stiReport["@PersonalCode"] = "123456";
                stiReport.Variables["FullName"] = "Ali Parhizkar";
                var query = provider.GetService<OrderDetailService>().GetAll().Where(t => t.Order.CustomerId != null);
                var list = await new ReportEngine(provider.CreateScope()).GetData(reportId, query);
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
