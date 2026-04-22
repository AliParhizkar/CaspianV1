using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("SupplyingScopes", Schema = "pcm")]
    public class SupplyingScope
    {
        [Key]
        public int Id { get; set; }

        public int SupplierGroupId { get; set; }

        [ForeignKey(nameof(SupplierGroupId))]
        public SupplierGroup SupplierGroup { get; set; }

        [DisplayName("گروه قلم خریدنی")]
        public int? ProcurementItemGroupId { get; set; }

        [ForeignKey(nameof(ProcurementItemGroupId))]
        public ProcurementItemGroup ProcurementItemGroup { get; set; }

        public int? ProcurementItemId { get; set; }

        [ForeignKey(nameof(ProcurementItemId))]
        public ProcurementItem ProcurementItem { get; set; }
    }
}
