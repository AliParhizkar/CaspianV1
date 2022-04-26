using Caspian.Common.Attributes;

namespace Caspian.Engine
{
    /// <summary>
    /// سطحی از داده ها که در سمت زیرگزارش قرار می گیرند.
    /// </summary>
    public enum SubReportLevel: byte
    {
        /// <summary>
        /// سطح1
        /// </summary>
        [EnumField("سطح1")]
        Level1 = 1,

        /// <summary>
        /// سطح1و2
        /// </summary>
        [EnumField("سطح1و2")]
        Level1_2
    }

    /// <summary>
    /// نوع کنترلهای جستجو
    /// </summary>
    public enum FilteringControlType: byte
    {
        /// <summary>
        /// نوع شمارشی
        /// </summary>
        Enums = 1,

        /// <summary>
        /// کلید خارجی
        /// </summary>
        ForeignKey = 2,

        /// <summary>
        /// مقادیر عددی-تاریخ و کامپلکس تایپ ها
        /// </summary>
        FromTo = 3,

        /// <summary>
        /// نوع بولین
        /// </summary>
        Boolean = 4,
    }

    public enum CompositionMethodType: byte
    {
        [EnumField("تعداد")]
        Count= 1,

        [EnumField("مینیمم")]
        Min,

        [EnumField("ماکزیمم")]
        Max,

        [EnumField("مجموع")]
        Sum,

        [EnumField("میانگین")]
        Avg
    }

    /// <summary>
    /// نوع TextBox
    /// </summary>
    public enum InputFieldType: byte
    {
        /// <summary>
        /// عدد صحیح
        /// </summary>
        Integer,

        /// <summary>
        /// عدد اعشاری
        /// </summary>
        Number,

        /// <summary>
        /// رشته
        /// </summary>
        String
    }

    public enum DynamicFieldType
    {
        Number,
        ComplexType,
        String,
        ForeignKey
    }

    public enum CompareToType: byte
    {
        /// <summary>
        /// کوچکتر مساوی صفر
        /// </summary>
        LTEZero,

        /// <summary>
        /// بزرگتر مساوی صفر
        /// </summary>
        GTEZero
    }
}
