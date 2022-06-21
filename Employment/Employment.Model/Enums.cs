using Caspian.Common.Attributes;

namespace Employment.Model
{
    /// <summary>
    /// پایه
    /// </summary>
    public enum BaseType
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
    public enum Gender
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
}

