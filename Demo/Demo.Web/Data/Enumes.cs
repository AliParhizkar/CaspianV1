using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Demo.Model
{
    public enum ControlSize
    {
        [Display(Name = "Small")]
        Small,

        [Display(Name = "Medium")]
        Medium,

        [Display(Name = "Large")]
        Large,

        [Display(Name = "Auto")]
        Auto
    }
}
