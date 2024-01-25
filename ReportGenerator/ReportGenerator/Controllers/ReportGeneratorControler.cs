using System.Text.Json;
using Caspian.Report.Data;
using Caspian.Engine.Service;
using Microsoft.AspNetCore.Mvc;
using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace ReportGenerator.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportGeneratorController : ControllerBase
    {
        IWebHostEnvironment Environment;
        ReportService Service;
        ReportParamService ReportParamService;
        public ReportGeneratorController(IWebHostEnvironment environment, ReportService service, ReportParamService paramService)
        {
            Environment = environment;
            this.Service = service;
            ReportParamService = paramService;
        }

        [HttpGet]
        public async Task<PageData> GetReportData(int reportId)
        {
            var report = await Service.SingleAsync(reportId);
            var fileName = "m55lnmmc.fjx" ?? Path.GetRandomFileName();
            var path = $"{Environment.ContentRootPath}/Report/View/{fileName}.js";
            var content = System.IO.File.ReadAllText(path);
            return JsonSerializer.Deserialize<PageData>(content);
        }

        [HttpGet]
        public async Task<IList<SelectListItem>> GetReportParameters(int reportId, int dataLevel)
        {
            return await ReportParamService.GetAll().Where(t => t.ReportId == reportId && (t.DataLevel == null || t.DataLevel == dataLevel))
                .Select(t => new SelectListItem
                {
                    Text = t.Alias,
                    Value = t.TitleEn
                }).ToListAsync();
        }

        public void SaveReport(PageData page)
        {
            var fileName = "m55lnmmc.fjx" ?? Path.GetRandomFileName();
            var path = $"{Environment.ContentRootPath}/Report/View/{fileName}.js";
            var json = JsonSerializer.Serialize(page);
            System.IO.File.WriteAllText(path, json);
        }
    }
}
