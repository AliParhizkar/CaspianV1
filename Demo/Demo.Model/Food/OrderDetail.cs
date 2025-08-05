using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("OrderDetail", Schema = "demo")]
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Order")]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        [DisplayName("Descript"), MaxLength(50)]
        public string Descript { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [DisplayName("Price")]
        public int Price { get; set; }

        [Display()]
        public int Discount { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        [DisplayName("Sum"), ComputedSqlColumn("[Price] * [Quantity]")]
        public int Result { get;set; }
    }
}
