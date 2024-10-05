using System.ComponentModel.DataAnnotations;

namespace Caspian.UI
{
    public enum WindowStatus : byte
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
}
