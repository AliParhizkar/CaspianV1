using Demo.Service;
using Caspian.Report;
using Caspian.Common;
using System.Text.Json;
using Stimulsoft.Report;
using System.Reflection;
using Caspian.Report.Data;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Mvc;
using ReportGenerator.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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
                var path = $"{environment.ContentRootPath}/ReportFiles/View/{report.PrintFileName}.json";
                var content = System.IO.File.ReadAllText(path);
                return JsonSerializer.Deserialize<ReportPageData>(content);
            }
            byte maxDataLevel = 1;
            if (report.ReportType != Caspian.Engine.ReportType.Aggregate)
            {
                var parameters = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId).ToListAsync();
                if (!parameters.Any())
                    
                maxDataLevel = parameters.Max(t => t.DataLevel);
            }
            ReportPageData pageData = new ReportPageData()
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
                pageData.Bound.Items.Add(new BoundItemData()
                {
                    BondType = (BondType)(level + 2),
                    Height = 30
                });
            }
            return pageData;
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
        public async Task<IList<ParameterData>> GetReportParameters(int reportId, int dataLevel)
        {
            var report = await GetService<ReportService>().GetAll().Include(t => t.ReportGroup).SingleAsync(reportId);
            var mainType = new AssemblyInfo().GetReturnType(report.ReportGroup);
            if (report.ReportType == Caspian.Engine.ReportType.Aggregate)
            {
                var groupParameters = await GetService<AggregateReportGroupParameterService>().GetAll()
                    .Where(t => t.ReportGroupId == report.ReportGroupId).ToListAsync();
                var parameters = await GetService<AggregateReportParameterService>().GetAll().Where(t => t.ReportId == reportId)
                    .ToListAsync();
                var list = new List<ParameterData>();
                foreach(var parameter in parameters.Select(t => t.AggregateReportGroupParameter))
                {
                    var path = parameter.Path;
                    string allis = parameter.Allis;
                    switch (parameter.AggregateParameterType)
                    {
                        case AggregateParameterType.Grouping:
                        case AggregateParameterType.Selecting:
                            if (parameter.ParentParameter != null)
                            {
                                var parentPath = parameter.ParentParameter.Path;
                                var property = mainType.GetMyProperty(parentPath);
                                var type = property.PropertyType.GetUnderlyingType();
                                if (type == typeof(DateOnly))
                                {
                                    var index = parentPath.LastIndexOf('.') + 1;
                                    parentPath = parentPath.Substring(0, index);
                                    var persianDateInfo =  property.DeclaringType.GetProperties().Single(t => t.PropertyType == typeof(PersianDateTable) &&
                                        t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == property.Name);
                                    parentPath += persianDateInfo.Name;
                                    path = parentPath + path;
                                    allis = $"{parameter.ParentParameter.Allis}({allis})";
                                }
                            }
                            list.Add(new ParameterData(path.Replace(".", ""), allis));
                            break;
                        case AggregateParameterType.AggregateFunction:
                            var functionType = parameter.AggregateFunctionType.Value;
                            var path1 = parameter.ParentParameter.Path.Replace(".", "") + functionType.ToString();
                            allis = $"{functionType.GetAggregateFunctionName()}({parameter.ParentParameter.Allis})"; 
                            list.Add(new ParameterData(path1, allis));
                            break;
                    }
                }
                return list;
            }
            var result = await GetService<ReportParamService>().GetAll().Where(t => t.ReportId == reportId && (t.DataLevel == dataLevel))
                .Select(t => new SelectListItem
                {
                    Text = t.ReportGroupParameter.Alias,
                    Value = t.ReportGroupParameter.PropertyPath
                }).ToListAsync();
            
            return result.Select(t => new ParameterData
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
            var service = GetService<ReportService>();
            var report = await service.SingleAsync(page.ReportId);
            if (report.PrintFileName == null)
            {
                report.PrintFileName = Path.GetRandomFileName();
                await service.SaveChangesAsync();
            }
            ReportComponentExtension.PPC = page.PixelsPerCentimetre;

            var path = $"{environment.ContentRootPath}/ReportFiles/View/{report.PrintFileName}.json";
            var json = JsonSerializer.Serialize(page);
            System.IO.File.WriteAllText(path, json);
               
            ///mrt file
            try
            {
                var doc = await page.GetXMLDocument(provider);
                path = $"{environment.ContentRootPath}/ReportFiles/Print/{report.PrintFileName}.mrt";
                doc.Save(path);
            }
            catch(Exception ex)
            {

            }
        }

        [HttpGet]
        public async Task<FileContentResult> GetReport(int reportId)
        {
            var query = GetService<OrderDetailService>().GetAll().Where(t => t.Order.CustomerId != null);
            var list = await new ReportEngine(provider.CreateScope()).GetData(reportId, query);
            var report = await GetService<ReportService>().SingleAsync(reportId);
            var path = $"{environment.ContentRootPath}/ReportFiles/Print/{report.PrintFileName}.mrt";
            var stiReport = new StiReport();
            stiReport.RegBusinessObject("list", list);
            stiReport.Load(path);
            stiReport.Render(false);

            var stream = new MemoryStream();
            stiReport.ExportDocument(StiExportFormat.Pdf, stream);
            return File(stream.ToArray(), "application/pdf");
            //return File(stream.ToArray(), "HTML");
        }
    }
}
