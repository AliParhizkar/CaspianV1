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

    public enum DefaultAddressManagment: byte
    {
        [Display(Name = "کاربر")]
        User = 1,
        
        [Display(Name = "پرسیده شود")]
        Confirm,
        
        [Display(Name = "سیستم")]
        System
    }
}
