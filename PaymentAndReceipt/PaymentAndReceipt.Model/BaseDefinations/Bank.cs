using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("Banks", Schema = "par")]
    public class Bank
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("نوع بانک")]
        public BankType BankType { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("تلفن روابط عمومی"), MaxLength(11)]
        public string Tell { get; set; }

        [DisplayName("وب سایت"), MaxLength(100)]
        public string WebSite { get; set; }

        [DisplayName("توضیحات"), MaxLength(255)]
        public string Description { get; set; }

        [CheckOnDelete("بانک دارای شعبه می باشد و امکان حذف آن وجود ندارد")]
        public IList<BankBranch> Branches { get; set; }
    }
}
