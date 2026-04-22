using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("SupplierGroups", Schema = "pcm")]
    public class SupplierGroup
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        public int? ParentGroupId { get; set; }

        [DisplayName("سطح کد")]
        public CodingLevels CodingLevels { get; set; }

        [ForeignKey(nameof(ParentGroupId))]
        public SupplierGroup ParentGroup { get; set; }

        [CheckOnDelete("گروه تامین کنندگان دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplierGroup> Groups { get; set; }

        [CheckOnDelete("گروه دارای تامین کننده می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplierGroupMembership> Memberships { get; set; }

        [CheckOnDelete("گروه تامین دارای حوزه ی تامین می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SupplyingScope> SupplyingScopes { get; set; }
    }
}
