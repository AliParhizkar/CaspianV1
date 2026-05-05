using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("SupplierGroupMemberships" ,Schema = "pcm")]
    public class SupplierGroupMembership
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("تامین کننده")]
        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; }

        public int SupplierGroupId { get; set; }

        [ForeignKey(nameof(SupplierGroupId))]
        public SupplierGroup SupplierGroup { get; set; }
    }
}
