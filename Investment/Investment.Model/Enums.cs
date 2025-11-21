using System.ComponentModel.DataAnnotations;

namespace Investment.Model
{
    public enum LocationType: byte
    {
        [Display(Name = "قاره")]
        Continent,

        [Display(Name = "کشور")]
        Country,

        [Display(Name = "استان")]
        Province,

        [Display(Name = "شهر")]
        City,

        [Display(Name = "محله")]
        Region
    }

    public enum ActiveStatus: byte
    {
        [Display(Name = "فعال")]
        Active,

        [Display(Name = "غیرفعال")]
        Deactivate
    }
}
