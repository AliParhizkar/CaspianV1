using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Model
{
    [Table("CostCenters", Schema = "acc")]
    public class CostCenter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public CostCenter Parent { get; set; }

        [CheckOnDelete("مرکز هزینه دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<CostCenter> Children { get; set; }
    }
}
