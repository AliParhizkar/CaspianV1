using Caspian.Common.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Demo.Model
{
    public enum ControlSize
    {
        [Display(Name = "Small")]
        Small = 1,

        [Display(Name = "Medium")]
        Medium,

        [Display(Name = "Large")]
        Large,

        [Display(Name = "Auto")]
        Auto
    }
}
