using System.ComponentModel.DataAnnotations;

namespace Caspian.UI
{
    public enum BindingType
    {
        [Display(Name = "On change")]
        OnChange = 1,

        [Display(Name = "On input")]
        OnInput
    }
}
