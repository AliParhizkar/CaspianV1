using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("ReservationGoods", Schema = "wh")]
    public class ReservationGoods
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کالا")]
        public int GoodsId { get; set; }

        [ForeignKey(nameof(GoodsId))]
        public Goods Goods { get; set; }

        [DisplayName("رزرو")]
        public int ReservationId { get; set; }

        [ForeignKey(nameof(ReservationId))]
        public Reservation Reservation { get; set; }

        [Precision(15, 3)]
        [DisplayName("تعداد/مقدار")]
        public double Quantity { get; set; }
    }
}
