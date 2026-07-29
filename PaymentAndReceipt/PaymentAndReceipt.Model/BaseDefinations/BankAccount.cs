using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("BankAccount", Schema = "par")]
    public class BankAccount
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("شعبه")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }

        [DisplayName("نوع حساب بانکی")]
        public int BankAccountTypeId { get; set; }

        [ForeignKey(nameof(BankAccountTypeId))]
        public BankAccountType BankAccountType { get; set; }

        [DisplayName("شعبه بانک")]
        public int BankBranchId { get; set; }

        [ForeignKey(nameof(BankBranchId))]
        public BankBranch BankBranch { get; set; }

        [DisplayName("شماره حساب")]
        public string AccountNumber { get; set; }

        [DisplayName("ارز")]
        public int CurrencyId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Currency Currency { get; set; }

        [DisplayName("شماره شبا")]
        public string Shaba {  get; set; }

        [DisplayName("موجودی ذخیره")]
        public int? ReserveBalance {  get; set; }

        [DisplayName("مبلغ اعتبار حساب")]
        public int? CreditLimit { get; set; }
    }
}
