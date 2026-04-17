using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("GoodsFlow", Schema = "wh")]
    public class GoodsFlow
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("گردش کالا")]
        public int StockFlowId { get; set; }

        [ForeignKey(nameof(StockFlowId))]
        public StockFlow StockFlow { get; set; }

        [DisplayName("کالا")]
        public int GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        [DisplayName("تعداد/مقدار")]
        public decimal Quantity { get; set; }
    }
}
