using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "حقیقی")]
        Real = 1,

        /// <summary>
        /// حقوقی
        /// </summary>
        [Display(Name = "حقوقی")]
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
        [Display(Name = "مرد")]
        Male = 1,

        /// <summary>
        /// زن
        /// </summary>
        [Display(Name = "زن")]
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
        [Display(Name = "فعال")]
        Enable = 1,

        /// <summary>
        /// غیرفعال
        /// </summary>
        [Display(Name = "غیرفعال")]
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
        [Display(Name = "سالن")]
        Salon = 1,

        /// <summary>
        /// بیرون بر
        /// </summary>
        [Display(Name = "بیرون بر")]
        Package,

        /// <summary>
        /// تلفنی
        /// </summary>
        [Display(Name = "تلفنی")]
        Tel,

        /// <summary>
        /// اینترنتی
        /// </summary>
        [Display(Name = "اینترنتی")]
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
        [Display(Name = "لغو")]
        Canceled = 1,

        /// <summary>
        /// نهایی
        /// </summary>
        [Display(Name = "نهایی")]
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
        [Display(Name = "سالن")]
        Salo = 1,

        /// <summary>
        /// بیرون بر
        /// </summary>
        [Display(Name = "بیرون بر")]
        Package = 2,

        /// <summary>
        /// تلفنی
        /// </summary>
        [Display(Name = "تلفنی")]
        Tel = 4,

        /// <summary>
        /// اینترنتی
        /// </summary>
        [Display(Name = "اینترنتی")]
        Internet = 8
    }

    public enum Meal : byte
    {
        /// <summary>
        /// صبحانه
        /// </summary>
        [Display(Name = "صبحانه")]
        Breakfast = 1,

        /// <summary>
        /// نهار
        /// </summary>
        [Display(Name = "نهار")]
        Lunch = 2,

        /// <summary>
        /// شام
        /// </summary>
        [Display(Name = "شام")]
        Dinner = 4,
    }
}