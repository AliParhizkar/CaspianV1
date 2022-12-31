using Caspian.Common.Attributes;

namespace Demo.Model
{
    /// <summary>
    /// نوع مشتری
    /// </summary>
    public enum CustomerType: byte
    {
        /// <summary>
        /// حقیقی
        /// </summary>
        [EnumField("حقیقی")]
        Real = 1,

        /// <summary>
        /// حقوقی
        /// </summary>
        [EnumField("حقوقی")]
        Legal
    }

    /// <summary>
    /// جنسیت
    /// </summary>
    public enum Gender: byte
    {
        /// <summary>
        /// مرد
        /// </summary>
        [EnumField("مرد")]
        Male = 1,

        /// <summary>
        /// زن
        /// </summary>
        [EnumField("زن")]
        Female
    }

    /// <summary>
    /// وضعیت فعالیت
    /// </summary>
    public enum ActiveType: byte
    {
        /// <summary>
        /// فعال
        /// </summary>
        [EnumField("فعال")]
        Enable = 1,

        /// <summary>
        /// غیرفعال
        /// </summary>
        [EnumField("غیرفعال")]
        Disable
    }

    /// <summary>
    /// نوع سفارش
    /// </summary>
    public enum OrderType: byte
    {
        /// <summary>
        /// سالن
        /// </summary>
        [EnumField("سالن")]
        Salon = 1,

        /// <summary>
        /// بیرون بر
        /// </summary>
        [EnumField("بیرون بر")]
        Package,

        /// <summary>
        /// تلفنی
        /// </summary>
        [EnumField("تلفنی")]
        Tel,

        /// <summary>
        /// اینترنتی
        /// </summary>
        [EnumField("اینترنتی")]
        Internet
    }

    /// <summary>
    /// وضعیت
    /// </summary>
    public enum OrderStatus: byte
    {
        /// <summary>
        /// لغو
        /// </summary>
        [EnumField("لغو")]
        Canceled = 1,

        /// <summary>
        /// نهایی
        /// </summary>
        [EnumField("نهایی")]
        Finaled
    }

    /// <summary>
    /// نوع سفارش
    /// </summary>
    public enum OrderKind2 : byte
    {
        /// <summary>
        /// سالن
        /// </summary>
        [EnumField("سالن")]
        Salo = 1,

        /// <summary>
        /// بیرون بر
        /// </summary>
        [EnumField("بیرون بر")]
        Package = 2,

        /// <summary>
        /// تلفنی
        /// </summary>
        [EnumField("تلفنی")]
        Tel = 4,

        /// <summary>
        /// اینترنتی
        /// </summary>
        [EnumField("اینترنتی")]
        Internet = 8
    }

    public enum Meal : byte
    {
        /// <summary>
        /// صبحانه
        /// </summary>
        [EnumField("صبحانه")]
        Breakfast = 1,

        /// <summary>
        /// نهار
        /// </summary>
        [EnumField("نهار")]
        Lunch = 2,

        /// <summary>
        /// شام
        /// </summary>
        [EnumField("شام")]
        Dinner = 4,
    }
}