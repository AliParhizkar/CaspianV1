using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("ProcurementItemsGrouping", Schema = "pcm")]
    public class ProcurementItemGrouping
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("قلم خریدنی")]
        public int ProcurementItemId { get; set; }

        [ForeignKey(nameof(ProcurementItemId))]
        public ProcurementItem ProcurementItem { get; set; }

        [DisplayName("گروه اقلام خریدنی")]
        public int ProcurementItemGroupId { get; set; }

        [ForeignKey(nameof(ProcurementItemGroupId))]
        public ProcurementItemGroup ProcurementItemGroup{ get; set; }
    }
}
