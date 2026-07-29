using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("BankAccountType", Schema = "par")]
    public class BankAccountType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("دسته چک دارد")]
        public bool HasCheckbook { get; set; }

        [CheckOnDelete("نوع حساب دارای حساب بانکی می باشد و امکان حذف آن وجود ندارد")]
        public IList<BankAccount> BankAccounts { get; set; }
    }
}
