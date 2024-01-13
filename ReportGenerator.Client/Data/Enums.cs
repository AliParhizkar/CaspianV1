using System.ComponentModel.DataAnnotations;

namespace Caspian.Report
{
    /// <summary>
    /// نوع چاپ سرصفحه در گزارش
    /// </summary>
    public enum PrintOnType
    {
        /// <summary>
        /// همه ی صفحات
        /// </summary>
        [Display(Name = "همه ی صفحات")]
        AllPages = 1,

        /// <summary>
        /// بجز صفحه ی اول
        /// </summary>
        [Display(Name = "بجز صفحه ی اول")]
        ExceptFirstPage,

        /// <summary>
        /// بجز صفحه ی آخر
        /// </summary>

        [Display(Name = "بجز صفحه ی آخر")]
        ExceptLastPage,

        /// <summary>
        /// بجز صفحه ی اول و آخر
        /// </summary>
        [Display(Name = "بجز صفحه ی اول و آخر")]
        ExceptFirstAndLastPage
    }

    public enum ReportPageType
    {
        /// <summary>
        /// 21.59 cm * 27.94 cm
        /// </summary>
        Letter = 1,

        /// <summary>
        /// 29.7 cm * 42 cm
        /// </summary>
        A3,

        /// <summary>
        /// 21 cm * 29.7 cm
        /// </summary>
        A4,

        /// <summary>
        /// 25.7 cm * 36.4 cm
        /// </summary>
        B4,

        /// <summary>
        /// 18.2 cm * 25.7 cm
        /// </summary>
        B5,

        Custome
    }

    public enum BondType
    {
        ReportTitle,

        PageHeader,

        DataHeader,

        FirstDataLevel,

        SecondDataLevel,

        ThirdDataLevel,

        DataFooter,

        PageFooter,
    }

    public enum ChangeType
    {
        Move,

        TopResize,

        BottomResize,

        LeftResize,

        RightResize,
    }

    public enum ChangeKind
    {
        ColumnResize,

        RowResize
    }
}
