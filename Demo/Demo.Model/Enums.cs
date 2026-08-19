using Caspian.Common.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.Model
{
    public enum CustomerType: byte
    {
        [Display(Name = "Real person")]
        Real,

        [Display(Name = "Legal person")]
        Legal
    }

    public enum Gender: byte
    {
        [Display(Name = "Male")]
        Male = 1,

        [Display(Name = "Female")]
        Female
    }

    public enum ActiveType: byte
    {
        [Display(Name = "Enable")]
        Enable,

        [Display(Name = "Disable")]
        Disable
    }

    public enum OrderType: byte
    {
        [Display(Name = "سالن")]
        Salon = 1,

        [Display(Name = "بیرون بر")]
        Takeout,

        [Display(Name = "Tel")]
        Tel,

        [Display(Name = "Internet")]
        Internet
    }

    public enum OrderStatus: byte
    {
        [Display(Name = "Canceled")]
        Canceled = 1,

        [Display(Name = "Finaled")]
        Finaled
    }

    public enum OrderKind2 : byte
    {
        [Display(Name = "Salon")]
        Salo = 1,

        [Display(Name = "Take out")]
        Takeout = 2,

        [Display(Name = "Tel")]
        Tel = 4,

        [Display(Name = "Internet")]
        Internet = 8
    }

    [EnumType(IsBitwise = true)]
    public enum Meal : byte
    {
        [Display(Name = "Breakfast")]
        Breakfast = 1,

        [Display(Name = "Lunch")]
        Lunch = 2,

        [Display(Name = "Dinner")]
        Dinner = 4,
    }

    public enum LearningScope: byte
    {
        [Display(Name = "حوزه ی شخصی")]
        Peronality = 1,

        [Display(Name = "حوزه ی آموزش")]
        Learning
    }

    public enum ServerType:byte
    {
        [Display(Name = "Open video")]
        OpenVideo = 1,
        
        [Display(Name = "Adobe Content")]
        AdobeContent,

        [Display(Name = "Janus")] 
        Janus,

        [Display(Name = "Big Blue Button")]
        BigBlueButton
    }

    public enum SimpleDataType: byte
    {
        [Display(Name = "حوزه")]
        Scope = 1,

        [Display(Name = "مرکز هزینه")]
        CostCenter,

        [Display(Name = "نوع استخدام")]
        EmploymentType,

        [Display(Name = "نوع کارمندی")]
        EmployeeType,

        [Display(Name = "مشاغل")]
        Jobs,

        [Display(Name = "دین")]
        Religion,
        
        [Display(Name = "گروه کاری")]
        WorkGroup,
    }
}