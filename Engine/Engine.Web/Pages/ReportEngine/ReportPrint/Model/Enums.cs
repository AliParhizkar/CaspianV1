using Caspian.Common.Attributes;

namespace ReportUiModels
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
        [EnumField("همه ی صفحات")]
        AllPages = 1,

        /// <summary>
        /// بجز صفحه ی اول
        /// </summary>
        [EnumField("بجز صفحه ی اول")]
        ExceptFirstPage,

        /// <summary>
        /// بجز صفحه ی آخر
        /// </summary>

        [EnumField("بجز صفحه ی آخر")]
        ExceptLastPage,

        /// <summary>
        /// بجز صفحه ی اول و آخر
        /// </summary>
        [EnumField("بجز صفحه ی اول و آخر")]
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
        [EnumField(",")]
        Camma = 1,

        /// <summary>
        /// ،
        /// </summary>
        [EnumField(".")]
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
        [EnumField(".")]
        Dot = 1,

        /// <summary>
        /// /
        /// </summary>
        [EnumField("/")]
        Slash
    }

    public enum SystemFiledType
    {
        /// <summary>
        /// شماره ردیف
        /// </summary>
        [EnumField("شماره ردیف")]
        Line = 1,

        /// <summary>
        /// شماره صفحه
        /// </summary>
        [EnumField("شماره صفحه")]
        PageNumber,

        /// <summary>
        /// شماره ی صفحه از کل
        /// </summary>
        [EnumField("شماره صفحه از کل")]
        PageNofM,

        /// <summary>
        /// تعداد کل صفحات
        /// </summary>
        [EnumField("تعداد کل صفحات")]
        TotalPageCount
    }

    /// <summary>
    /// متغیرهای سیستمی
    /// </summary>
    public enum SystemVariable
    {
        /// <summary>
        /// تاریخ جاری سیستم
        /// </summary>
        [EnumField("تاریخ جاری")]
        Date = 1,

        /// <summary>
        /// نام کاربرجاری
        /// </summary>
        [EnumField("نام کاربر")]
        FName,

        /// <summary>
        /// نام خانوادگی کاربرجاری
        /// </summary>
        [EnumField("نام خانوادگی کاربر")]
        LName,

        /// <summary>
        /// نام نام خانوادگی کاربر جاری
        /// </summary>
        [EnumField("نام و نام خانوادگی کاربر")]
        FLName,

        /// <summary>
        /// کد کاربری/پرسنلی کاربر جاری
        /// </summary>
        [EnumField("کد کاربری/پرسنلی کاربر")]
        UserId,

        /// <summary>
        /// کد ملی کاربر جاری
        /// </summary>
        [EnumField("کد ملی کاربر")]
        NatinalCode
    }

    public enum HorizontalAlign
    {
        Width = 1,
        Right = 2,
        Center,
        Left,
    }

    public enum VerticalAlign
    {
        Bottom = 1,
        Center,
        Top
    }

    public enum BorderStyle
    {
        Solid = 1,
        Dash,
        Dot,
        Double
    }

    public enum BorderKind
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = 15
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
        [EnumField("مجموع")]
        Sum,

        /// <summary>
        /// میانگین
        /// </summary>
        [EnumField("میانگین")]
        Avg,

        /// <summary>
        /// تعداد
        /// </summary>
        [EnumField("تعداد")]
        Count,

        /// <summary>
        /// مینیمم
        /// </summary>
        [EnumField("مینیمم")]
        Min,

        /// <summary>
        /// ماکزیمم
        /// </summary>
        [EnumField("ماکزیمم")]
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
        [EnumField("تک ستونه")]
        Once = 1,

        [EnumField("دو ستونه")]
        Tow,

        [EnumField("سه ستونه")]
        Tree,

        [EnumField("چهار ستونه")]
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