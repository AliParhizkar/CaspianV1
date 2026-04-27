using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("SupplyingUnitsMembership", Schema = "pcm")]
    public class SupplyingUnitMembership
    {
        [Key]
        public int Id { get; set; }

        public int SupplyingUnitId { get; set; }

        [ForeignKey(nameof(SupplyingUnitId))]
        public SupplyingUnit SupplyingUnit { get; set; }

        public int PurchasingSpecialistId { get; set; }

        [ForeignKey(nameof(PurchasingSpecialistId))]
        public PurchasingSpecialist PurchasingSpecialist { get; set; }
    }
}
