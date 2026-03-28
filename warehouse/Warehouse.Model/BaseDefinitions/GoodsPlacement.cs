using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("GoodsPlacements", Schema = "wh")]
    public class GoodsPlacement
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("انبار")]
        public int StockRoomId { get; set; }

        public int GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        [ForeignKey(nameof(StockRoomId))]
        public StockRoom StockRoom { get; set; }

        [DisplayName("بخش داخلی")]
        public int? LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public MaterialLocation Location { get; set; }
    }
}
