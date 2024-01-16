using System.ComponentModel.DataAnnotations;

namespace Caspian.Report
{
    /// <summary>
    /// Page header print type
    /// </summary>
    public enum PrintOnType
    {
        [Display(Name = "All Pages")]
        AllPages = 1,

        [Display(Name = "Except First Page")]
        ExceptFirstPage,

        [Display(Name = "Except Last Page")]
        ExceptLastPage,

        [Display(Name = "Except First & LastPage")]
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
