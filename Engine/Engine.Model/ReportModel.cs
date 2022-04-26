using System.Collections;

namespace Caspian.Engine
{
    public class ReportModel
    {
        public IEnumerable Data { get; set; }

        public int ReportId { get; set; }

        public ReportParam ReportParam1 { get; set; }

        public ReportParam ReportParam2 { get; set; }
    }
}
