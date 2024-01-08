using System.ComponentModel;

namespace Caspian.Report
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

        /// <summary>
        /// ارتفاع صفحه در گزارش
        /// </summary>
        [DisplayName("ارتفاع")]
        public double PageHeight { get; set; }

        /// <summary>
        /// عرض صفحه در گزارش
        /// </summary>
        [DisplayName("عرض")]
        public double PageWidth { get; set; }

        /// <summary>
        /// اندازه
        /// </summary>
        [DisplayName("اندازه")]
        public ReportPageType? PageType { get; set; }

        /// <summary>
        /// گزارش دارای سر گزارش می باشد
        /// </summary>
        [DisplayName("سرگزارش")]
        public bool ReportTitle { get; set; }

        /// <summary>
        /// گزراش دارای سرصفحه می باشد.
        /// </summary>
        [DisplayName("سرصفحه")]
        public bool PageHeader { get; set; }

        /// <summary>
        /// نوع چاپ سرصفحه در سرگزارش
        /// </summary>
        [DisplayName("نوع چاپ")]
        public PrintOnType PrintOn { get; set; }

        /// <summary>
        /// گزارش دارای سرداده می باشد
        /// </summary>
        [DisplayName("سرداده")]
        public bool DataHeader { get; set; }

        /// <summary>
        /// گزراش دارای ته داده می باشد
        /// </summary>
        [DisplayName("ته داده")]
        public bool DataFooter { get; set; }

        /// <summary>
        /// گزارش دارای ته صفحه می باشد
        /// </summary>
        [DisplayName("ته صفحه")]
        public bool PageFooter { get; set; }
    }
}
