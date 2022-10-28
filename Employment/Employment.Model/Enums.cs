using Caspian.Common.Attributes;

namespace Employment.Model
{
    /// <summary>
    /// رتبه
    /// </summary>
    [EnumType("رتبه")]
    public enum BaseType : byte
    {
        [EnumField("مقدماتی")]
        Preliminary = 1,

        [EnumField("مهارتی")]
        Skills,

        [EnumField("رتبه 3")]
        Rank3,

        [EnumField("رتبه 2")]
        Rank2,

        [EnumField("رتبه 1")]
        Rank1
    }

    [EnumType("پایه تحصیلی")]
    public enum BaseStudy: byte
    {
        /// <summary>
        /// زیردیپلم
        /// </summary>
        [EnumField("زیردیپلم")]
        Highschool = 1,

        /// <summary>
        /// دیپلم
        /// </summary>
        [EnumField("دیپلم")]
        Diploma,

        /// <summary>
        /// فوق دیپلم
        /// </summary>
        [EnumField("فوق دیپلم")]
        AssociateDegree,

        /// <summary>
        /// لیسانس
        /// </summary>
        [EnumField("لیسانس")]
        Bachelor,

        /// <summary>
        /// فوق لیسانس
        /// </summary>
        [EnumField("فوق لیسانس")]
        Master,

        /// <summary>
        /// دکترا
        /// </summary>
        [EnumField("دکترا")]
        PHD
    }

    /// <summary>
    /// زن
    /// </summary>
    [EnumType("جنسیت")]
    public enum Gender:byte
    {
        /// <summary>
        /// مرد
        /// </summary>
        [EnumField("مرد")]
        Male,

        /// <summary>
        /// زن
        /// </summary>
        [EnumField("زن")]
        Female
    }

    [EnumType("وضعیت فعال")]
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

}

