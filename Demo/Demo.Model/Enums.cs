using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Demo.Model
{
    public enum CustomerType: byte
    {
        [Display(Name = "Real person")]
        Real = 1,

        [Display(Name = "Legal person")]
        Legal
    }

    public enum Gender: byte
    {
        [Display(Name = "Male")]
        Male = 1,

        [Display(Name = "Female")]
        Female
    }

    public enum ActiveType: byte
    {
        [Display(Name = "Enable")]
        Enable = 1,

        [Display(Name = "Disable")]
        Disable
    }

    public enum OrderType: byte
    {
        [Display(Name = "Salon")]
        Salon = 1,

        [Display(Name = "Take out")]
        Takeout,

        [Display(Name = "Tel")]
        Tel,

        [Display(Name = "Internet")]
        Internet
    }

    public enum OrderStatus: byte
    {
        [Display(Name = "Canceled")]
        Canceled = 1,

        [Display(Name = "Finaled")]
        Finaled
    }

    public enum OrderKind2 : byte
    {
        [Display(Name = "Salon")]
        Salo = 1,

        [Display(Name = "Take out")]
        Takeout = 2,

        [Display(Name = "Tel")]
        Tel = 4,

        [Display(Name = "Internet")]
        Internet = 8
    }

    [EnumType(Isbitw = true)]
    public enum Meal : byte
    {
        [Display(Name = "Breakfast")]
        Breakfast = 1,

        [Display(Name = "Lunch")]
        Lunch = 2,

        [Display(Name = "Dinner")]
        Dinner = 4,
    }
}