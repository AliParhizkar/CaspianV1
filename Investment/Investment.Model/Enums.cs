using System.ComponentModel.DataAnnotations;

namespace Investment.Model
{
    public enum LocationType : byte
    {
        [Display(Name = "قاره")]
        Continent,

        [Display(Name = "کشور")]
        Country,

        [Display(Name = "استان")]
        Province,

        [Display(Name = "شهر")]
        City    
    }

    public enum ServiceGroupLevelType : byte
    {
        [Display(Name = "سطح اول")]
        Level1,

        [Display(Name = "سطح دوم")]
        Level2,

        [Display(Name = "سطح سوم")]
        Level3,

        [Display(Name = "سطح چهارم")]
        Level4,

        [Display(Name = "سطح پنجم")]
        Level5,

        [Display(Name = "سطح ششم")]
        Level6,

        [Display(Name = "سطح هفتم")]
        Level7
    }

    public enum ActiveStatus : byte
    {
        [Display(Name = "فعال")]
        Active,

        [Display(Name = "غیرفعال")]
        Deactivate
    }

    public enum CodingLevelType : byte
    {
        [Display(Name = "سطح اول")]
        Level1,

        [Display(Name = "سطح دوم")]
        Level2,

        [Display(Name = "سطح سوم")]
        Level3,

        [Display(Name = "سطح چهارم")]
        Level4,

        [Display(Name = "سطح پنجم")]
        Level5,

        [Display(Name = "سطح ششم")]
        Level6,

        [Display(Name = "سطح هفتم")]
        Level7,

        [Display(Name = "سطح هشتم")]
        Level8
    }

    public enum InsuranceCompanyType: byte
    {
        [Display(Name = "بیمه گر اصلی")]
        Main,
        
        [Display(Name = "نمایندگی")]
        Agancy
    }

    public enum CalculateMethod: byte
    {
        [Display(Name = "خط مستقیم")]
        Linear,

        [Display(Name = "مانده نزولی")]
        Remain,

        [Display(Name = "بدون روش")]
        None
    }
}
