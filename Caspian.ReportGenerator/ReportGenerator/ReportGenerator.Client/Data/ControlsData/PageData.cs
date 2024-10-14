namespace Caspian.Report.Data
{
    public class PageData
    {
        public PageData()
        {
            Border = new Border();
        }

        public int ReportId { get; set; }

        public ReportSetting Setting { get; set; }

        public BoundData Bound { get; set; }

        public int Width { get; set; }

        public Color BackGroundColor { get; set; }

        public double PixelsPerCentimetre { get; set; }

        public Border Border { get; set; }
    }
}
