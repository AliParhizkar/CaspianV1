using System.ComponentModel.DataAnnotations;

namespace Caspian.UI
{
    public enum UiControlType
    {
        TextBox = 1,

        DatePicker,

        TimePicker,

        Window
    }

    public enum BindingType
    {
        [Display(Name = "On change")]
        OnChange = 1,

        [Display(Name = "On input")]
        OnInput
    }

    public enum FilterMode
    {
        StartsWith = 1,

        Contains = 2
    }

    public class ClassNameContainner
    {
        public string ClassName { get; set; }
    }
}
