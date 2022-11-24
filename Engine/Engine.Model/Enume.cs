using Caspian.Common.Attributes;

namespace Caspian.Engine
{
    public enum CalculationType: byte
    {
        [EnumField("کاربر")]
        UserData = 1,

        [EnumField("فرم")]
        FormData
    }

    public enum DataParameterType: byte
    {
        [EnumField("پارامترهای پویا")]
        DynamicParameters = 1,

        [EnumField("خصوصیت های موجودیت")]
        EntityProperties,

        [EnumField("قوانین")]
        FormRule
    }

    public enum CompareType : byte
    {
        /// <summary>
        /// بزرگتر از
        /// </summary>
        [EnumField("بزرگتر از")]
        GreaterThan = 1,

        /// <summary>
        /// بزرگتر مساوی با
        /// </summary>
        [EnumField("بزرگتر مساوی با")]
        GreaterThanOrEqual,

        /// <summary>
        /// کوچکتر از
        /// </summary>
        [EnumField("کوچکتر از")]
        LessThan,

        /// <summary>
        /// کوچکتر مساوی با
        /// </summary>
        [EnumField("کوچکتر مساوی با")]
        LessThanOrEqual,

        /// <summary>
        /// برابر با
        /// </summary>
        [EnumField("برابر با")]
        Equal,

        /// <summary>
        /// مخالف با
        /// </summary>
        [EnumField("مخالف با")]
        NotEqual
    }

    public enum ActivityType : byte
    {
        /// <summary>
        /// حالت شروع
        /// </summary>
        Start = 1,

        /// <summary>
        /// پردازش کاربر
        /// </summary>
        User,

        /// <summary>
        /// انجام عملیات توسط سیستم
        /// </summary>
        Validator,

        /// <summary>
        /// پایان
        /// </summary>
        End,

        /// <summary>
        /// توضیحات
        /// </summary>
        Comment,

        /// <summary>
        /// عملیات بررسی توسط سیستم
        /// </summary>
        Parallelogram1
    }

    public enum DynamicParameterType: byte
    {
        User,
        Form,
        Rule
    }
}
