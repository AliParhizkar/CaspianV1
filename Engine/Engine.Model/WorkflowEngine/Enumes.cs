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


    public enum ControlType: byte
    {
        String = 1,

        [EnumField("عدد صحیح")]
        Integer,
        
        [EnumField("عدد اعشاری")]
        Numeric,

        Date,

        [EnumField("چند انتخابی")]
        DropdownList,

        CheckBox,

        TreeStateCheckBox,

        CheckListBox,

        Label,

        Time,

        ComboBox
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

    public enum DataModelFieldType: byte
    {
        [EnumField("زشته ای")]
        String = 1,

        [EnumField("تاریخ")]
        Date,

        [EnumField("زمان")]
        Time,

        [EnumField("تاریخ و زمان")]
        DateAndTime,

        [EnumField("عدد صحیح")]
        Integer,

        [EnumField("عدد اعشاری")]
        Decimal,
    }
}