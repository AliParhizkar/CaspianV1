using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Level 1")]
        Level1 = 1,

        /// <summary>
        /// سطح1و2
        /// </summary>
        [Display(Name = "Level1,2")]
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
        [Display(Name = "تعداد")]
        Count= 1,

        [Display(Name = "مینیمم")]
        Min,

        [Display(Name = "ماکزیمم")]
        Max,

        [Display(Name = "مجموع")]
        Sum,

        [Display(Name = "میانگین")]
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
