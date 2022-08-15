using Caspian.Common.Attributes;

namespace Caspian.Engine
{
    /// <summary>
    /// نوع حکم و یا قرارداد
    /// </summary>
    //public enum ReportType
    //{
    //    /// <summary>
    //    /// کارکنان
    //    /// </summary>
    //    [EnumField("کارکنان")]
    //    Karkonan = 1,

    //    /// <summary>
    //    /// هیئت علمی
    //    /// </summary>
    //    [EnumField("هیئت علمی")]
    //    HiatElmi = 2,

    //    /// <summary>
    //    /// قراردادی
    //    /// </summary>
    //    [EnumField("قراردادی")]
    //    Contract = 3,

    //    /// <summary>
    //    /// کارمند
    //    /// </summary>
    //    [EnumField("کارمند")]
    //    Employ = 4,

    //    [EnumField("مشخصات کارمند")]
    //    Officer = 5,

    //    [EnumField("پست سازمانی")]
    //    OrganPost = 6,

    //    [EnumField("مشخصات فرزندان")]
    //    Children = 7,

    //    [EnumField("هیئت امنائی")]
    //    HiatOmanay = 8
    //}

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

    public enum WhereControlType: byte
    { 
        FromTo = 1,
        Enums = 2,
        ForeignKey
    }

    public enum InputControlType : byte
    {
        /// <summary>
        /// نوع عدد صحیح
        /// </summary>
        [EnumField("عدد صحیح")]
        Integer = 1,

        /// <summary>
        /// نوع عدد اعشاری
        /// </summary>
        [EnumField("عدد اعشاری")]
        Numeric,

        /// <summary>
        /// نوع تاریخ
        /// </summary>
        [EnumField("تاریخ")]
        Date,

        /// <summary>
        /// رشته
        /// </summary>
        [EnumField("رشته")]
        String
    }

    public enum FormControlType : byte
    {
        Lable = 1,
        TextBox,
        CheckListBox,
        DropdownList,
        Panel
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
}
