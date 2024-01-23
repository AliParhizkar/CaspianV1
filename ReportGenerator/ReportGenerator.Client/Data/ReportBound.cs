using Microsoft.AspNetCore.Components;

namespace Caspian.Report.Data
{
    public class ReportBound
    {
        public double? TitleHeight { get; set; }

        public double? PageHeaderHeight { get; set;}

        public double? DataHeaderHeight { get; set; }

        public double? DataFooterHeight { get; set; }

        public double? PageFooterHeight { get; set;}

        public double FirstDLHeight { get; set; }

        public double? SecondDLHeight { get; set; }

        public double? ThirdDLHeight { get; set; }

        public byte DataLevel { get; set; }
    }
}
