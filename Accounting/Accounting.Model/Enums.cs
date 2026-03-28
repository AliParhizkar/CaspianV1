using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Model
{
    public enum SimpleDataType: byte
    {
        [Display(Name = "واحد مالی")]
        FinancialUnit
    }

    public enum CodingLevelType: byte
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

    public enum NatureType: byte
    {
        [Display(Name = "بدهکار")]
        Debtor,

        [Display(Name = "بستانکار")]
        Creditor,

        [Display(Name = "هردو")]
        Both
    }

    public enum AccountType: byte
    {
        [Display(Name = "دائم")]
        Permanent,

        [Display(Name = "موقت")]
        Temporary
    }

    public enum OtherPartyType: byte
    {
        [Display(Name = "مرکز هزینه")]
        CostCenter
    }
}
