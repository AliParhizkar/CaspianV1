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
        [EnumField("زیردیپلم")]
        Highschool = 1,

        [EnumField("دیپلم")]
        Diploma,

        [EnumField("فوق دیپلم")]
        AssociateDegree,

        [EnumField("لیسانس")]
        Bachelor,

        [EnumField("فوق لیسانس")]
        Master,

        [EnumField("دکتری")]
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

