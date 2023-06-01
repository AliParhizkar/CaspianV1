using Caspian.Common.Attributes;
using System.ComponentModel;
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
        /// رویداد
        /// </summary>
        Event,

        /// <summary>
        /// توضیحات
        /// </summary>
        Comment,

        
    }

    public enum TaskType: byte
    {
        /// <summary>
        /// فعالیت کاربر
        /// </summary>
        [Display(Name = "فعالیت-کاربر")]
        User = 1,

        /// <summary>
        /// فعالیت قوانین تجاری
        /// </summary>
        [Display(Name = "فعالیت-قوانین تجاری")]
        BusinessRule,

        /// <summary>
        /// فعالیت سرویس
        /// </summary>
        [Display(Name = "فعالیت-سرویس")] 
        Service,

        /// <summary>
        /// فعالیت ارسال
        /// </summary>
        [Display(Name = "فعالیت-ارسال")] 
        Send,

        /// <summary>
        /// فعالیت دریافت
        /// </summary>
        [Display(Name = "فعالیت-دریافت")] 
        Receive,

        /// <summary>
        /// فعالیت کدنویسی
        /// </summary>
        [Display(Name = "فعالیت-کدنویسی")] 
        Script,

        /// <summary>
        /// فعالیت دستی
        /// </summary>
        [Display(Name = "فعالیت-دستی")] 
        Manual
    }

    public enum GatewayType: byte
    {
        /// <summary>
        /// درگاه انحصاری
        /// </summary>
        [Display(Name = "انحصاری")]
        Exclusive = 1,

        /// <summary>
        /// درگاه انحصاری مبتنی بر رویداد
        /// </summary>
        [Display(Name = "انحصاری مبتنی بر رویداد")] 
        Eventbased,

        /// <summary>
        /// درگاه موازی
        /// </summary>
        [Display(Name = "موازی")]
        Parallel,

        /// <summary>
        /// درگاه موازی مبتنی بر رویداد
        /// </summary>
        [Display(Name = "موازی مبتنی بر رویداد")]
        ParallelEventbased,

        /// <summary>
        /// درگاه فراگیر
        /// </summary>
        [Display(Name = "فراگیر")]
        Inclusive,

        /// <summary>
        /// درگاه پیچیده
        /// </summary>
        [Display(Name = "پیچیده")]
        Complex
    }

    public enum EventTriggerType:byte
    {
        [Display(Name = "علامت")]
        Signal = 1,

        [Display(Name = "پیام")]
        Message,

        [Display(Name = "تایمر")]
        Timer,

        Terminate,

        [Display(Name = "خطا")]
        Error,

        [Display(Name = "شرطی")]
        Conditional,

        Multiple,

        [Display(Name = "موازی")]
        Parallel,

        [Display(Name = "لغو")]
        Cancel
    }

    public enum DynamicParameterType: byte
    {
        User,
        Form,
        Rule
    }
}
