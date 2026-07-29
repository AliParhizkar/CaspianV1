using System.ComponentModel.DataAnnotations;

namespace PaymentAndReceipt.Model
{
    public enum ActiveType : byte
    {
        [Display(Name = "فعال")]
        Enable,

        [Display(Name = "غیرفعال")]
        Disable
    }

    public enum BankType: byte
    {
        [Display(Name = "بانک")]
        Bank,

        [Display(Name = "موسسه مالی و اعتباری")]
        FinancialInstitution,

        [Display(Name = "صندوق قرض الحسنه")]
        InterestFreeFund
    }
}
