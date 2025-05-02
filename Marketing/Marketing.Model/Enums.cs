using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Marketing.Model
{
    public enum ActiveType :byte
    {
        [Display(Name = "فعال")]
        Active = 1,

        [Display(Name = "غیرفعال")]
        DeActive
    }

    public enum DefaultAddressManagement: byte
    {
        [Display(Name = "کاربر")]
        User = 1,
        
        [Display(Name = "پرسیده شود")]
        Confirm,
        
        [Display(Name = "سیستم")]
        System
    }

    public enum SettleType: byte
    {
        [Display(Name = "کارت")]
        Card,

        [Display(Name = "نقد")]
        Cash,

        [Display(Name = "کارت و نقد")] 
        CardAndCash,

        [Display(Name = "اعتباری")] 
        Accounting 
    }

    public enum CategoryType: byte
    {
        [Display(Name = "تکی")]
        Single = 1,

        [Display(Name = "دو تایی")]
        Couple,

        [Display(Name = "چند تایی")]
        Multiple
    }
}
