using System;
using System.Text;
using System.Linq;
using Caspian.Common;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("BankBranches", Schema = "par")]
    public class BankBranch
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("بانک")]
        public int BankId { get; set; }

        [ForeignKey(nameof(BankId))]
        public Bank Bank { get; set; }

        [DisplayName("نام شعبه")]
        public string Name { get; set; }

        [DisplayName("کد شعبه")]
        public string Code { get; set; }

        [DisplayName("شهر")]
        public int? CityId { get; set; }

        [ForeignKey(nameof (CityId))]
        public City City { get; set; }

        [DisplayName("تلفن")]
        public string Tel { get; set; }

        [DisplayName("وب سایت"), MaxLength(100)]
        public string Website { get; set; }

        [DisplayName("آدرس"), MaxLength(255)]
        public string Address { get; set; }

        [CheckOnDelete("برای این شعبه حساب بانکی تعریف شده و امکان حذف آن وجود ندارد")]
        public ICollection<BankAccount> BankAccounts { get; set; }
    }
}
