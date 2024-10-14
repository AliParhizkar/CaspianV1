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
            //var report = await GetService<ReportService>().SingleAsync(reportId);
            //if (report.PrintFileName.HasValue())
            //{
            //    var path = $"{environment.ContentRootPath}/Report/View/{report.PrintFileName}.json";
            //    var content = System.IO.File.ReadAllText(path);
            //    try
            //    {
            //        return JsonSerializer.Deserialize<PageData>(content);

            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //var parameters = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId).ToListAsync();
            //var maxDataLevel = parameters.Max(t => t.DataLevel);
            byte maxDataLevel = 1;
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
        public IList<string> GetFonts()
        {
            //using var service = GetService<CaspianFontService>();
            //return await service.GetAll().Select(t => t.Name).ToListAsync();
            return new List<string>()
            {
                "B Nazanin", "B Roya", "New Time"
            };
        }

        [HttpGet]
        public IList<SelectListItem> GetReportParameters(int reportId, int dataLevel)
        {
            //var result = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId && (t.DataLevel == null || t.DataLevel == dataLevel))
            //    .Select(t => new SelectListItem
            //    {
            //        Text = t.Alias ?? t.TitleEn,
            //        Value = t.TitleEn
            //    }).ToListAsync();
            var result = new List<SelectListItem>()
            {
                new SelectListItem("Order.Date", "Order Date"),
                new SelectListItem("Order.Customer.CustomerType", "Customer Type"),
                new SelectListItem("Order.Customer.FName", "First Name"),
                new SelectListItem("Order.Customer.LName", "Last Name"),
                new SelectListItem("Order.Customer.Gender", "Gender"),
                new SelectListItem("Order.Customer.CompanyName", "Company Name"),
                new SelectListItem("Order.Customer.MobileNumber", "Mobile Number"),
                new SelectListItem("Product.Title", "Product Title"),
                new SelectListItem("Price", "Price"),
                new SelectListItem("Quantity", "Quantity"),
                new SelectListItem("Order.OrderType", "Order Type"),
            };
            return result;
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
                throw ex;
            }
            //return File(stream.ToArray(), "HTML");
        }
    }
}
