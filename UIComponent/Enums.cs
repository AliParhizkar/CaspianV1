﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Caspian.UI
{
    public enum IconType
    {
        AngleDoubleUp,
        AngleDoubleDown,
        AngleDoubleLeft,
        AngleDoubleRight,
        AngleUp,
        AngleDown,
        AngleLeft,
        AngleRight,
        Refresh,
        ExternalLink,
        Wpforms,
        Ban,
        Plus,
        Minus,
    }

    public enum CalendarType
    {
        Gregorian = 1,

        Persian
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
}
