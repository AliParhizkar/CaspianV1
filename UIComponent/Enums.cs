using System.ComponentModel.DataAnnotations;

namespace Caspian.UI
{
    public enum IconType
    {
        [Display(Name = "Angle-Double-Up")]
        AngleDoubleUp,

        [Display(Name = "Angle-Double-Down")]
        AngleDoubleDown,

        [Display(Name = "Angle-Double-Left")]
        AngleDoubleLeft,

        [Display(Name = "Angle-Double-Right")]
        AngleDoubleRight,

        [Display(Name = "Angle-Up")]
        AngleUp,

        [Display(Name = "Angle-Down")]
        AngleDown,

        [Display(Name = "Angle-Left")]
        AngleLeft,

        [Display(Name = "Angle-Right")]
        AngleRight,

        [Display(Name = "Refresh")]
        Refresh,

        [Display(Name = "External-Link")]
        ExternalLink,

        [Display(Name = "Wpforms")]
        WpfForms,

        [Display(Name = "File-Word-O")]
        FileWordO,

        [Display(Name = "Ban")]
        Ban,

        [Display(Name = "Plus")]
        Plus,

        [Display(Name = "Minus")]
        Minus,

        [Display(Name = "Html5")]
        Html5
    }

    public enum BindingType
    {
        [Display(Name = "On change")]
        OnChange = 1,

        [Display(Name = "On input")]
        OnInput
    }

    public enum CommandButtonType
    {
        Default,
        
        Simple,

        WithoutIcon,

        WidthoutTitle
    }

    public enum WindowStatus: byte
    {
        Close = 1,

        Open
    }

    public enum MessageType
    {
        Info = 1,

        Quession,
    }

    public enum DefaultLayout
    {
        [Display(Name = "Center")]
        Center = 1,

        [Display(Name = "flex-start")]
        FlexStart,

        [Display(Name = "flex-end")]
        FlexEnd,

        [Display(Name = "Space-between")]
        SpaceBetween,

        [Display(Name = "Space-around")]
        SpaceAround,

        [Display(Name = "evenly")]
        SpaceEvenly,
    }



    public enum VerticalAlign
    {
        Top = 1,
        Middle,
        Bottom,
    }

    public enum HorizontalAlign
    {
        Left = 1,
        Center,
        Right,
    }

    public enum VerticalAnchor
    {
        Top = 1,
        Middle,
        Bottom,
    }

    public enum HorizontalAnchor
    {
        Left = 1,
        Center,
        Right,
    }

    public enum UpsertType
    {
        Inline,
        Popup,
        InlinPopup
    }

    public enum ValueTypeControl
    {
        Numeric,
        Date,
        Time,
        Boolean
    }
}
