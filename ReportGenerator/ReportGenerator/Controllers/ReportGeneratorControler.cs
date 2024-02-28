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
using Demo.Model;

namespace ReportGenerator.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
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
                return JsonSerializer.Deserialize<PageData>(content);
            }
            var parameters = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId).ToListAsync();
            var maxdataLevel = parameters.Max(t => t.DataLevel).GetValueOrDefault(1);
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
                    DataLevel = maxdataLevel,
                    Items = new List<BoundItemData>()
                },
                ReportId = reportId
            };
            for (var level = 1; level <= maxdataLevel; level++)
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
        public async Task<IList<SelectListItem>> GetReportParameters(int reportId, int dataLevel)
        {
            var result = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId && (t.DataLevel == null || t.DataLevel == dataLevel))
                .Select(t => new SelectListItem
                {
                    Text = t.Alias,
                    Value = t.TitleEn
                }).ToListAsync();
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
        public async Task<FileContentResult> GetReport(int reportId )
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
            var result = new List<object>() 
            { 
                new 
                {
                    Order_Customer_FName = "Ali",
                    OrderDeatils = new List<object>()
                    {
                        new 
                        {
                            Product_Title = "Pizza",
                        },
                        new
                        {
                            Product_Title = "Hamberger"
                        }
                    }
                },
                new
                {
                    Order_Customer_FName = "Amir",
                    OrderDeatils = new List<object>()
                    {
                        new
                        {
                            Product_Title = "Frnch fries",
                        },
                        new
                        {
                            Product_Title = "Hotdog"
                        }
                    }
                },
                new
                {
                    Order_Customer_FName = "Reza",
                    OrderDeatils = new List<object>()
                    {
                        new
                        {
                            Product_Title = "Sousuge",
                        },
                        new
                        {
                            Product_Title = "Hotdog"
                        }
                    }
                }
            };
            result = new List<object>
            {
                new
                {
                    Order_Customer_FName = "Ali",
                    Orders = new List<object>()
                    {
                        new 
                        {
                            Order_Date = "2024/01/02",
                            OrderDeatils = new List<object>()
                            {
                                new
                                {
                                    Product_Title = "Pizza"
                                }
                            }
                        },
                        new
                        {
                            Order_Date = "2024/01/03",
                            OrderDeatils = new List<object>()
                            {
                                new
                                {
                                    Product_Title = "Hamberger"
                                }
                            }
                        }
                    }
                }
            };
            var query = provider.GetService<OrderDeatilService>().GetAll();
            var qqq = new ReportPrintEngine(provider).GetData(reportId, query);

            stiReport.RegBusinessObject("list", qqq);
            stiReport.Load(path);
            stiReport.Render(false);
            var stream = new MemoryStream();
            stiReport.ExportDocument(StiExportFormat.Pdf, stream);
            return File(stream.ToArray(), "application/pdf");
            //return File(stream.ToArray(), "HTML");
        }
    }
}
