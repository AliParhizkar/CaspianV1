using System.ComponentModel.DataAnnotations;

namespace Recruiting.Model
{
    public enum ActiveType : byte
    {
        [Display(Name = "Enable")]
        Enable = 1,

        [Display(Name = "Disable")]
        Disable
    }
}
