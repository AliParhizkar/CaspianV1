using Caspian.Common.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "ایجاد کننده")]
        Creator = 1,
        
        /// <summary>
        /// مافوق کاربر 
        /// </summary>
        [Display(Name = "مافوق کاربر")]
        Master,

        /// <summary>
        /// مافوق مافوق کاربر
        /// </summary>
        [Display(Name = "مافوق مافوق کاربر")]
        MasterOfMaster,
        
        /// <summary>
        /// کاربری با کد خاص
        /// </summary>
        [Display(Name = "کاربری با کد خاص")]
        UserBySpecialId,
        
        /// <summary>
        /// کاربری با پست خاص
        /// </summary>
        [Display(Name = "کاربری با پست خاص")]
        UserBySpecialPost
    }

    public enum ResultType : byte
    {
        [Display(Name = "صحیح")]
        Integer,

        [Display(Name = "اعشاری")]
        Numeric
    }

    public enum ControlType: byte
    {
        String = 1,

        [Display(Name = "عدد صحیح")]
        Integer,
        
        [Display(Name = "عدد اعشاری")]
        Numeric,

        Date,

        [Display(Name = "چند گزینه ای")]
        DropdownList,

        [Display(Name = "درست/نادرست")]
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
        [Display(Name = "نام و نام خانوادگی")]
        FLName = 1,

        /// <summary>
        /// امضاء
        /// </summary>
        [Display(Name = "امضاء")]
        Signture,

        /// <summary>
        /// کد پرسنلی
        /// </summary>
        [Display(Name = "کد پرسنلی")]
        PersonalCode,

        /// <summary>
        /// پست سازمانی
        /// </summary>
        [Display(Name = "پست سازمانی")]
        OrganPost,

        /// <summary>
        /// واحد سازمانی
        /// </summary>
        [Display(Name = "واحد سازمانی")]
        OrganUnit,

        /// <summary>
        /// کد کاربری
        /// </summary>
        [Display(Name = "کد کاربری")]
        UserId,

        /// <summary>
        /// نام
        /// </summary>
        [Display(Name = "نام")]
        FName,

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [Display(Name = "نام خانوادگی")]
        LName,

        /// <summary>
        /// جنسیت
        /// </summary>
        [Display(Name = "جنسیت")]
        Gender
    }

    public enum DataModelFieldType: byte
    {
        [Display(Name = "رشته ای")]
        String = 1,

        [Display(Name = "تاریخ")]
        Date,

        [Display(Name = "زمان")]
        Time,

        [Display(Name = "تاریخ و زمان")]
        DateAndTime,

        [Display(Name = "عدد صحیح")]
        Integer,

        [Display(Name = "عدد اعشاری")]
        Decimal,

        [Display(Name = "چند گزینه ای")]
        MultiSelect
    }
}