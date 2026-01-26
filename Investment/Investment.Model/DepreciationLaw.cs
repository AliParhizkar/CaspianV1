using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("DepreciationLaws", Schema = "ivm")]
    public class DepreciationLaw
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("گروه قوانین استهلاک")]
        public int DepreciationLawGroupId { get; set; }

        [ForeignKey(nameof(DepreciationLawGroupId))]
        public DepreciationLawGroup DepreciationLawGroup { get; set; }

        [DisplayName("نام قانون")]
        public string Name { get; set; }

        [DisplayName("روش محاسبه")]
        public CalculateMethod CalculateMethod { get; set; }

        [DisplayName("نرخ")]
        public int Rate { get; set; }

        [DisplayName("عمر مفید")]
        public int UsefulLife { get; set; }

        [CheckOnDelete("قانون استهلاک دارای کالا می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<InvestmentCoding> InvestmentsCoding { get; set; }
    }
}
