using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Engine
{
    public enum CalculationType: byte
    {
        [Display( Name = "کاربر")]
        UserData = 1,

        [Display(Name = "فرم")]
        FormData
    }

    public enum DataParameterType: byte
    {
        [Display(Name = "پارامترهای پویا")]
        DynamicParameters = 1,

        [Display(Name = "خصوصیت های موجودیت")]
        EntityProperties,

        [Display(Name = "قوانین")]
        FormRule
    }

    public enum CompareType : byte
    {
        /// <summary>
        /// بزرگتر از
        /// </summary>
        [Display(Name = "بزرگتر از")]
        GreaterThan = 1,

        /// <summary>
        /// بزرگتر مساوی با
        /// </summary>
        [Display(Name = "بزرگتر مساوی با")]
        GreaterThanOrEqual,

        /// <summary>
        /// کوچکتر از
        /// </summary>
        [Display(Name = "کوچکتر از")]
        LessThan,

        /// <summary>
        /// کوچکتر مساوی با
        /// </summary>
        [Display(Name = "کوچکتر مساوی با")]
        LessThanOrEqual,

        /// <summary>
        /// برابر با
        /// </summary>
        [Display(Name = "برابر با")]
        Equal,

        /// <summary>
        /// مخالف با
        /// </summary>
        [Display(Name = "مخالف با")]
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
