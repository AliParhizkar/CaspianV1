using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("Currencies", Schema = "par")]
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("این ارز در حساب بانکی مورد استفاده قرار گرفته و امکان حذف آن وجود ندارد")]
        public ICollection<BankAccount> BankAccounts { get; set; }
    }
}
