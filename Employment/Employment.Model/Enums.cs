using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    /// <summary>
    /// رتبه
    /// </summary>
    public enum BaseType : byte
    {
        [Display(Name = "مقدماتی")]
        Preliminary = 1,

        [Display(Name = "مهارتی")]
        Skills,

        [Display(Name = "رتبه 3")]
        Rank3,

        [Display(Name = "رتبه 2")]
        Rank2,

        [Display(Name = "رتبه 1")]
        Rank1
    }

    public enum BaseStudy: byte
    {
        /// <summary>
        /// زیردیپلم
        /// </summary>
        [Display(Name = "زیردیپلم")]
        Highschool = 1,

        /// <summary>
        /// دیپلم
        /// </summary>
        [Display(Name = "دیپلم")]
        Diploma,

        /// <summary>
        /// فوق دیپلم
        /// </summary>
        [Display(Name = "فوق دیپلم")]
        AssociateDegree,

        /// <summary>
        /// لیسانس
        /// </summary>
        [Display(Name = "لیسانس")]
        Bachelor,

        /// <summary>
        /// فوق لیسانس
        /// </summary>
        [Display(Name = "فوق لیسانس")]
        Master,

        /// <summary>
        /// دکترا
        /// </summary>
        [Display(Name = "دکترا")]
        PHD
    }

    /// <summary>
    /// زن
    /// </summary>
    public enum Gender:byte
    {
        /// <summary>
        /// مرد
        /// </summary>
        [Display(Name = "مرد")]
        Male,

        /// <summary>
        /// زن
        /// </summary>
        [Display(Name = "زن")]
        Female
    }

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

}

