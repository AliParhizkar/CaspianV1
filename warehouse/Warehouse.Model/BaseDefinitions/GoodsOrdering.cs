using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("GoodsOrderings", Schema = "wh")]
    public class GoodsOrdering
    {
        [Key]
        public int Id { get; set; }

        public int GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        [DisplayName("مرکز نگهداری")]
        public int KeepCenterId { get; set; }

        [ForeignKey(nameof(KeepCenterId))]
        public KeepCenter KeepCenter { get; set; }

        [DisplayName("حداقل موجودی")]
        public decimal? MinimumInventory { get; set; }

        [DisplayName("حداکثر موجودی")]
        public decimal MaximumInventory { get; set; }

        [DisplayName("نقطه ی سفارش")]
        public decimal OrderingPoint { get; set; }

        [DisplayName("مقدار اقتصادی سفارش")]
        public decimal EconomicOrder { get; set; }

        [DisplayName("برآورد مصرف(ماه)")]
        public int? ConsumeUltimate { get; set; }
    }
}
