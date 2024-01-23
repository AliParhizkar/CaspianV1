using Caspian.Report.Data;
using Microsoft.AspNetCore.Mvc;

namespace ReportGenerator.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportGeneratorController : ControllerBase
    {

        public ReportGeneratorController()
        {

        }

        [HttpGet]
        public ReportPage GetReportData(int reportId)
        {
            return new ReportPage()
            {
                ReportBound = new ReportBound()
                {
                    TitleHeight = 60,
                    ThirdDLHeight = 30,
                    DataLevel = 1
                },
                Setting = new ReportSetting() 
                {
                    PageWidth = 800
                }
            };
        }
    }
}
