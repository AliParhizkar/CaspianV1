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

    public enum SystemVariable
    {
        [Display(Name = "Date")]
        Date = 1,

        [Display(Name = "First Name")]
        FName,

        [Display(Name = "Last Name")]
        LName,

        [Display(Name = "Name")]
        FLName,

        [Display(Name = "Personnel Code")]
        UserId
    }

    public enum BorderStyle
    {
        Solid = 1,
        Dash,
        Dot,
        Double
    }

    public enum VerticalAlign
    {
        Top = 1,
        Middle,
        Bottom,
    }

    public enum HorizontalAlign
    {
        Left = 1,
        Center,
        Right,
        Justify
    }

    public enum TotalFuncType
    {
        [Display(Name = "Sum")]
        Sum,

        [Display(Name = "Avrage")]
        Avg,

        [Display(Name = "Count")]
        Count,

        [Display(Name = "Min")]
        Min,

        [Display(Name = "Max")]
        Max
    }

    public enum SystemFiledType
    {
        [Display(Name = "Row Number")]
        Line = 1,

        [Display(Name = "Page Number")]
        PageNumber,

        [Display(Name = "Page Number From Total")]
        PageNofM,

        [Display(Name = "Total Page Count")]
        TotalPageCount
    }

    public enum BorderKind
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
    }

    public enum DecimalChar
    {
        [Display(Name = ".")]
        Dot = 1,

        [Display(Name = "/")]
        Slash
    }

    public enum GroupNumberChar
    {
        [Display(Name = ",")]
        Camma = 1,

        [Display(Name = ".")]
        Dot,
    }
}
