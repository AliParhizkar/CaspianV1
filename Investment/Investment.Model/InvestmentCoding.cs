using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("InvestmentsCoding", Schema = "ivm")]
    public class InvestmentCoding
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public InvestmentCoding Parent { get; set; }

        [DisplayName("سطح کد")]
        public int Level { get; set; }

        [DisplayName("قانون استهلاک")]
        public int DepreciationLawId { get; set; }

        [MaxLength(200), DisplayName("ملاحظات")]
        public string Description { get; set; }

        [ForeignKey(nameof(DepreciationLawId))]
        public DepreciationLaw DepreciationLaw { get; set; }

        [CheckOnDelete("اموال دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public IList<InvestmentCoding> InvestmentsCoding { get; set; }
    }
}
