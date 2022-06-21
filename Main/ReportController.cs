﻿using Caspian.Common;
using Caspian.Engine.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Main
{
    [ApiController]
    public class ReportController : Controller
    {
        IServiceScopeFactory ScopeFactory;
        IHostEnvironment Environment;
        public ReportController(IServiceScopeFactory scopeFactory, IHostEnvironment environment)
        {
            ScopeFactory = scopeFactory;
            Environment = environment;
        }

        [Route("[controller]/GetReport")]
        public async Task<IActionResult> GetReport(int reportId)
        {
            using var scope = ScopeFactory.CreateScope();
            var report = await new ReportService(scope).GetAll().Include("ReportGroup")
                .SingleAsync(t => t.Id == reportId);
            var basePath = Environment.ContentRootPath ;
            if (!report.PrintFileName.HasValue())
                throw new MyException("گزارش ثبت نشده است لطفا ابتدا گزارش را ثبت کرده سپس اقدام به چاپ پیش نمایش نمائید");
            var group = report.ReportGroup;
            var data = new AssemblyInfo().InvokeReportMethod(group.SubSystem, group.ClassTitle, group.MethodName, scope);
            var print = new ReportPrintEngine(scope);
            var result = print.GetData(reportId, data.AsQueryable());
            //var printReport = new StiReport();
            //printReport.RegBusinessObject("list", result);
            //var path = basePath + "/Data/Report/Print/" + report.PrintFileName + ".mrt";
            //try
            //{
            //    printReport.Load(path);
            //}
            //catch (Exception ex)
            //{

            //}
            var stream = new MemoryStream();
            //printReport["Date"] = DateTime.Now.ToPersianDate().ToString();
            ////var service = new UserService(reportService.Context);
            ////var user = this.GetCurentUser();
            //printReport["FName"] = "علی";
            //printReport["LName"] = "پرهیزکار";
            //printReport["FLName"] = "علی پرهیزکار";
            //printReport["UserId"] = "1";
            //printReport["CodeMelli"] = "5909433830";
            //printReport.Render(false);
            //printReport.ExportDocument(StiExportFormat.Pdf, stream);
            //Response.Headers.Add("Content-Disposition", new StringValues("inline; filename=" + "a.pdf"));
            return File(stream.ToArray(), "application/pdf");
        }
    }
}