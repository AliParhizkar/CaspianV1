using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("Branches", Schema = "par")]
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        [DisplayName("آدرس"), MaxLength(255)]
        public string Address { get; set; }

        [DisplayName("شرح"), MaxLength(255)]
        public string Description { get; set; }

        [CheckOnDelete("بانک دارای حساب بانکی می باشد و امکان حذف آن وجود ندارد")]
        public IList<BankAccount> BankAccounts { get; set; }
    }
}
