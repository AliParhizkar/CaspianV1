using System.ComponentModel;

namespace Caspian.Report.Data
{
    public class PageSize
    {
        public double PageWidth { get; set; }

        public double PageHeight { get; set; }
    }

    public class ReportSetting
    {
        private readonly PageSize[] list = new PageSize[]
            {
                new PageSize() { PageWidth = 21.59, PageHeight = 27.94},
                new PageSize(){PageWidth = 29.7, PageHeight = 42},
                new PageSize(){PageWidth = 21, PageHeight = 29.7},
                new PageSize(){PageWidth = 25.7, PageHeight = 36.4},
                new PageSize(){PageWidth = 18.2, PageHeight = 25.7},
            };

        public ReportSetting()
        {

        }

        public ReportSetting(double width, double height)
        {
            PageWidth = width;
            PageHeight = height;
        }

        public ReportSetting PageSetting(ReportPageType type)
        {
            switch (type)
            {
                case ReportPageType.A3:
                    return new ReportSetting(29.7, 42);
                case ReportPageType.A4:
                    return new ReportSetting(21, 29.7);
                case ReportPageType.B4:
                    return new ReportSetting(25.7, 36.4);
                case ReportPageType.B5:
                    return new ReportSetting(18.2, 25.7);
                case ReportPageType.Custome:
                    return new ReportSetting(PageWidth, PageHeight);
                case ReportPageType.Letter:
                    return new ReportSetting(21.59, 27.94);
            }
            throw new Exception("این حالت برای نوع اندازه صفحه نامعتبر می باشد.");
        }

        public ReportPageType GetPageType()
        {
            var width = PageWidth.Floor(2);
            var height = PageHeight.Floor(2);
            var type = ReportPageType.Custome;
            var index = 1;
            foreach (var item in list)
            {
                if (item.PageWidth == width && item.PageHeight == height)
                    type = (ReportPageType)index;
                index++;
            }
            return type;
        }

        [DisplayName("Page Height")]
        public double PageHeight { get; set; }

        [DisplayName("Page Width")]
        public double PageWidth { get; set; }

        [DisplayName("Page Type")]
        public ReportPageType? PageType { get; set; }

        [DisplayName("Report Title")]
        public bool ReportTitle { get; set; }

        [DisplayName("Page Header")]
        public bool PageHeader { get; set; }

        [DisplayName("Print Type")]
        public PrintOnType PrintOn { get; set; }

        [DisplayName("Data Header")]
        public bool DataHeader { get; set; }

        [DisplayName("Data Footer")]
        public bool DataFooter { get; set; }

        [DisplayName("Page Footer")]
        public bool PageFooter { get; set; }
    }
}
