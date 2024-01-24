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
        public PageData GetReportData(int reportId)
        {
            return new PageData()
            {
                Bound = new BoundData()
                {
                    DataLevel = 1,
                    Items = new List<BoundItemData>()
                    {
                        new BoundItemData()
                        {
                            BondType = Caspian.Report.BondType.ReportTitle,
                            Height = 70
                        },
                        new BoundItemData()
                        {
                            BondType = Caspian.Report.BondType.FirstDataLevel,
                            Height = 45
                        },
                        //new BoundItemData()
                        //{
                        //    BondType = Caspian.Report.BondType.SecondDataLevel,
                        //    Height = 45
                        //},
                        //new BoundItemData()
                        //{
                        //    BondType = Caspian.Report.BondType.ThirdDataLevel,
                        //    Height = 45
                        //},
                    }
                },
                Width = 800
            };
        }
    }
}
