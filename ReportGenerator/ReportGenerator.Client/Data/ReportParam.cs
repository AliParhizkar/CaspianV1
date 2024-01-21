using Caspian.Common;

namespace Caspian.Report
{
    public class ReportParam
    {
        public string Text { get; set; }

        public string Title { get; set; }

        public string Alias { get; set; }

        public BondType BondType { get; set; }

        public TotalFuncType? TotalFuncType { get; set; }

        public SystemVariable? SystemVariable { get; set; }

        public SystemFiledType? SystemFiledType { get; set; }
    }
}
