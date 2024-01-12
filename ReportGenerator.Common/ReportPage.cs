namespace Caspian.Common
{
    public class ReportPrintPage : ReportObject
    {
        public static int PageId { get; private set; }

        public static void ResetPageId()
        {
            ReportPrintPage.PageId = 0;
        }

        public ReportPrintPage()
        {
            ReportPrintPage.PageId++;
        }

        public ReportPrint Report { get; set; }

        public int ReportId { get; set; }

        public decimal Height { get; set; }

        public decimal Width { get; set; }

        public decimal LeftMargin { get; set; }

        public decimal RightMargin { get; set; }

        public decimal TopMargin { get; set; }

        public decimal BottmMargin { get; set; }

        public Border Border { get; set; }

        public Color BackGroundColor { get; set; }

        public ReportPageType PageType { get; set; }

        public bool IsSubReport { get; set; }

        public Color Color { get; set; }

        public Font Font { get; set; }

        public string GuId { get; set; }

        public IEnumerable<Bond> Bonds { get; set; }

        public override string GetJson()
        {
            throw new NotImplementedException();
        }
    }
}