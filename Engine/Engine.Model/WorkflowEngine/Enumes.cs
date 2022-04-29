using Caspian.Common.Attributes;

namespace Caspian.Engine
{
    /// <summary>
    /// نوع عامل
    /// </summary>
    public enum ActorType: byte
    {
        /// <summary>
        /// کاربر جاری
        /// </summary>
        [EnumField("ایجاد کننده")]
        Creator = 1,
        
        /// <summary>
        /// مافوق کاربر 
        /// </summary>
        [EnumField("مافوق کاربر")]
        Master,

        /// <summary>
        /// مافوق مافوق کاربر
        /// </summary>
        [EnumField("مافوق مافوق کاربر")]
        MasterOfMaster,
        
        /// <summary>
        /// کاربری با کد خاص
        /// </summary>
        [EnumField("کاربری با کد خاص")]
        UserBySpecialId,
        
        /// <summary>
        /// کاربری با پست خاص
        /// </summary>
        [EnumField("کاربری با پست خاص")]
        UserBySpecialPost
    }

    /// <summary>
    /// نحوه ی نمایش فیلدها
    /// </summary>
    public enum ShowType: byte
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
        Disable,

        /// <summary>
        /// مخفی
        /// </summary>
        [EnumField("مخفی")]
        Hidden
    }

    /// <summary>
    /// زمان نمایش فیلدهای خاص
    /// </summary>
    public enum ShowTime: byte
    {
        /// <summary>
        /// قبل از ارجاع
        /// </summary>
        [EnumField("قبل از ارجاع")]
        BeforRef = 1,

        /// <summary>
        /// بعد از ارجاع
        /// </summary>
        [EnumField("بعد از ارجاع")]
        OfterRef
    }

    public enum ControlType: byte
    {
        String = 1,
        Integer,
        Numeric,
        Date,
        DropdownList,
        CheckBox,
        TreeStateCheckBox,
        CheckListBox,
        Label
    }

    public enum ConnectorPortType: byte
    {
        Top = 1,
        Right,
        Bottom,
        left
    }

    public enum SystemActionType: byte
    {
        /// <summary>
        /// عملیات چک
        /// </summary>
        Checking = 1,

        /// <summary>
        /// عملیات ثبت چاپ و ...
        /// </summary>
        Operation
    }

    public enum SpecialFieldType: byte
    {
        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        [EnumField("نام و نام خانوادگی")]
        FLName = 1,

        /// <summary>
        /// امضاء
        /// </summary>
        [EnumField("امضاء")]
        Signture,

        /// <summary>
        /// کد پرسنلی
        /// </summary>
        [EnumField("کد پرسنلی")]
        PersonalCode,

        /// <summary>
        /// پست سازمانی
        /// </summary>
        [EnumField("پست سازمانی")]
        OrganPost,

        /// <summary>
        /// واحد سازمانی
        /// </summary>
        [EnumField("واحد سازمانی")]
        OrganUnit,

        /// <summary>
        /// کد کاربری
        /// </summary>
        [EnumField("کد کاربری")]
        UserId,

        /// <summary>
        /// نام
        /// </summary>
        [EnumField("نام")]
        FName,

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [EnumField("نام خانوادگی")]
        LName,

        /// <summary>
        /// جنسیت
        /// </summary>
        [EnumField("جنسیت")]
        Gender
    }
}