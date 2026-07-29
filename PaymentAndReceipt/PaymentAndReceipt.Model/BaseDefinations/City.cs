using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("Cities", Schema = "par")]
    public class City
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("استان")]
        public int ProvinceId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public Province Province { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [CheckOnDelete("برای انی شهر شعبه بانکی ثبت شده است و امکان حذف آن وجود ندارد")]
        public IList<BankBranch> BankBranches { get; set; }
    }
}
