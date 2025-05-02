using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("OrderDetailToppings", Schema = "mrk")]
    public class OrderDetailTopping
    {
        [Key]
        public int Id { get; set; }

        public int OrderDetailId { get; set; }

        [ForeignKey(nameof(OrderDetailId))]
        public OrderDetail OrderDetail { get; set; }

        public int ToppingId { get; set; }

        [ForeignKey(nameof(ToppingId))]
        public Topping Topping { get; set; }

        [DisplayName("قیمت")]
        public int Price { get; set; }

        [DisplayName("تعداد")]
        public decimal Quantity { get; set; }

        [ComputedSqlColumn("[Price] * [Quantity]"), DisplayName("قیمت کل")]
        public decimal PriceTotal { get; }
    }
}
