using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("ProcurementItemGroups", Schema = "pcm")]
    public class ProcurementItemGroup
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("گروه بالاتر")]
        public int? ParentGroupId { get; set; }

        [DisplayName("سطح کد")]
        public CodingLevels CodingLevels { get; set; }

        [ForeignKey(nameof(ParentGroupId))]
        public ProcurementItemGroup ParentGroup { get; set; }

        [CheckOnDelete("گروه اقلام خریدنی دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ProcurementItemGroup> Groups { get; set; }

        [CheckOnDelete("گروه دارای اقلام خریدنی می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ProcurementItemGrouping> Memberships { get; set; }

        [CheckOnDelete("گروه دارای حوزه تامین می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplyingScope> SupplyingScopes { get; set; }
    }
}
