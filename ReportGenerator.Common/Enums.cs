using System.ComponentModel.DataAnnotations;

namespace Caspian.Common
{
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

    /// <summary>
    /// نوع تغییر ترتیب فیلدها
    /// </summary>
    public enum OrderChangeType
    {
        /// <summary>
        /// افزاایش
        /// </summary>
        IncOrder = 1,

        /// <summary>
        /// کاهش
        /// </summary>
        DecOrder
    }

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

    public enum ReportType
    {
        Report,
        View
    }

    /// <summary>
    /// کاراکتر جداساز سه رقم سه رقم ارقام
    /// </summary>
    public enum GroupNumberChar
    {
        /// <summary>
        /// ,
        /// </summary>
        [Display(Name = ",")]
        Camma = 1,

        /// <summary>
        /// ،
        /// </summary>
        [Display(Name = ".")]
        Dot,
    }

    /// <summary>
    /// کاراکتری که بعنوان ممیز مورد استفاده قرار می گیرد
    /// </summary>
    public enum DecimalChar
    {
        /// <summary>
        /// .
        /// </summary>
        [Display(Name = ".")]
        Dot = 1,

        /// <summary>
        /// /
        /// </summary>
        [Display(Name = "/")]
        Slash
    }

    public enum SystemFiledType
    {
        [Display(Name = "Row Number")]
        Line = 1,

        [Display(Name = "Page Number")]
        PageNumber,

        [Display(Name = "Page Number From Total")]
        PageNofM,

        [Display(Name = "Total Pages")]
        TotalPageCount
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

    public enum HorizontalAlign
    {
        Left = 1,
        Center,
        Right,
        Justify
    }

    public enum VerticalAlign
    {
        Top = 1,
        Middle,
        Bottom,
    }

    public enum BorderStyle
    {
        Solid = 1,
        Dashed,
        Dotted,
        Double
    }

    public enum BorderKind
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
    }

    public enum ReportControlType
    {
        Page = 1,
        Bond,
        TextBox,
        PictureBox,
        SubReport
    }

    public enum BondType
    {
        ReportTitle = 1,
        PageHeader,
        DataHeader,
        DataBond,
        DataFooter,
        PageFooter
    }

    public enum TotalFuncType
    {
        /// <summary>
        /// مجموع
        /// </summary>
        [Display(Name = "مجموع")]
        Sum,

        /// <summary>
        /// میانگین
        /// </summary>
        [Display(Name = "میانگین")]
        Avg,

        /// <summary>
        /// تعداد
        /// </summary>
        [Display(Name = "تعداد")]
        Count,

        /// <summary>
        /// مینیمم
        /// </summary>
        [Display(Name = "مینیمم")]
        Min,

        /// <summary>
        /// ماکزیمم
        /// </summary>
        [Display(Name = "ماکزیمم")]
        Max
    }

    public enum CellType
    {
        ColumnHeader = 1,
        RowHeader,
        Cell
    }

    public enum ColumnCountType
    {
        [Display(Name = "تک ستونه")]
        Once = 1,

        [Display(Name = "دو ستونه")]
        Tow,

        [Display(Name = "سه ستونه")]
        Tree,

        [Display(Name = "چهار ستونه")]
        Four
    }

    public enum ReportWindowType
    {
        Setting = 1,

        Text,

        PictureBox,

        Formula,

        Subreport,

        DigitsFormat,

        ColumnWindow,

        WorkflowPrint
    }
}