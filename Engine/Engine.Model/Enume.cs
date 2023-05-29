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
        /// فعالیت
        /// </summary>
        Task,

        /// <summary>
        /// درگاه
        /// </summary>
        Gateway,

        /// <summary>
        /// پایان
        /// </summary>
        End,

        /// <summary>
        /// توضیحات
        /// </summary>
        Comment,
    }

    public enum TaskType
    {
        /// <summary>
        /// فعالیت کاربر
        /// </summary>
        UserTask = 1,

        /// <summary>
        /// فعالیت قوانین تجاری
        /// </summary>
        BusinessRuleTask,

        /// <summary>
        /// فعالیت سرویس
        /// </summary>
        ServiceTask,

        /// <summary>
        /// فعالیت ارسال
        /// </summary>
        SendTask,

        /// <summary>
        /// فعالیت دریافت
        /// </summary>
        ReceiveTask,

        /// <summary>
        /// فعالیت کدنویسی
        /// </summary>
        ScriptTask,

        /// <summary>
        /// فعالیت دستی
        /// </summary>
        ManualTask
    }

    public enum GatewayType
    {
        /// <summary>
        /// درگاه انحصاری
        /// </summary>
        ExclusiveGateway = 1,

        /// <summary>
        /// درگاه انحصاری مبتنی بر رویداد
        /// </summary>
        EventbasedGateway,

        /// <summary>
        /// درگاه موازی
        /// </summary>
        ParallelGateway,

        /// <summary>
        /// درگاه موازی مبتنی بر رویداد
        /// </summary>
        ParallelEventbasedGateway,

        /// <summary>
        /// درگاه فراگیر
        /// </summary>
        InclusiveGatewa,

        /// <summary>
        /// درگاه پیچیده
        /// </summary>
        ComplexGatewa
    }

    public enum DynamicParameterType: byte
    {
        User,
        Form,
        Rule
    }
}
