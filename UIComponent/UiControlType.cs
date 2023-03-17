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
        OnChange = 1,

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
